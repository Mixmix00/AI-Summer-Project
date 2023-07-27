using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour, IDamagable
{
    [SerializeField] private float health;
    public void TakeDamage(float damage){
        health -= damage;
        Debug.Log(health);
        if(health <= 0){
            Destroy(gameObject);
        }
    }
    public float GetHealth(){
        return health;
    }
}
