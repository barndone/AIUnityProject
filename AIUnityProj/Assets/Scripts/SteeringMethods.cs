using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SteeringMethods
{
    /// <summary>
    /// Returns a steering force to drive an agent towards a target
    /// </summary>
    /// <param name="currentPos"></param>
    /// <param name="seekPos"></param>
    /// <param name="currentVelocity"></param>
    /// <param name="maxVelocity"></param>
    /// <returns></returns>
    public static Vector3 Seek(Vector3 currentPos, Vector3 seekPos, Vector3 currentVelocity, float maxVelocity)
    {
        Vector3 desiredVelocity = (seekPos - currentPos).normalized * maxVelocity;
        return desiredVelocity - currentVelocity;
    }


    /// <summary>
    /// The opposite of seek:
    /// Applies a steering force to drive an agent away from a target
    /// </summary>
    /// <param name="currentPos"></param>
    /// <param name="fleePos"></param>
    /// <param name="currentVelocity"></param>
    /// <param name="maxVelocity"></param>
    /// <returns></returns>
    public static Vector3 Flee(Vector3 currentPos, Vector3 fleePos, Vector3 currentVelocity, float maxVelocity)
    {
        Vector3 desiredVelocity = (currentPos - fleePos).normalized * maxVelocity;
        return desiredVelocity - currentVelocity;
    }

    /// <summary>
    /// Have wander randomly along its given movement path
    /// </summary>
    /// <param name="currentPos"></param>
    /// <param name="sphereRad"></param>
    /// <param name="jitter"></param>
    /// <param name="currentVel"></param>
    /// <param name="maxVelocity"></param>
    /// <returns></returns>
    public static Vector3 Wander(Vector3 currentPos, float sphereRad, float jitter, Vector3 currentVel, float maxVelocity)
    {
        Vector3 spherePoint = Random.onUnitSphere * sphereRad;
        Vector3 jitterVec = new Vector3(Random.value, Random.value, Random.value).normalized * jitter;

        spherePoint = (spherePoint + jitterVec).normalized * sphereRad;

        return Seek(currentPos, currentPos + currentVel + spherePoint, currentVel, maxVelocity);
    }

    public static Vector3 Pursue(Vector3 currentPos, Vector3 targetPos, Vector3 currentVelocity, Vector3 targetVelocity, float maxVelocity)
    {
        Vector3 desiredVelocity = (targetPos + targetVelocity - currentPos).normalized * maxVelocity;
        return desiredVelocity - currentVelocity;
    }
}
