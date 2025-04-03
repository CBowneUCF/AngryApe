using ObjectBasedStateMachine.UnLayered;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MarioMovementGrounded : StateBehaviour
{
    #region Config
    public float speed;
    public float acceleration;
    public float decceleration;
    #endregion
    #region References
    public InputActionReference r_inputHorizontal;
    public MarioMovementJumping jumpState;
    MarioPhysicsBody body;
    #endregion

    public override void OnAwake()
    {
        Machine.TryGetComponent(out body);
        //MarioController.Gameplay.Jump.performed += Jump;
    }

    public override void OnFixedUpdate()
    {
        float input = r_inputHorizontal.action.ReadValue<float>();

        if(input == 0f && body.velocity.x != 0)
            body.velocity.x = Mathf.MoveTowards(body.velocity.x, 0, decceleration);
        else if (input > 0)//right
            body.velocity.x = Mathf.MoveTowards(body.velocity.x, speed, acceleration);
        else if (input < 0)//left
            body.velocity.x = Mathf.MoveTowards(body.velocity.x, -speed, acceleration);
    }

    //void Jump(InputAction.CallbackContext context)
    //{
    //    if(state == false) return;
    //    jumpState.BeginJump();
    //}


}
