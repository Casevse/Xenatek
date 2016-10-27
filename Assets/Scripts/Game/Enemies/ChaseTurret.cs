using UnityEngine;
using System.Collections;

public class ChaseTurret : Enemy {

    public Transform jointTransform;
    public Transform attackSource;
    public float speed = 5.0f;
    public float maxAngle = 90.0f;
    public float fieldOfView = 60.0f;
    public ChaseBullet bullet;
    public ParticleSystem deadParticles;

    private TurretStates state;
    private Transform target = null;
    private Vector3 shotPrediction;
    private float timeToCheck = 0.0f;
    private Vector3 patrolForward;
    private float patrolSense = 1.0f;

    private enum TurretStates {
        PATROL, ALERTED
    }

	void Start () {
        state = TurretStates.PATROL;
        patrolForward = transform.forward;
	}
	
	void Update () {
        if (target == null) {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if (isDead) {
            if (!deadParticles.IsAlive()) {
                Destroy(this.gameObject);
            }
            return;
        }

        switch (state) {
            case TurretStates.PATROL: {
                patrolForward = Quaternion.AngleAxis(patrolSense * speed * 5.0f * Time.deltaTime, transform.up) * patrolForward;
                shotPrediction = transform.position + transform.up + patrolForward;
                Aim();

                if (Time.time > timeToCheck) {
                    if (LookForTarget() == true) {
                        state = TurretStates.ALERTED;
                        nextAttack = Time.time + 1.0f;
                        timeToCheck = Time.time + 1.0f;
                    }
                    else {
                        RaycastHit hit;
                        if (Physics.Raycast(attackSource.position, attackSource.forward, out hit, 2.0f)) {
                            patrolSense = patrolSense * -1.0f;
                        }
                        timeToCheck = Time.time + 0.25f;
                    }
                }
                break;
            }
            case TurretStates.ALERTED: {
                shotPrediction = target.position;
                Aim();
                Attack();
                if (Time.time > timeToCheck) {
                    if (LookForTarget() == false) {
                        state = TurretStates.PATROL;
                        timeToCheck = Time.time + 1.0f;
                    }
                    else {
                        timeToCheck = Time.time + 0.25f;
                    }
                }
                break;
            }
        }

	}

    protected override void Die() {
        deadParticles.Play();
    }

    public override void Alert(Vector3 alertSource) {
        if (state == TurretStates.PATROL) {
            state = TurretStates.ALERTED;
            nextAttack = Time.time + 1.0f;
        }
    }

    // NOTE: This is the same as the normal tower.
    private bool LookForTarget() {
        RaycastHit hit;
        Vector3 direction = target.position - attackSource.position;

        if (Vector3.Angle(direction, attackSource.forward) < fieldOfView) {
            if (Physics.Raycast(attackSource.position, direction, out hit, attackRange)) {
                if (hit.transform.tag == target.tag) {
                    return true;
                }
            }
        }
        return false;
    }

    // NOTE: This is the same as the normal tower.
    private void Aim() {
        Vector3 shotDestiny = shotPrediction - attackSource.position;

        if (Vector3.Angle(shotDestiny, transform.up) > maxAngle) {
            Vector3 axis = Vector3.Cross(transform.up, shotDestiny);
            shotDestiny = Quaternion.AngleAxis(maxAngle, axis) * transform.up;
        }

        Quaternion rotation = Quaternion.LookRotation(shotDestiny, transform.up);
        jointTransform.rotation = Quaternion.Slerp(jointTransform.rotation, rotation, speed * Time.deltaTime);
    }

    private void Attack() {
        if (Time.time > nextAttack) {
            ChaseBullet bulletClone = Instantiate(bullet, attackSource.position, attackSource.rotation) as ChaseBullet;
            bulletClone.SetTarget(target);
            bulletClone.SetDamage(baseDamage, false);
            bulletClone.isPlayerBullet = false;

            nextAttack = Time.time + attackRate;
        }
    }

}
