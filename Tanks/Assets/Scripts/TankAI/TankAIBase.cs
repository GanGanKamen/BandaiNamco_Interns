using UnityEngine;


/// 戦車のAIのベースクラス
/// @ingroup	AIServices
public class TankAIBase
{
	/// プレイヤー名
	public virtual string MyName { get; } = null;
    /// プレイヤーカラー
    public virtual Color MyColor { get; } = Random.ColorHSV();

    /// 戦車AI用サービスクラス
    /// @note	このクラスを通じて、AIを構築するのに便利なサービスを享受できます。
    public TankManager.TankServices Services { get; }


	// コンストラクタ
	public TankAIBase(TankManager.TankServices services) => Services = services;


	/// AIのメイン処理
	/// 
	/// ゲームシステム(正確には TankManager)から、毎フレーム呼び出されます。@n
	/// このメソッドをオーバーライドし、AIを実装します。
	/// @param[in]	controller	戦車制御クラスの参照
	public virtual void Process(TankController controller)
	{
		controller.Aim(0.0f, 0.0f);
		if (Input.GetButtonDown("Fire1")) controller.Fire(10);

		var forward = Input.GetAxis("Vertical1");
		var horizontal = Input.GetAxis("Horizontal1");
		controller.Move(forward > 0.0f, horizontal < 0.0f, horizontal > 0.0f);
	}

}


/// @file
/// 戦車のAIのベースクラス
