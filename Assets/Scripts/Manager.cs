using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField]
    ClothSim clothSim;

    [SerializeField]
    GameObject[] prefabs;

    [SerializeField]
    SpriteRenderer spawnPosIcon;

    private GameObject activeGO;

    [SerializeField]
    Vector3 prefabSpawnOffset = Vector3.zero;

    [SerializeField]
    float objectSpeed = 1;

    [SerializeField]
    float objectSize = 0.5f;

    public float ObjectSpeed { get { return objectSpeed; } set { objectSpeed = value; } }
    public float ObjectSize { get { return objectSize; } set { objectSize = value; } }
    public Vector3 PrefabSpawnOffset { get { return prefabSpawnOffset; } set { prefabSpawnOffset = value; } }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (activeGO == null)
            {
                activeGO = Instantiate(prefabs[0], clothSim.ClothAvarage + prefabSpawnOffset, prefabs[0].transform.rotation);
                activeGO.transform.localScale = Vector3.one * objectSize;
                activeGO.GetComponent<ClothCollider>().colliderSize = objectSize;
                activeGO.GetComponent<Move>().direction = -prefabSpawnOffset.normalized * objectSpeed;

                clothSim.colliders.Add(activeGO.GetComponent<ClothCollider>());
            }
            else
            {
                clothSim.colliders.Remove(activeGO.GetComponent<ClothCollider>());
                Destroy(activeGO);
                activeGO = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            clothSim.Reset();
        }
    }

    private void FixedUpdate()
    {
        spawnPosIcon.transform.position = clothSim.ClothAvarage + prefabSpawnOffset;
        spawnPosIcon.transform.right = (spawnPosIcon.transform.position - clothSim.ClothAvarage);
    }
}
