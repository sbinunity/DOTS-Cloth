using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public struct ClothComponent: IComponentData
{
    public Entity massEntity;
    public int width;
    public int height;

    public float3 Gravity ;
    public float damping ;
    public float SpringK ;

}

public class ClothAuthoring : MonoBehaviour
{
    public GameObject partical;
    public int width;
    public int height;
    public float3 Gravity= new float3(0,-9.8f,0);
    public float damping=0.01f;
    public float SpringK=30f;

    class ClothBaker : Baker<ClothAuthoring>
    {
        public override void Bake(ClothAuthoring authoring)
        {
            AddComponent( GetEntity( TransformUsageFlags.WorldSpace), new ClothComponent{
                massEntity = GetEntity( authoring.partical, TransformUsageFlags.Dynamic)
                ,width= authoring.width,
                height=authoring.height,
                 Gravity= authoring.Gravity,
                 SpringK= authoring.SpringK,
                  damping= authoring.damping,
            }) ;
        }
    }
}
