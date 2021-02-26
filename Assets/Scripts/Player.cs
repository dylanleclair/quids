
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Team
{
    Gryffindor,
    Slytherin,
    Empty
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

    public float speed = 3.0f;

    public float ADJACENCY_RADIUS = 8;

    public float gravity;
    
    public bool exhausted = false;
    public float exhaustion = 0.0f;
    public float exhaustTimer = 0.0f;
    public float EXHAUST_PAUSE_DURATION = 1.0f;

    private Vector3 dir;
    public float force = 115f; // each broomstick will propel a player with the same force - accel is calculated by F/m = a (by physics laws)
    private float velocity = 0;


   

    private LineRenderer LR;

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

        snitch = GameObject.FindGameObjectWithTag("Snitch");

        rb = GetComponent<Rigidbody>();

        rb.mass = T.Weight;
        rb.useGravity = false;



    }

    // Update is called once per frame
    void Update()
    {


    }


    /// <summary>
    /// Physics bad boys go in here
    /// </summary>
    private void FixedUpdate()
    {




        Vector3 interstitial = new Vector3();
        Collider[] c = Physics.OverlapSphere(transform.position, ADJACENCY_RADIUS, LayerMask.GetMask("Player"));

        foreach (var col in c)
        {
            interstitial += (transform.position - col.transform.position) * 0.3f;
        }

        // find vectors towards other players, invert them
        Vector3 snitchDir = (snitch.transform.position - transform.position).normalized;


        dir = snitchDir += interstitial;



        if (exhaustion > T.MaxExhaustion)
        {
            exhausted = true;
        }

        if (!exhausted)
        {
            // F = ma
            // F / m = a
            // I am not going to convert between weight and mass ;-;
            float accel = force / T.Weight;
            velocity += accel * Time.deltaTime;

            // Clamp velocity so that the player does not move too fast
            velocity = Mathf.Clamp(velocity, 0, T.MaxVelocity);

            transform.position = transform.position + (dir * (speed * velocity)) * Time.deltaTime;
            exhaustion += (velocity / 40);
        } else
        {
            if (exhaustTimer < EXHAUST_PAUSE_DURATION)
            {
                exhaustTimer += Time.deltaTime;
            } else
            {
                velocity = 0.0f;
                exhaustTimer = 0.0f;
                exhaustion = 0.0f;
                exhausted = false;
            }
        }



        

        // Draw the directional arrows
        LR.SetPosition(0, transform.position);
        LR.SetPosition(1, dir.normalized * 1.5f + transform.position);

    }


    

}
