using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct TestComponentA 
{
    public TestComponentB value_b;
    public NativeReference<TestComponentB> ref_b;
    public Entity entity_b;
    public RefRW<TestComponentB> rwB;
}
