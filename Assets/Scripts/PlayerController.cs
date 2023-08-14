using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private MovementHandler move;
    [SerializeField] private JumpHandler jump;
    [SerializeField] private GroundedCheck ground;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;

    private bool grounded, lastGrounded;

    private float jumpsquat = 0.1f;
    private float jumpsquatTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        lastGrounded = grounded;
        grounded = ground.CheckGrounded();


        if (InputHandler.Instance.move.pressed) {
            move.StartAcceleration(InputHandler.Instance.dir);
            sprite.flipX = InputHandler.Instance.dir < 0;
            animator.SetBool("running", true);
        } else if (InputHandler.Instance.move.down) {
            move.UpdateMovement(InputHandler.Instance.dir);
            sprite.flipX = InputHandler.Instance.dir < 0;
            animator.SetBool("running", true);
        } else {
            move.StartDeceleration();
            animator.SetBool("running", false);
        }

        if (grounded && InputHandler.Instance.jump.pressed) {
            jump.StartJump();
            grounded = false;
            jumpsquatTime = Time.time + jumpsquat;

            animator.SetBool("grounded", false);

            animator.SetTrigger("jump");
        }

        animator.SetBool("grounded", grounded && Time.time > jumpsquatTime);
    }
}
