using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// ゲームマネージャー
/// @ingroup	System
public class GameManager : MonoBehaviour
{
	// ラウンド開始までのウェイト
	static readonly float StartDelay = 3f;
	// ラウンド終了時のウェイト
	static readonly float EndDelay = 5f;

	/// インスタンス
	public static GameManager Instance { get; private set; }


	[SerializeField, Header("プレイヤー数")]
	int _NumberOfPlayers;
	[SerializeField, Header("勝利に必要な回数")]
	int _NumberOfRoundsToWin = 3;

	[SerializeField, Header("戦車出現位置半径")]
	float _TankLocationRadius = 75.0f;


	[Space, Header("-------- 以下は弄るべからず --------")]

	[SerializeField]
	CameraControl _CameraControl = null;

	[SerializeField]
	Text _MessageText;

	[SerializeField]
	GameObject _TankPrefab;
	[SerializeField]
	Transform _TanksRoot;

	[SerializeField]
	UIPlayerStatus _HudPrefab;
	[SerializeField]
	Transform _HudsRoot;

	TankManager[] Tanks { get; set; }

	public TankController[] TankControllers { get; private set; }

	public List<ShellExplosion> Shells { get; private set; } = new List<ShellExplosion>();

	int RoundNumber;
	WaitForSeconds StartWait;
	WaitForSeconds EndWait;

	public TankManager RoundWinner { get; private set; }
	public TankManager GameWinner { get; private set; }


	/// プレイヤー人数
	public int NumberOfPlayers => _NumberOfPlayers;
	/// 戦車の出現位置の半径
	public float TankLocationRadius => _TankLocationRadius;

	// 戦車プレハブ
	public GameObject TankPrefab => _TankPrefab;
	// 戦車GameObjectの親
	public Transform TanksRoot => _TanksRoot;
	// HUD用プレイヤーステータスのプレハブ
	public UIPlayerStatus HudPrefab => _HudPrefab;
	// HUD用プレイヤーステータスの親
	public Transform HudsRoot => _HudsRoot;

	// プレイヤーの出現位置をランダムにするためのテーブル
	public int[] SpawnPositionTable { get; private set; }


	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		// Create the delays so they only have to be made once.
		StartWait = new WaitForSeconds(StartDelay);
		EndWait = new WaitForSeconds(EndDelay);

		ShuffleSpawnPosition();

		SpawnAllTanks();
		SetCameraTargets();

