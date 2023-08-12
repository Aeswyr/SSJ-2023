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
        } else if (InputHandler.Instance.move.down) {
            move.UpdateMovement(InputHandler.Instance.dir);
            sprite.flipX = InputHandler.Instance.dir < 0;
        } else {
            move.StartDeceleration();
        }

        if (grounded && InputHandler.Instance.jump.pressed) {
            jump.StartJump();
        }

        animator.SetBool("grounded", grounded);
    }
}
