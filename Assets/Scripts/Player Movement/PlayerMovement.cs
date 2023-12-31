using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FMOD.Studio;

public class PlayerMovement : MonoBehaviour
{
    //[SerializeField] private TextMeshPro speedText;
    [Header("Movement")]
    private float moveSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    private float scaledWalkSpeed;
    private float scaledSprintSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float slideSpeed;
    public float vaultSpeed;
    public float airMinSpeed;


    public float coyoteTime;
    private float coyoteTimeCounter;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    public float scaledJumpForce;

    [Header("Crouching")]
    private float scaledCrouchSpeed;
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Sound")]
    private EventInstance playerFootsteps;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public float scaledPlayerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Scaling")]
    public int playerScale;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    public float staticMass;
    private float scaledMass;
    public MovementState state;
    public enum MovementState
    {
        freeze,
        unlimited,
        walking,
        sprinting,
        vaulting,
        crouching,
        sliding,
        air
    }

    public bool sliding;
    public bool crouching;

    public bool vaulting;

    public bool freeze;
    public bool unlimited;
    
    public bool restricted;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;

        playerFootsteps = AudioManager.instance.CreateEventInstance(FMODEvents.instance.playerFootsteps);

        ScalePlayer(playerScale);

    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, scaledPlayerHeight * 0.5f + scaledPlayerHeight * 0.1f, whatIsGround);
        //Debug.DrawRay(transform.position, Vector3.down, new Color(200,200,0), scaledPlayerHeight * 0.5f + 0.2f);
        //Debug.DrawRay(cameraPos.transform.position, cameraPos.transform.forward, new Color(255,0,0), scaledPlayerHeight *1.3f);
        

        MyInput();
        SpeedControl();
        StateHandler();
        // handle drag
        if (grounded)
        {
            rb.drag = groundDrag;
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            rb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
        UpdateSound();
        UpdateText();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && (grounded || coyoteTimeCounter > 0f))
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // start crouch
        if (Input.GetKeyDown(crouchKey) && horizontalInput == 0 && verticalInput == 0)
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale * playerScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

            crouching = true;
        }

        // stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);

            crouching = false;
        }
    }

    bool keepMomentum;
    private void StateHandler()
    {
        // Mode - Freeze
        if (freeze)
        {
            state = MovementState.freeze;
            restricted = true;
            rb.velocity = Vector3.zero;
            desiredMoveSpeed = 0f;
        }

        // Mode - Unlimited
        else if (unlimited)
        {
            state = MovementState.unlimited;
            desiredMoveSpeed = 999f;
            restricted = false;
        }

        // Mode - Vaulting
        else if (vaulting)
        {
            state = MovementState.vaulting;
            desiredMoveSpeed = vaultSpeed;
            restricted = false;
        }

        // Mode - Sliding
        else if (sliding)
        {
            state = MovementState.sliding;
            restricted = false;

            // increase speed by one every second
            if (OnSlope() && rb.velocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed;
                keepMomentum = true;
            }

            else
                desiredMoveSpeed = sprintSpeed;
        }

        // Mode - Crouching
        else if (crouching)
        {
            state = MovementState.crouching;
            desiredMoveSpeed = scaledCrouchSpeed;
            restricted = false;
        }

        // Mode - Sprinting
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = scaledSprintSpeed;
            restricted = false;
        }

        // Mode - Walking
        else if (grounded)
        {
            restricted = false;
            state = MovementState.walking;
            desiredMoveSpeed = scaledWalkSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;
            if (moveSpeed < airMinSpeed)
                desiredMoveSpeed = airMinSpeed;
        }


        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;

        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;

        // deactivate keepMomentum
        if (Mathf.Abs(desiredMoveSpeed - moveSpeed) < 0.1f) keepMomentum = false;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        if (restricted) return;

        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirection.y = 0;
        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

    }

    public void ScalePlayer(int newScale)
    {   if (newScale <= 0)
        {
            ScalePlayer(1);
        }
        if(newScale > 0)
        {
            transform.localScale = new Vector3(newScale,newScale,newScale);
            scaledPlayerHeight = playerHeight * newScale;
            scaledWalkSpeed = walkSpeed * newScale;
            scaledSprintSpeed = sprintSpeed * newScale;
            scaledCrouchSpeed = crouchSpeed * newScale;
            scaledJumpForce = jumpForce * newScale;
            scaledMass = staticMass * newScale;
            startYScale = newScale;
            if (scaledMass < 4f)
            {
                rb.mass = scaledMass-0.25f;
            }else
            {
                rb.mass = scaledMass;
            }
            playerScale = newScale;
        }
    }

    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * scaledJumpForce, ForceMode.Impulse);

        coyoteTimeCounter = 0f;
    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, scaledPlayerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }


    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }
    private void UpdateSound()
    {
        if(state == MovementState.walking && MathF.Sqrt(rb.velocity.x + rb.velocity.z) != 0 )
        {
            PLAYBACK_STATE playbackState;
            playerFootsteps.getPlaybackState(out playbackState);
            playerFootsteps.setParameterByName("Speed",0);
            if(playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                playerFootsteps.start();
            }
        }
        else if(state == MovementState.sprinting)
        {
            playerFootsteps.setParameterByName("Speed",1);
        }
        else
        {
            playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }
    private void UpdateText()
    {
        //speedText.GetComponent<TMP_Text>().SetText("Speed:" + moveSpeed.ToString());
    }
}
