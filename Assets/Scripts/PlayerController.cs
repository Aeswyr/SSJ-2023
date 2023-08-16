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

    [Header("Dodge Data")]
    [SerializeField] private AnimationCurve groundedDodge;
    [SerializeField] private float groundDodgeSpeed;
    [SerializeField] private AnimationCurve airDodge;
    [SerializeField] private float airDodgeSpeed;

    [Header("Attack Data")]
    [SerializeField] private AnimationCurve attackCurve;
    [SerializeField] private GameObject attackPrefab;

    private bool grounded, lastGrounded;
    private bool acting;
    private bool canAirdodge;
    private bool canJumpCancel;

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
        grounded = ground.CheckGrounded() && Time.time > jumpsquatTime;

        if (grounded) {
            canAirdodge = true;
        }

        if (!acting && InputHandler.Instance.move.pressed) {
            move.StartAcceleration(InputHandler.Instance.dir);
            sprite.flipX = InputHandler.Instance.dir < 0;
            animator.SetBool("running", true);
        } else if (!acting && InputHandler.Instance.move.down) {
            move.UpdateMovement(InputHandler.Instance.dir);
            sprite.flipX = InputHandler.Instance.dir < 0;
            animator.SetBool("running", true);
        } else if (!acting) {
            move.StartDeceleration();
            animator.SetBool("running", false);
        }

        if (((!acting  && grounded) || canJumpCancel) && InputHandler.Instance.jump.pressed) {
            if (canJumpCancel) {
                VFXManager.Instance.VFXBuilder(VFXManager.VFXType.JUMP_PAD, transform.position, layer: "entity_fx", flipX: sprite.flipX);
                EndAction();
            }
            jump.StartJump();
            grounded = false;
            jumpsquatTime = Time.time + jumpsquat;

            animator.SetBool("grounded", false);

            animator.SetTrigger("jump");
        } else if (!acting && (grounded || canAirdodge) && InputHandler.Instance.dodge.pressed) {
            StartAction();
            int dir = sprite.flipX ? -1 : 1;
            if (InputHandler.Instance.dir != 0)
                dir = (int)InputHandler.Instance.dir;
            if (grounded && !InputHandler.Instance.move.down) {
                // P A R R Y
            } else if (grounded) {
                move.OverrideCurve(groundDodgeSpeed, groundedDodge, dir);
                animator.SetTrigger("dodge");
            } else {
                VFXManager.Instance.VFXBuilder(VFXManager.VFXType.AIRDASH_PAD, transform.position, layer: "entity_fx", flipX: sprite.flipX);
                canJumpCancel = true;
                canAirdodge = false;
                jump.SetGravity(0, true);
                move.OverrideCurve(airDodgeSpeed, airDodge, dir);
                animator.SetTrigger("dodge");
            }
            
        } else if (!acting && grounded && InputHandler.Instance.drink.pressed) {
            StartAction();
            move.StartDeceleration();
            animator.SetTrigger("drink");
        } else if (!acting && InputHandler.Instance.primary.pressed) {
            StartAction();
            move.StartDeceleration();
            animator.SetTrigger("attack");
        } else if (!acting && InputHandler.Instance.secondary.pressed) {
            StartAction();
            // THROW
        }



        animator.SetBool("grounded", grounded);
    }

    private void TriggerAttack() {
        int dir = sprite.flipX ? -1 : 1;
        move.OverrideCurve(20 * -1, attackCurve, dir);

        VFXManager.Instance.VFXBuilder(VFXManager.VFXType.GUN_BLAST, transform.position + (4 * dir * Vector3.right) + (4 * Vector3.down), flipX: sprite.flipX);
    }

    private void StartAction() {
        acting = true;
        animator.SetBool("acting", acting);
    }

    private void EndAction() {
        acting = false;
        canJumpCancel = false;
        animator.SetBool("acting", acting);
        move.ResetCurves();
        if (!InputHandler.Instance.move.down)
            move.StartDeceleration();
        jump.ResetGravity();        
    }
}
