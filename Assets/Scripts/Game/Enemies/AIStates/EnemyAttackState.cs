using UnityEngine;
using System.Collections;

public class EnemyAttackState : FSMState<SmartEnemy> {

    public override void Enter(SmartEnemy enemy) {
        entity = enemy;
    }

    public override void Execute() {
        entity.Rotate(entity.target.position - entity.transform.position, entity.chaseSpeed);

        if (Time.time > entity.nextAttack) {
            // First check if the target is still visible.
            if (entity.LookForTarget() == false) {
                EnemySearchState newState = new EnemySearchState(entity.transform.position, entity.target.position - entity.transform.position);
                entity.stateMachine.ChangeState(newState);
            }
            else if (Vector3.Distance(entity.transform.position, entity.target.position) > entity.attackRange) {
                // If the target is out of the attack range, look for it again.
                EnemySearchState newState = new EnemySearchState(entity.transform.position, entity.target.position - entity.transform.position);
                entity.stateMachine.ChangeState(newState);
            }
            else {
                entity.Attack();
                entity.nextAttack = Time.time + entity.attackRate;
            }
        }

    }

    public override void Exit() {

    }

}
