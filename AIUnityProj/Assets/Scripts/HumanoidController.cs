using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidController : MonoBehaviour
{
    [SerializeField] private HumanoidMotor motor;

    // Start is called before the first frame update
    void Start()
    {
        if (motor == null)
        {
            TryGetComponent<HumanoidMotor>(out motor);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //  for testing purposes only
        motor.MoveWish = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
        motor.CrouchWish = Input.GetButton("Crouch");
        motor.SprintWish = Input.GetButton("Sprint");

        //  clamp the movement vector (solves the issue of moving diagonally being faster)
        motor.MoveWish = Vector3.ClampMagnitude(motor.MoveWish, 1.0f);
    }
}
