using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpeedAuthoring : MonoBehaviour
{
    public float value;
}

public class SpeedBaker : Baker<SpeedAuthoring> {


    public override void Bake(SpeedAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity,new Speed
        {
            value = authoring.value
        });
    }
}