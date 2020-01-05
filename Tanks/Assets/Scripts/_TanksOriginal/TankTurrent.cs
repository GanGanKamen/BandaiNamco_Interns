using UnityEngine;


/// 戦車の砲塔の制御を担っているコンポーネント
/// 
/// 砲塔の向きの変更、および砲弾の射出の処理を担います。
public class TankTurrent : MonoBehaviour, ITankTurrent
{
	/// 見た目的な砲塔の仰角(僅かに上を向いている)
	static readonly float TurrentPitch = -13.0f;


	/// 砲弾のプレハブ
	[SerializeField]
	Rigidbody _ShellPrefab;

	/// 砲弾を発射する位置と向き
	[SerializeField]
	Transform _FireTransform;

	/// 発射音源
	/// @note	TankMovement.MovementAudio と別の AudioSource を指定すること！
	[SerializeField]
	AudioSource _ShootingAudio;


	/// 砲弾を射出する位置
	public Vector3 FirePosition => _FireTransform.localPosition;
	public Transform FireTransform => _FireTransform;


	void ITankTurrent.Aim(float yaw, float pitch)
	{
		// 砲塔の向きをセット
		// (ピッチが反転してるのと、絵柄的な砲塔の傾きを考慮していることに注意)
		if (-45.0f <= yaw && yaw <= 45.0f)
		{
			transform.localRotation = Quaternion.Euler(-pitch - TurrentPitch, yaw, 0.0f);
		}
	}

	void ITankTurrent.Fire(int force)
	{
		// 砲弾を生成して、発射速度を設定する
		ShellExplosion.Create(this, force, _ShellPrefab, _FireTransform);

		// 射出音を再生
		_ShootingAudio.Play();
	}

}

/// @file
/// 戦車の砲塔
