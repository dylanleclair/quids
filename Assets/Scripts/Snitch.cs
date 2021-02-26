using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snitch : MonoBehaviour
{

    private Rigidbody rb;
    private float movementX;
    private float movementY;
    private LineRenderer LR;
    private bool init = true;
    private float timer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        LR = gameObject.AddComponent<LineRenderer>();
        LR.positionCount = 2;
        LR.startWidth = 0.5f;
        LR.endWidth = 0.5f;
        LR.startColor = Color.yellow;
        LR.endColor = Color.yellow;

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


        timer += Time.deltaTime;

        if (timer >= 2.0f || init)
        {
            if (init)
            {
                init = false;
            }
            float x = (UnityEngine.Random.value - 0.5f);
            float y = (UnityEngine.Random.value - 0.5f);
            float z = (UnityEngine.Random.value - 0.5f);

            Vector3 randomForce = new Vector3(x, y, z);


            Vector3 toCenter = 0.5f * (new Vector3(0.0f, 25.0f, 0.0f) - transform.position).normalized + 0.5f * (randomForce).normalized;

            Debug.Log(toCenter);

            rb.AddForce(600 * toCenter);

            /// TODO 
            // calculate some small offset of the previous direction instead of having it be truly random

            timer = 0.0f;
        }



        LR.SetPosition(0, transform.position);
        LR.SetPosition(1, transform.forward * rb.velocity.magnitude + transform.position); 
        
    }

}
