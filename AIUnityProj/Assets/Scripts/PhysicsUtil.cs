using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PhysicsUtil
{
    public static Collider[] OverlapCapsule(Vector3 start, Vector3 end, float radius, LayerMask mask, bool showDebug = false)
    {
        if (showDebug)
        {
            Debug.DrawLine(start, end, Color.red);
        }

        return Physics.OverlapCapsule(start, end, radius, mask);
    }
}
