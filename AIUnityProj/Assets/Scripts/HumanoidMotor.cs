using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class HumanoidMotor : MonoBehaviour
{
    [field: SerializeField] public CharacterController motor;
    [field: SerializeField] public CapsuleCollider motorCollider;

    [Header("Movement Options")]
    //  crouch speed multiplier
    [SerializeField][Range(0.0f, 1.0f)] public float Crouch_Multiplier = 0.5f;
    //  walk speed
    [SerializeField][Range(1.0f, 10.0f)] public float Walk_Speed = 1.0f;
    //  sprint speed multiplier
    [SerializeField][Range(1.0f, 2.0f)] public float Sprint_Multiplier = 1.5f;

    //  Property for returning how fast the Agent is moving
    public float Move_Speed
    {
        get { return SprintWish ? Walk_Speed * Sprint_Multiplier: Walk_Speed; }
    }

    //  engine editable crouch Height & walk Height
    [SerializeField] public float CrouchHeight = 1.0f;
    [SerializeField] public float WalkHeight = 2.0f;

    [Header("Movement Wishes")]
    //  boolean for tracking if the Agent should sprint or not
    public bool SprintWish = false;
    //  boolean for tracking if the Agent should crouch or not
    public bool CrouchWish = false;
    //  vector for tracking where the Agent should move to
    public Vector3 MoveWish;

    //  velocity used for gravity
    private float yVelocity;
    //  tracks if the Agent is actively crouching
    private bool IsCrouching = false;

    public bool isScary = false;

    //  Toggle between states of CharacterController being active (to allow to transform position)
    void CharacterControllerToggle()
    {
        motor.enabled = !motor.enabled;
    }

    void Start()
    {
        //  if the Agent was not assigned in the inspector,
        if (motor == null)
        {
            //  assign it here
            TryGetComponent<CharacterController>(out motor);
        }

        if (motorCollider == null)
        {
            TryGetComponent<CapsuleCollider>(out motorCollider);
        }
    }

    void FixedUpdate()
    {
        //  combine forces
        Vector3 BaseMove = MoveWish * Walk_Speed;
        yVelocity += Physics.gravity.y * Time.deltaTime;

        if (motor.isGrounded)
        {
            yVelocity = Physics.gravity.y * Time.deltaTime;
        }

        //  check if the Agent should be sprinting
        if (SprintWish)
        {
            //  if so: multiply BaseMove by Sprint_Multiplier as well
            BaseMove *= Sprint_Multiplier;
        }

        //  check if the Agent should be crouching
        if (CrouchWish)
        {
            //  if the Agent is not currently crouching:
            if (!IsCrouching)
            {
                //  make the Agent crouch:
                //  adjust the height of the collider to the crouch height
                motorCollider.height = CrouchHeight;
                motor.height = CrouchHeight;

                IsCrouching = true;
            }
            //  otherwise, the Agent is currently crouching
            else
            {
                //  apply the movement multiplier
                BaseMove *= Crouch_Multiplier;
            }
        }
        else if (!CrouchWish && IsCrouching)
        {
            //  mark crouch wish as false
            CrouchWish = false;
            //  mark that the Agent is no longer crouching!
            IsCrouching = false;
            //  reverse the changed made when first crouching
            motorCollider.height = WalkHeight;
            motor.height = WalkHeight;
        }

        //  Apply the movement to the Agent
        motor.Move(BaseMove * Time.deltaTime);
        motor.Move(new Vector3(0, yVelocity, 0) * Time.deltaTime);
    }
}
