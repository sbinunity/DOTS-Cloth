using System;
using System.Collections.Generic;

using UnityEngine;

public class MyCloth : MonoBehaviour
{
    public Vector3 Gravity= Vector3.up*-9.8f;
    public float damping = 0.001f;
    public float SpringK = 30f;

    public float restLength = 3f;

    public int width;
    public int height;

    private List<Spring> structuralSprings=new List<Spring>();
    private List<Spring> shearSpring=new List<Spring>();
    private List<Spring> bendSpring=new List<Spring>();


    private List<Spring> Springs=new List<Spring>();
    private List<Mass> masses=new List<Mass>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                var partical = GameObject.CreatePrimitive( PrimitiveType.Sphere);
                partical.transform.parent = transform;
                partical.transform.position = new Vector3((j - width / 2)* (restLength-0), (height/2-i )* (restLength-0), 0);
                var mass= partical.AddComponent<Mass>();
                masses.Add(mass);
                

                if(j>0)
                {
                    var s = new Spring();
                    s.Mass1= masses[masses.Count-2];
                    s.Mass2 = mass;
                    s.RestLength= restLength;
                    s.Mass1.LastPosition = s.Mass1.Position;
                    s.Mass2.LastPosition= s.Mass2.Position;
                    Springs.Add(s);
                }

                // bending spring
                if(j>1)
                {
                    var s = new Spring();
                    s.Mass1 = masses[masses.Count - 3];
                    s.Mass2 = mass;
                    s.RestLength = restLength*2;
                    s.Mass1.LastPosition = s.Mass1.Position;
                    s.Mass2.LastPosition = s.Mass2.Position;
                    Springs.Add(s);
                }

                if(i>0)
                {
                    // Structual Spring
                    var s = new Spring();
                    s.Mass1 = masses[masses.Count - width-1];
                    s.Mass2 = mass;
                    s.RestLength = restLength;
                    s.Mass1.LastPosition = s.Mass1.Position;
                    s.Mass2.LastPosition = s.Mass2.Position;
                    Springs.Add(s);

                    //Shear Spring
                    if (j == 0)
                    {
                        var ss = new Spring();
                        int index = masses.Count - width ;
                        ss.Mass1 = masses[index];
                        ss.Mass2 = mass;
                        ss.RestLength = restLength * Mathf.Sqrt(2);
                        ss.Mass1.LastPosition = ss.Mass1.Position;
                        ss.Mass2.LastPosition = ss.Mass2.Position;
                        Springs.Add(ss);
                    }
                    else if(j==width-1)
                    {
                        var ss = new Spring();
                        int index = masses.Count - width-2;
                        ss.Mass1 = masses[index];
                        ss.Mass2 = mass;
                        ss.RestLength = restLength* Mathf.Sqrt(2);
                        ss.Mass1.LastPosition = ss.Mass1.Position;
                        ss.Mass2.LastPosition = ss.Mass2.Position;
                        Springs.Add(ss);
                    }
                    else
                    {
                        var ss = new Spring();
                        int index = masses.Count - width;
                        ss.Mass1 = masses[index];
                        ss.Mass2 = mass;
                        ss.RestLength = restLength * Mathf.Sqrt(2);
                        ss.Mass1.LastPosition = ss.Mass1.Position;
                        ss.Mass2.LastPosition = ss.Mass2.Position;
                        Springs.Add(ss);


                         ss = new Spring();
                         index = masses.Count - width - 2;
                        ss.Mass1 = masses[index];
                        ss.Mass2 = mass;
                        ss.RestLength = restLength * Mathf.Sqrt(2);
                        ss.Mass1.LastPosition = ss.Mass1.Position;
                        ss.Mass2.LastPosition = ss.Mass2.Position;
                        Springs.Add(ss);
                    }

                }

                if(i>1)
                {
                    var s = new Spring();
                    s.Mass1 = masses[masses.Count - width* 2-1];
                    s.Mass2 = mass;
                    s.RestLength = restLength*2;
                    s.Mass1.LastPosition = s.Mass1.Position;
                    s.Mass2.LastPosition = s.Mass2.Position;
                    Springs.Add(s);
                }
            }
        }

        masses[0].isPined= true;
        masses[width-1].isPined= true;

       
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //if(Input.GetKeyDown(KeyCode.Space))
         VerletMathod(Time.fixedDeltaTime);


        masses[UnityEngine.Random.Range(masses.Count-masses.Count/width*3, masses.Count )].Position += Vector3.forward * MathF.Max(0, MathF.Sin(Time.time) * UnityEngine.Random.Range(0.1f, 0.3f));
        masses[UnityEngine.Random.Range(masses.Count - masses.Count / width * 3, masses.Count)].Position += Vector3.forward * MathF.Max(0, MathF.Sin(Time.time) * UnityEngine.Random.Range(0.1f, 0.3f));

    }

    // Verlet mathod
    // 
    public void VerletMathod(float deltaTime)
    {
        //foreach (var item in structuralSprings)
        //{
        //    // spring force
        //    Vector3 springforce = SpringK * item.GetCurentLength().normalized * (item.GetCurentLength().magnitude - item.RestLength);
        //    // 因为每个质量可能存在于多个spring中，受力就会有多份，所以要累加
        //    item.Mass1.Force += springforce;
        //    item.Mass2.Force -= springforce;
        //    Debug.DrawLine(item.Mass1.Position, item.Mass2.Position,Color.red);
        //}


        foreach (var item in Springs)
        {
            // spring force
            Vector3 springforce = SpringK * item.GetCurentLength().normalized * (item.GetCurentLength().magnitude - item.RestLength);
            // 因为每个质量可能存在于多个spring中，受力就会有多份，所以要累加
            item.Mass1.Force += springforce;
            item.Mass2.Force -= springforce;
            Debug.DrawLine(item.Mass1.Position, item.Mass2.Position,Color.green);
        }

        // first , get the final position of mass
        // second , constraint the positon to the rest length

        foreach (var item in masses)
        {
            if (!item.isPined)
            {

                item.Force += Gravity * item.MassValue;
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
           // Debug.DrawLine(item.Mass1.Position, item.Mass2.Position);
        }

        //foreach (var item in shearSpring)
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
        //   // Debug.DrawLine(item.Mass1.Position, item.Mass2.Position);
        //}
    }
}
