using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MassComponentAuthoring : MonoBehaviour
{
    class MassComponentBaker : Baker<MassComponentAuthoring>
    {
        public override void Bake(MassComponentAuthoring authoring)
        {
            AddComponent(GetEntity( TransformUsageFlags.WorldSpace), new MassComponent { MassValue = 1, isPinned = false });
        }
    }
}
