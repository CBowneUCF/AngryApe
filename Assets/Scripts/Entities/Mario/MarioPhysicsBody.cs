using System.Collections;
using System.Collections.Generic;
using ObjectBasedStateMachine.UnLayered;
using UnityEngine;
using UnityEngine.UIElements;

public class MarioPhysicsBody : MonoBehaviour
{
    #region Config
    public int movementProjectionSteps;
    public float maxSlopeNormalAngle = 20f;
    public ContactFilter2D groundFilter;
    public ContactFilter2D ladderFliter;

    #endregion
    #region References
    private Rigidbody2D rb;
    private new Collider2D collider;
    private StateMachine Machine;
    public MarioMovementGrounded groundedState;
    public MarioMovementJumping airState;
    public MarioMovementClimbing climbingState;
    #endregion
    #region Data

    public Vector2 velocity = Vector2.zero; 
    public Vector2 position
    {
        get => rb.position;
        set
        {
            rb.position = value;
            rb.MovePosition(value);
        }
    }
    public void SetPosition(float? x = null, float? y = null)
    {
        x ??= position.x;
        y ??= position.y;
        position = new(x.Value, y.Value);
    }
    public bool Grounded
    {
        get => _grounded;

        set
        {
            if (_grounded == value) return;
            _grounded = value;

            if (_grounded)
            {
                Machine.SendSignal("Land", overrideReady: true);
                velocity.y = 0; 
            }
            else
            {

            }
        }
    }
    private bool _grounded;
    #endregion


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Machine = GetComponent<StateMachine>();
        collider = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {

        initVelocity = velocity;
        initNormal = Vector2.up;

        if (Grounded)
        {
            if (rb.Cast(new Vector2(0, -.02f), out groundHit, groundFilter))
            {

                initNormal = groundHit.normal;
                if (WithinSlopeAngle(groundHit.normal))
                {
                    Grounded = true;
                    velocity.y = 0;
                    initVelocity.y = 0;
                    initVelocity = initVelocity.ProjectAndScale(groundHit.normal);
                }
            }
            else
            {
                Grounded = false;
                Machine.SendSignal("WalkOff", overrideReady: true);
            }
        }
        if (initVelocity.x == 0 && initVelocity.y == 0) return;
        Move(initVelocity * Time.fixedDeltaTime, initNormal);


    }

    /// <summary>
    /// The Collide and Slide Algorithm.
    /// </summary>
    /// <param name="vel">Input Velocity.</param>
    /// <param name="prevNormal">The Normal of the previous Step.</param>
    /// <param name="step">The current step. Starts at 0.</param>
    private void Move(Vector2 vel, Vector2 prevNormal, int step = 0)
    {
        if (rb.Cast(vel, out RaycastHit2D hit, groundFilter))
        {
            Vector2 snapToSurface = vel.normalized * hit.distance;
            Vector2 leftover = vel - snapToSurface;
            Vector2 nextNormal = hit.normal;
            position += snapToSurface;

            if (step == movementProjectionSteps) return;

            if (Grounded)
            {
                //Runs into wall/to high incline.
                if (Mathf.Approximately(hit.normal.y, 0) || (hit.normal.y > 0 && !WithinSlopeAngle(hit.normal)))
                    Stop(prevNormal);

                if (Grounded && prevNormal.y > 0 && hit.normal.y < 0) //Floor to Cieling
                    FloorCeilingLock(prevNormal, hit.normal);
                else if (Grounded && prevNormal.y < 0 && hit.normal.y > 0) //Ceiling to Floor
                    FloorCeilingLock(hit.normal, prevNormal);
            }
            else
            {
                if (vel.y < .1f && WithinSlopeAngle(hit.normal))
                {
                    Grounded = true;
                    leftover.y = 0;
                }
                else
                {
                    //leftover = leftover.ProjectAndScale(hit.normal);
                }
            }

            void FloorCeilingLock(Vector2 floorNormal, Vector2 ceilingNormal) =>
                Stop(floorNormal.y * floorNormal.y != floorNormal.sqrMagnitude ? floorNormal : ceilingNormal);

            void Stop(Vector3 newNormal)
            {
                nextNormal = newNormal.XZ().normalized;
            }

            Vector3 newDir = leftover.ProjectAndScale(nextNormal) /* *(Vector3.Dot(leftover.normalized, nextNormal) + 1)*/;
            Move(newDir, nextNormal, step + 1);
        }
        else
        {
            position += vel;
            //Snap to ground when walking on a downward slope.
            if (Grounded && initVelocity.y <= 0)
            {
                if (rb.Cast(Vector3.down, out RaycastHit2D groundHit))
                    position += Vector2.down * groundHit.distance;
                else
                {
                    Grounded = false;
                    Machine.SendSignal("WalkOff", overrideReady: true);
                }
            }
        }
    }
    Vector2 initVelocity;
    Vector2 initNormal;
    RaycastHit2D groundHit;

    private bool WithinSlopeAngle(Vector2 inNormal) => Vector2.Angle(Vector2.up, inNormal) < maxSlopeNormalAngle;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 contactPoint = collision.GetContact(0).normal;
        if (!Grounded && velocity.y > .1f && Vector3.Dot(contactPoint, Vector3.up) < -0.75f)
            velocity.y = 0;
        else if (!Grounded && WithinSlopeAngle(contactPoint))
            Grounded = true;

    }

    public void ClimbAction(bool downwards)
    {
        if(groundedState && rb.Cast(Vector2.zero, out RaycastHit2D hit, ladderFliter) && hit.transform.TryGetComponent(out Ladder ladder))
        {
            float heightDifferenceTop = Mathf.Abs(ladder.topEdge - position.y);
            float heightDifferenceBot = Mathf.Abs(ladder.bottomEdge - position.y);
            if(heightDifferenceBot < heightDifferenceTop && !downwards)
            {
                position = new Vector2(ladder.transform.position.x, ladder.bottomEdge);
                climbingState.Enter(ladder);
            }
            else if (heightDifferenceTop < heightDifferenceBot && downwards)
            {
                position = new Vector2(ladder.transform.position.x, ladder.topEdge);
                climbingState.Enter(ladder);
            }
        }
    }

    public void SetCollider(bool value) => collider.enabled = value;

    public void SnapToGround()
    {
        position += Vector2.up * .05f;
        rb.Cast(Vector2.down, out RaycastHit2D result, groundFilter);
        position += Vector2.down * result.distance;
    }
}

public static class _physicsExtensions
{
    public static Vector2 ProjectAndScale(this Vector2 v, Vector2 normal) => (v - Vector2.Dot(v, normal.normalized) * normal.normalized) * v.magnitude;

    public static Vector2 AngleToNormal(this float angle) => new(angle.Cos(), angle.Sin());
    public static float NormalToAngle(this Vector2 normal) => (normal.y / normal.x).ATan();

    public static bool Cast(this Rigidbody2D rb, Vector2 direction, out RaycastHit2D hit, ContactFilter2D filter = default)
    {
        var hits = new RaycastHit2D[1];
        int count = rb.Cast(direction.normalized, filter, hits, direction.magnitude);
        hit = hits[0];
        return count > 0;
    }
}