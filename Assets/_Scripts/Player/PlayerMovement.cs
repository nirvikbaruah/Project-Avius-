using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerState))]
public class PlayerMovement : MonoBehaviour
{

    public float WalkSpeed = 5f;
    public float SprintSpeed = 10f;

    public float DoubleTapSpeed = 0.5f;

    public float Stamina = 10f;
    public float MaxStamina = 10f;
    public float StaminaUsedPerSecond = 1f;
    public float StaminaRegenPerSecond = 0.5f;

    public float sprintingJumpIncrease;
    public float walkingJumpIncrease;

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

    public void FixedUpdate()
    {
        grounded = stateMachine.IsGrounded();

        float Horizontal = Input.GetAxisRaw("Horizontal");
        float HorzSpeed = (grounded ? Horizontal * WalkSpeed : RB.velocity.x * 0.99f);

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
            JumpForce = setSpeed + sprintingJumpIncrease;
        }
        else
        {
            if (!(currentStamina / MaxStamina >= 1))
            {
                currentStamina += StaminaRegenPerSecond * Time.deltaTime;
            }
        }

        //Move
        if (grounded)
        {
            RB.velocity = new Vector2(HorzSpeed, RB.velocity.y);
        }

        if (Input.GetButtonDown("Jump") && grounded)
        {
            RB.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        }

        //Animation state machine config
        if (anim)
        {
            if (Horizontal != 0)
            {
                transform.localScale = new Vector3((Horizontal >= 0 ? StartingScale.x : -StartingScale.x), StartingScale.y);
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

        UpdateStateMachine();
    }

    public void OnGUI()
    {
        GUI.TextArea(new Rect(5, 5, 100, 25), TapCount.ToString());
        GUI.TextArea(new Rect(5, 35, 100, 25), TapCooler.ToString());
    }

    public void OnDestroy()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    void UpdateStateMachine()
    {
        stateMachine.currentStamina = currentStamina;
        stateMachine.maxStamina = MaxStamina;
        //stateMachine.currentSpeed = RB.velocity;
    }
}
