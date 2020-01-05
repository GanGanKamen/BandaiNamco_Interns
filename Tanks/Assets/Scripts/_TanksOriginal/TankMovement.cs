using UnityEngine;


/// 戦車の移動処理を担っているコンポーネント
/// 
/// 戦車の移動と向きの変更処理を担います。
public partial class TankMovement : MonoBehaviour, ITankMovement
{
	public AudioSource m_MovementAudio;         // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
	public AudioClip m_EngineIdling;            // Audio to play when the tank isn't moving.
	public AudioClip m_EngineDriving;           // Audio to play when the tank is moving.
	public float m_PitchRange = 0.2f;           // The amount by which the pitch of the engine noises can vary.


	public float MovementInputValue { get; set; }
	public float TurnInputValue { get; set; }


	new Rigidbody rigidbody;



	/// AudioSource の元のピッチ
	float OriginalPitch;
	/// パーティクル
	private ParticleSystem[] SmokeParticles; // References to all the particles systems used by the Tanks


	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();

		SmokeParticles = GetComponentsInChildren<ParticleSystem>();

		// ランダムなピッチの効果音を出すために、元のピッチを覚えておく
		OriginalPitch = m_MovementAudio.pitch;
	}


	private void OnEnable()
	{
		// When the tank is turned on, make sure it's not kinematic.
		rigidbody.isKinematic = false;

		// Also reset the input values.
		MovementInputValue = 0f;
		TurnInputValue = 0f;

		// 煙パーティクルを再生
		SmokeParticles.ForEach(smoke => smoke.Play());
	}


	private void OnDisable()
	{
		// When the tank is turned off, set it to kinematic so it stops moving.
		rigidbody.isKinematic = true;

		// 煙パーティクルを停止
		SmokeParticles.ForEach(smoke => smoke.Stop());
	}


	void ITankMovement.Move(bool isForward, bool isLeft, bool isRight)
	{
		MovementInputValue = isForward ? 1.0f : 0.0f;
		TurnInputValue = isLeft ? isRight ? 0.0f : -1.0f : isRight ? 1.0f : 0.0f;
	}



	private void Update()
	{
		EngineAudio();
	}


	private void EngineAudio()
	{
		// If there is no input (the tank is stationary)...
		if (Mathf.Abs(MovementInputValue) < 0.1f && Mathf.Abs(TurnInputValue) < 0.1f)
		{
			// ... and if the audio source is currently playing the driving clip...
			if (m_MovementAudio.clip == m_EngineDriving)
			{
				// ... change the clip to idling and play it.
				m_MovementAudio.clip = m_EngineIdling;
				m_MovementAudio.pitch = Random.Range(OriginalPitch - m_PitchRange, OriginalPitch + m_PitchRange);
				m_MovementAudio.Play();
			}
		}
		else
		{
			// Otherwise if the tank is moving and if the idling clip is currently playing...
			if (m_MovementAudio.clip == m_EngineIdling)
			{
				// ... change the clip to driving and play.
				m_MovementAudio.clip = m_EngineDriving;
				m_MovementAudio.pitch = Random.Range(OriginalPitch - m_PitchRange, OriginalPitch + m_PitchRange);
				m_MovementAudio.Play();
			}
		}
	}


	void FixedUpdate()
	{
		// Adjust the rigidbodies position and orientation in FixedUpdate.
		Move();
		Turn();
	}


	void Move()
	{
		// Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
		Vector3 movement = transform.forward * MovementInputValue * Const.MoveSpeed * Time.deltaTime;

		// Apply this movement to the rigidbody's position.
		rigidbody.MovePosition(rigidbody.position + movement);
	}


	void Turn()
	{
		// Determine the number of degrees to be turned based on the input, speed and time between frames.
		float turn = TurnInputValue * Const.TurnSpeed * Time.deltaTime;

		// Make this into a rotation in the y axis.
		Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);

		// Apply this rotation to the rigidbody's rotation.
		rigidbody.MoveRotation(rigidbody.rotation * turnRotation);
	}
}


