using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothBoxCollider : ClothCollider
{
    [SerializeField]
    Vector3 boundingBox;

    public override bool CheckCollision(ClothParticle p, bool elastic, ClothSim cloth)
    {
        Vector3 dir = p.pos - transform.position;

        if (p.pos.x < transform.position.x + (boundingBox.x / 2) &&
            p.pos.x > transform.position.x - (boundingBox.x / 2) &&
            p.pos.y < transform.position.y + (boundingBox.y / 2) &&
            p.pos.y > transform.position.y - (boundingBox.y / 2) &&
            p.pos.z < transform.position.z + (boundingBox.z / 2) &&
            p.pos.z > transform.position.z - (boundingBox.z / 2))
        {
            //dir.Normalize();
            p.acceleration = Vector3.zero;
            return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, boundingBox);
    }
}
