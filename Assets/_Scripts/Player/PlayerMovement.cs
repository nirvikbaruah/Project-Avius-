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
	public float slideDistance;
	public bool slide = false;

	private float jumpHold;

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
    }

    bool grounded = false;
    float LastHorz = 0;
    bool isDashing = true;
    bool hasWallJumped = false;


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

        }

        if (isSprinting)
        {
            currentStamina -= StaminaUsedPerSecond * Time.deltaTime;
            HorzSpeed = Horizontal * SprintSpeed;
            JumpForce = setSpeed + SprintingJumpIncrease;

			if (Input.GetKeyDown ("s")) {
				slide = true;
			}
        }
        else
        {
			if (slide) {
				slide = false;
			}
            if (!(currentStamina / MaxStamina >= 1))
            {
                currentStamina += StaminaRegenPerSecond * Time.deltaTime;
            }
        }

        //Move
        RB.velocity = new Vector2(HorzSpeed, RB.velocity.y);


        if (Input.GetButtonUp("Jump") && (grounded || stateMachine.IsNextToWall()))
        {
            if (stateMachine.IsNextToWall()) { hasWallJumped = true; }

            float forceToJumpAt = stateMachine.IsNextToWall() ? WallJumpForce : JumpForce;
            if (isSprinting) { forceToJumpAt += SprintingJumpIncrease; }
            else if (HorzSpeed != 0) { forceToJumpAt += WalkingJumpIncrease; }
			RB.AddForce(Vector2.up * forceToJumpAt, ForceMode2D.Impulse);
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
