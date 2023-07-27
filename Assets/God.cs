using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour
{
    GameObject Target;
    GameObject dude;
    [SerializeField] GunData gunDataRed;
    //[SerializeField] GunData gunDataBlue;
    // Start is called before the first frame update
    void Start()
    {

        Target = GameObject.Find("Target");
        dude = GameObject.Find("dude");
        gunDataRed.totalAmmo = 61;
        gunDataRed.currentAmmo = 0;

        //gunDataBlue.totalAmmo = 61;
        //gunDataBlue.currentAmmo = 0;

        
    }

    // Update is called once per frame
    void Update()
    {
        IDamagable damageable = Target.GetComponent<IDamagable>();
        IDamagable damageable2 = dude.GetComponent<IDamagable>();

        if(gunDataRed.totalAmmo + gunDataRed.currentAmmo == 0 && gunDataBlue.totalAmmo + gunData.currentAmmo == 0){
            if(damageable?.GetHealth() == damageable2.GetHealth()){
                //Tie
            }else{
                damageable?.TakeDamage(0.001d);
                damageable2?.TakeDamage(0.001d);
            }
        }
    }
}
