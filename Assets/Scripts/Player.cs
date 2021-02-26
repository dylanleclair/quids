
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics;


public enum Team
{
    Gryffindor,
    Slytherin
}

[AddComponentMenu("Characters/Traits")]
public class Traits
{
    public float Weight { get; set; }
    public float MaxVelocity { get; set; }
    public float Aggressiveness { get; set; }
    public float MaxExhaustion { get; set; }

    public Traits(Team t)
    {
        switch (t)
        {
            case Team.Slytherin:
                Weight = (float) GenerateGaussianValue(85, 17);
                MaxVelocity = (float) GenerateGaussianValue(16, 2);
                Aggressiveness = (float) GenerateGaussianValue(30, 7);
                MaxExhaustion = (float) GenerateGaussianValue(50, 15);
                break;
            case Team.Gryffindor:
                Weight = (float) GenerateGaussianValue(75,12);
                MaxVelocity = (float) GenerateGaussianValue(18, 2);
                Aggressiveness = (float) GenerateGaussianValue(22, 3);
                MaxExhaustion = (float) GenerateGaussianValue(65, 13);
                break;
        }

    }


    public double GenerateGaussianValue(int mean, int stddeviation)
    {
        // Generate the values according to the box-muller transform

        // generate a random value between 0 and 1
        double val = UnityEngine.Random.Range(0, 100);
        double val2 = UnityEngine.Random.Range(0, 100);
        double u1 = val / 100.00;
        double u2 = val2 / 100.00;

        // pass through the box-muller transform
        double z0 = Math.Sqrt(-2 * Math.Log(u1)) * Math.Cos(2 * (float)Math.PI * u2);

        // discard the second number lol
        //double z1 = Math.Sqrt(-2 * Math.Log(u1)) * Math.Sin(2 * (float)Math.PI * u2);

        // correct to appropriate mean and standard deviation

        z0 = (z0 * stddeviation) + mean;

        return z0;
    }


}

/// <summary>
/// Represents a player in the game of quidditch
/// </summary>
[AddComponentMenu("Characters")]
public class Player : MonoBehaviour
{

    /// <summary>
    /// After trying to wrestle with Unity's dumb forces implementation for a couple of eons
    /// I decided to go old school and implement my own. 
    /// </summary>


    public float ADJACENCY_RADIUS = 8;

    public float gravity;

    private Vector3 dir;
    public float force = 115f; // each broomstick will propel a player with the same force - accel is calculated by F/m = a (by physics laws)
    private float velocity = 0;

    private LineRenderer LR;

    private List<GameObject> others;
    private List<Vector3> adjacencyForces;

    private Rigidbody rb;
    private GameObject snitch;
    /// <summary>
    /// The player's traits
    /// </summary>
    public Traits T { get; set; }
    /// <summary>
    /// The players team (either gryffindor or slytherin)
    /// </summary>
    public Team House { get; set; }

    public Vector3 Direction { get; set; }
    private void Awake()
    {
        if (tag == "Slytherin")
        {
            House = Team.Slytherin;
        }
        else // Gryffindor
        {
            House = Team.Gryffindor;
        }

        T = new Traits(House);
        Direction = Vector3.up;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Update the generated values to be reflected by their rigidbody
        LR = GetComponent<LineRenderer>();
        LR.positionCount = 2;
        LR.material = GetComponent<Renderer>().material;
        LR.startWidth = 0.3f;
        LR.endWidth = 0.3f;
        


        rb = GetComponent<Rigidbody>();

        rb.mass = T.Weight;
        rb.useGravity = false;


        

        foreach (var s in GameObject.FindGameObjectsWithTag("Slytherin"))
        {
            if (s.name != name)
                others.Add(s);
        }
        foreach (var g in GameObject.FindGameObjectsWithTag("Gryffindor")) {
            if (g.name != name)
                others.Add(g);
        }

    }

    // Update is called once per frame
    void Update()
    {

        foreach (var item in others)
        {
            Vector3 diff = item.transform.position - transform.position;\
            if (diff.magnitude >= ADJACENCY_RADIUS)
            {
                adjacencyForces.Add(diff);
            }
            
        }

    }


    /// <summary>
    /// Physics bad boys go in here
    /// </summary>
    private void FixedUpdate()
    {
        // calculate the direction of the snitch

        snitch = GameObject.FindGameObjectWithTag("Snitch");
        Vector3 snitchDir = (snitch.transform.position - transform.position).normalized;

        // find vectors towards other players, invert them

        dir = snitchDir;

        // F = ma
        // F / m = a
        // I am not going to convert between weight and mass ;-;
        float accel = force / T.Weight;
        velocity += accel * Time.deltaTime;

        // Clamp velocity so that the player does not move too fast
        velocity = Mathf.Clamp(velocity, 0, T.MaxVelocity);

        transform.position = transform.position + (dir * velocity) * Time.deltaTime;

        // Draw the directional arrows
        LR.SetPosition(0, transform.position);
        LR.SetPosition(1, dir * 1.5f + transform.position);

    }


    

}
