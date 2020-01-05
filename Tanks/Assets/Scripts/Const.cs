
/// 定数定義クラス
/// @ingroup	Const
public class Const
{
	/// @name	戦車
	//@{
	/// 戦車の移動速度(毎秒 nメートル)
	public static float MoveSpeed { get; } = 10.0f;
	/// 戦車の転回速度(毎秒 n度)
	public static float TurnSpeed { get; } = 90.0f;
	//@}

	/// @name	エネルギー
	//@{
	/// 最大エネルギー
	public static float FullEnergy { get; } = 10000.0f;

	/// 何もしてない時のエネルギー回復量
	public static float RegainingEnergy { get; } = 5.0f;

	/// 移動コスト
	public static float MoveCost { get; } = 1.0f;
	/// 転回コスト
	public static float TurnCost { get; } = 1.0f;


	/// 砲弾の威力 1 に対するエネルギー消費量
	public static float FireCost { get; } = 100.0f;

	/// 指定ミスによるエネルギー消費量ペナルティ
	public static float PenaltyCost { get; } = 1000.0f;

	//@}


	/// @name	ヘルス
	//@{

	/// 戦車の耐久値
	public static float FullHealth { get; } = 1000.0f;

	/// 砲弾のダメージ基礎値
	public static float BaseDamage { get; } = 20f;


	/// 砲弾の威力の最小値
	public static int ForceMin { get; } = 1;
	/// 砲弾の威力の最大値
	public static int ForceMax { get; } = 10;
	//@}
}


/// @file
/// 定数定義
