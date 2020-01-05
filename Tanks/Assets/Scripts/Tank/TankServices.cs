using System.Linq;
using UnityEngine;


public partial class TankManager
{

	public TankServices Services { get; private set; }


	/// 戦車AI用サービスクラス
	/// @ingroup	AIServices
	public class TankServices
	{
		TankManager TankManager { get; set; }

		public TankServices(TankManager manager) => TankManager = manager;


		/// プレイヤー番号
		public int PlayerNumber => TankManager.PlayerNumber;
		/// 戦車の耐久力
		public float Health => TankManager.Health.Health;
		/// 戦車の残エネルギー
		public float Energy => TankManager.Energy;


		/// 戦車の位置
		public Vector3 Position => TankManager.Instance.transform.position;

		/// 戦車の向き
		public float Angle => TankManager.Instance.transform.rotation.eulerAngles.y;


		/// 指定プレイヤー番号の戦車を取得する
		/// @param[in]	playerNumber	プレイヤー番号
		/// @return		指定したプレイヤー番号の戦車(存在しない時は null)
		public TankController GetTank(int playerNumber)
		{
			var index = playerNumber - 1;
			if (index < 0 || index >= GameManager.Instance.TankControllers.Length) return null;
			return GameManager.Instance.TankControllers[index];
		}

		/// 指定戦車との距離を求める
		/// @param[in]	other	別の戦車
		/// @return		指定した別の戦車との距離(指定した戦車が存在しない時は 0.0f)
		public float DistanceTo(TankController other)
		{
			if (other == null) return 0.0f;
			return (other.Services.Position - Position).magnitude;
		}

		/// 指定戦車がいる方向を求める
		/// @param[in]	other	別の戦車
		/// @return		自分の向きからの相対角度(-180 ～ +180)(指定した戦車が存在しない時は 0.0f)
		/// @note		戻り値が負の時は向かって右側に、正の時は向かって左側に指定戦車があることになります。
		public float AngleTo(TankController other)
		{
			if (other == null) return 0.0f;

			var targetDir = other.Services.Position - Position;
			return Vector3.SignedAngle(targetDir, TankManager.Instance.transform.forward, Vector3.up);
		}

		/// 一番近い敵戦車を取得する
		/// @return		一番近い敵戦車(無ければ null)
		public TankController GetNearestTank()
		{
			var tanks = GameManager.Instance.TankControllers;
			return tanks.Where(tank => tank != TankManager.Controller).OrderBy(tank => DistanceTo(tank)).FirstOrDefault();
		}


		/// 一番近い敵の砲弾を取得する
		/// @return		敵の砲弾(無ければ null)
		public ShellExplosion GetNearestShell()
		{
			var shells = GameManager.Instance.Shells;
			return shells
				.Where(shell => !IsMine(shell))
				.OrderBy(shell => (shell.transform.position - Position).sqrMagnitude)
				.FirstOrDefault();
		}

		/// 指定砲弾が自分が撃ったモノか否か
		/// @param[in]	shell	砲弾
		/// @retval		true	自分が撃った砲弾
		/// @retval		false	自分が撃った砲弾ではない
		public bool IsMine(ShellExplosion shell)
		{
			return shell ? shell.Shooter == TankManager.TurrentInterface : false;
		}
	}
}

/// @file
/// 戦車AI用のサービス群