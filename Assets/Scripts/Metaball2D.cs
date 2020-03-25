using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Metaball2D : MonoBehaviour
{
    private new CircleCollider2D collider;

    private void Awake()
    {
        collider = GetComponent<CircleCollider2D>();
        MetaballSystem2D.Add(this);
    }

    public float GetRadius()
    {
        return collider.radius;
    }

    private void OnDestroy()
    {
        MetaballSystem2D.Remove(this);
    }
}
