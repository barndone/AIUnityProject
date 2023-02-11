using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidMotor : MonoBehaviour
{
    [field: HideInInspector]
    public Transform CachedTransform { get; private set; }
    [SerializeField] private Rigidbody rb;

    public float moveSpeed = 20.0f;
    public float turnSpeed = 180.0f;
    

    void Awake()
    {
        if (rb == null)
        {
            TryGetComponent<Rigidbody>(out rb);
        }

        CachedTransform = transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rb.velocity.sqrMagnitude > 0.0f)
        {
            //  rotate towards the 
            rb.rotation = Quaternion.RotateTowards(
                rb.rotation, 
                Quaternion.LookRotation(rb.velocity.normalized, Vector3.up), 
                turnSpeed * Time.deltaTime);
        }
    }

    private void Reset()
    {
        CachedTransform = transform;
    }
}
