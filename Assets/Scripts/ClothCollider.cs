using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClothCollider : MonoBehaviour
{
    public float colliderSize = 1;

    public abstract bool CheckCollision(ClothParticle p, bool elastic, ClothSim cloth);
}
