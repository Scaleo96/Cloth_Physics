using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothPlaneCollider : ClothCollider
{
    public override bool CheckCollision(ClothParticle p, bool elastic, ClothSim cloth)
    {
        Vector3 dir = p.pos - transform.position;

        if (p.pos.y < transform.position.y)
        {
            p.pos = new Vector3(p.pos.x, transform.position.y, p.pos.z);
            //p.acceleration = Vector3.zero;
            return true;
        }
        return false;
    }
}
