using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public Material slyth;
    public Material gryff;

    public Snitch snitch;

    public Text UILabel;

    public Team lastTeamToScore = Team.Empty;

    public int gryffScore;
    public int slythScore;

    public int PlayersOnEachTeam = 10;

    public int winningScore = 50;
    public int WorldSize { get; set; }


    // create an adjacency matrix of each players distance from one another


    void Awake()
    {

        WorldSize = 50;

        int playerLayer = LayerMask.NameToLayer("PlayerLayer");

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
        snitch = GameObject.FindGameObjectWithTag("Snitch").GetComponent<Snitch>();
        UILabel = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        // Update the UI

         

        if (slythScore >= winningScore)
        {
            UILabel.text = $"Slytherin wins with {slythScore} points!";
        }
        else if (gryffScore >= winningScore)
        {
            UILabel.text = $"Gryffindor wins with {gryffScore} points!";
        }
        else UILabel.text = $"Gryffindor: {gryffScore} - Slytherin: {slythScore}";



        if (snitch.caught)
        {
            snitch.caught = false;

            if (snitch.pointForGryff)
            {
                snitch.pointForGryff = false;

                if (lastTeamToScore == Team.Gryffindor)
                {
                    gryffScore += 2;
                }
                else gryffScore++;

                lastTeamToScore = Team.Gryffindor;
            }
            else if (snitch.pointForSlyth)
            {
                
                snitch.pointForSlyth = false;

                if (lastTeamToScore == Team.Slytherin)
                {
                    slythScore += 2;
                } else slythScore++;

                lastTeamToScore = Team.Slytherin;

            }


            // alter position to new random location

            //generate a random position in play area
            float x = Random.Range(-45, 45);
            float y = Random.Range(-45, 45);
            float z = Random.Range(-45, 45);

            Vector3 newSnitchPos = new Vector3(x, y, z);
            snitch.transform.position = newSnitchPos;

        }


    }

    private void FixedUpdate()
    {

    }

}
