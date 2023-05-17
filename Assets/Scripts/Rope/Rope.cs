using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InterationMethod
{
    Euler_explicit, //显式积分
    Euler_semi_implicit, //半隐式积分
    Verlet //维特积分
}

public class Rope : MonoBehaviour
{
    public Vector3 Gravity;
    public float Springk;
    public float damping=0.001f;
    public float SpingRestLength;


    public List<Mass> Masses;
    public List<Spring> Springs;

    public InterationMethod MethodType;
    
    private void Start()
    {
        Masses = new List<Mass>();
        Springs = new List<Spring>();
        InitMass();
    }

    public void InitMass()
    {
        // 初始化质点
        Masses = new List<Mass>(this.GetComponentsInChildren<Mass>());

        Masses[0].isPined = true;

        for (int i = 1; i < Masses.Count; i++)
        {
            Spring s = new Spring();
            s.Mass1 = Masses[i - 1];
            s.Mass2 = Masses[i];
            s.KValue = Springk;
            s.RestLength =  SpingRestLength;
            s.Mass1.LastPosition = s.Mass1.Position;
            s.Mass2.LastPosition = s.Mass2.Position;
            Springs.Add(s);
        }
    }

    private void FixedUpdate()
    {
        if (MethodType == InterationMethod.Euler_explicit)
        {
            ExplicitEulerMathod(Time.fixedDeltaTime,true);
        }
        else if (MethodType == InterationMethod.Euler_semi_implicit)
        {
            ExplicitEulerMathod(Time.fixedDeltaTime,false);
        }
        else
        {
            VerletMathod(Time.fixedDeltaTime);
        }
    }

    // Euler
    // Most integral mathod have convergance feature
    // So In Explicit Euler Mathod , 
    // I give a air resistance to speed up the aconvergance
    public void ExplicitEulerMathod(float deltaTime, bool is_explicit)
    {
        foreach (var item in Springs)
        {
            // euler
            // spring force

            Vector3 spingforce = Springk * item.GetCurentLength().normalized * (item.GetCurentLength().magnitude - item.RestLength);
            item.Mass1.Force += spingforce;
            item.Mass2.Force -= spingforce;
            Debug.DrawLine(item.Mass1.Position, item.Mass2.Position);
        }

      
        foreach (var item in Masses)
        {
            if (!item.isPined)
            {
                item.Force += Gravity* item.MassValue;
               // damping = 0.01f;
                if ( is_explicit)
                {
                    damping = 7;// damping
                }
                item.Force -= item.Velocity * damping;


                Vector3 a = item.Force / item.MassValue;
                Vector3 v = item.Velocity;
                Vector3 v_nx = v + a * deltaTime;

                item.Velocity += a * deltaTime;
                if (is_explicit)
                {
                    item.Position += v * deltaTime;
                }
                else
                {
                    item.Position += v_nx * deltaTime;
                }

                
                
            }
            item.Force = Vector3.zero;
        }

        //foreach (var item in Springs)
        //{
        //    Vector3 subtract = item.Mass1.Position - item.Mass2.Position;
        //    subtract = subtract.normalized * (subtract.magnitude - item.RestLength);

        //    if (item.Mass1.isPined)
        //    {
        //        item.Mass2.Position += subtract;
        //    }
        //    else if (item.Mass2.isPined)
        //    {
        //        item.Mass1.Position -= subtract;
        //    }
        //    else
        //    {
        //        item.Mass1.Position -= subtract / 2.0f;
        //        item.Mass2.Position += subtract / 2.0f;
        //    }
        //    Debug.DrawLine(item.Mass1.Position, item.Mass2.Position);
        //}
    }

    // Verlet mathod
    // 
    public void VerletMathod(float deltaTime)
    {
        foreach (var item in Springs)
        {
            // spring force
            Vector3 springforce = Springk * item.GetCurentLength().normalized * (item.GetCurentLength().magnitude - item.RestLength);
            // 因为每个质量可能存在于多个spring中，受力就会有多份，所以要累加
            item.Mass1.Force += springforce; 
            item.Mass2.Force -= springforce;
            Debug.DrawLine(item.Mass1.Position, item.Mass2.Position);
        }

        // first , get the final position of mass
        // second , constraint the positon to the rest length

        foreach (var item in Masses)
        {
            if (!item.isPined)
            {

                item.Force += Gravity* item.MassValue;
                Vector3 a = item.Force / item.MassValue;
                Vector3 x_t0 = item.LastPosition;
                Vector3 x_t1 = item.Position;

               //  damping = 0.01f;
                Vector3 x_t2 = x_t1 + (1 - damping) * (x_t1 - x_t0) + a * deltaTime * deltaTime;

                item.LastPosition = x_t1;
                item.Position = x_t2;
            }

            //每个质量存在于多个spring中，一次计算结束后，清空受力，下一次重新计算受力情况
            item.Force = Vector3.zero;
        }



        foreach (var item in Springs)
        {
            Vector3 subtract = item.Mass1.Position - item.Mass2.Position;
            subtract = subtract.normalized * (subtract.magnitude - item.RestLength);

            if (item.Mass1.isPined)
            {
                item.Mass2.Position += subtract;
            }
            else if (item.Mass2.isPined)
            {
                item.Mass1.Position -= subtract;
            }
            else
            {
                item.Mass1.Position -= subtract / 2.0f;
                item.Mass2.Position += subtract / 2.0f;
            }
            Debug.DrawLine(item.Mass1.Position, item.Mass2.Position);
        }
    }
}
