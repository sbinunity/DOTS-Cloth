using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial struct TestSystem : ISystem
{

    private TestComponentA a;
    private TestComponentB b;

    private NativeList<TestComponentB> list;

    NativeList<Entity> entities;


    public void OnCreate(ref SystemState state)
    {

      


    }

     public void OnUpdate(ref SystemState state)
    {
        //ClothComponent spawner;
        //bool b = SystemAPI.TryGetSingleton(out spawner);
        //if (!b) return;


        //entities=new NativeList<Entity>(Allocator.Persistent);

        //var ecb = new EntityCommandBuffer(Allocator.Persistent);

        //int count = 0;

        //for (int i = 0; i < 10; i++)
        //{
        //    for (int j = 0; j < 10; j++)
        //    {
        //        var e = ecb.Instantiate(spawner.massEntity);
        //        MassComponent mass = new MassComponent
        //        {
        //            MassValue = 1,
        //        };
        //        ecb.AddComponent<MassComponent>(e, mass);
        //        entities.Add(e);
        //        count++;

        //        if (i * j == 50)
        //        {
        //            var s = new SpringComponent()
        //            {
        //                   Mass1 = entities[count - 2],
        //                   Mass2 = e,
        //                RestLength = 3
        //            };
        //            var se = ecb.CreateEntity();
        //            ecb.AddComponent<SpringComponent>(se, s);

        //        }



        //    }
        //}

        //ecb.Playback(state.EntityManager);

        //state.Enabled = false;


    }
}
