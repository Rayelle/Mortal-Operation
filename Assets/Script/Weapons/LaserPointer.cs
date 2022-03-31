using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour
{
    private LineRenderer myLR;

    [SerializeField]
    public LayerMask laserCollideable ;

    [SerializeField]
    float length;
    void Start()
    {
        //setting refrence to Line Renderer
        myLR = GetComponent<LineRenderer>();

    }

   
    void Update()
    {
        //setting start Position of Line Renderer
        myLR.SetPosition(0, transform.position);

        RaycastHit hit;

        //Raycast forward, ignore "ignoreLaserLayer", get hit
        if(Physics.Raycast(transform.position,transform.forward,out hit,length, laserCollideable))
        {
            if (hit.collider)
            {
                //if something is hit end the Line Renderer there (simulates Laser hitting Objects)
                myLR.SetPosition(1, hit.point);
            }
        }
        else
        {  
            //if nothing is hit end Line Renderer at set length
            myLR.SetPosition(1, this.transform.position+transform.forward*length);
        }
    }
}
