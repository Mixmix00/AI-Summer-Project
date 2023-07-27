using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{

    NavMeshAgent agent;
    [SerializeField] GunData gunData;

    GameObject Target;
    GameObject dude;

    public LayerMask whatIsGround, whatIsPlayer;

    public Vector3 walkPoint;
    bool walkPointSet;
    float walkPointRange;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    
    private bool twoAreAlive(){
        try{
        
        IDamagable damageable = Target.GetComponent<IDamagable>();
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
        PlayerShoot.reloadInput?.Invoke();
    }

    private void GettingAmmo(){

    }

    private void Patrolling(){
        if(!walkPointSet) SearchWalkPoint();

        if(walkPointSet){
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkpoint = transform.position - walkPoint;
        if(distanceToWalkpoint.magnitude < 1f){
            walkPointSet  = false;
        }
    }

    private void SearchWalkPoint(){
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 20f, whatIsGround)){
            walkPointSet = true;
        }
    }

    private void Chasing(){
        agent.SetDestination(Target.position);
    }

    private void Attacking(){
        agent.SetDestination(transform.position);

        transform.LookAt(Target);

        PlayerShoot.shootInput?.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {

        Target = GameObject.Find("Target");
        dude = GameObject.Find("dude");

        agent = GetComponent<NavMeshAgent>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(twoAreAlive()){
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if(gunData.currentAmmo < 8){
                if(gunData.totalAmmo > 31){
                    Reloading();
                }else if(gunData.totalAmmo > 0){
                    Reloading();
                }else if(gunData.totalAmmo == 0){
                    GettingAmmo();
                }
            }

            if(!playerInSightRange && !playerInAttackRange) Patrolling();
            if(playerInSightRange && !playerInAttackRange) Chasing();
            if(playerInSightRange && playerInAttackRange) Attacking();
        }
    }
}
