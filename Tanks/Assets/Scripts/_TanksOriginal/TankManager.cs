using System.Linq;
using UnityEngine;


/// 戦車マネージャークラス
/// @ingroup	System
public partial class TankManager : ITankUpdater, ITankMovement, ITankTurrent
{
	/// 操作が許可されている戦車
	public static TankManager AcceptedTank { get; private set; }


	// 戦車のGameObject
	public GameObject Instance { get; private set; }
	// 勝利回数
	public int Wins { get; private set; }
	// スコア
	public int Score { get; private set; }


	// 移動コンポーネントの参照
	TankMovement Movement { get; set; }
	// 砲塔コンポーネントの参照
	TankTurrent Turrent { get; set; }

	// 移動コンポーネントの参照
	ITankMovement MovementInterface { get; set; }
	// 砲塔コンポーネントの参照
	ITankTurrent TurrentInterface { get; set; }

	// 耐久値コンポーネントの参照
	public TankHealth Health { get; private set; }


	// 戦車AI
	TankAIBase TankAI { get; set; }
	/// 戦車制御
	public TankController Controller { get; private set; }

	// UIコンポーネントの参照
	UIPlayerStatus UIPlayerStatus { get; set; }

	// UI用GameObjectの参照
	GameObject CanvasGameObject { get; set; }


	// プレイヤー番号
	public int PlayerNumber { get; private set; }
	// プレイヤー番号文字列(リッチテキストによる色付けあり)
	public string ColoredPlayerText { get; private set; }

	// 砲弾射出位置(戦車原点からの相対)
	public Vector3 FirePosition { get; private set; }

	// ゲームマネージャー
	GameManager GameManager { get; set; }


	// エネルギー
	float energy;
	public float Energy
	{
		get => energy;
		private set => energy = Mathf.Clamp(value, 0.0f, Const.FullEnergy);
	}

	// 死亡フラグ
	bool isDead;


	// コンストラクタ
	public TankManager(GameManager gameManager, int index)
	{
		GameManager = gameManager;
		PlayerNumber = index + 1;

		Energy = Const.FullEnergy;

		// 戦車を生成
		Spawn(gameManager.TankPrefab, gameManager.TanksRoot);
		FirePosition = Turrent.FireTransform.position;
		// 戦車の位置を設定
		SetPosition();

		// HUDを生成
		UIPlayerStatus = GameObject.Instantiate<UIPlayerStatus>(gameManager.HudPrefab, gameManager.HudsRoot);

		// AIの準備
		SetupAI();
	}

	// 戦車を生成する
	void Spawn(GameObject tankPrefab, Transform parent)
	{
		// 戦車のGameObjectを生成し、AIコンポーネントをアタッチ
		Instance = GameObject.Instantiate<GameObject>(tankPrefab, parent);
		Instance.name = $"Tank {PlayerNumber}";

		// 操作対象のコンポーネントの取得
		Movement = Instance.GetComponentInChildren<TankMovement>();
		MovementInterface = Movement as ITankMovement;
		Turrent = Instance.GetComponentInChildren<TankTurrent>();
		TurrentInterface = Turrent as ITankTurrent;
		Health = Instance.GetComponentInChildren<TankHealth>();

		CanvasGameObject = Instance.GetComponentInChildren<Canvas>().gameObject;
	}

	// 位置と向きの設定
	void SetPosition()
	{
		float v = GameManager.SpawnPositionTable[PlayerNumber - 1] / (float)GameManager.NumberOfPlayers;
		float degree = 360.0f * v;
		float radian = 2.0f * Mathf.PI * v;
		float r = GameManager.TankLocationRadius;
		Instance.transform.localPosition = new Vector3(Mathf.Cos(radian) * r, 0.0f, Mathf.Sin(radian) * r);
		Instance.transform.localRotation = Quaternion.Euler(0.0f, 270.0f - degree, 0.0f);
	}

	// AIセットアップ
	void SetupAI()
	{
		// AI用サービスクラスを生成
		Services = new TankServices(this);
		// AIを生成
		var className = System.String.Format("TankAI_{0:00}", PlayerNumber);
		var type = System.Type.GetType(className);
		TankAI = (TankAIBase)System.Activator.CreateInstance(type, Services);

		// コントローラーとハンドラーを生成
		Controller = new TankController(this);
		Instance.AddComponent<TankHandler>().Setup(this);

		// 名前と色を設定
		UIPlayerStatus.Name = TankAI.MyName ?? $"Player {PlayerNumber}";
		SetPlayerColor(TankAI.MyColor);
	}

