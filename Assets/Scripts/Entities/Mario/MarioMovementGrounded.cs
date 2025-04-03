using ObjectBasedStateMachine.UnLayered;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MarioMovementGrounded : StateBehaviour
{
    public float speed;

    public InputActionReference r_inputHorizontal;
    public InputActionReference r_inputJump;
    public MarioPhysicsBody body;

    public override void OnFixedUpdate()
    {
        body.velocity.x = r_inputHorizontal.action.ReadValue<float>() * speed;
    }



}
