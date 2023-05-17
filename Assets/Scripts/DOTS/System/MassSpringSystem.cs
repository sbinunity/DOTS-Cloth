
using System;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Core;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.VersionControl;
using UnityEngine;


[BurstCompile]
[UpdateAfter(typeof(ClothSystem))]
public partial struct MassSpringSystem : ISystem
{
  
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        ClothComponent cloth;
        if (!SystemAPI.TryGetSingleton<ClothComponent>(out cloth)) return;
        


        foreach (var item in SystemAPI.Query<SpringComponent>())
        {


                RefRW<MassComponent> mass1 = SystemAPI.GetComponentRW<MassComponent>(item.Mass1, false);
                RefRW<MassComponent> mass2 = SystemAPI.GetComponentRW<MassComponent>(item.Mass2, false);

                float3 d = mass2.ValueRW.Position - mass1.ValueRW.Position;
                // spring force
                float3 springforce = cloth.SpringK * math.normalize(d) * (math.length(d) - item.RestLength);
                // 因为每个质量可能存在于多个spring中，受力就会有多份，所以要累加
                mass1.ValueRW.Force += springforce;
                mass2.ValueRW.Force -= springforce;
                UnityEngine.Debug.DrawLine(mass1.ValueRW.Position, mass2.ValueRW.Position, UnityEngine.Color.green);


                float3 subtract = mass1.ValueRW.Position - mass2.ValueRW.Position;
                subtract = math.normalize(subtract) * (math.length(subtract) - item.RestLength);

                if (mass1.ValueRW.isPinned)
                {
                    mass2.ValueRW.Position += subtract;
                }
                else if (mass2.ValueRW.isPinned)
                {
                    mass1.ValueRW.Position -= subtract;
                }
                else
                {
                    mass1.ValueRW.Position -= subtract / 2.0f;
                    mass2.ValueRW.Position += subtract / 2.0f;
                }

            }

            // first , get the final position of mass
            // second , constraint the positon to the rest length

            new MassJob()
            {
                damping = cloth.damping,
                Gravity = cloth. Gravity,
                deltaTime = SystemAPI.Time.fixedDeltaTime
            }.ScheduleParallel(state.Dependency).Complete();

    }
}


[BurstCompile]
public partial struct MassJob : IJobEntity
{

    public float3 Gravity;
    public float damping;
    public float deltaTime;
    [BurstCompile]
    public void Execute(RefRW<LocalTransform> t, RefRW<MassComponent> item)
    {
        if (!item.ValueRW.isPinned)
        {
            //  UnityEngine.Debug.LogWarning(item.ValueRW.MassValue+"    "+ Gravity+"       "+ SpringK);

            item.ValueRW.Force += Gravity * item.ValueRW.MassValue;
            float3 a = item.ValueRW.Force / item.ValueRW.MassValue;
            float3 x_t0 = item.ValueRW.LastPosition;
            float3 x_t1 = item.ValueRW.Position;

            //  damping = 0.01f;
            float3 x_t2 = x_t1 + (1 - damping) * (x_t1 - x_t0) + a * deltaTime * deltaTime;

            item.ValueRW.LastPosition = x_t1;
            item.ValueRW.Position = x_t2;

            t.ValueRW.Position = item.ValueRW.Position;

        }

        //每个质量存在于多个spring中，一次计算结束后，清空受力，下一次重新计算受力情况
        item.ValueRW.Force = float3.zero;
    }
}


public partial struct SpringJob : IJobEntity
{

    public void Execute(SpringComponent item)
    {

                //RefRW<MassComponent> mass1 = SystemAPI.GetComponentRW<MassComponent>(item.Mass1, false);
                //RefRW<MassComponent> mass2 = SystemAPI.GetComponentRW<MassComponent>(item.Mass2, false);

                //float3 d = mass2.ValueRW.Position - mass1.ValueRW.Position;
                //// spring force
                //float3 springforce = SpringK * math.normalize(d) * (math.length(d) - item.RestLength);
                //// 因为每个质量可能存在于多个spring中，受力就会有多份，所以要累加
                //mass1.ValueRW.Force += springforce;
                //mass2.ValueRW.Force -= springforce;
                //UnityEngine.Debug.DrawLine(mass1.ValueRW.Position, mass2.ValueRW.Position, UnityEngine.Color.green);


                //float3 subtract = mass1.ValueRW.Position - mass2.ValueRW.Position;
                //subtract = math.normalize(subtract) * (math.length(subtract) - item.RestLength);

                //if (mass1.ValueRW.isPined)
                //{
                //    mass2.ValueRW.Position += subtract;
                //}
                //else if (mass2.ValueRW.isPined)
                //{
                //    mass1.ValueRW.Position -= subtract;
                //}
                //else
                //{
                //    mass1.ValueRW.Position -= subtract / 2.0f;
                //    mass2.ValueRW.Position += subtract / 2.0f;
                //}

    }
}