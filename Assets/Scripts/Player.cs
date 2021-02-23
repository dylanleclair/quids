
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Team
{
    Gryffindor,
    Slytherin
}
public class Traits
{
    int Weight;
    int MaxVelocity;
    int Aggressiveness;
    int MaxExhaustion;

    public Traits(Team t)
    {
        switch (t)
        {
            case Team.Gryffindor:
                break;
            case Team.Slytherin:
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
public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
