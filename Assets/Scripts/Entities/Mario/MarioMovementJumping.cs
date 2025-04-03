using ObjectBasedStateMachine.UnLayered;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MarioMovementJumping : StateBehaviour
{
    public InputActionReference r_inputHorizontal;
    public float inputHorizontal => r_inputHorizontal.action.ReadValue<float>();

}
