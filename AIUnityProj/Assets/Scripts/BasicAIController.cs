using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAIController : MonoBehaviour
{
    [SerializeField] private HumanoidMotor motor;

    [SerializeField] private Transform[] waypoints;
    [SerializeField] private int currentWaypoint;

    public float threshold = 0.3f;

    // Update is called once per frame
    void Update()
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

        //  clamp the magnitude of offset
        offset = Vector3.ClampMagnitude(offset, 1.0f);

        //  Pass the offset to the motor for movement to begin
        motor.MoveWish = offset;

        //
        //  determining if a crouch is required:
        //

        Vector3 top = motor.motorCollider.center + new Vector3(0, motor.motorCollider.height / 2 - motor.motorCollider.radius, 0);
        top = motor.transform.TransformPoint(top);

        Vector3 bot = motor.motorCollider.center - new Vector3(0, motor.motorCollider.height / 2 - motor.motorCollider.radius, 0);
        bot = motor.transform.TransformPoint(bot);


        Debug.DrawLine(top, bot, Color.red);

        //Physics.CapsuleCast();

    }
}
