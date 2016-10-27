using UnityEngine;
using System.Collections;

public class Crushing : SmartEnemy {

    public Renderer mesh;
    public ParticleSystem attackParticles;
    public ParticleSystem deadParticles;

	private void Start() {
        controller = GetComponent<CharacterController>();
        stateMachine = new FiniteStateMachine<SmartEnemy>();
        stateMachine.Init(this);
	}
	
	private void Update() {
        if (!isDead) {
            if (target == null) {
                target = GameObject.FindGameObjectWithTag("Player").transform;
                EnemyPatrolState newState = new EnemyPatrolState();
                stateMachine.ChangeState(newState);
            }

            stateMachine.Update();

            // If the target has been detected, start attacking.
            if (stateMachine.currentState is EnemyChaseState) {
                if (Time.time > nextAttack) {
                    Attack();
                    nextAttack = Time.time + attackRate;
                }
            }
        }
        else {
            if (mesh.material.color.a > 0.0f) {
                Color newColor = mesh.material.color;
                newColor.a -= Time.deltaTime * 5.0f;
                mesh.material.color = newColor;
            }
            if (!deadParticles.IsAlive()) {
                Destroy(this.gameObject);
            }
        }

        // Gravity is allways present.
        controller.Move(new Vector3(0.0f, -9.8f * Time.deltaTime, 0.0f));
	}

    protected override void Die() {
        deadParticles.Play();
    }

    public override void Attack() {
        attackParticles.Play();
        if (Vector3.Distance(target.position, transform.position) < attackRange * 1.5f) {
            target.GetComponent<Player>().TakeDamage(baseDamage);
        }
    }

}
