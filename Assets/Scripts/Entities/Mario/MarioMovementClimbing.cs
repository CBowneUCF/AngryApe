using ObjectBasedStateMachine.UnLayered;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MarioMovementClimbing : StateBehaviour
{
    public InputActionReference r_inputVertical;
    public float inputVertical => r_inputVertical.action.ReadValue<float>();

}
