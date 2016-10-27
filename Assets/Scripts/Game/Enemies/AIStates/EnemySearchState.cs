using UnityEngine;
using System.Collections;

public class EnemySearchState : FSMState<SmartEnemy> {

    private float searchTime;
    private float timeToCheck = 0.0f;
    private Vector3 lastTargetPosition;
    private Vector3 lastTargetDirection;
    private bool positionReached = false;
    private float timeToRotate;
    private Vector3 directionToRotate;

    public EnemySearchState(Vector3 lastTargetPosition, Vector3 lastTargetDirection) {
        this.lastTargetPosition = lastTargetPosition;
        this.lastTargetDirection = lastTargetDirection;
    }

    public override void Enter(SmartEnemy enemy) {
        entity = enemy;
        searchTime = Time.time + entity.searchPersistence;
    }

    public override void Execute() {

        if (Time.time > searchTime) {
            EnemyPatrolState newState = new EnemyPatrolState();
            entity.stateMachine.ChangeState(newState);
        }

        if (!positionReached) {
            // Move to the last position where the target was seen...
            entity.Move(lastTargetPosition - entity.transform.position, entity.chaseSpeed);
            if (Vector3.Distance(lastTargetPosition, entity.transform.position) < 1.0f) {
                positionReached = true;
                directionToRotate = lastTargetDirection;
                timeToRotate = Time.time + 1.0f;
            }
        }
        else {
            // ... and look around from that position.
            if (Time.time > timeToRotate) {
                directionToRotate = Quaternion.Euler(0.0f, (Random.value > 0.5f) ? 90.0f : -90.0f, 0.0f) * entity.transform.forward;
                timeToRotate = Time.time + 2.0f;
            }

            entity.Move(directionToRotate, entity.chaseSpeed * 0.35f);
        }

        if (Time.time > timeToCheck) {
            if (entity.LookForTarget() == true) {
                EnemyChaseState newState = new EnemyChaseState();
                entity.stateMachine.ChangeState(newState);
            }
            else {
                timeToCheck = Time.time + 0.4f; // Check with a moderated frecuency.
            }
        }

    }

    public override void Exit() {

    }

}