
using Unity.Entities;
using Unity.Mathematics;

public struct MassComponent : IComponentData
{
    public int id;
    public float MassValue;
    public bool isPinned;

    public float3 startPosition;
    public float3 Position;
    public float3 LastPosition;
    public float3 Force;
    public float3 Velocity;
}
