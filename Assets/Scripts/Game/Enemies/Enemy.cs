using UnityEngine;
using System.Collections;

abstract public class Enemy : MonoBehaviour {

    public float maxHealth = 100.0f;
    public float health = 100.0f;
    public float baseDamage;
    public float attackRange;
    public float attackRate;
    public float nextAttack = 0.0f;

    protected bool isDead = false;

    // Delegate for the mission controller.
    public delegate void EnemyKilled();
    public EnemyKilled OnEnemyKilled;

    public void TakeDamage(float amount) {
        if (!isDead) {
            health -= amount;
            if (health <= 0.0f) {
                health = 0.0f;
                isDead = true;
                Die();
                if (OnEnemyKilled != null) {
                    OnEnemyKilled();    // Implemented by Mission.cs (the corresponding).
                }
            }
            Alert(Vector3.zero);    // Pass a null vector, because the Alert method searches the actual target.
        }
    }

    abstract public void Alert(Vector3 alertSource);

    abstract protected void Die();

}
