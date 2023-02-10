using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAIController : MonoBehaviour
{
    [SerializeField] private HumanoidMotor motor;

    [SerializeField] private Transform[] waypoints;
    [SerializeField] private int currentWaypoint;

    [SerializeField] private float seekStrenght = 2.0f;
    [SerializeField] private float fleeStrength = 2.0f;
    [SerializeField] private float wanderStrenght = 3.0f;
    [SerializeField] private float wanderRadius = 3.0f;
    [SerializeField] private float jitterStr = 0.5f;

    public float threshold = 0.3f;

    public LayerMask worldMask;

    void Start()
    {
        if (motor == null)
        {
            TryGetComponent<HumanoidMotor>(out motor);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //  Where am I?
        Vector3 curPos = motor.transform.position;
        //  Where do I want to be?
        Vector3 destination = waypoints[currentWaypoint].position;
        //  What is the delta between those two
        Vector3 offset = destination - curPos;

        if (offset.sqrMagnitude < threshold * threshold)
        {
            ++currentWaypoint;

            if (currentWaypoint >= waypoints.Length)
            {
                currentWaypoint = 0;
            }

            destination = waypoints[currentWaypoint].transform.position;
            offset = destination - curPos;
        }

        Vector3 seekForce = SteeringMethods.Seek(curPos, destination, motor.MoveWish, 1.0f);
        Vector3 wanderForce = SteeringMethods.Wander(curPos, wanderRadius, jitterStr, motor.MoveWish, 1.0f);

        wanderForce.y = 0.0f;
        wanderForce.Normalize();

        motor.MoveWish += seekForce * (seekStrenght * Time.deltaTime);
        motor.MoveWish += wanderForce * (wanderStrenght * Time.deltaTime);

        motor.MoveWish.Normalize();

        //
        //  determining if a crouch is required
        //

        Vector3 WalkTop = motor.motorCollider.center + new Vector3(0, motor.WalkHeight / 2 - motor.motorCollider.radius, 0);
        WalkTop = motor.transform.TransformPoint(WalkTop);

        Vector3 bot = motor.motorCollider.center - new Vector3(0, motor.WalkHeight / 2 - motor.motorCollider.radius, 0);
        bot = motor.transform.TransformPoint(bot);

        Vector3 CrouchTop;


        //Debug.DrawLine(WalkTop, bot, Color.red);

        bool isBlocked = Physics.CapsuleCast(WalkTop, bot, motor.motorCollider.radius, offset.normalized, motor.Move_Speed * 1.1f * Time.deltaTime);
        bool crouchedThisFrame = false;
        if (isBlocked && !motor.CrouchWish)
        {
            CrouchTop = new Vector3(0.0f, motor.CrouchHeight / 2, 0.0f) + new Vector3(0, motor.CrouchHeight / 2 - motor.motorCollider.radius, 0);
            CrouchTop = motor.transform.TransformPoint(CrouchTop);

            bot = new Vector3(0.0f, motor.CrouchHeight / 2.0f, 0.0f) - new Vector3(0, motor.CrouchHeight / 2 - motor.motorCollider.radius, 0);
            bot = motor.transform.TransformPoint(bot);

            bool shouldCrouch = !Physics.CapsuleCast(CrouchTop, bot, motor.motorCollider.radius, offset.normalized, motor.Move_Speed * 1.1f * Time.deltaTime);
            motor.CrouchWish = shouldCrouch;
            crouchedThisFrame = shouldCrouch;
        }

        if (!crouchedThisFrame && motor.CrouchWish)
        {
            Vector3 standingTop = new Vector3(0.0f, motor.WalkHeight / 2, 0.0f ) + new Vector3(0.0f, motor.WalkHeight / 2 - motor.motorCollider.radius, 0.0f);
            standingTop = motor.transform.TransformPoint(standingTop);

            bot = new Vector3(0.0f, motor.WalkHeight / 2.0f, 0.0f) - new Vector3(0, motor.WalkHeight / 2 - motor.motorCollider.radius, 0);
            bot = motor.transform.TransformPoint(bot);

            var overlaps = PhysicsUtil.OverlapCapsule(standingTop, bot, motor.motorCollider.radius, worldMask, true);
        
            if (overlaps.Length == 0)
            {
                motor.CrouchWish = false;
            }
        }
    }
}
