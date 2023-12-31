using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
	[SerializeField] GunData gunData;
	private float timeSinceLastShot;
	private void Start(){
		PlayerShoot.shootInput += Shoot;
		PlayerShoot.reloadInput += StartReload;
		AI.shoot += Shoot;
		AI.reload += StartReload;
		BlueAI.shoot += Shoot;
		BlueAI.reload += StartReload;
		timeSinceLastShot = 10000f;
		gunData.reloading = false;
	}
	private bool CanShoot(){
		//return true;
		//Debug.Log("relod: " + gunData.reloading + " tslf: " + (timeSinceLastShot > 1f / (gunData.fireRate /60f)));
		
		return !gunData.reloading && timeSinceLastShot > 1f / (gunData.fireRate /60f); 
	} 
	public void StartReload(){
		Debug.Log("Here");
		if(!gunData.reloading){
			StartCoroutine(Reload());
		}
	}

	private IEnumerator Reload(){
		Debug.Log("Reload is being called");
		gunData.reloading = true;

		yield return new WaitForSeconds(gunData.reloadTime);
		if(gunData.totalAmmo + gunData.currentAmmo >= 31){
			gunData.totalAmmo -= gunData.magSize - gunData.currentAmmo;
			gunData.currentAmmo = gunData.magSize;
		}else{
			gunData.currentAmmo += gunData.totalAmmo;
			gunData.totalAmmo = 0;
		}
		gunData.reloading = false;
	}
	private void Shoot(){
		if(gunData.currentAmmo > 0){
			Debug.Log("bang");
			if(CanShoot()){
				Debug.Log("canshoot " + gunData.currentAmmo);
				if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, gunData.maxDistance) && hitInfo.transform.name != "ground"){
					Debug.Log("Hit!");
					IDamagable damageable = hitInfo.transform.GetComponent<IDamagable>();
					Debug.Log("SA: " + damageable);
					if(hitInfo.distance < 30f){
						damageable?.TakeDamage(gunData.closeDamage);
					}else if(hitInfo.distance < 100f){
						damageable?.TakeDamage(gunData.midDamage);
					}else{
						damageable?.TakeDamage(gunData.farDamage);
					}
					
				}
			gunData.currentAmmo--;
			timeSinceLastShot = 0;
			OnGunShot();
		} 	

		}
	}

	private void Update(){
		timeSinceLastShot += Time.deltaTime;
	}

	private void OnGunShot(){
		//Optional fx
	}
}
