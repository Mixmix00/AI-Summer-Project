using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable{
    
    public void TakeDamage(float damage);

    public float GetHealth();
}
