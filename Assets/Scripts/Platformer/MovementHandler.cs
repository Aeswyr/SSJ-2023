using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rbody;
    [SerializeField] private AnimationCurve accelerationCurve;
    private float accelerationTime;
    [SerializeField] private AnimationCurve decelerationCurve;
    private float decelerationTime;
    private AnimationCurve currentCurve;
    private float currentSpeed;
    private float curveTime;
    private float timestamp;
    private float dir;
    bool moving = false;

    bool paused;
    float pausedUntil;
    float pauseStarted;
    float storedVelocity;
    void Awake() {
        accelerationTime = accelerationCurve[accelerationCurve.length - 1].time;
        decelerationTime = decelerationCurve[decelerationCurve.length - 1].time;
    }

    void FixedUpdate()
    {
        if (Time.time < pausedUntil)
            return;

        if (paused) {
            paused = false;
            rbody.velocity = new Vector2(storedVelocity, rbody.velocity.y);
            timestamp += pausedUntil - pauseStarted;
        }

        if (Time.time < timestamp) {
            rbody.velocity = new Vector2(currentSpeed * dir * currentCurve.Evaluate(Time.time - timestamp + curveTime), rbody.velocity.y);
        } else if (moving) {
            rbody.velocity = new Vector2(speed * dir, rbody.velocity.y);
        } else {
            rbody.velocity = rbody.velocity.y * Vector2.up;
        }
    }

    public void StartDeceleration() {
        moving = false;

        currentCurve = decelerationCurve;
        curveTime = decelerationTime;
        currentSpeed = speed;
        if (Mathf.Abs(rbody.velocity.x) < currentSpeed)
            currentSpeed = Mathf.Abs(rbody.velocity.x);

        timestamp = Time.time + curveTime;
    }

    public void StartAcceleration(float dir) {
        moving = true;
        
        currentCurve = accelerationCurve;
        curveTime = accelerationTime;
        currentSpeed = speed;

        timestamp = Time.time + curveTime;
    }

    public void UpdateMovement(float dir) {
        this.dir = dir;
        moving = true;
    }

    public void OverrideCurve(float speed, AnimationCurve curve, float dir) {
        moving = true;
        this.dir = dir;
        
        currentCurve = curve;
        curveTime = curve[curve.length - 1].time;
        currentSpeed = speed;

        timestamp = Time.time + curveTime;
    }

    public void ForceStop() {
        moving = false;
        timestamp = 0;
        rbody.velocity = rbody.velocity.y * Vector2.up;
    }

    public void ResetCurves() {
        currentSpeed = speed;
    }

    public void Pause(float endPause) {
        if (paused) {
            if (endPause > pausedUntil)
                pausedUntil = endPause;
        } else {
            paused = true;
            pauseStarted = Time.time;
            pausedUntil = endPause;
            storedVelocity = rbody.velocity.x;
            rbody.velocity = new Vector2(0, rbody.velocity.y);
        }
    }
}
