using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerState))]
public class PlayerMovement : MonoBehaviour
{

    public float WalkSpeed = 5f;
    public float SprintSpeed = 10f;
    public float AirMoveSpeed = 2f;

    public float DoubleTapSpeed = 0.5f;

    public float Stamina = 10f;
    public float MaxStamina = 10f;
    public float StaminaUsedPerSecond = 1f;
    public float StaminaRegenPerSecond = 0.5f;

    public float SprintingJumpIncrease = 4;
    public float WalkingJumpIncrease = 2;
    public float WallJumpForce = 10;
	public float slideDistance = 1f;
	public bool slide = false;


	private float jumpHold;
	private bool addSlide = false;

	//Distance player can slide in seconds
	private float initialDistance = 1f;


    public float AirdashForce = 100000f;
	private bool airDash = false;

    float currentStamina;
    float TapCooler;
    int TapCount = 0;
    bool isSprinting = false;

    //float airDashCooler;
    //int airDashTapCount = 0;

    //public LayerMask ground;

    //public float GroundCheckDistance = 1.01f;
    public float JumpForce = 1f;
    private float setSpeed;

	public GameObject line;

    Rigidbody2D RB;
    Animator anim;

    Vector3 StartingScale;

    PlayerState stateMachine;

    public void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        setSpeed = JumpForce;

        StartingScale = transform.localScale;
        TapCooler = DoubleTapSpeed;
        stateMachine = GetComponent<PlayerState>();

        currentStamina = Stamina;
		initialDistance = slideDistance;

		this.GetComponent<DistanceJoint2D>().enabled = false;
    }

    bool grounded = false;
    float LastHorz = 0;
    bool isDashing = true;
    bool hasWallJumped = false;

	public void Update(){
		if (!slide && addSlide) {
			if (slideDistance < initialDistance) {
				slideDistance = initialDistance;
			} else {
				addSlide = false;
			}
		}
		if (slide) {
			slideDistance -= Time.deltaTime;
			addSlide = false;

			if (slideDistance <= 0f) {
				isSprinting = false;
				slide = false;
			}
		}
	}

    public void FixedUpdate()
    {
        grounded = stateMachine.IsGrounded();
        if (grounded) { hasWallJumped = false; }

        float Horizontal = Input.GetAxisRaw("Horizontal");
        float HorzSpeed = (grounded || hasWallJumped ? Horizontal * WalkSpeed : Horizontal * AirMoveSpeed);


        //Check for a double tap
        if (Mathf.Abs(Horizontal) != 0 && Mathf.Abs(LastHorz) == 0)
        {
            //Tap
            if (grounded && TapCooler > 0 && TapCount == 1)
            {
                isSprinting = true;
            }
            else
            {
                if (!grounded && TapCooler > 0 && TapCount == 1)
                {
                    if (Horizontal > 0)
                    {
						airDash = true;
						Vector3 velocity = RB.velocity;
						velocity = Vector3.right * 100f;
						RB.velocity = velocity;
                        TapCount++;
                    }
                    else if (Horizontal < 0)
                    {
						RB.AddForce (Vector2.left * AirdashForce, ForceMode2D.Impulse);
                        TapCount++;
						airDash = true;
						Debug.Log ("Left");
                    }
                }
                else
                {
                    TapCooler = DoubleTapSpeed;
                    TapCount += 1;
                }   
            }
        }

		if (TapCooler > 0) {
			TapCooler -= 1 * Time.deltaTime;
		} else if (grounded) {
			TapCount = 0;
			airDash = false;
		} else {
			TapCount = 0;
		}

        if (Horizontal == 0 || currentStamina <= 0)
        {
            isSprinting = false;

        }

		if (isSprinting) {
			currentStamina -= StaminaUsedPerSecond * Time.deltaTime;
			HorzSpeed = Horizontal * SprintSpeed;
			JumpForce = setSpeed + SprintingJumpIncrease;
		}
			
		if (Input.GetKey ("s") && slideDistance > 0f && isSprinting && grounded) {
			slide = true;
		} else {
			if (slide) {
				slide = false;
			}
			if (!(currentStamina / MaxStamina >= 1)) {
				currentStamina += StaminaRegenPerSecond * Time.deltaTime;
			}
				
		}

		if (Input.GetKeyUp ("s") && slideDistance <= 0f) {
			addSlide = true;
		}
		//Move
		RB.velocity = new Vector2 (HorzSpeed, RB.velocity.y);

		if (Input.GetKey ("left shift")) {
			line.SetActive (true);
			if (Input.GetMouseButtonDown (1) && !grounded) {
				this.GetComponent<DistanceJoint2D> ().enabled = true;
			}
		}
		if (Input.GetKeyUp ("left shift")) {
			line.SetActive (false);
			this.GetComponent<DistanceJoint2D> ().enabled = false;
		}
			


		if (Input.GetButtonDown ("Jump") && (grounded || stateMachine.IsNextToWall ())) {
			if (stateMachine.IsNextToWall ()) {
				hasWallJumped = true;
			}

			float forceToJumpAt = stateMachine.IsNextToWall () ? WallJumpForce : JumpForce;
			if (isSprinting) {
				forceToJumpAt += SprintingJumpIncrease;
			} else if (HorzSpeed != 0) {
				forceToJumpAt += WalkingJumpIncrease;
			}
			RB.AddForce (Vector2.up * forceToJumpAt, ForceMode2D.Impulse);

		}

		//Animation state machine config
		if (anim) {
			if (Horizontal != 0) {
				transform.localScale = new Vector3 ((Horizontal >= 0 ? StartingScale.x : -StartingScale.x), StartingScale.y);
				anim.SetBool ("PlayRunAnim", true);
				JumpForce = setSpeed + WalkingJumpIncrease;
			} else {
				anim.SetBool ("PlayRunAnim", false);
				JumpForce = setSpeed;
			}


			anim.SetBool ("isMoving", RB.velocity.x != 0);
			anim.SetBool ("Jumped", !grounded && !airDash);
			anim.SetBool ("Landed", grounded);
			anim.SetBool ("Slide", slide);
			anim.SetBool ("AirDash", airDash);

		}

		LastHorz = Horizontal;

		UpdateStateMachine ();
	}

	public void OnDestroy()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	void UpdateStateMachine()
	{
		stateMachine.currentStamina = currentStamina;
		stateMachine.maxStamina = MaxStamina;
		//stateMachine.currentSpeed = RB.velocity;
	}
}
