using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BasicAIController : MonoBehaviour
{
    [Header("Initialization Options")]
    [SerializeField] private HumanoidMotor motor;                           //  the HumanoidMotor component of the Agent
    [Space]
    [Header("Waypoint Options")]
    [SerializeField] private Transform[] waypoints;                         //  list of waypoints the agent has
    [SerializeField] private int currentWaypoint;                           //  the current waypoint the agent is focused on
    public float threshold = 0.3f;                                          //  the threshold for arriving at a waypoint
    [SerializeField] private GameObject target;                             //  the target entity of the GameObject
    [SerializeField] private Rigidbody targetRb;                            //  the rigidbody attached to the Target
    [SerializeField] private HumanoidMotor targetMotor;                     //  the motor attached to the Target
    [SerializeField] private bool hasMotor;                                 //  flag for if the target has a motor or a rigidbody
    [Space]
    [Header("Steering Behavior Options")]
    [SerializeField] private bool isSeeking;                                //  flag for if the agent is seeking   
    [SerializeField] private bool isFleeing;                                //  flag for if the agent is fleeing
    [SerializeField] private bool isWandering;                              //  flag for if the agent is wandering
    [SerializeField] private bool isPursuing;                               //  flag for if the agent is pursuing
    [SerializeField] private bool isEvading;                                //  flag for if the agent is evading
    [SerializeField] private bool isArriving;                               //  flag for if the agent is arriving
    [Space]
    [SerializeField] private float seekStrenght = 2.0f;                     //  the strength applied to the result of the seek behavior
    [SerializeField] private float fleeStrength = 2.0f;                     //  the strength applied to the result of the flee behavior
    [Space]
    [SerializeField] private float wanderStrenght = 3.0f;                   //  the strength applied to the result of the wander behavior
    [SerializeField] private float wanderRadius = 3.0f;                     //      the radius of the wander behavior for this agent
    [SerializeField] private float jitterStr = 0.5f;                        //      the jitter strenght of the wander behavior applied to this agent
    [Space]
    [SerializeField] private float pursueStrength = 2.0f;                   //  the strength applied to the result of the pursue behavior
    [SerializeField] private float evadeStrength = 2.0f;                    //  the strength applied to the result of the evade behavior
    [Space]
    [SerializeField] private float arrivalStrength = 2.0f;                  //  the strength applied to the result of the arrival behavior
    [SerializeField] private float arrivalRadius = 3.0f;                    //      the radius before the arrival behavior begins to apply
    [Space]
    [Header("Obstacle Detection Options")]
    public LayerMask worldMask;

    void Start()
    {
        //  if this AI agent does not currently have a reference to its HumanoidMotor component
        if (motor == null)
        {
            //  output the HumanoidMotor component to motor to store a reference to it
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

        // I don't know why we use squared values: easier to compute?
        //  if the length of the offset (delta distance) squared is less than the threshold squared:
        if (offset.sqrMagnitude < threshold * threshold)
        {
            //  we have arrived at the current waypoint!
            //  increment the current waypoint
            ++currentWaypoint;

            //  if the current waypoint would be out of bounds for the waypoint array:
            if (currentWaypoint >= waypoints.Length)
            {
                //  assign current to 0
                currentWaypoint = 0;
            }

            //  re-calculate a destination with the new currentWaypoint
            destination = waypoints[currentWaypoint].transform.position;
            //  calculate a new offset
            offset = destination - curPos;
        }

        //  if the agent is seeking:
        if (isSeeking)
        {
            //  calculate the seek force
            Vector3 seekForce = SteeringMethods.Seek(curPos, destination, motor.MoveWish, 1.0f);
            //  apply the seek force to the movewish over time
            motor.MoveWish += seekForce * (seekStrenght * Time.deltaTime);
        }

        //  if the agent is fleeing:
        if(isFleeing)
        {
            //  calculate the flee force
            Vector3 fleeForce = SteeringMethods.Flee(curPos, destination, motor.MoveWish, 1.0f);
            //  apply the flee force to the movewish over time
            motor.MoveWish += fleeForce * (fleeStrength * Time.deltaTime);
        }

        //  if the agent is wandering:
        if (isWandering)
        {
            //  calculate the wander force
            Vector3 wanderForce = SteeringMethods.Wander(curPos, wanderRadius, jitterStr, motor.MoveWish, 1.0f);

            //  zero out the y component to avoid jumping
            wanderForce.y = 0.0f;
            //  normalize the vector
            wanderForce.Normalize();

            //  apply the wander force to movewish over time
            motor.MoveWish += wanderForce * (wanderStrenght * Time.deltaTime);
        }

        //  if the agent is pursuing a target
        if (isPursuing)
        {

        }

        //  if the agent is evading a target
        if (isEvading)
        {

        }

        //  if the agent is arriving at a location
        if (isArriving)
        {

        }


        //  normalize the MoveWish vector
        motor.MoveWish.Normalize();

        //
        //  TODO: determine which steering behavior to utilize
        //

        

        //
        //  determining if a crouch is required
        //

        //  cache the Y position of the top of the AI Agent's top "sphere" from walking height
        Vector3 WalkTop = motor.motorCollider.center + new Vector3(0, motor.WalkHeight / 2 - motor.motorCollider.radius, 0);
        //  transform that position to the X and Z positions of the AI agent
        WalkTop = motor.transform.TransformPoint(WalkTop);

        //  cache the Y position of the bottom of the AI Agent's bottom "sphere"
        Vector3 bot = motor.motorCollider.center - new Vector3(0, motor.WalkHeight / 2 - motor.motorCollider.radius, 0);
        //  transform that position to the X and Z positions of the AI agent
        bot = motor.transform.TransformPoint(bot);

        //  initialize a vector for the top "sphere" of the AI agent while crouching
        Vector3 CrouchTop;

        //  by performing a CapsuleCast:
        //      check if the agent is blocked by an obstacle given:
        //          the top portion of the walking agent, the bottom portion of the walking agent
        //          the radius of the attached collider, the direction of the agent, and the move speed over time scaled by 1.1f
        bool isBlocked = Physics.CapsuleCast(WalkTop, bot, motor.motorCollider.radius, offset.normalized, motor.Move_Speed * 1.1f * Time.deltaTime);
        //  initialize boolean for if the Agent crouched this frame
        bool crouchedThisFrame = false;

        //  if the agent is blocked and crouch wish is false
        if (isBlocked && !motor.CrouchWish)
        {
            //  cache the Y position of the top of the AI Agent's top "sphere" from crouch height
            CrouchTop = new Vector3(0.0f, motor.CrouchHeight / 2, 0.0f) + new Vector3(0, motor.CrouchHeight / 2 - motor.motorCollider.radius, 0);
            //  transform that position to the X and Z positions of the AI Agent
            CrouchTop = motor.transform.TransformPoint(CrouchTop);

            //  re-calculate the bottom sphere since the center of the Agent will change when crouching
            //      new center is half of crouch height rather than half of walk height as calculated before
            bot = new Vector3(0.0f, motor.CrouchHeight / 2.0f, 0.0f) - new Vector3(0, motor.CrouchHeight / 2 - motor.motorCollider.radius, 0);
            //  transform that position to the X and Z positions of the AI agent
            bot = motor.transform.TransformPoint(bot);

            //  by performing a CapsuleCast:
            //      check if the agent would be able to crouch past the obstacle given:
            //          the top portion of the crouched agent, the bottom portion of the crouched agent
            //          the radius of the attached collider, the direction of the agent, and the move speed over time scaled by 1.1f
            bool shouldCrouch = !Physics.CapsuleCast(CrouchTop, bot, motor.motorCollider.radius, offset.normalized, motor.Move_Speed * 1.1f * Time.deltaTime);

            //  if the agent would not be blocked by the obstacle:
            //  assign CrouchWish and crouchedThisFrame to true
            motor.CrouchWish = shouldCrouch;
            crouchedThisFrame = shouldCrouch;
        }

        //  if the agent DID NOT crouch this frame and crouch wish is true:
        if (!crouchedThisFrame && motor.CrouchWish)
        {
            //  re-calculate the top and bottom portions of the agent at standing height
            //      center is now walkingHeight / 2
            //  re-calculation for y-position of bottom portion of Agent at standing height using the standing center
            Vector3 standingTop = new Vector3(0.0f, motor.WalkHeight / 2, 0.0f ) + new Vector3(0.0f, motor.WalkHeight / 2 - motor.motorCollider.radius, 0.0f);
            //  transform that location to the X and Z positions of the Agent
            standingTop = motor.transform.TransformPoint(standingTop);

            //  re-calculation for y-position of bottom portion of Agent at standing height using the standing center
            bot = new Vector3(0.0f, motor.WalkHeight / 2.0f, 0.0f) - new Vector3(0, motor.WalkHeight / 2 - motor.motorCollider.radius, 0);
            //  transform that location to the X and Z positions of the Agent
            bot = motor.transform.TransformPoint(bot);

            //  store an array of objects that are currently above the Agent given:
            //      the top portion if the agent were standing, the bottom portion if the agent were standing
            //      the radius of the attached collider, the layermask of the obstacles, debug boolean
            var overlaps = PhysicsUtil.OverlapCapsule(standingTop, bot, motor.motorCollider.radius, worldMask, true);
        
            //  if the length is zero (aka empty)
            if (overlaps.Length == 0)
            {
                //  set crouch wish to false, it is safe to stand
                motor.CrouchWish = false;
            }
        }
    }
}
