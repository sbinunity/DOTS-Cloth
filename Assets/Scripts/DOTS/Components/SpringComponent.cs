
using Unity.Entities;
using Unity.Mathematics;

public struct SpringComponent :IComponentData
{
    //public RefRW<MassComponent> Mass1;
    //public RefRW<MassComponent> Mass2;

    public Entity Mass1;
    public Entity Mass2;

    //public int id_Mass1;
    //public int id_Mass2;

    public float KValue;
    public float RestLength;

    //public float3 GetCurrentLength()
    //{

    //    return Mass2.ValueRW.Position - Mass1.ValueRW.Position;

    //    //return Mass2.Position - Mass1.Position;
    //}

}
