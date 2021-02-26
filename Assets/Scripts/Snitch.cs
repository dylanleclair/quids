using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snitch : MonoBehaviour
{
    public LinkedList<Vector3> dirQueue;
    private Vector3 target;
    public Vector3 dir;
    public float theta = 90;
    public float mass = 75;
    public float r = 20; 
    public float maxvelocity = 27;
    public float velocity = 20;
    public float force = 115f;
    private LineRenderer LR;


    float bufferedR;
    float bufferedTheta;

    // Start is called before the first frame update
    void Start()
    {
        LR = gameObject.AddComponent<LineRenderer>();
        LR.positionCount = 2;
        LR.startWidth = 0.5f;
        LR.endWidth = 0.5f;
        LR.startColor = Color.yellow;
        LR.endColor = Color.yellow;

        target = GenerateTarget();
        dir = Vector3.up;
    }

    // Update is called once per frame
    void Update()
    {


    }
    

    /// <summary>
    /// Physics calculations go here!
    /// </summary>
    private void FixedUpdate()
    {
        Debug.Log(Vector3.Distance(transform.position, target));
        if (! (Vector3.Distance(transform.position, target) < 8f))
        {
            // move closer to it
            dir = (target - transform.position).normalized;

            // potentially add some variance

        } else
        {
            // set a new target

            target = GenerateTarget();


        }
        

        float accel = force / mass;
        velocity += accel * Time.deltaTime;

        // Clamp velocity so that the player does not move too fast
        velocity = Mathf.Clamp(velocity, 0, 25);

        transform.position = transform.position + (dir * velocity) * Time.deltaTime;

        LR.SetPosition(0, transform.position);
        LR.SetPosition(1, dir * 4 + transform.position); 
        
    }



    public Vector3 GenerateTarget()
    {


        float tr = bufferedR + Random.Range(-100, 101) / 10;
        Debug.Log("tr: " + tr);
        float theta = bufferedTheta + 0.6f * (Random.value );


        tr = Mathf.Clamp(tr, -40,40);
        

        Vector3 t = new Vector3(tr * Mathf.Cos(theta), tr,tr * Mathf.Sin(theta));

        bufferedR = tr;
        bufferedTheta = theta;
        
        //Debug.Log("Target: " + t);
        return t;
    }

    public float AngleBetweenVectors(Vector3 a, Vector3 b)
    {
        a = a.normalized;
        b = b.normalized;
        float num = Vector3.Dot(a, b);
        float denom = a.magnitude * b.magnitude;

        return Mathf.Acos(num / denom);
    }



}
