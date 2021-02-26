using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Material slyth;
    public Material gryff;

    public int PlayersOnEachTeam = 10;

    public List<GameObject> Gryffindor { get; set; }
    public List<GameObject> Slytherin { get; set; }

    public int WorldSize { get; set; }


    // create an adjacency matrix of each players distance from one another


    void Awake()
    {

        WorldSize = 50;

        int playerLayer = LayerMask.GetMask("Player");

        // Generate all the Gryffindor players
        for (int i = 1; i <= PlayersOnEachTeam; i++)
        {
            GameObject G = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            G.name = "Gryffindor " + i;
            G.tag = "Gryffindor";
            G.GetComponent<Renderer>().material = gryff;
            G.AddComponent<Rigidbody>();
            G.AddComponent<LineRenderer>();
            G.layer = playerLayer;


            // add player component
            G.AddComponent<Player>();
            float x, y, z;
            x = (float) UnityEngine.Random.Range(1, WorldSize);
            y = (float)UnityEngine.Random.Range(1, WorldSize);
            z = (float)UnityEngine.Random.Range(1, WorldSize);
            G.transform.position = new Vector3(x, y, z);
            
        }

        // Generate all the Slytherin Players
        for (int i = 1; i <= PlayersOnEachTeam; i++)
        {
            GameObject G = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            G.name = "Slytherin " + i;
            G.tag = "Slytherin";
            G.GetComponent<Renderer>().material = slyth;
            G.AddComponent<Rigidbody>();
            G.AddComponent<LineRenderer>();
            G.layer = playerLayer;
            // add the slytherin 
            G.AddComponent<Player>();

            float x, y, z;
            x = (float)UnityEngine.Random.Range(-45, 45);
            y = (float)UnityEngine.Random.Range(1, 45);
            z = (float)UnityEngine.Random.Range(-45, 45);
            G.transform.position = new Vector3(x, y, z);
        }


        





    }

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
