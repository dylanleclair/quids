

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


    public float GenerateGaussianValue(int mean, int stddeviation)
    {
        // Generate the values according to the box-muller transform

        // generate a random value between 0 and 1
        float val = UnityEngine.Random.Range(0, 100);
        float val2 = UnityEngine.Random.Range(0, 100);
        float u1 = val / 100.00f;
        float u2 = val2 / 100.00f;

        // pass through the box-muller transform
        float z0 = Mathf.Sqrt(-2 * Mathf.Log(u1)) * Mathf.Cos(2 * Mathf.PI * u2);

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


    public bool hitGround;
    public float unconsciousTimer = 0;
    public float unconsciousTimeOut = 3.0f;

    public bool unconscious = false;

    // FOR THE FUTURE: use statemachine / enum instead of bools for unconscious/exhaustion

    private Vector3 dir;
    public float force = 115f; // each broomstick will propel a player with the same force - accel is calculated by F/m = a (by physics laws)
    private float velocity = 0;

    private Vector3 respawn;

    private System.Random random;
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

        random = new System.Random();
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

        if (House == Team.Gryffindor)
        {
            respawn = GameObject.FindGameObjectWithTag("GRespawn").transform.position;
            Debug.Log(respawn);
        } else
        {
            respawn = GameObject.FindGameObjectWithTag("SRespawn").transform.position;
        }

        

    }

    // Update is called once per frame
    void Update()
    {


    }


    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerLayer"))
        {
            GameObject g = collision.gameObject;
            Player p = g.GetComponent<Player>(); 


            Debug.Log(p == this);

            double pval1 = p.T.Aggressiveness * (random.NextDouble() * (1.2 - 0.8) +0.8) * (1 - (p.exhaustion / p.T.MaxExhaustion));
            
            // this players 
            double pval2 = T.Aggressiveness * (random.NextDouble() * (1.2 - 0.8) +0.8) * (1 - (exhaustion / T.MaxExhaustion));

            // lower value becomes unconscious

            // Same team collision
            if (p.House == House)
            {
                if (random.Next(0,100) < 5)
                {
                    if (pval2 < pval1)
                    {
                        // this player becomes unconscious
                        unconscious = true;
                    }
                    else if (pval2 > pval1)
                    {
                        // other player unconscious
                        p.unconscious = true;
                    }
                }

            } else // enemy player collision
            {
                if (pval2 < pval1)
                {
                    // this player becomes unconscious
                    unconscious = true;
                }
                else if (pval2 > pval1)
                {
                    // other player unconscious
                    p.unconscious = true;
                }
            }
            

        }


        if (collision.gameObject.layer == LayerMask.NameToLayer("WallLayer"))
        {
            hitGround = true;
            transform.position = respawn;
        }

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

        if (!exhausted && !unconscious)
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
        } else if (exhausted)
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
        } else if (unconscious)
        {
            // make it fall

            rb.useGravity = true;

            if (hitGround)
            {
                rb.useGravity = false;
                // timer 
                unconsciousTimer += Time.deltaTime;
                if (unconsciousTimer >= unconsciousTimeOut)
                {
                    unconscious = false;
                    unconsciousTimer = 0;
                    hitGround = false;
                    exhaustion = 0;
                    velocity = 2;
                    
                }

            }

            // when collides with ground, move to team start
            // put on hold for some time out
        }



        

        // Draw the directional arrows
        LR.SetPosition(0, transform.position);
        LR.SetPosition(1, dir.normalized * 1.5f + transform.position);

    }


    

}
