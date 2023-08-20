using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnImpact : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other) {
        Destroy(transform.parent.gameObject);
    }
}
