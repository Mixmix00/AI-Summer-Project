using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class reloadArea : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GunData gunDataRed;
    [SerializeField] GunData gunDataBlue;
    [SerializeField] float timedAmmoSpeed;

    private DateTime lastReloadRed;
    private DateTime lastReloadBlue;
    private DateTime timedAmmo;

    private int ammoRemaining;

    GameObject Red;
    GameObject Blue;
    //private GameObject Blue;
    void Start()
    {
        Red = GameObject.Find("dude");
        Blue = GameObject.Find("Sphere");
        lastReloadRed = DateTime.Now;
        lastReloadBlue = DateTime.Now;
        timedAmmo = DateTime.Now;

        ammoRemaining = 30;
        
    }

    private void run()
    {
        Debug.Log("ammoRemaining: " + ammoRemaining);
        if((DateTime.Now - timedAmmo).TotalSeconds > timedAmmoSpeed)
        {
            timedAmmo = DateTime.Now;
            ammoRemaining++;
        }
        //Debug.Log("reloadArea: " + Vector3.Distance(Red.transform.position, transform.position));
        if (Vector3.Distance(Red.transform.position, transform.position) < 50f && (DateTime.Now - lastReloadRed).TotalSeconds > 0.5f)
        {
            if(ammoRemaining > 0)
            {
                gunDataRed.totalAmmo++;
                ammoRemaining--;
                lastReloadRed = DateTime.Now;
            }
            
        }

        if(Vector3.Distance(Blue.transform.position, transform.position) < 50f && (DateTime.Now - lastReloadBlue).TotalSeconds > 0.5f)
        {
            gunDataBlue.totalAmmo++;
            ammoRemaining--;
            lastReloadBlue = DateTime.Now;
        }
    }

    // Update is called once per frame
    void Update()
    {
        run();
    }
}
