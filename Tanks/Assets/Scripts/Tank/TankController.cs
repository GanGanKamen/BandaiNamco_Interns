using UnityEngine;


/// 戦車制御クラス
/// 
/// 戦車AIを通して、戦車をコントロールします。@n
/// 実質、戦車AIのためのサービスを提供しているクラスです。
/// @ingroup	AIServices
public class TankController
{
	/// 戦車の移動制御インターフェース
	ITankMovement Movement { get; set; }
	/// 戦車の砲塔制御インターフェース
	ITankTurrent Turrent { get; set; }

	/// AI用サービスクラス
	public TankManager.TankServices Services { get; private set; }


	/// コンストラクタ
	/// @param[in]	manager	戦車マネージャー
	public TankController(TankManager manager)
	{
		Services = manager.Services;

		// 戦車インターフェースの取得
		Movement = manager as ITankMovement;
		Turrent = manager as ITankTurrent;
	}


	/// @name	戦車の制御
	//@{

	/// 戦車の移動
	/// 
	/// 戦車を移動させます。(いわゆるラジコン操作です。)@n
	/// 3つのフラグによって、前進と、左右の向き変えが行えます。@n
	/// 前進は10m/s、左右の転回は90度/sの速度です。
	/// @param[in]	isForward	前進するか否か
	/// @param[in]	isLeft		左に回頭するか否か
	/// @param[in]	isRight		右に回頭するか否か
	/// @attention	isLeft と isRight が共に true の時は転回しません。
	public void Move(bool isForward, bool isLeft, bool isRight) => Movement.Move(isForward, isLeft, isRight);

	/// 砲塔の転回
	/// 
	/// 砲塔の向きを変えます。@n
	/// 現在の向きに関わらず一瞬で指定した向きになりますが、変化した角度の分だけエネルギーを消費します。
	/// @param[in]	yaw		向き(0～360) 0で正面、90で真右
	/// @param[in]	pitch	仰角(0～90)  0で正面、90で真上
	/// @note		座標系的には真上を示す pitch は -90 ですが、ここでは値を反転して使用しています。
	/// @attention	引数が規定範囲内を超えている場合は、1000エネルギーを消耗します！
	public void Aim(float yaw, float pitch) => Turrent.Aim(yaw, pitch);

	/// 砲弾を発射
	/// 
	/// あらかじめ設定された砲塔の向きに、指定威力で砲弾を射出します。@n
	/// 威力によって飛距離が変わることはありません。(飛距離は砲塔の仰角で変わります。)@n
	/// 指定した威力の100倍のエネルギーを消費し、砲弾が命中した物体に威力の1.5乗のダメージを与えます。@n
	/// |威力値|消費エネルギー量|予ダメージ|効率|
	/// |------|----------------|----------|----|
	/// |  1   |  100           |    1     |100%|
	/// |  5   |  500           | 約11     |220%|
	/// | 10   | 1000           | 約31     |310%|
	/// 
	/// 威力は 1 ～ 10 の10段階です。それ以外の値の時は何もしません。@n
	/// @param[in]	force		威力値
	/// @note		エネルギーが不足している場合は何もしません。
	/// @attention	引数が規定範囲内を超えている場合は、1000エネルギーを消耗します！
	public void Fire(int force) => Turrent.Fire(force);

	//@}

}


/// @file
/// 戦車制御
