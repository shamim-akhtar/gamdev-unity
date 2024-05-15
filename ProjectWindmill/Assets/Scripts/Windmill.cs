using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Rendering;

public class Windmill : MonoBehaviour
{
    public Transform rotor;
    public float rpm1 = 10;
    public float rpm2 = 25;
    public float rpm3 = 50;

    private float rpm = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // We shall use the keys A, B, C and D to toggle between
        // rpm1, rpm2, rpm3 and 0.
        if(Input.GetKeyDown(KeyCode.A))
        {
            rpm = rpm1;
        }
        if(Input.GetKeyDown(KeyCode.B))
        {
            rpm = rpm2;
        }
        if(Input.GetKeyDown(KeyCode.C))
        { 
            rpm = rpm3; 
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            rpm = 0;
        }

        Rotate();
    }

    void Rotate()
    {
        float degreesPerSecond = rpm * 360 / 60.0f;
        float degreesPerFrame = degreesPerSecond * Time.deltaTime;

        rotor.Rotate(0.0f, 0.0f, degreesPerFrame);
    }
}
