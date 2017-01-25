using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D))]
public class JumpmanControls : MonoBehaviour {

	private Rigidbody2D rb2d;
	[SerializeField]private Transform groundCheck = null;
	
	public bool isOnGround = false;						 // Feet touching the ground
	public bool hammerTime = false;						 // Hammer picked up
	public bool isOnLadder = false;						 // On a ladder trigger

	public AudioClip deathClip;
	public AudioClip hammerClip;
	public AudioClip jumpClip;
	public AudioClip walkingClip;

	private List<AudioSource> sources = new List<AudioSource>();	// List of all audio clips
	private bool standing;										  // Mario's speed is 0
	private float jumpForce = 30f;								   // Mario's jump height
	private float speed = 10f;									  // Mario's walking speed
	private float climbSpeed = 5f;								  // Mario's climbing speed
	private Vector2 maxVelocity = new Vector2(3, 5);				// Max walking and climbing speed
	private Controller controller;								  // Input detector script
	public Animator animator;									  // Mario's animator
	private int maxAudioSourceCount = 10;						   // Max number of AudioSources allowed
	private bool dead = false;

	private float width = 13.0f;
	private float height = 10.0f;

	private Vector3 initialPosition;
	void Awake()
	{
		rb2d = GetComponent<Rigidbody2D>();
		controller = GetComponent<Controller>();

		// Save initial postion
		initialPosition = gameObject.transform.position;
	}