		// Once the tanks have been created and the camera is using them as targets, start the game.
		StartCoroutine(GameLoop());
	}


	void ShuffleSpawnPosition()
	{
		if (SpawnPositionTable == null)
		{
			SpawnPositionTable = Enumerable.Range(0, NumberOfPlayers).ToArray();
		}

		for (int i = 0; i < 1000; ++i)
		{
			var a = (int)(Random.value * NumberOfPlayers) % NumberOfPlayers;
			var b = (int)(Random.value * NumberOfPlayers) % NumberOfPlayers;
			var tmp = SpawnPositionTable[a];
			SpawnPositionTable[a] = SpawnPositionTable[b];
			SpawnPositionTable[b] = tmp;
		}

	}

	void SpawnAllTanks()
	{
		Tanks = Enumerable.Range(0, _NumberOfPlayers).Select(i => new TankManager(this, i)).ToArray();
		TankControllers = Tanks.Select(tank => tank.Controller).ToArray();
	}


	private void SetCameraTargets()
	{
		_CameraControl.m_Targets = Tanks.Select(tank => tank.Instance.transform).ToArray();
	}


	// This is called from start and will run each phase of the game one after another.
	private IEnumerator GameLoop()
	{
		// Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
		yield return StartCoroutine(RoundStarting());

		// Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
		yield return StartCoroutine(RoundPlaying());

		// Once execution has returned here, run the 'RoundEnding' coroutine, again don't return until it's finished.
		yield return StartCoroutine(RoundEnding());

		// This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found.
		if (GameWinner != null)
		{
			yield return StartCoroutine(GameResult());

			// If there is a game winner, restart the level.
			SceneManager.LoadScene(0);
		}
		else
		{
			// If there isn't a winner yet, restart this coroutine so the loop continues.
			// Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
			StartCoroutine(GameLoop());
		}
	}


	private IEnumerator RoundStarting()
	{
		// 戦車の出現位置をシャッフルする
		ShuffleSpawnPosition();

		// As soon as the round starts reset the tanks and make sure they can't move.
		ResetAllTanks();
		DisableTankControl();

		// Snap the camera's zoom and position to something appropriate for the reset tanks.
		_CameraControl.SetStartPositionAndSize();

		// Increment the round number and display text showing the players what round it is.
		RoundNumber++;
		_MessageText.text = "ROUND " + RoundNumber;

		// Wait for the specified length of time until yielding control back to the game loop.
		yield return StartWait;

		_MessageText.text = "Ready ?";
		yield return new WaitForSeconds(1.0f);

		_MessageText.text = "Go !";
		yield return new WaitForSeconds(1.0f);
	}


	private IEnumerator RoundPlaying()
	{
		// As soon as the round begins playing let the players control the tanks.
		EnableTankControl();

		// Clear the text from the screen.
		_MessageText.text = string.Empty;

		// While there is not one tank left...
		while (!OneTankLeft())
		{
			// ... return on the next frame.
			yield return null;
		}
	}


	private IEnumerator RoundEnding()
	{
		// Stop tanks from moving.
		DisableTankControl();

		// Clear the winner from the previous round.
		RoundWinner = null;

		// See if there is a winner now the round is over.
		RoundWinner = GetRoundWinner();

		// If there is a winner, increment their score.
		if (RoundWinner != null)
		{
			RoundWinner.SetWin();
		}

		// Now the winner's score has been incremented, see if someone has one the game.
		GameWinner = GetGameWinner();

		// Get a message based on the scores and whether or not there is a game winner and display it.
		string message = EndMessage ();
		_MessageText.text = message;

		// Wait for the specified length of time until yielding control back to the game loop.
		yield return EndWait;

		while (!Input.GetMouseButtonDown(0))
		{
			yield return null;
		}
	}
	private IEnumerator GameResult()
	{
		string message = "";

		var ranking = Tanks.OrderByDescending(tank => tank.Score);
		ranking.ForEach((tank, i) => message += $"{i + 1}位　　{tank.Score}点　{tank.ColoredPlayerText}\n");

		_MessageText.text = message;
		yield return EndWait;

		while (!Input.GetMouseButtonDown(0))
		{
			yield return null;
		}
	}


	// This is used to check if there is one or fewer tanks remaining and thus the round should end.
	private bool OneTankLeft()
	{
		// Start the count of tanks left at zero.
		int numTanksLeft = 0;

		// Go through all the tanks...
		for (int i = 0; i < Tanks.Length; i++)
		{
			// ... and if they are active, increment the counter.
			if (Tanks[i].Instance.activeSelf)
				numTanksLeft++;
		}

		// If there are one or fewer tanks remaining return true, otherwise return false.
		return numTanksLeft <= 1;
	}


	// This function is to find out if there is a winner of the round.
	// This function is called with the assumption that 1 or fewer tanks are currently active.
	private TankManager GetRoundWinner()
	{
		// Go through all the tanks...
		for (int i = 0; i < Tanks.Length; i++)
		{
			// ... and if one of them is active, it is the winner so return it.
			if (Tanks[i].Instance.activeSelf)
				return Tanks[i];
		}

		// If none of the tanks are active it is a draw so return null.
		return null;
	}


	// This function is to find out if there is a winner of the game.
	private TankManager GetGameWinner()
	{
		// Go through all the tanks...
		for (int i = 0; i < Tanks.Length; i++)
		{
			// ... and if one of them has enough rounds to win the game, return it.
			if (Tanks[i].Wins == _NumberOfRoundsToWin)
				return Tanks[i];
		}

		// If no tanks have enough rounds to win, return null.
		return null;
	}


	// Returns a string message to display at the end of each round.
	private string EndMessage()
	{
		// By default when a round ends there are no winners so the default end message is a draw.
		string message = "DRAW!";

		// If there is a winner then change the message to reflect that.
		if (RoundWinner != null)
			message = RoundWinner.ColoredPlayerText + " WINS THE ROUND!";

		// Add some line breaks after the initial message.
		message += "\n\n\n\n";

		// Go through all the tanks and add each of their scores to the message.
		for (int i = 0; i < Tanks.Length; i++)
		{
			message += Tanks[i].ColoredPlayerText + ": " + Tanks[i].Wins + " WINS\n";
		}

		// If there is a game winner, change the entire message to reflect that.
		if (GameWinner != null)
			message = GameWinner.ColoredPlayerText + " WINS THE GAME!";

		return message;
	}


	// This function is used to turn all the tanks back on and reset their positions and properties.
	private void ResetAllTanks()
	{
		for (int i = 0; i < Tanks.Length; i++)
		{
			Tanks[i].Reset();
		}
	}


	void EnableTankControl()
	{
		for (int i = 0; i < Tanks.Length; i++)
		{
			Tanks[i].EnableControl();
		}
	}

	void DisableTankControl()
	{
		Tanks.ForEach(tank => tank.DisableControl());
	}


	public void AddShell(ShellExplosion shell)
	{
		if (Shells.Contains(shell)) return;
		Shells.Add(shell);
	}

	public void RemoveShell(ShellExplosion shell)
	{
		if (Shells.Contains(shell)) Shells.Remove(shell);
	}
}

/// @brief
/// ゲームマネージャー
