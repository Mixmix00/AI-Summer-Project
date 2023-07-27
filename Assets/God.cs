using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour
{
    [SerializeField] GunData gunDataRed;
    //[SerializeField] GunData gunDataBlue;
    // Start is called before the first frame update
    void Start()
    {
        gunDataRed.totalAmmo = 61;
        gunDataRed.currentAmmo = 0;

        //gunDataBlue.totalAmmo = 61;
        //gunDataBlue.currentAmmo = 0;

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
