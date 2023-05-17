
using System;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.Search;
using UnityEngine;

[BurstCompile]
[UpdateBefore(typeof(MassSpringSystem))]
public partial struct ClothSystem : ISystem
{
    private bool _initialized;

    NativeList<Entity> entities;

    EntityQuery query;

    float t;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

        _initialized = false;
        query = state.GetEntityQuery(ComponentType.ReadOnly<MassComponent>());

    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!_initialized)
        {
            #region 初始化布料

            ClothComponent spawner;
            bool b = SystemAPI.TryGetSingleton(out spawner);
            if (!b || spawner.massEntity == Entity.Null) return;

            var ecb = new EntityCommandBuffer(Allocator.Persistent);
            entities = new NativeList<Entity>(Allocator.Persistent);

            int height = spawner.height;
            int width = spawner.width;
            int count = 0;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    var e = ecb.Instantiate(spawner.massEntity);

                    bool isPined = false;
                    if (i == 0 && j == 0 || (i == 0 && j == width - 1))
                        isPined = true;

                    MassComponent mass = new MassComponent
                    {
                        id = count,
                        MassValue = 1,
                        isPinned = isPined,
                        Position = new float3((j - width / 2) * (3 - 0), (height / 2 - i) * (3 - 0), 0),
                        LastPosition = new float3((j - width / 2) * (3 - 0), (height / 2 - i) * (3 - 0), 0)

                    };
                    ecb.AddComponent<MassComponent>(e,mass);
                    entities.Add(e);
                    count++;


                    if (j > 0)
                    {

                        var s = new SpringComponent()
                        {

                            Mass1 = entities[count - 2],
                            Mass2 = e,

                            RestLength = 3
                        };
                        var se = ecb.CreateEntity();
                        ecb.AddComponent<SpringComponent>(se,s);
                    }

                    if (j > 1)
                    {
                        var s = new SpringComponent()
                        {

                            Mass1 = entities[count - 3],
                            Mass2 = e,

                            RestLength = 6
                        };
                        var se = ecb.CreateEntity();
                        ecb.AddComponent<SpringComponent>(se,s);
                    }

                    if (i > 0)
                    {

                        int index = count - width - 1;

                        var s = new SpringComponent()
                        {

                            Mass1 = entities[index],
                            Mass2 = e,
                            RestLength = 3
                        };
                        var se = ecb.CreateEntity();
                        ecb.AddComponent<SpringComponent>(se,s);


                        //Shear Spring
                        if (j == 0)
                        {
                            index = count - width;
                            var ss = new SpringComponent()
                            {

                                Mass1 = entities[index],
                                Mass2 = e,

                                RestLength = 3 * math.sqrt(2)
                            };
                            var see = ecb.CreateEntity();
                            ecb.AddComponent<SpringComponent>(see,ss);


                        }
                        else if (j == width - 1)
                        {

                            index = count - width - 2;
                            var ss = new SpringComponent()
                            {

                                Mass1 = entities[index],
                                Mass2 = e,
                                RestLength = 3 * math.sqrt(2)
                            };
                            var see = ecb.CreateEntity();
                            ecb.AddComponent<SpringComponent>(see,ss);


                        }
                        else
                        {
                            index = count - width;

                            var ss = new SpringComponent()
                            {

                                Mass1 = entities[index],
                                Mass2 = e,
                                RestLength = 3 * math.sqrt(2)
                            };
                            var see = ecb.CreateEntity();
                            ecb.AddComponent<SpringComponent>(see,ss);



                            index = count - width - 2;
                            var ss2 = new SpringComponent()
                            {

                                Mass1 = entities[index],
                                Mass2 = e,
                                RestLength = 3 * math.sqrt(2)
                            };
                            var ssee = ecb.CreateEntity();
                            ecb.AddComponent<SpringComponent>(ssee,ss2);

                        }

                    }

                    if (i > 1)
                    {
                        int index = count - width * 2 - 1;

                        var s = new SpringComponent()
                        {

                            Mass1 = entities[index],
                            Mass2 = e,
                            RestLength = 3 * 2
                        };
                        var se = ecb.CreateEntity();
                       ecb.AddComponent<SpringComponent>(se,s);

                    }

                }
            }

            ecb.Playback(state.EntityManager);

            #region state.EntityManager.CreateEntity
            //for (int i = 0; i < height; i++)
            //{
            //    for (int j = 0; j < width; j++)
            //    {
            //        var e = state.EntityManager.Instantiate(spawner.massEntity);

            //        bool isPined = false;
            //        if (i == 0 && j == 0 || (i == 0 && j == width - 1))
            //            isPined = true;

            //        MassComponent mass = new MassComponent
            //        {
            //            id = count,
            //            MassValue = 1,
            //            isPined = isPined,
            //            Position = new float3((j - width / 2) * (3 - 0), (height / 2 - i) * (3 - 0), 0),
            //            LastPosition = new float3((j - width / 2) * (3 - 0), (height / 2 - i) * (3 - 0), 0)

            //        };
            //        state.EntityManager.AddComponent<MassComponent>(e);
            //        state.EntityManager.SetComponentData<MassComponent>(e, mass);
            //        entities.Add(e);
            //        count++;


            //        if (j > 0)
            //        {

            //            var s = new SpringComponent()
            //            {

            //                Mass1 = entities[count - 2],
            //                Mass2 = e,

            //                RestLength = 3
            //            };
            //            var se = state.EntityManager.CreateEntity();
            //            state.EntityManager.AddComponent<SpringComponent>(se);
            //            state.EntityManager.SetComponentData<SpringComponent>(se, s);
            //        }

            //        if (j > 1)
            //        {
            //            var s = new SpringComponent()
            //            {

            //                Mass1 = entities[count - 3],
            //                Mass2 = e,

            //                RestLength = 6
            //            };
            //            var se = state.EntityManager.CreateEntity();
            //            state.EntityManager.AddComponent<SpringComponent>(se);
            //            state.EntityManager.SetComponentData<SpringComponent>(se, s);
            //        }

            //        if (i > 0)
            //        {

            //            // var s = ecb.CreateEntity();
            //            int index = count - width - 1;

            //            var s = new SpringComponent()
            //            {

            //                Mass1 = entities[index],
            //                Mass2 = e,
            //                RestLength = 3
            //            };
            //            var se = state.EntityManager.CreateEntity();
            //            state.EntityManager.AddComponent<SpringComponent>(se);
            //            state.EntityManager.SetComponentData<SpringComponent>(se, s);
            //            // Structual Spring


            //            //Shear Spring
            //            if (j == 0)
            //            {
            //                index = count - width;
            //                var ss = new SpringComponent()
            //                {

            //                    Mass1 = entities[index],
            //                    Mass2 = e,

            //                    RestLength = 3 * math.sqrt(2)
            //                };
            //                var see = state.EntityManager.CreateEntity();
            //                state.EntityManager.AddComponent<SpringComponent>(see);
            //                state.EntityManager.SetComponentData<SpringComponent>(see, ss);


            //            }
            //            else if (j == width - 1)
            //            {

            //                index = count - width - 2;
            //                var ss = new SpringComponent()
            //                {

            //                    Mass1 = entities[index],
            //                    Mass2 = e,
            //                    RestLength = 3 * math.sqrt(2)
            //                };
            //                var see = state.EntityManager.CreateEntity();
            //                state.EntityManager.AddComponent<SpringComponent>(see);
            //                state.EntityManager.SetComponentData<SpringComponent>(see, ss);


            //            }
            //            else
            //            {
            //                index = count - width;

            //                var ss = new SpringComponent()
            //                {

            //                    Mass1 = entities[index],
            //                    Mass2 = e,
            //                    RestLength = 3 * math.sqrt(2)
            //                };
            //                var see = state.EntityManager.CreateEntity();
            //                state.EntityManager.AddComponent<SpringComponent>(see);
            //                state.EntityManager.SetComponentData<SpringComponent>(see, ss);



            //                index = count - width - 2;
            //                var ss2 = new SpringComponent()
            //                {

            //                    Mass1 = entities[index],
            //                    Mass2 = e,
            //                    RestLength = 3 * math.sqrt(2)
            //                };
            //                var ssee = state.EntityManager.CreateEntity();
            //                state.EntityManager.AddComponent<SpringComponent>(ssee);
            //                state.EntityManager.SetComponentData<SpringComponent>(ssee, ss2);

            //            }

            //        }

            //        if (i > 1)
            //        {
            //            int index = count - width * 2 - 1;

            //            var s = new SpringComponent()
            //            {

            //                Mass1 = entities[index],
            //                Mass2 = e,
            //                RestLength = 3 * 2
            //            };
            //            var se = state.EntityManager.CreateEntity();
            //            state.EntityManager.AddComponent<SpringComponent>(se);
            //            state.EntityManager.SetComponentData<SpringComponent>(se, s);

            //        }

            //    }
            //}

            #endregion



            foreach (var (t, m) in SystemAPI.Query<RefRW<LocalTransform>, MassComponent>())
            {
                t.ValueRW.Position = m.Position;
            }
            _initialized = true;

            #endregion
        }

        int e_count = query.CalculateEntityCount();

        //  UnityEngine.Debug.Log(e_count);

        Unity.Mathematics.Random random = new Unity.Mathematics.Random(1);
        t += SystemAPI.Time.DeltaTime;

        JobHandle handle = new WindJob()
        {
            random = random,
            time = t,
            count = e_count,
        }.ScheduleParallel(state.Dependency);


        handle.Complete();

    }
}

[BurstCompile]
public partial struct WindJob : IJobEntity
{
    public float time;
    public Unity.Mathematics.Random random;
    public int count;
    [BurstCompile]
    public void Execute(RefRW<MassComponent> mass)
    {
        if (!mass.ValueRW.isPinned)
        {
            if (mass.ValueRW.id == random.NextInt(count - count / 2, count) || mass.ValueRW.id == random.NextInt(count - count / 2, count))
            {
                mass.ValueRW.Position += new float3(0, 0, 1f) * math.max(0, math.sin(time) * random.NextFloat(0.1f, 0.3f));
                mass.ValueRW.Position += new float3(0, 0, 1f) * math.max(0, math.sin(time) * random.NextFloat(0.1f, 0.3f));
            }
        }
    }
}


