using UnityEngine;
using System.Collections;

public class EnemyChaseState : FSMState<SmartEnemy> {

    private float timeToCheck = 0.0f;
    private Vector3 lastTargetPosition;

    public override void Enter(SmartEnemy enemy) {
        entity = enemy;
        lastTargetPosition = entity.target.position;
    }

    public override void Execute() {
        entity.Move(entity.target.position - entity.transform.position, entity.chaseSpeed);

        if (Time.time > timeToCheck) {
            if (entity.LookForTarget() == false) {
                // Enter seach state.
                EnemySearchState newState = new EnemySearchState(lastTargetPosition + entity.transform.forward * 2.0f, entity.target.position - lastTargetPosition);
                entity.stateMachine.ChangeState(newState);
            }
            else {
                if (Vector3.Distance(entity.transform.position, entity.target.position) < entity.attackRange - entity.controller.radius) {
                    EnemyAttackState newState = new EnemyAttackState();
                    entity.stateMachine.ChangeState(newState);
                }
                else {
                    lastTargetPosition = entity.target.position;
                    timeToCheck = Time.time + 0.2f; // Check with more frecuency, because the enemy is chasing.
                }
            }
        }
    }

    public override void Exit() {

    }

}