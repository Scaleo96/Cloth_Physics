using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    GameObject menu;

    [SerializeField]
    ClothSim clothSim;
    [SerializeField]
    Manager manager;

    [SerializeField]
    Slider dampSlider;

    [SerializeField]
    Text gravityX;
    [SerializeField]
    Text gravityY;
    [SerializeField]
    Text gravityZ;

    [SerializeField]
    Text windX;
    [SerializeField]
    Text windY;
    [SerializeField]
    Text windZ;

    [SerializeField]
    Text spawnOffsetX;
    [SerializeField]
    Text spawnOffsetY;
    [SerializeField]
    Text spawnOffsetZ;

    [SerializeField]
    Text colums;

    [SerializeField]
    Text radius;

    [SerializeField]
    Text tension;
    [SerializeField]
    Text updates;

    [SerializeField]
    Text mass;

    [SerializeField]
    Text damp;

    [SerializeField]
    Text objectSpeed;

    [SerializeField]
    Text objectSize;

    private void Start()
    {
        dampSlider.value = clothSim.Damp;

        gravityX.text = clothSim.Gravity.x.ToString();
        gravityY.text = clothSim.Gravity.y.ToString();
        gravityZ.text = clothSim.Gravity.z.ToString();

        colums.text = clothSim.GridSize.ToString();

        radius.text = clothSim.Radius.ToString();

        tension.text = clothSim.Tension.ToString();
        updates.text = clothSim.ConstraintUpdate.ToString();

        mass.text = clothSim.ParticleMass.ToString();

        damp.text = "(" + clothSim.Damp.ToString() + ")";

        windX.text = "(" + clothSim.Wind.x.ToString() + ")";
        windY.text = "(" + clothSim.Wind.y.ToString() + ")";
        windZ.text = "(" + clothSim.Wind.z.ToString() + ")";

        objectSpeed.text = "(" + manager.ObjectSpeed.ToString() + ")";
        objectSize.text = "(" + manager.ObjectSize.ToString() + ")";

        spawnOffsetX.text = "(" + manager.PrefabSpawnOffset.x.ToString() + ")";
        spawnOffsetY.text = "(" + manager.PrefabSpawnOffset.y.ToString() + ")";
        spawnOffsetZ.text = "(" + manager.PrefabSpawnOffset.z.ToString() + ")";
    }

    public void ColumsChange(InputField inputField)
    {
        int i;
        int.TryParse(inputField.text, out i);
        clothSim.GridSize = i;
    }

    public void RadiusChange(InputField inputField)
    {
        float i;
        if (float.TryParse(inputField.text, out i))
            clothSim.Radius = i;
    }

    public void GravityXChange(InputField inputField)
    {
        float i;
        if (float.TryParse(inputField.text, out i))
            clothSim.Gravity = new Vector3(i, clothSim.Gravity.y, clothSim.Gravity.z);
    }

    public void GravityYChange(InputField inputField)
    {
        float i;
        if (float.TryParse(inputField.text, out i))
            clothSim.Gravity = new Vector3(clothSim.Gravity.x, i, clothSim.Gravity.z);
    }

    public void GravityZChange(InputField inputField)
    {
        float i;
        if (float.TryParse(inputField.text, out i))
            clothSim.Gravity = new Vector3(clothSim.Gravity.x, clothSim.Gravity.y, i);
    }

    public void TensionChange(InputField inputField)
    {
        float i;
        if (float.TryParse(inputField.text, out i))
            clothSim.Tension = i;
    }

    public void UpdateChange(InputField inputField)
    {
        clothSim.ConstraintUpdate = int.Parse(inputField.text);
    }

    public void MassChange(InputField inputField)
    {
        float i;
        if (float.TryParse(inputField.text, out i))
            clothSim.ParticleMass = i;
    }

    public void DampChange(Slider slider)
    {
        clothSim.Damp = slider.value;
        damp.text = "(" + clothSim.Damp.ToString() + ")";
    }

    public void SheerChange()
    {
        clothSim.SheerGrid = !clothSim.SheerGrid;
    }

    public void BendChange()
    {
        clothSim.BendGrid = !clothSim.BendGrid;
    }

    public void CollisionChange()
    {
        clothSim.ConstraintOnlyCollision = !clothSim.ConstraintOnlyCollision;
    }

    public void ConnectionChange(Dropdown dropdown)
    {
        if (dropdown.value == 0)
        {
            clothSim.ClothTypes = ClothSim.ClothType.spring;
        }
        else
        {
            clothSim.ClothTypes = ClothSim.ClothType.constraint;
        }

        clothSim.Reset();
    }

    public void ChangeSpeed(InputField inputField)
    {
        float i;
        if (float.TryParse(inputField.text, out i))
            manager.ObjectSpeed = i;
        objectSpeed.text = "(" + manager.ObjectSpeed.ToString() + ")";
    }

    public void ChangeSize(InputField inputField)
    {
        float i;
        if (float.TryParse(inputField.text, out i))
            manager.ObjectSize = i;
        objectSize.text = "(" + manager.ObjectSize.ToString() + ")";
    }

    public void SpawnOffsetXChange(InputField inputField)
    {
        manager.PrefabSpawnOffset = new Vector3(float.Parse(inputField.text), manager.PrefabSpawnOffset.y, manager.PrefabSpawnOffset.z);
        spawnOffsetX.text = "(" + manager.PrefabSpawnOffset.x.ToString() + ")";
    }

    public void SpawnOffsetYChange(InputField inputField)
    {
        float i;
        if (float.TryParse(inputField.text, out i))
            manager.PrefabSpawnOffset = new Vector3(manager.PrefabSpawnOffset.x, i, manager.PrefabSpawnOffset.z);
        spawnOffsetY.text = "(" + manager.PrefabSpawnOffset.y.ToString() + ")";
    }

    public void SpawnOffsetZChange(InputField inputField)
    {
        float i;
        if (float.TryParse(inputField.text, out i))
            manager.PrefabSpawnOffset = new Vector3(manager.PrefabSpawnOffset.x, manager.PrefabSpawnOffset.y, i);
        spawnOffsetZ.text = "(" + manager.PrefabSpawnOffset.z.ToString() + ")";
    }

    public void WindXChange(InputField inputField)
    {
        float i;
        if (float.TryParse(inputField.text, out i))
            clothSim.Wind = new Vector3(i, clothSim.Wind.y, clothSim.Wind.z);
        windX.text = "(" + clothSim.Wind.x.ToString() + ")";
    }

    public void WindYChange(InputField inputField)
    {
        float i;
        if (float.TryParse(inputField.text, out i))
            clothSim.Wind = new Vector3(clothSim.Wind.x, i, clothSim.Wind.z);
        windY.text = "(" + clothSim.Wind.y.ToString() + ")";
    }

    public void WindZChange(InputField inputField)
    {
        float i;
        if (float.TryParse(inputField.text, out i))
            clothSim.Wind = new Vector3(clothSim.Wind.x, clothSim.Wind.y, i);
        windZ.text = "(" + clothSim.Wind.z.ToString() + ")";
    }

    public void ShowParticlesChange(Toggle toggle)
    {
        clothSim.SheerGrid = !clothSim.SheerGrid;
    }

    public void ToggleMenu()
    {
        menu.SetActive(!menu.activeInHierarchy);
    }
}
