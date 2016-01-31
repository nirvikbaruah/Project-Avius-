using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerState))]
public class PlayerMovement : MonoBehaviour {

    public float WalkSpeed = 5f;
    public float SprintSpeed = 10f;
    public float AirMoveSpeed = 1f;
    public float AirDashForce = 20f;

	public float DoubleTapSpeed = 0.5f;

	public float Stamina = 10f;
    public float MaxStamina = 10f;
	public float StaminaUsedPerSecond = 1f;
	public float StaminaRegenPerSecond = 0.5f;

	public float sprintingJumpIncrease;
	public float walkingJumpIncrease;
	public float airdashDistance = 10f;
	private bool alreadyDashed = false;

	float currentStamina;
	float SprintCooler;
	int SprintTapCount = 0;
	bool isSprinting = false;

	double lastAirDash;
	bool airDashed = false;

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
		SprintCooler = DoubleTapSpeed;
		stateMachine = GetComponent<PlayerState> ();

		currentStamina = Stamina;
    }

    bool grounded = false;

	float LastHorz = 0;

    public void Update()
    {
		grounded = stateMachine.IsGrounded ();

        float Horizontal = Input.GetAxisRaw("Horizontal");
		float HorzSpeed = Horizontal * (grounded ? WalkSpeed : AirMoveSpeed);

		if (!grounded && !alreadyDashed) {
			if (Input.GetKeyDown (KeyCode.D)){
				RB.velocity = new Vector2(airdashDistance * 1000f, 5f);
				Debug.Log ("Airdash right");
				alreadyDashed = true;
			}
			if (Input.GetKeyDown (KeyCode.A)){
				RB.velocity = new Vector2(airdashDistance * -1000f, 5f);
				Debug.Log ("Airdash left");
				alreadyDashed = true;
			}
		}


		if (grounded) {
			alreadyDashed = false;
		}

		//Check for a double tap to see if sprinting
		if (Mathf.Abs(Horizontal) != 0 && Mathf.Abs(LastHorz) == 0) {
			//Tap
			if (grounded && SprintCooler > 0 && SprintTapCount == 1){
				isSprinting = true;
			} else {
				SprintCooler = DoubleTapSpeed;
				SprintTapCount += 1;
			}
		}

		if ( SprintCooler > 0 ) {
			SprintCooler -= 1 * Time.deltaTime ;
		} else{
			SprintTapCount = 0 ;
		}

		if (Horizontal == 0 || currentStamina <= 0) {
			isSprinting = false;
		}

		if (isSprinting) {
			currentStamina -= StaminaUsedPerSecond * Time.deltaTime;
			HorzSpeed = Horizontal * SprintSpeed;
			JumpForce = setSpeed + sprintingJumpIncrease;
		} else {
            if (!(currentStamina / MaxStamina >= 1))
            {
                currentStamina += StaminaRegenPerSecond * Time.deltaTime;
            }
		}

		RB.velocity = new Vector2(HorzSpeed, RB.velocity.y);

		if (Input.GetButtonDown("Jump") && grounded)
        {
            RB.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        }
        if (anim)
        {
            if (Horizontal != 0)
            {
                transform.localScale = new Vector3((RB.velocity.x >= 0 ? StartingScale.x : -StartingScale.x), StartingScale.y);
                anim.SetBool("PlayRunAnim", true);
				JumpForce = setSpeed + walkingJumpIncrease;
            }
            else
            {
                anim.SetBool("PlayRunAnim", false);
				JumpForce = setSpeed;
            }

            anim.SetBool("isMoving", RB.velocity.x != 0);
            anim.SetBool("Jumped", !grounded);
            anim.SetBool("Landed", grounded);
        }

		LastHorz = Horizontal;

		UpdateStateMachine ();
    }
		

    public void OnDestroy()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

	void UpdateStateMachine() {
		stateMachine.currentStamina = currentStamina;
        stateMachine.maxStamina = MaxStamina;
		//stateMachine.currentSpeed = RB.velocity;
	}
}
