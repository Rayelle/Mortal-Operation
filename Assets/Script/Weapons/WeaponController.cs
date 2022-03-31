using ExitGames.Client.Photon;
using NUnit.Framework;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponController : MonoBehaviour, IOnEventCallback
{
    //List of available Weapons of Player
    [SerializeField]
    List<WType> myWeapons=new List<WType>();

    //Weapons of the Player (they get activated if picked up specific Weapon)
    [SerializeField]
    private Weapon myPistol, mySMG, mySniper;

    [SerializeField]
    PhotonView myPV;

    [SerializeField]
    private AudioSource myAS;

    //Sounds of Players Weapons
    [SerializeField]
    private AudioClip pistolSound, smgSound, sniperSound;

    //Renderer of Weapons for making them invisible if not equipped and vise versa
    [SerializeField]
    private Renderer smgRenderer, sniperRenderer;

    //UI Images for Weapons
    private Image sniperI, smgI;

    //UI ammo counter for active Weapon
    private TextMeshProUGUI ammoCounter;

    private AudioClip currentSound;

    //bool to track wich weapons are available to player right now
    bool hasSniper=false, hasSMG = false;

    float offsetTimer;

    Weapon activeWeapon;

    bool hasControl = true;

    public List<WType> MyWeapons { get => myWeapons; set => myWeapons = value; }
    public AudioSource MyAS { get => myAS; set => myAS = value; }
    public Image SniperI {set => sniperI = value; }
    public Image SmgI {set => smgI = value; }
    public TextMeshProUGUI AmmoCounter {set => ammoCounter = value; }

    void Start()
    {
        //at the start the only Weapon is Pistol
        activeWeapon = myPistol;
        myAS.clip = pistolSound;

    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);

    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);

    }


    void Update()
    {
        //is the Photon View controlled by this client?
        if (myPV.IsMine)
        {
            //do i have control over my Weapons?
            if (hasControl)
            {
                //did Mouse 0 get pressed?
                if (Input.GetMouseButton(0))
                {
                    //is my current Offset not already at max?
                    if (offsetTimer < activeWeapon.MaxOffset)
                    {
                        //counts up offset if Mouse 0 is pressed
                        offsetTimer += Time.deltaTime;
                    }

                    //apllies offset 
                    activeWeapon.offset = new Vector3(Random.Range(-offsetTimer * activeWeapon.OffsetMultiplier, offsetTimer * activeWeapon.OffsetMultiplier), 0);
                    
                    //Shoots forward with apllied Offset
                    activeWeapon.Shoot();
                    
                }

                //counts down offset if Mouse 0 isnt pressed
                if (Input.GetMouseButton(0) == false)
                {
                    //is there an offset?
                    if (offsetTimer > 0)
                    {
                        //count offset down
                        offsetTimer -= Time.deltaTime * 2;
                    }

                }

                //pressed 1 on Keyboard?
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    //is Pistol inactive?
                    if (activeWeapon.MyType != WType.Pistol)
                    {
                        //activate Pistol
                        activeWeapon = myPistol;

                        //set Audio to Pistol
                        myAS.clip = pistolSound;

                        //make all Weapons but this one invisible
                        smgRenderer.enabled = false;
                        sniperRenderer.enabled = false;

                        // deactivate/activate UI Images
                        sniperI.enabled = false;
                        smgI.enabled = false;
                    }
                }

                //pressed 2 on Keyboard?
                if (Input.GetKeyDown(KeyCode.Alpha2) && hasSMG)
                {
                    //is SMG inactive?
                    if (activeWeapon.MyType != WType.SMG)
                    {
                        //activate SMG
                        activeWeapon = mySMG;

                        //set Audio to SMG
                        myAS.clip = smgSound;

                        //make all Weapons but this one invisible
                        sniperRenderer.enabled = false;
                        smgRenderer.enabled = true;

                        // deactivate/activate UI Images
                        sniperI.enabled = false;
                        smgI.enabled = true;
                    }
                }

                //pressed 3 on Keyboard?
                if (Input.GetKeyDown(KeyCode.Alpha3) && hasSniper)
                {
                    //is Sniper inactive?
                    if (activeWeapon.MyType != WType.Sniper)
                    {
                        //activate SMG
                        activeWeapon = mySniper;

                        //set Audio to SMG
                        myAS.clip = sniperSound;

                        //make all Weapons but this one invisible
                        sniperRenderer.enabled = true;
                        smgRenderer.enabled = false;

                        // deactivate/activate UI Images
                        sniperI.enabled = true;
                        smgI.enabled = false;
                    }
                }
            }
        }

        //is Pistol active?
        if(activeWeapon.MyType == WType.Pistol)
        {
            //indicate in UI that Pistol has unlimited ammo
            if(ammoCounter!=null)
                ammoCounter.text = ("∞");
        }
        //anything other active than Pistol?
        else
        {
            //show current Ammo and possible maximum Ammo
            if (ammoCounter != null)
                ammoCounter.text = ($"{activeWeapon.Ammo}/{activeWeapon.StartingAmmo}");
        }
    }

    public void PickUpSMG()
    {
        //activate SMG and set Ammo to max
        if (!hasSMG) hasSMG = true;
        mySMG.Ammo = mySMG.StartingAmmo;

    }

    public void PickUpSniper()
    {
        //activate Sniper and set ammo to max
        if (!hasSniper) hasSniper = true;
        mySniper.Ammo = mySniper.StartingAmmo;

    }

    public void ResetWeapons()
    {
        //deactivate all but pistol, reset UI and make all but Pistol invisible

        activeWeapon = myPistol;
        hasSMG = false;
        hasSniper = false;

        myAS.clip = pistolSound;

        smgRenderer.enabled = false;
        sniperRenderer.enabled = false;

        if(sniperI!=null)
            sniperI.enabled = false;
        if(smgI!=null)
            smgI.enabled = false;
    }

    public void OnEvent(EventData photonEvent)
    {
        byte raiseEventCode = photonEvent.Code;

        if (raiseEventCode == RaiseEventCodes.LooseControl && myPV.IsMine)
        {
            
            object[] data = (object[])photonEvent.CustomData;
            if ((Player)data[0] == myPV.Owner)
            {
                hasControl = false;
            }
        }

        if (raiseEventCode == RaiseEventCodes.GainControl && myPV.IsMine)
        {

            object[] data = (object[])photonEvent.CustomData;
            if ((Player)data[0] == myPV.Owner)
            {
                hasControl = true;
            }
        }
        if (raiseEventCode == RaiseEventCodes.PlayerRespawn)
        {
            object[] data = (object[])photonEvent.CustomData;
            if ((Player)data[0] == myPV.Owner)
            {
                ResetWeapons();
            }
        }
    }
}
