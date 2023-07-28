using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour
{
    bool noMoreAmmo;
    GameObject Sphere;
    GameObject dude;
    [SerializeField] GunData gunDataRed;
    [SerializeField] GunData gunDataBlue;
    // Start is called before the first frame update
    void Start()
    {

        Sphere = GameObject.Find("Sphere");
        dude = GameObject.Find("dude");
        gunDataRed.totalAmmo = 0;
        gunDataRed.currentAmmo = 0;

        gunDataBlue.totalAmmo = 0;
        gunDataBlue.currentAmmo = 0;
        noMoreAmmo = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        IDamagable damageable = Sphere.GetComponent<IDamagable>();
        IDamagable damageable2 = dude.GetComponent<IDamagable>();

        GameObject ReloadArea;

        try{
            ReloadArea = GameObject.Find("AmmoArea");
        }catch (MissingReferenceException){
            noMoreAmmo = true;
        }

        if(Sphere.transform.position.y < -15){
            damageable?.TakeDamage(1000000f);
        }

        if(dude.transform.position.y < -15){
            damageable2?.TakeDamage(1000000f);
        }

        if(noMoreAmmo){
            if(gunDataRed.totalAmmo + gunDataRed.currentAmmo == 0 && gunDataBlue.totalAmmo + gunDataBlue.currentAmmo == 0){
                damageable?.TakeDamage(0.001f);
                damageable2?.TakeDamage(0.001f);
            }
        }
    }
}
