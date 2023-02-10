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
    [SerializeField] [Range(0.0f, 1.0f)] private float Crouch_Multiplier = 0.5f;
    //  walk speed
    [SerializeField] [Range(1.0f, 10.0f)] private float Walk_Speed = 1.0f;
    //  sprint speed multiplier
    [SerializeField] [Range(1.0f, 2.0f)] private float Sprint_Multiplier = 1.5f;

    //  engine editable crouch Height & walk Height
    [SerializeField] private float CrouchHeight = 1.0f;
    [SerializeField] private float WalkHeight = 2.0f;

    [Header("Movement Wishes")]
    //  boolean for tracking if the Motor should sprint or not
    public bool SprintWish = false;
    //  boolean for tracking if the Motor should crouch or not
    public bool CrouchWish = false;
    //  vector for tracking where the Motor should move to
    public Vector3 MoveWish;

    //  velocity used for gravity
    private float yVelocity;
    //  tracks if the motor is actively crouching
    private bool IsCrouching = false;

    void Start()
    {
        //  if the motor was not assigned in the inspector,
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

    // Update is called once per frame
    void Update()
    {

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

        //  check if the motor should be sprinting
        if (SprintWish)
        {
            //  if so: multiply BaseMove by Sprint_Multiplier as well
            BaseMove *= Sprint_Multiplier;
        }

        //  check if the motor should be crouching
        if (CrouchWish)
        {
            //  if the motor is not currently crouching:
            if (!IsCrouching)
            {
                //  make the humanoid crouch:
                //  adjust the height of the collider to the crouch height
                motorCollider.height = CrouchHeight;

                IsCrouching = true;

                //  adjust for Controller scaling from the center
                //  height adjustment: crouchHeight subtracted from the current y transform
                transform.position = new Vector3(transform.position.x, 
                                                    transform.position.y - CrouchHeight, 
                                                    transform.position.z);

                transform.localScale = new Vector3(transform.localScale.x,
                                                    CrouchHeight / 2,
                                                    transform.localScale.z);
            }
            //  otherwise, the motor is currently crouching
            else
            {
                //  apply the movement multiplier
                BaseMove *= Crouch_Multiplier;
            }
        }
        else if (!CrouchWish)
        {
            //  mark crouch wish as false
            CrouchWish = false;
            //  mark that the motor is no longer crouching!
            IsCrouching = false;
            //  reverse the changed made when first crouching
            motorCollider.height = WalkHeight;
            //  adjust for Controller scaling from the center
            //  height adjustment: CrouchHeight added to the current y transform
            transform.position = new Vector3(transform.position.x,
                                                transform.position.y + CrouchHeight,
                                                transform.position.z);

            transform.localScale = new Vector3(transform.localScale.x,
                                                CrouchHeight,
                                                transform.localScale.z);
        }

        //  Apply the movement
        motor.Move(BaseMove * Time.deltaTime);
        motor.Move(new Vector3(0, yVelocity, 0) * Time.deltaTime);
    }
}
