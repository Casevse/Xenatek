using UnityEngine;
using System.Collections;

public class EnemyPatrolState : FSMState<SmartEnemy> {

    private Vector3 randomDirection;
    private float distanceToCheck = 3.0f;
    private float timeToCheck = 0.0f;
    private float timeToLook = 0.0f;

    public override void Enter(SmartEnemy enemy) {
        entity = enemy;
        randomDirection = enemy.transform.forward;
    }

    public override void Execute() {

        if (Time.time > timeToCheck) {
            if (CheckNearObstacle() == true) {
                randomDirection = GenerateRandomDirection();
            }
            timeToCheck = Time.time + 0.2f;
        }

        if (Time.time > timeToLook) {
            if (entity.LookForTarget() == true) {
                EnemyChaseState newState = new EnemyChaseState();
                entity.stateMachine.ChangeState(newState);
            }
            else {
                timeToLook = Time.time + 1.0f;  // While patroling takes more time to detect the player, because the enemy is cold.
            }
        }

        entity.Move(randomDirection, entity.patrolSpeed);
    }

    public override void Exit() {

    }

    private Vector3 GenerateRandomDirection() {
        Vector3 direction;
        RaycastHit hit;

        direction = new Vector3(Random.value * (Random.value > 0.5f ? 1.0f : -1.0f), 0.0f, Random.value * (Random.value > 0.5f ? 1.0f : -1.0f));
        if (Physics.Raycast(entity.transform.position, direction, out hit, distanceToCheck)) {
            direction = Quaternion.Euler(0.0f, 90.0f, 0.0f) * hit.normal;
        }

        return direction;
    }

    private bool CheckNearObstacle() {
        return Physics.Raycast(entity.transform.position, entity.transform.forward, distanceToCheck);
    }

}