using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Gun", menuName="Weapon/Gun")]
public class GunData : ScriptableObject
{
	[Header("Info")]
	public new string name;
	
	[Header("Shooting")]
	public float damage; //TODO: delete this and change it in the code
	public float closeDamage; //0-30 raycast distance
	public float midDamage; //30-100 raycast distance
	public float farDamage; //100+ raycast distance
	public float headshotMultiplier;
	public float maxDistance;
	

	[Header("Reloading")]
	public int totalAmmo;
	public int currentAmmo;
	public int magSize;
	public int fireRate; //RPM
	public float reloadTime;
	[HideInInspector]
	public bool reloading;
}
