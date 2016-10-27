using UnityEngine;
using System.Collections;

abstract public class SmartEnemy : Enemy {

    // NOTE: This type of enemy used Finite State Machines for the AI.

    public float patrolSpeed = 2.5f;
    public float chaseSpeed = 5.0f;
    public float visionRange = 20.0f;
    public float fieldOfView = 60.0f;
    public float searchPersistence = 5.0f;

    public FiniteStateMachine<SmartEnemy> stateMachine {
        get;
        protected set;
    }
    public CharacterController controller {
        get;
        protected set;
    }
    public Transform target {
        get;
        protected set;
    }

    abstract public void Attack();

    public void Move(Vector3 direction, float speed) {
        Rotate(direction, speed);
        controller.Move(transform.forward * speed * Time.deltaTime);
    }

    public void Rotate(Vector3 direction, float speed) {
        if (direction == Vector3.zero) return;
        Quaternion rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), speed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(new Vector3(0.0f, rotation.eulerAngles.y, 0.0f));
    }

    public bool LookForTarget() {
        RaycastHit hit;
        Vector3 direction = target.position - transform.position;

        if (Vector3.Angle(direction, transform.forward) < fieldOfView) {
            if (Physics.Raycast(transform.position, direction, out hit, visionRange)) {
                if (hit.transform.tag == target.tag) {
                    return true;
                }
            }
        }
        return false;
    }

    public override void Alert(Vector3 alertSource) {
        // Search the source of the alert...
        if (stateMachine.currentState is EnemyPatrolState) {
            if (alertSource == Vector3.zero) {  // This call is done from TakeDamage().
                alertSource = target.position;
            }
            EnemySearchState newState = new EnemySearchState(alertSource + transform.forward * 2.0f, transform.forward);
            stateMachine.ChangeState(newState);
        }
    }

}
