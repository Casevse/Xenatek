using UnityEngine;
using System.Collections;

public class Turret : Enemy {

    public Transform jointTransform;
    public Transform attackSource;
    public float speed = 5.0f;
    public float maxAngle = 90.0f;
    public float fieldOfView = 60.0f;
    public LaserBullet bullet;
    public ParticleSystem deadParticles;

    private Transform target = null;
    private Vector3 shotPrediction;
    private TurretStates state;
    private float stateTime = 0.0f;
    private float timeToCheck = 0.0f;
    private Vector3 patrolForward;
    private float patrolSense = 1.0f;
    private Vector3 lastTargetPosition;

    private enum TurretStates {
        PATROL, ALERTED, DESPERATE
    }

    private void Start() {
        state = TurretStates.PATROL;
        patrolForward = transform.forward;
    }

    private void Update() {
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
                // Rotate slowly to analize the environment.
                patrolForward = Quaternion.AngleAxis(patrolSense * speed * 5.0f * Time.deltaTime, transform.up) * patrolForward;
                // Use this variable to aim where the turret is rotating.
                shotPrediction = transform.position + (transform.up * 1.25f) + patrolForward;
                Aim();
                
                // Check if the turret sees the player with frecuency.
                if (Time.time > timeToCheck) {
                    if (LookForTarget() == true) {
                        state = TurretStates.ALERTED;
                        stateTime = Time.time + 0.5f;
                    }
                    else {
                        // Check if where the turret looks is a very close obstacle, and change the direction.
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
                if (Time.time > timeToCheck) {
                    PredictShot();
                }
                Aim();
                Attack();

                // Wait a little before check again if the turret sees the target.
                if (Time.time > stateTime && Time.time > timeToCheck) {
                    if (LookForTarget() == false) {
                        state = TurretStates.DESPERATE;
                        stateTime = Time.time + 5.0f;
                    }
                    else {
                        timeToCheck = Time.time + 0.25f;
                    }
                }
                break;
            }
            case TurretStates.DESPERATE: {
                if (Time.time > stateTime) {
                    state = TurretStates.PATROL;
                }
                else {
                    // Desperate attack...
                    Attack();

                    // ... and check again if the turret sees the player.
                    if (Time.time > timeToCheck) {
                        if (LookForTarget() == true) {
                            state = TurretStates.ALERTED;
                            stateTime = Time.time + 0.5f;
                        }
                        else {
                            timeToCheck = Time.time + 0.25f;
                        }
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
        state = TurretStates.ALERTED;
        stateTime = Time.time + 0.5f;
    }

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

    private void Aim() {
        Vector3 shotDestiny = shotPrediction - attackSource.position;

        if (Vector3.Angle(shotDestiny, transform.up) > maxAngle) {
            Vector3 axis = Vector3.Cross(transform.up, shotDestiny);
            shotDestiny = Quaternion.AngleAxis(maxAngle, axis) * transform.up;
        }

        Quaternion rotation = Quaternion.LookRotation(shotDestiny, transform.up);
        jointTransform.rotation = Quaternion.Slerp(jointTransform.rotation, rotation, speed * Time.deltaTime);
    }

    private void PredictShot() {
        Vector3 headingTarget = target.position - lastTargetPosition;
        Vector3 headingTurret = target.position - attackSource.position;

        // The shot depends if the target has moved and the distance to it.
        shotPrediction = target.position + headingTarget.normalized * (headingTarget.magnitude * headingTurret.magnitude * 0.4f);

        lastTargetPosition = target.position;
    }

    private void Attack() {
        if (Time.time > nextAttack) {
            LaserBullet bulletClone = Instantiate(bullet, attackSource.position, attackSource.rotation) as LaserBullet;
            bulletClone.SetDamage(baseDamage, false);
            bulletClone.isPlayerBullet = false;

            nextAttack = Time.time + attackRate;
        }
    }

}