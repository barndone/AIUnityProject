using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AIControllerUtils
{
    public static Vector3 GetTargetVelocity(GameObject target)
    {
        Rigidbody targetRb = target.GetComponentInParent<Rigidbody>();
        HumanoidMotor targetMotor = target.GetComponentInParent<HumanoidMotor>();

        //  does the target have the rigidbody component?
        if (targetRb != null)
        {
            //  if so, return the velocity of the target
            return targetRb.velocity;
        }

        //  otherwise, does the target have the HumanoidMotor component?
        else if (targetMotor != null)
        {
            //  if so, return the velocity of the target
            return targetMotor.MoveWish;
        }
        //  otherwise it will not have a velocity
        else
        {
            //  Print that it does not have a component that will track velocity
            Debug.LogError("Target does not have a component that tracks Velocity");
            //  return a zero vector
            return Vector3.zero;
        }
    }
}
