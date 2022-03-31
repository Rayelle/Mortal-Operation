using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [SerializeField]
    PhotonView myPV;


    // Update is called once per frame
    void Update()
    {
        //only control your own player
        if (myPV.IsMine)
        {
            //rotate player towards mousePosition
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(mouseRay, out hit, 1000f, 1 << 9);
            Vector3 lookPos = hit.point;

            //only ever change y-rotation
            lookPos.y = this.transform.position.y;
            this.transform.LookAt(lookPos);
        }
        
    }
}
