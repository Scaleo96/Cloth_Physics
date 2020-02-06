using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField]
    public Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void FixedUpdate()
    {
        transform.position += direction * Time.fixedDeltaTime;
    }
}
