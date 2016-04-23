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
<<<<<<< HEAD
	public float slideDistance;
	public bool slide = false;

	private float jumpHold;
=======

	//Distance player can slide in seconds
	public float slideDistance = 1f;
	private float initialDistance;
	public bool slide = false;

>>>>>>> f8fcf238038c747f7178b56cf32da024825f1e2f

    public float AirdashForce = 10f;

    float currentStamina;
    float TapCooler;
    int TapCount = 0;
    bool isSprinting = false;

    //float airDashCooler;
    //int airDashTapCount = 0;

    //public LayerMask ground;

    //public float GroundCheckDistance = 1.01f;
    public float JumpForce = 10f;
    private float setSpeed;

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
<<<<<<< HEAD
=======
		initialDistance = slideDistance;
>>>>>>> f8fcf238038c747f7178b56cf32da024825f1e2f
    }

    bool grounded = false;
    float LastHorz = 0;
    bool isDashing = true;
    bool hasWallJumped = false;

<<<<<<< HEAD
=======
	public void Update(){
		if (slide) {
			slideDistance -= Time.deltaTime;
			Debug.Log (slideDistance);

			if (slideDistance <= 0f) {
				isSprinting = false;
			}
		}
	}
>>>>>>> f8fcf238038c747f7178b56cf32da024825f1e2f

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
                    Debug.Log("Airdash");
                    if (Horizontal > 0)
                    {
                        RB.AddForce(new Vector2(AirdashForce, 9.8f), ForceMode2D.Impulse);
                        Debug.Log("Airdash right");
                        TapCount++;
                    }
                    else if (Horizontal < 0)
                    {
                        RB.AddForce(new Vector2(-AirdashForce, 9.8f), ForceMode2D.Impulse);
                        Debug.Log("Airdash left");
                        TapCount++;
                    }
                }
                else
                {
                    TapCooler = DoubleTapSpeed;
                    TapCount += 1;
                }   
            }
        }

        if (TapCooler > 0)
        {
            TapCooler -= 1 * Time.deltaTime;
        }
        else
        {
            TapCount = 0;
        }

        if (Horizontal == 0 || currentStamina <= 0)
        {
            isSprinting = false;
<<<<<<< HEAD

=======
>>>>>>> f8fcf238038c747f7178b56cf32da024825f1e2f
        }

        if (isSprinting)
        {
            currentStamina -= StaminaUsedPerSecond * Time.deltaTime;
            HorzSpeed = Horizontal * SprintSpeed;
            JumpForce = setSpeed + SprintingJumpIncrease;

<<<<<<< HEAD
			if (Input.GetKeyDown ("s")) {
=======
			if (Input.GetKeyDown ("s") && slideDistance > 0f) {
>>>>>>> f8fcf238038c747f7178b56cf32da024825f1e2f
				slide = true;
			}
        }
        else
<<<<<<< HEAD
        {
			if (slide) {
				slide = false;
			}
=======
   		{
			if (slide) {
				slide = false;
				slideDistance = initialDistance;
			}

>>>>>>> f8fcf238038c747f7178b56cf32da024825f1e2f
            if (!(currentStamina / MaxStamina >= 1))
            {
                currentStamina += StaminaRegenPerSecond * Time.deltaTime;
            }
        }

        //Move
        RB.velocity = new Vector2(HorzSpeed, RB.velocity.y);

<<<<<<< HEAD

        if (Input.GetButtonUp("Jump") && (grounded || stateMachine.IsNextToWall()))
=======
        if (Input.GetButtonDown("Jump") && (grounded || stateMachine.IsNextToWall()))
>>>>>>> f8fcf238038c747f7178b56cf32da024825f1e2f
        {
            if (stateMachine.IsNextToWall()) { hasWallJumped = true; }

            float forceToJumpAt = stateMachine.IsNextToWall() ? WallJumpForce : JumpForce;
            if (isSprinting) { forceToJumpAt += SprintingJumpIncrease; }
            else if (HorzSpeed != 0) { forceToJumpAt += WalkingJumpIncrease; }
<<<<<<< HEAD
			RB.AddForce(Vector2.up * forceToJumpAt, ForceMode2D.Impulse);
=======

            RB.AddForce(Vector2.up * forceToJumpAt, ForceMode2D.Impulse);
>>>>>>> f8fcf238038c747f7178b56cf32da024825f1e2f
        }

        //Animation state machine config
        if (anim)
        {
            if (Horizontal != 0)
            {
                transform.localScale = new Vector3((Horizontal >= 0 ? StartingScale.x : -StartingScale.x), StartingScale.y);
                anim.SetBool("PlayRunAnim", true);
                JumpForce = setSpeed + WalkingJumpIncrease;
            }
            else
            {
                anim.SetBool("PlayRunAnim", false);
                JumpForce = setSpeed;
            }

            anim.SetBool("isMoving", RB.velocity.x != 0);
            anim.SetBool("Jumped", !grounded);
            anim.SetBool("Landed", grounded);
			anim.SetBool("Slide", slide);
<<<<<<< HEAD
=======

>>>>>>> f8fcf238038c747f7178b56cf32da024825f1e2f
        }

        LastHorz = Horizontal;

        UpdateStateMachine();
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
