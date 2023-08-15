using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : Singleton<VFXManager>
{
    [SerializeField] private GameObject vfxTemplate;
    [SerializeField] private AnimationClip[] vfxList;

    Vector3 cameraPos;
    private void Start() {
        cameraPos = Camera.main.transform.position;
    }
    
    public void VFXBuilder(VFXType type, Vector3 position = default, bool destroyOnExpire = true, string layer = "VFX", bool flipX = false) {
        GameObject particle = Instantiate(vfxTemplate, position, Quaternion.identity);

        var animator = particle.GetComponent<Animator>();

        AnimatorOverrideController anim = new AnimatorOverrideController(animator.runtimeAnimatorController);
        anim["vfx"] = vfxList[(int)(type - 1)];

        animator.runtimeAnimatorController = anim;

        if (destroyOnExpire) {
            particle.AddComponent<DestroyAfterDelay>().Init(vfxList[(int)(type - 1)].length - 0.03f);
        }

        var sprite = particle.GetComponent<SpriteRenderer>();
        sprite.sortingLayerName = layer;
        sprite.flipX = flipX;
    }

    public void ScreenShake(float duration, float magnitude) {
        StartCoroutine(Shake(duration, magnitude));
    }

    private IEnumerator Shake(float duration, float magnitude) {
        int scale = 60;
        int scaledDuration = (int)(duration * scale);
        for (int i = scaledDuration; i > 0; i--) {
            Camera.main.transform.position = cameraPos + (Vector3) (magnitude * scaledDuration * Random.insideUnitCircle);

            yield return new WaitForSeconds(1f/scale);
        }

        Camera.main.transform.position = cameraPos;
    }

    public enum VFXType {
        DEFAULT, AIRDASH_PAD, JUMP_PAD,
    }
}
