using ObjectBasedStateMachine.UnLayered;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MarioMovementClimbing : StateBehaviour
{
    #region Config
    public float climbSpeed;
    public InputActionReference r_inputVertical;
    #endregion
    #region References
    private Ladder currentLadder;
    MarioPhysicsBody body;
    #endregion
    #region Data
    #endregion

    public override void OnAwake() => Machine.TryGetComponent(out body);

    public void Enter(Ladder targetLadder)
    {
        Enter();
        currentLadder = targetLadder;
        body.SetCollider(false);
        body.velocity = Vector2.zero;
    }
    public override void OnFixedUpdate()
    {
        float input = r_inputVertical.action.ReadValue<float>();

        if (input == 0) return;

        float currentPosition = body.position.y;

        if(input > 0)
        {
            currentPosition = currentPosition.MoveUp(climbSpeed * input * Time.fixedDeltaTime, currentLadder.topEdge);
            body.SetPosition(y: currentPosition);
            if(currentPosition == currentLadder.topEdge)
            {
                if (currentLadder.topAtPlatform)
                {
                    body.groundedState.Enter();
                    body.SnapToGround();
                }
                else
                {
                    body.airState.Enter();
                }
            }
        }
        else if(input < 0)
        {
            currentPosition = currentPosition.MoveDown(climbSpeed * -input * Time.fixedDeltaTime, currentLadder.bottomEdge);
            body.SetPosition(y: currentPosition);
            if (currentPosition == currentLadder.bottomEdge)
            {
                if (currentLadder.bottomAtPlatform)
                {
                    body.groundedState.Enter();
                    body.SnapToGround();
                }
                else
                {
                    body.airState.Enter();
                }
            }
        }
    }
    public override void OnExit(State next)
    {
        currentLadder = null;
        body.SetCollider(true);
    }

}
