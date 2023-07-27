using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Movment : MonoBehaviour
{
    //Right now im thinking to make the AI have choices to do stuff based on these int's.
    //The ints could be stored in an array for the bot to use.
    //Our AI wants to kill/damage the other bot, but also not get hurt in the process.
    //This is where the score based AI works well.

    /*

    */
    [SerializeField] private Vector3 _rotation;
    [SerializeField] private float _speed;
    GameObject Target;
    GameObject dude;
    GameObject eyes;
    GameObject Sphere;
    GameObject sCHA;
    public Rigidbody rb;
    private PID pid_moving;
    // Walking
    private DateTime lastDate;
    [SerializeField] GunData gunData;

    NavMeshAgent nav;
    private bool cover;

    public float attackRange = 1f;
    public float retreatThreshold = 20f;

    private float health;
    private float ammunition_in_gun;
    private float stored_ammunition;
    private float coverAvailability;
    private bool ignoreReloading = false;

    private float enemyDist;
    private float lowestDist;

    GameObject reloadArea;

    private enum Action
    {
        RELOAD,
        FINDAMMO,
        ROTATE,
        FINDCOVER,
        ATTACK,
        RETREAT
    }

    private enum STATE{
        RELOADING,
        FINDINGAMMO,
        SEEKINGCOVER,
        ATTACKING,
        RETREATING,
        CHASING,
        PATROLLING
    }

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        pid_moving = new PID(0.01f, 0, 0);
        lastDate = DateTime.Now;
        
        Target = GameObject.Find("Target");
        dude = GameObject.Find("dude");
        eyes = GameObject.Find("front"); // Front of the player with the little red rectangle sticking out
        Sphere = GameObject.Find("Sphere");
        sCHA = GameObject.Find("God");
        reloadArea = GameObject.Find("AmmoArea");
    }

    // Done
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

    IEnumerator Delay(){
        yield return new WaitForSeconds(1);
    }
/*
    private void whatCanSee(){
        Debug.Log("I can see: ");
        float radius = 500F;
        Vector3 origin = transform.position;
        origin[2] += 500f;
        RaycastHit[] sphereCastHits = Physics.SphereCastAll(origin, radius, Vector3.forward, 0); //check for collisions in a radius around us
        Debug.Log("Z: " + origin[2]);
        for(int i = 0; i< sphereCastHits.Length; i++){
            //Gizmos.color = Color.red; FOR use in OnDrawGizmos() and not here
            //Gizmos.DrawWireSphere(sphereCastHits[i].point, 5f);
            Debug.Log(sphereCastHits[i].collider.gameObject.name);
        }
    }*/

    private Vector3 distanceToCover(){
        float radius = 500F;
        Vector3 origin = transform.position;
        

        RaycastHit[] sphereCastHits = Physics.SphereCastAll(origin, radius, Vector3.forward, 0); //check for collisions in a radius around us

        for(int i = 0; i<sphereCastHits.Length; i++){
            if(sphereCastHits[i].collider.transform.name.Substring(0,1) == "C"){ // C will always have to be the starting letter of a cover
                return sphereCastHits[i].collider.transform.position - transform.position;
            }
        }
        throw new Exception("No cover found");
    }

    //Done
    private float calculateNeedingAmmoScore(){
        if(gunData.currentAmmo + gunData.totalAmmo >= 2 * gunData.magSize){
            return 0f;
        }
        else if(gunData.currentAmmo + gunData.totalAmmo < gunData.magSize){
            return (gunData.magSize-(gunData.currentAmmo + gunData.totalAmmo))/gunData.magSize;
        }else if(gunData.currentAmmo + gunData.totalAmmo < 2 * gunData.magSize){
            return (( 2* gunData.magSize -(gunData.currentAmmo + gunData.totalAmmo))/gunData.magSize)/2;
        } else{
            throw new Exception("Something went wrong with the ammo score");
        }
    }

    //Done
    private float calculateReloadingScore(){
        if(ignoreReloading){
            return 0f;
        }
        
        if(gunData.currentAmmo == gunData.magSize || gunData.totalAmmo == 0){
            return 0f;
        }else if(gunData.currentAmmo ==0){
            return 1f;
        }else if(gunData.currentAmmo < gunData.magSize && gunData.totalAmmo > 0){
            return (gunData.magSize-gunData.currentAmmo)/gunData.magSize;
        }else{
            throw new Exception("Something went wrong with the reloading score");
        }
    }

    private RaycastHit[] whatCanSee(){
        Vector3 origin = transform.position;
        origin[2] += 500f;
        RaycastHit[] sphereCastHits = Physics.SphereCastAll(origin, 500, Vector3.forward, 0); //check for collisions in a radius around us
        for(int i = 0; i< sphereCastHits.Length; i++){
            //Debug.Log(sphereCastHits[i].collider.gameObject.name);
        }
        return sphereCastHits;
    }

    //Done
    private float getDudeHealth(){
        IDamagable damageable = dude.GetComponent<IDamagable>();
        return (float) damageable?.GetHealth();
    }