	// プレイヤーカラーを設定する
	public void SetPlayerColor(Color color)
	{
		// プレイヤー名を示すリッチテキスト文字列の生成
		ColoredPlayerText = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{UIPlayerStatus.Name}({PlayerNumber})</color>";
		// 戦車を構成する全てのレンダラーにプレイヤーのカラーを設定
		Instance.GetComponentsInChildren<MeshRenderer>().ForEach(renderer => renderer.material.color = color);

		// HUDの色
		UIPlayerStatus.Color = color;
	}

	// 戦車をリセットする
	public void Reset()
	{
		SetPosition();
		Energy = Const.FullEnergy;
		UIPlayerStatus.SetAlive();

		isDead = false;

		Instance.SetActive(false);
		Instance.SetActive(true);
	}


	// 戦車の制御を無効にする
	public void DisableControl() => SetControl(false);

	// 戦車の制御を有効にする
	public void EnableControl() => SetControl(true);


	void SetControl(bool isEnabled)
	{
		Movement.enabled = isEnabled;
		Turrent.enabled = isEnabled;

		CanvasGameObject.SetActive(isEnabled);
	}

	public void SetWin()
	{
		if (GameManager.Instance.RoundWinner == this)
		{
			const int RANK1_SCORE = 30;		// 1位の時の点数(参加人数より大きい必要がある！)

			++Wins;
			Score += RANK1_SCORE;
			Debug.Assert(GameManager.Instance.NumberOfPlayers < RANK1_SCORE);

			UIPlayerStatus.Ranking = $"1位 ({Score})";

			UIPlayerStatus.SetDead();	// 死ぬわけじゃないけど、ステータスバーを消すために呼ぶ
		}
	}

	// 毎フレームの処理
	void ITankUpdater.Process()
	{
		AcceptedTank = this;

		if (Health.Health <= 0.0f)
		{
			if (!isDead)
			{
				// 死亡した
				UIPlayerStatus.SetDead();
				var rank = GameManager.Instance.TankControllers.Count(tank => tank.Services.Health > 0.0f) + 1;
				Score += GameManager.Instance.NumberOfPlayers - rank;
				UIPlayerStatus.Ranking = $"{rank}位 ({Score})";

				isDead = true;
			}
		}
		else
		{
			var now = Energy;
			TankAI.Process(Controller);
			if (now == Energy)
			{
				// 何もしなかった時（エネルギー残量が変わってない時）はエネルギーが回復する
				Energy += Const.RegainingEnergy;
			}
		}
		// HUDの更新
		UIPlayerStatus.Energy = Energy / Const.FullEnergy;
		UIPlayerStatus.Health = Health.Health / Const.FullHealth;

		AcceptedTank = null;
	}


	// 戦車の移動
	void ITankMovement.Move(bool isForward, bool isLeft, bool isRight)
	{
		if (AcceptedTank != this) return;

		// コスト処理
		var cost = isForward ? Const.MoveCost : 0.0f;
		cost += isLeft ? Const.TurnCost : 0.0f;
		cost += isRight ? Const.TurnCost : 0.0f;
		if (cost > Energy) return;

		Energy -= cost;

		MovementInterface.Move(isForward, isLeft, isRight);
	}

	// 砲塔の転回
	void ITankTurrent.Aim(float yaw, float pitch)
	{
		if (AcceptedTank != this) return;

		// 引数の範囲内チェック(範囲外の時はペナルティ！)
		if (yaw < -45.0f || yaw > 45.0f || pitch < 0.0f || pitch > 90.0f)
		{
			Energy -= Const.PenaltyCost;
		}
		else
		{
			TurrentInterface.Aim(yaw, pitch);
		}
	}

	// 砲弾を発射
	void ITankTurrent.Fire(int force)
	{
		if (AcceptedTank != this) return;

		// 引数の範囲内チェック(範囲外の時はペナルティ！)
		if (Const.ForceMin <= force && force <= Const.ForceMax)
		{
			var cost = force * Const.FireCost;
			if (cost > Energy) return;

			Energy -= cost;
			TurrentInterface.Fire(force);
		}
		else
		{
			Energy -= Const.PenaltyCost;
		}
	}

}


/// @file
/// 戦車マネージャー
