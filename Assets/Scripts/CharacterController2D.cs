using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
	[Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;   // How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
	AudioManager audioManager;
	bool playingWalkSound = false;
	bool playingJetpackStartup = false;
	bool playingJetpackHover = false;
	bool playedJetpackStartSound = false;
	bool playedJetpackStopSound = false;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();
	}

	void Start()
	{
		audioManager = FindObjectOfType<AudioManager>();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}

	public void Move(float move, bool jump)
	{
		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
		}
		// If the player should jump...
		if (jump)
		{
			m_Grounded = false;
			if (m_Rigidbody2D.velocity.y < 20)
            {
				// Add a vertical force to the player.
				m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            }

            if (!playedJetpackStartSound)
            {
				audioManager.Stop("JetpackStop");
				playedJetpackStartSound = true;
				playedJetpackStopSound = false;
				StartCoroutine(PlayJetpackStartupSound());
            }
            else if(playedJetpackStartSound && !playingJetpackStartup && !playingJetpackHover)
            {
				playingJetpackHover = true;
				audioManager.Play("JetpackHover");
			}
		}
        else if (!m_Grounded && !jump)
        {
			audioManager.Stop("JetpackStartup");
			audioManager.Stop("JetpackHover");
			if (!playedJetpackStopSound && (playingJetpackStartup || playingJetpackHover))
            {
				playedJetpackStopSound = true;
				audioManager.Play("JetpackStop");
            }
			playedJetpackStartSound = false;
			playingJetpackHover = false;
		}

        if (m_Grounded && !playingWalkSound && move != 0)
        {
			StartCoroutine(PlayWalkingSound());
		}
	}
	
	IEnumerator PlayJetpackStartupSound()
	{
		playingJetpackStartup = true;

		audioManager.Play("JetpackStartup");

		yield return new WaitForSeconds(0.9f);

		playingJetpackStartup = false;
	}

	IEnumerator PlayWalkingSound()
	{
		playingWalkSound = true;

		int rand = Mathf.RoundToInt(Random.Range(1, 5));
		audioManager.Play("RockWalk" + rand);

		yield return new WaitForSeconds(0.3f);

		playingWalkSound = false;
	}

	public void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		transform.Rotate(0f, 180f, 0f);
	}
}