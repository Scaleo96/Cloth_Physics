using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    ClothSim clothSim;

    [SerializeField] private float scrollSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float idelOffsetDistance;

    private void Update()
    {
        Vector3 direction;

        if (Input.mouseScrollDelta.magnitude > 0)
        {
            direction = clothSim.ClothAvarage - transform.position;

            if (direction == Vector3.zero)
            {
                direction = Vector3.one;
            }

            transform.position += direction.normalized * Input.mouseScrollDelta.y * scrollSpeed;
            idelOffsetDistance = Vector3.Distance(clothSim.ClothAvarage, transform.position);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.RotateAround(clothSim.ClothAvarage, transform.up, Input.GetAxis("Mouse X") * rotationSpeed);
            transform.RotateAround(clothSim.ClothAvarage, transform.right, Input.GetAxis("Mouse Y") * rotationSpeed);
        }

        transform.LookAt(clothSim.ClothAvarage);
        direction = clothSim.ClothAvarage - transform.position;
        transform.position = clothSim.ClothAvarage + direction.normalized * -idelOffsetDistance;
    }
}
