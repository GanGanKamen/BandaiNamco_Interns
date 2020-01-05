using UnityEngine;

/// 砲弾コンポーネント
public class ShellExplosion : MonoBehaviour
{
	public LayerMask m_TankMask;                        // Used to filter what the explosion affects, this should be set to "Players".
	public ParticleSystem m_ExplosionParticles;         // Reference to the particles that will play on explosion.
	public AudioSource m_ExplosionAudio;                // Reference to the audio that will play on explosion.

	/// 砲弾を射出する時の力
	static readonly float LaunchForce = 20.0f;
	/// 爆風の吹き飛ばし威力
	static readonly float ExplosionForce = 100f;

	/// 最大生存時間
	static readonly float MaxLifeTime = 10f;
	/// 着弾した時の爆発の影響範囲(半径)
	static readonly float ExplosionRadius = 5f;

	/// 撃った戦車の砲台
	public ITankTurrent Shooter { get; private set; }
	/// 威力
	public float Force { get; set; }

	static int ID = 0;
	public int id;

	public static void Create(ITankTurrent shooter, float force, Rigidbody prefab, Transform parent)
	{
		var shellInstance = Instantiate(prefab, parent.position, parent.rotation);
		shellInstance.velocity = LaunchForce * parent.forward;

		var shell = shellInstance.GetComponent<ShellExplosion>();
		shell.Shooter = shooter;
		shell.Force = Mathf.Pow(force, 1.5f);
		shell.id = ID++;
		shell.name = $"Shell {shell.id}";
	}

	void Start()
	{
		GameManager.Instance.AddShell(this);

		// 一定時間後に自動的に消滅するようにしておく
		Destroy(gameObject, MaxLifeTime);
	}

	void OnDestroy()
	{
		GameManager.Instance.RemoveShell(this);
	}


	void OnTriggerEnter(Collider other)
	{
		// Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
		Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRadius, m_TankMask);

		// Go through all the colliders...
		for (int i = 0; i < colliders.Length; i++)
		{
			// ... and find their rigidbody.
			Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody> ();

			// If they don't have a rigidbody, go on to the next collider.
			if (!targetRigidbody)
				continue;

			// Add an explosion force.
			targetRigidbody.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius);

			// Find the TankHealth script associated with the rigidbody.
			TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth> ();

			// If there is no TankHealth script attached to the gameobject, go on to the next collider.
			if (!targetHealth)
				continue;

			// Calculate the amount of damage the target should take based on it's distance from the shell.
			float damage = CalculateDamage (targetRigidbody.position, other == colliders[i]);

			// Deal this damage to the tank.
			targetHealth.TakeDamage(damage);
		}

		// Unparent the particles from the shell.
		m_ExplosionParticles.transform.parent = null;

		// Play the particle system.
		m_ExplosionParticles.Play();

		// Play the explosion sound effect.
		m_ExplosionAudio.Play();

		// Once the particles have finished, destroy the gameobject they are on.
		ParticleSystem.MainModule mainModule = m_ExplosionParticles.main;
		Destroy(m_ExplosionParticles.gameObject, mainModule.duration);

		// Destroy the shell.
		Destroy(gameObject);
	}


	float CalculateDamage(Vector3 targetPosition, bool bDirect)
	{
		// Create a vector from the shell to the target.
		Vector3 explosionToTarget = targetPosition - transform.position;

		// Calculate the distance from the shell to the target.
		float explosionDistance = explosionToTarget.magnitude;

		// Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
		float relativeDistance = bDirect ? 1.0f : (ExplosionRadius - explosionDistance) / ExplosionRadius;

		// Calculate damage as this proportion of the maximum possible damage.
		float damage = relativeDistance * Const.BaseDamage * Force;

		// Make sure that the minimum damage is always 0.
		damage = Mathf.Max(0f, damage);

		return damage;
	}
}


/// @file
/// 砲弾
