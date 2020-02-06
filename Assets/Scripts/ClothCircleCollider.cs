using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothCircleCollider : ClothCollider
{
    [SerializeField]
    public float radius;

    private void Start()
    {
        radius *= colliderSize;
    }

    public override bool CheckCollision(ClothParticle p, bool elastic, ClothSim cloth)
    {
        Vector3 dif = p.pos - transform.position;

        if (dif.magnitude < radius)
        {
            Vector3 dir = dif.normalized;
            dir *= (radius + 0.001f);

            p.pos = dir + transform.position;
            p.acceleration = Vector3.zero;
            return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
