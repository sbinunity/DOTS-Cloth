using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MassSpring : MonoBehaviour
{
    public bool euler = true;
    public Transform fixedTransform;

    public float mass = 1; //kg
    public float k = 1;//弹性系数
    public float damping = 1;// 阻尼
    public float gravity = 9.8f;
    public float length = 5f;
    private Vector3 x_last;
    private Vector3 v;
    // Start is called before the first frame update
    void Start()
    {
       x_last= transform.position;
        v = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 deltaPos = fixedTransform.position - transform.position;
        Vector3 deltaDir = deltaPos.normalized;
        float dx = deltaPos.magnitude - length;

        Vector3 F = k * deltaDir * dx + Vector3.down * gravity * mass;
        Vector3 a = F / mass;

        if (euler)
        {
            //=================== euler integration (欧拉积分 半隐式法)===========
            v += a * Time.deltaTime;
            v = v * (1 - damping);

            Vector3 pos = transform.position + v * Time.deltaTime;
            transform.position = pos;
        }
        else
        {    //===================== verlet integation (维特积分)==================
            // x(i+1)=x(i)+ (1-damping)( x(i)-x(i-1) )+ dt^2 * a
            Vector3 pos = (transform.position - x_last) * (1-damping) + Time.deltaTime * Time.deltaTime * a;
            x_last = transform.position;
            transform.position += pos;
        }
       

    }
}
