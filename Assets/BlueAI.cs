using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BlueAI : MonoBehaviour
{

    
    [SerializeField] GunData gunData;

    DateTime lastReload;
    
    GameObject dude;
    GameObject Sphere;
    GameObject ReloadArea;

    public static Action reload;
    public static Action shoot;

    public LayerMask whatIsGround, whatIsPlayer;

    public Vector3 walkPoint;
    bool walkPointSet;
    float walkPointRange;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    
    private bool twoAreAlive(){
        try{
        
        IDamagable damageable = Sphere.GetComponent<IDamagable>();
        IDamagable damageable2 = dude.GetComponent<IDamagable>();
        if(damageable?.GetHealth() > 0 && damageable2?.GetHealth() > 0){
            
            return true;
        }
        else{
            Debug.Log("Target: " + damageable?.GetHealth() + " Dude: " + damageable2?.GetHealth() );
            return false;
        }
        }catch{
            Debug.Log("One of them has died or else is missing the Target script");
            return false;
        }
    }

    private void Reloading(){
        TimeSpan interval = DateTime.Now - lastReload;

        if(interval.TotalSeconds > 1.5f){
            reload?.Invoke();
            lastReload = DateTime.Now;
        }
    }

    private void GettingAmmo(){
        transform.LookAt(ReloadArea.transform);
        
        transform.position = Vector3.MoveTowards(transform.position, ReloadArea.transform.position, 0.25f);
    }

    private void Patrolling(){
        if(!walkPointSet) SearchWalkPoint();

        if(walkPointSet){
            transform.position = Vector3.MoveTowards(transform.position, walkPoint, 0.25f);
        }

        
        float distToWalkPoint = Vector3.Distance(transform.position, walkPoint);

        if(distToWalkPoint < 2f){
            walkPointSet  = false;
        }
    }

    private void SearchWalkPoint(){
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        
        walkPointSet = true;
        
    }

    private void Chasing(){
        transform.position = Vector3.MoveTowards(transform.position, dude.transform.position, 0.35f);
    }

    private void Attacking(){
        transform.position = Vector3.MoveTowards(transform.position, transform.position, 1f);

        transform.LookAt(dude.transform);

        shoot?.Invoke();

        Debug.Log("Blue is attacking");
        Debug.DrawRay(transform.position, transform.forward, Color.red, gunData.maxDistance);
        if(gunData.currentAmmo == 0){
            Reloading();
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        
        dude = GameObject.Find("dude");
        Sphere = GameObject.Find("Sphere");
        ReloadArea = GameObject.Find("AmmoArea");

        lastReload = DateTime.Now;
        walkPointRange = 25f;
    }

    // Update is called once per frame
    void Update()
    {
        if(twoAreAlive()){
            Debug.DrawRay(transform.position, dude.transform.position - transform.position, Color.blue, 1000f);
            if(Physics.Raycast(transform.position, dude.transform.position - transform.position, out RaycastHit hitInfo, sightRange, ~(1 << LayerMask.NameToLayer("IgnoreSelf")))){
                Debug.Log("BLUEAI: " + hitInfo.transform.name + " Distance: " + hitInfo.distance + "Attack Range: " + attackRange + "SightRange: " + sightRange);
                if(hitInfo.transform.name == "dude"){
                    if(hitInfo.distance <= attackRange){
                        playerInAttackRange = true;
                        playerInSightRange = true;
                    }else if(hitInfo.distance <= sightRange){
                        playerInAttackRange = false;
                        playerInSightRange = true;
                    }else{
                        playerInSightRange = false;
                        playerInAttackRange = false;
                    }
                }else{
                    playerInSightRange = false;
                    playerInAttackRange = false;
                }
            }else{
                playerInSightRange = false;
                playerInAttackRange = false;
            }
            

            if(gunData.currentAmmo < 2){
              if(gunData.totalAmmo == 0){
                    GettingAmmo();
                }else if(gunData.totalAmmo > 2 && Vector3.Distance(transform.position, ReloadArea.transform.position) > 50f){
                    Reloading();
                }else if(Vector3.Distance(transform.position, ReloadArea.transform.position) <= 50f){
                    //Do nothing
                }
            }

            if(!playerInSightRange && !playerInAttackRange && gunData.currentAmmo != 0) Patrolling();
            if(playerInSightRange && !playerInAttackRange && gunData.currentAmmo != 0) Chasing();
            if(playerInSightRange && playerInAttackRange && gunData.currentAmmo != 0) Attacking();
        }
    }
}
