using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public Transform frontTransform;
    public Transform backTransform;
    public bool isPlayerBullet = true;
    public float speed = 50.0f;
    public float duration = 5.0f;

    // Delegates.
    public delegate void TargetReached(float amount);
    public TargetReached OnTargetReached;

    private float damage;
    private bool isVampire = false;
    private Vector3 previousPosition;
    protected float destroyTime;

	void Start () {
        previousPosition = backTransform.position;
        destroyTime = Time.time + duration;
	}
	
	void Update () {
        if (Time.time > destroyTime) {
            Destroy(this.gameObject);
        }
        else {
            transform.Translate(0.0f, 0.0f, speed * Time.deltaTime);
        }
	}

    // This method is used for very fast bullets.
    private void FixedUpdate() {
        Vector3 movementThisStep = frontTransform.position - previousPosition;
        float magnitude = Mathf.Sqrt(movementThisStep.sqrMagnitude);
        RaycastHit hit;
        if (Physics.Raycast(previousPosition, movementThisStep, out hit, magnitude)) {
            if (hit.collider) {
                if (isPlayerBullet) {
                    if (hit.collider.tag == "Enemy") {
                        // Damage the enemy.
                        hit.collider.GetComponent<Enemy>().TakeDamage(damage);
                        if (isVampire) {
                            OnTargetReached(damage);    // Delegate. Implemented in Weapon.cs (its childs)
                        }
                    }
                    else if (hit.collider.tag == "Puzzle") {
                        hit.collider.GetComponent<PuzzlePiece>().Impact();
                    }
                }
                else if (!isPlayerBullet && hit.collider.tag == "Player") {
                    hit.collider.GetComponent<Player>().TakeDamage(damage);
                }
                if (hit.collider.gameObject != this.gameObject) {
                    Destroy(this.gameObject);
                }
            }
        }
        previousPosition = backTransform.position;
    }

    public void SetDamage(float damage, bool isVampire = false) {
        this.damage = damage;
        this.isVampire = isVampire;
    }

    public void SetSpeed(float speed) {
        this.speed = speed;
    }
}