private float calculateCoverAvalabilityScore()
{
    float bonus = 0f;
    Vector3 lowestCoverV3;
    Vector3 enemyV3;

    bool enemyInSight = isEnemyHittable();
    bool coverInSight = isCoverHittable();
    
    


    if(!enemyInSight && coverInSight){
        bonus += 0.21f;
    }else if(!enemyInSight)
    {
        bonus += 0.07f;
        return 0.01f;
    }else if(enemyInSight && !coverInSight){
        return 0f; //uh oh
    }

    if(coverInSight){
        lowestCoverV3 = findClosestCover();
        lowestDist = Vector3.Distance(lowestCoverV3, transform.position);
    }else{
        lowestDist = 1000000f;
    }

    if(enemyInSight){
        enemyV3 = findTheEnemy(enemyInSight);
        enemyDist = Vector3.Distance(enemyV3, transform.position);
    }else{
        enemyDist = 1000000f;
    }
    
    if(enemyDist < lowestDist){
        return cap(((1/(lowestDist - enemyDist))* (2/enemyDist)) + bonus, 0f, 0.35f);
    }else if(lowestDist < enemyDist){
        return cap((((-1) * lowestDist - enemyDist) / 200) + bonus, 0f, 1f);
    }else{
        return cap(bonus + 0.2f, 0f, 0.5f);
    }
}

    private void whereIsEnemy(){
        Vector3 origin = dude.transform.position;
        Vector3 targetDirection = Sphere.transform.position - origin;
        //float singleStep = 1000f * Time.deltaTime;
        //Vector3 up = new Vector3(0,10,0);
        Vector3 orientation = Vector3.RotateTowards(transform.forward, targetDirection, 1.5707963f, 0.0f);
        Debug.DrawRay(transform.position, orientation, Color.red);
        Debug.DrawRay(transform.position, targetDirection * 500, Color.red);
        int layerMask = ~(1 << LayerMask.NameToLayer("IgnoreEyesRaycast"));
        Debug.Log("targwet: " + targetDirection);
        if(Physics.Raycast(dude.transform.position, targetDirection, out RaycastHit hit, 500f, layerMask)){
            Debug.Log("S: " + hit.transform.name);
            Debug.DrawRay(transform.position, targetDirection * hit.distance, Color.green);
        }
    }

    private bool isEnemyHittable(){
        Vector3 origin = dude.transform.position;
        Vector3 targetDirection = Sphere.transform.position - origin;
        Vector3 orientation = Vector3.RotateTowards(transform.forward, targetDirection, 1.5707963f, 0.0f);
        Debug.DrawRay(transform.position, orientation, Color.red);
        Debug.DrawRay(transform.position, targetDirection * 500, Color.red);
        int layerMask = ~(1 << LayerMask.NameToLayer("IgnoreEyesRaycast"));
        Debug.Log("targwet: " + targetDirection);
        if(Physics.Raycast(dude.transform.position, targetDirection, out RaycastHit hit, 500f, layerMask) && hit.transform.name.Substring(0,1) == "T"){
            return true;
        }
        return false;
    }

    private bool isCoverHittable(){
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Cover");

        foreach (GameObject go in gos){
            Debug.DrawRay(transform.position, go.transform.position - dude.transform.position, Color.blue, 1000f);
            if(Physics.Raycast(transform.position, go.transform.position - dude.transform.position, out RaycastHit hit, 5000f, ~(1 << LayerMask.NameToLayer("IgnoreEyesRaycast")))){
                if(hit.transform.name.Substring(0,1) == "C"){
                    return true;
                }
            }
        
        }
        return false;
    }



    private Vector3 findTheEnemy(bool hitable){
        if(!hitable){
            throw new Exception("Enemy is not hittable");
        }

        Vector3 origin = dude.transform.position;
        Vector3 targetDirection = Sphere.transform.position - origin;
        int layerMask = ~(1 << LayerMask.NameToLayer("IgnoreEyesRaycast"));
        if(Physics.Raycast(dude.transform.position, targetDirection, out RaycastHit hit, 500f, layerMask)){
            return hit.transform.position;
        }else{
            throw new Exception("Something went really wrong with the raycast");
        }
    }

    private Vector3 findClosestCover(){

        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Cover");
        float lowestDist = 1000000f;
        int distanceIndex = -1;
        foreach (GameObject go in gos){
            //Debug.Log("Cover: " + go.transform.name);
            Debug.DrawRay(transform.position, go.transform.position - dude.transform.position, Color.blue, 1000f);
            if(Physics.Raycast(transform.position, go.transform.position - dude.transform.position, out RaycastHit hit, 5000f, ~(1 << LayerMask.NameToLayer("IgnoreEyesRaycast")))){
                //Debug.Log("Covera: " + hit.transform.name);
                if(hit.transform.name.Substring(0,1) == "C"){
                    if(hit.distance < lowestDist){
                        lowestDist = hit.distance;
                        distanceIndex = Array.IndexOf(gos, go);
                        Debug.Log("distomdws: " + Array.IndexOf(gos, go));
                    }
                }
            }
        
        }

        Debug.Log("DistanceIndex: " + distanceIndex);

        if(distanceIndex == -1){
            cover = false;
            throw new Exception("No cover found");
        }
        cover = true;
        return gos[distanceIndex].transform.position;
    }

    //must be called after findClosestCover
    private bool isThereCover(){
        return cover;
    }



    private float cap(float value, float min, float max){
        if(value < min){
            return min;
        }
        else if(value > max){
            return max;
        }
        else{
            return value;
        }
    }

    private float calculateRotateScore(){
        return 0f;
    }

    private float calculateAttackScore(){
        return 0f;
    }

    private float calculateRetreatScore(){
        return 0f;
    }
    private Action findHighestScore(float[] actionsScores){
        float highestScore = actionsScores[0];
        int highestScoreIndex = 0;
        Action highestScoreAction = Action.RELOAD;
        for(int i = 1; i < actionsScores.Length; i++){
            if(actionsScores[i] > highestScore){
                highestScore = actionsScores[i];
                highestScoreIndex = i;
            }
        }
        switch (highestScoreIndex)
        {
            case 0:
                highestScoreAction = Action.RELOAD;
                break;
            case 1:
                highestScoreAction = Action.FINDAMMO;
                break;
            case 2:
                highestScoreAction = Action.ROTATE;
                break;
            case 3:
                highestScoreAction = Action.FINDCOVER;
                break;
            case 4:
                highestScoreAction = Action.ATTACK;
                break;
            case 5:
                highestScoreAction = Action.RETREAT;
                break;
            default:
                break;
        }
        return highestScoreAction;
    }

    /*
For walking/running, the AI will have to use the walkXY and runXY functions to move around.
The difference between the two is that walkXY is slower than runXY, and so walkXY won't make any noise.
    */
    private void walkXY(float speedX, float speedY){rb.AddForce(cap(speedX,-1,1), 0, cap(speedY,-1,1), ForceMode.VelocityChange);}
    private void runXY(float speedX, float speedY){rb.AddForce(cap(speedX,-2,2), 0, cap(speedY,-2,2), ForceMode.VelocityChange);}
    
    private void rotate(float speed){transform.Rotate(_rotation*cap(speed,-2,2));}


    private void shoot(){PlayerShoot.shootInput?.Invoke();}
    private void reload(){PlayerShoot.reloadInput?.Invoke();}
    
    // Update is called once per frame
    void FixedUpdate()
    {
        IDamagable damageable2 = dude.GetComponent<IDamagable>();
        health = (float) damageable2?.GetHealth();
        attackRange = gunData.maxDistance;


        if(twoAreAlive()){

        //FOR RELOADING
            if(calculateReloadingScore() > 0.8f){
                reload();
            }
        //FOR COLLECTiNG AMMO
            if(calculateNeedingAmmoScore() > 0.8f || true){

                //?
                Vector3 e = reloadArea.transform.position;
                
                //transform.LookAt(e);

                if((Physics.Raycast(transform.position, reloadArea.transform.position - dude.transform.position, out RaycastHit hitInfo, 5000f, ~(1 << LayerMask.NameToLayer("IgnoreEyesRaycast"))))){
                    if(hitInfo.transform.name == "AmmoArea"){
                        //nav.SetDestination(e);
                    }
                }else{
                    shoot();
                }
                
            }



        //FOR ATTACKING
            if(isEnemyHittable() && getDudeHealth() < 30f){
                transform.LookAt(Target.transform);
                shoot();
            }else if(isEnemyHittable()){
                transform.LookAt(Target.transform);
                shoot();
                transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, 0.25f);
            }













            whereIsEnemy();
/*
            float[] actionsScores = new float[6];
            actionsScores[0] = calculateReloadingScore();
            actionsScores[1] = calculateNeedingAmmoScore();
            actionsScores[2] = calculateRotateScore();//TODO: IMPLEMENT ROTATING
            actionsScores[3] = calculateCoverAvalabilityScore();//TODO: FIX THIS
            actionsScores[4] = calculateAttackScore();//TODO: IMPLEMENT ATTACKING
            actionsScores[5] = calculateRetreatScore();//TODO: IMPLEMENT RETREATING
            Action highestScoreAction = findHighestScore(actionsScores);
            //TODO: make the AI do stuff
*/
            //TODO: get the AI's input of what it wants to do using these variables
            float runX = 0;
            float runY = 0;
            bool wantsToShoot = false;
            bool wantsToReload = false;
            float rotateSpeed = 0;
            AIPOSXYZ currentPosition = new AIPOSXYZ(transform.position.x, transform.position.y, transform.position.z);
            //TODO: give the AI info about what is around it

 

            //TODO: allow the AI to use these variables
            if(runX >1 || runX < -1 || runY > 1 || runY < -1){
                runXY(runX, runY);
            }
            else{
                walkXY(runX, runY);
            }
            if(wantsToReload){
                reload();
            }
            if(wantsToShoot){
                shoot();
            }
            rotate(rotateSpeed);


            //Vector3 cover = distanceToCover();
            Debug.Log("jdskm: " + calculateCoverAvalabilityScore());
            //Debug.Log("e: " + cover.x + " " + cover.y + " " + cover.z);
            //Debug.Log("Q: " + calculateCoverAvalabilityScore());
            Vector3 closestCover = findClosestCover();
            //Debug.Log("C: " + closestCover.x + " " + closestCover.y + " " + closestCover.z);
            /*
            Do pids later
            DateTime newDate = DateTime.Now;
            
            // Calculate the interval between the two dates.
            TimeSpan interval = newDate - lastDate;
            //Debug.Log("A: " + (float) pid_moving.PID_iterate(34.4, (double) transform.position.x, interval) + " " + (float) pid_moving.PID_iterate(60.0, (double) transform.position.z, interval));
            //runXY((float) pid_moving.PID_iterate(-34.4, (double) transform.position.x, interval), (float) pid_moving.PID_iterate(60.0, (double) transform.position.z, interval));
            Debug.Log("X: " + transform.position.x + " Z: " + transform.position.z);
            float xAxisError = transform.position.x - (-34.4f);
            float zAxisError = transform.position.z - 60.0f;
            runXY(pid_moving.GetOutput(-xAxisError, Time.fixedDeltaTime), pid_moving.GetOutput(-zAxisError, Time.fixedDeltaTime));
            Debug.Log("X: " + xAxisError + " Z: " + zAxisError);
            Debug.Log("xp: " + pid_moving.GetOutput(-xAxisError, Time.fixedDeltaTime) + " zp: " + pid_moving.GetOutput(-zAxisError, Time.fixedDeltaTime));
            */
            // if(choice == walkForward){
            //     rb.AddForce(0, 0, 1, ForceMode.VelocityChange);
            // }
            // else if(choice == walkBackward){
            //     rb.AddForce(0, 0, -1, ForceMode.VelocityChange);
            // }
            // else if(choice == walkRight){
            //     rb.AddForce(1, 0, 0, ForceMode.VelocityChange);
            // }
            // else if(choice == walkLeft){
            //     rb.AddForce(-1, 0, 0, ForceMode.VelocityChange);
            // }
            // else if(choice == shootGun){
            //     Debug.Log("shoot");
            // }
            // else{
            //     throw new System.Exception("Invalid choice");
            // }


        //lastDate = newDate;
    }
}
}