	void Update()
	{
		if (dead) return;

		// Restart
		if ( Input.GetKeyUp( KeyCode.R ) ) {
			gameObject.transform.position = initialPosition;
			hammerTime = false;

		} else if ( Input.GetKeyUp( KeyCode.H ) ) { // Hammer Time
			hammerTime = !hammerTime;
			OSCSender.SendMessage(OSCSender.PDClient, OSCSender.hammerTimeCmd, hammerTime );

		}
		


		CheckGround();

		var forceX = 0f;
		var forceY = 0f;

		var absVelX = Mathf.Abs(rb2d.velocity.x);
		var absVelY = Mathf.Abs(rb2d.velocity.y);

		if (absVelY < .2f)
		{
			standing = true;
		}
		else
		{
			standing = false;
		}
		
		// Moving Left or Right
		if (controller.moving.x != 0) 
		{
			if (absVelX < maxVelocity.x) 
			{
				forceX = standing ? speed * controller.moving.x : (speed * controller.moving.x);
				transform.localScale = new Vector3(forceX > 0 ? -3 : 3, 3, 0);
				OSCSender.SendMessage(OSCSender.PDClient, OSCSender.movingCmd, forceX > 0 ? -1.0f : 1.0f );
			}
		}
		// Standing
		else if (controller.moving.x == 0)
		{
			if (!hammerTime) this.animator.SetInteger("AnimState", 0);
			
			else if (hammerTime)
			{
				this.animator.SetInteger("AnimState", 5);
			}
			OSCSender.SendMessage(OSCSender.PDClient, OSCSender.movingCmd, 0.0f );
		}
		// Moving Up or Down, Only if Mario is on a ladder trigger
		if (controller.moving.y > 0 && isOnLadder == true)
		{
			if (absVelY < maxVelocity.y)
			{
				forceY = controller.moving.y * climbSpeed;
				this.animator.SetInteger("AnimState", 4);
			} else {
				this.animator.SetInteger("AnimState", 3);
			}
		}
		else if (absVelY > 0 && isOnGround == true)
		{
			this.animator.SetInteger("AnimState", 0);
		}
		// Jumping only if Mario is on the ground
		if (isOnGround == true && Input.GetKeyDown(KeyCode.Space))
		{
			Debug.Log("Space");
			rb2d.AddForce(Vector2.up * jumpForce);   
			this.animator.SetInteger("AnimState", 2);
			OSCSender.SendMessage(OSCSender.PDClient, OSCSender.jumpCmd, 1.0f );
			if (!hammerTime){
				PlaySound(this.jumpClip);
			}				
		}
		// Play sound clip once
		if (isOnGround == true && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)))
		{
			if (!hammerTime)
			{
				PlaySound(this.walkingClip);
				this.animator.SetInteger("AnimState", 1);
			}
			else if (hammerTime)
			{
				this.animator.SetInteger("AnimState", 6);
			}
		}

		rb2d.AddForce(new Vector2(forceX, forceY));

		// Lower left is 0 0 (6.5, 5.0)
		
		// float[] xy = {rb2d.transform.position.x, rb2d.transform.position.y};
		List<float> xy = new List<float>(); 
		xy.Add((rb2d.transform.position.x + width/2)/width);
		xy.Add(1.0f - ((rb2d.transform.position.y + height/2)/height));
		xy.Add(rb2d.velocity.x);
		xy.Add(rb2d.velocity.y);

		OSCSender.SendMessage(OSCSender.PDClient, OSCSender.positionCmd, xy);
	   
	}

	// Checks if the player is in contact with the ground
	private void CheckGround()
	{
		Collider2D collider = Physics2D.OverlapPoint(this.groundCheck.transform.position);
		this.isOnGround = (collider != null);
	}

	// Play sound clip from AudioSource
	private void PlaySound(AudioClip clip)
	{
		AudioSource source = GetAudioSource();
		source.clip = clip;
		source.Play();
	}

	// Colliders
	void OnTriggerEnter2D(Collider2D other)
	{
		if (dead) return;

		if (other.gameObject.tag == "Hammer")
		{
			Destroy(other.gameObject);
			//StartCoroutine(hammerTimer());
		}
		if (hammerTime == true && other.gameObject.tag == "Enemy")
		{
			PlayerData.Instance.Score += 100;
			Destroy(other.gameObject);
		}
		else if (hammerTime == false && other.gameObject.tag == "Enemy")
		{
			//dead = true;
			//this.animator.SetTrigger("DeathTrigger");
			OSCSender.SendMessage(OSCSender.PDClient, OSCSender.enemyCollisionCmd, 1.0f );
			gameObject.transform.position = initialPosition;
		}
		if (other.gameObject.tag == "WinLadder" && Input.GetKey(KeyCode.UpArrow))
		{
			//this.animator.SetTrigger("WinTrigger");
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		switch (other.gameObject.tag)
		{
			case "EG":
				OSCSender.SendMessage(OSCSender.PDClient, OSCSender.floorCmd, 0 );
				break;
			case "Floor1":
				OSCSender.SendMessage(OSCSender.PDClient, OSCSender.floorCmd, 1 );
				break;
			case "Floor2":
				OSCSender.SendMessage(OSCSender.PDClient, OSCSender.floorCmd, 2 );
				break;
			case "Floor3":
				OSCSender.SendMessage(OSCSender.PDClient, OSCSender.floorCmd, 3 );
				break;
			case "Floor4":
				OSCSender.SendMessage(OSCSender.PDClient, OSCSender.floorCmd, 4 );
				break;
			default:
			break;
		}

		if (other.gameObject.tag == "Ladder")
		{
			isOnLadder = true;
			if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
			{
				this.animator.SetInteger("AnimState", 3);
			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "Ladder")
		{
			isOnLadder = false;
			this.animator.SetInteger("AnimState", 0);
		}
		if (other.gameObject.tag == "Score")
		{
			PlayerData.Instance.Score += 100;
			Destroy(other.gameObject);
		}
	}



	// Audio Source
	private AudioSource GetAudioSource()
	{
		AudioSource source = this.gameObject.GetComponent<AudioSource>();
		if (source == null)
		{
			source = this.gameObject.AddComponent<AudioSource>();
			this.sources.Add(source);
		}
		return source;
	}

	private AudioSource GetAvailableSource()
	{
		if (this.sources == null)
		{
			this.sources = new List<AudioSource>();
		}
		if (this.sources.Count == 0)
		{
			AudioSource firstSource = this.gameObject.AddComponent<AudioSource>();
			this.sources.Add(firstSource);
		}

		for (int i = 0; i < this.sources.Count; i++)
		{
			AudioSource source = this.sources[i];
			if (source.isPlaying == false)
			{
				return source;
			}
		}

		if (this.sources.Count < this.maxAudioSourceCount)
		{
			AudioSource newSource = this.gameObject.AddComponent<AudioSource>();
			this.sources.Add(newSource);
			return newSource;
		}
		return null;
	}

	// Hammer Timer
	private IEnumerator hammerTimer()
	{
		hammerTime = true;
		yield return new WaitForSeconds(1.0f);
	}
}
