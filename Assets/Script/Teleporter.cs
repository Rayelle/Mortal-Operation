using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private Transform otherTeleporter;

    [SerializeField] private float teleportCooldown, timeSinceTP;

    private bool teleportUsed = false,timerStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(timerStarted)
        {
            timeSinceTP -= 1 * Time.deltaTime;
            if (timeSinceTP <= 0)
            {
                timerStarted = false;
                timeSinceTP = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!timerStarted && other.gameObject.tag == "Player")
        {
            timerStarted = true;
            timeSinceTP = teleportCooldown;
            other.transform.position = otherTeleporter.transform.position;
            teleportUsed = true;
        }
    }
}
