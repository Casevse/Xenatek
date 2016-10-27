using UnityEngine;
using System.Collections;

public class Bug : SmartEnemy {

    public Transform attackSource;
    public Renderer rend;
    public ParticleSystem deadParticles;

	void Start () {
        controller = GetComponent<CharacterController>();

        stateMachine = new FiniteStateMachine<SmartEnemy>();
        stateMachine.Init(this);

        EnemyPatrolState newState = new EnemyPatrolState();
        stateMachine.ChangeState(newState);
	}
	
	void Update () {
        if (!isDead) {
            if (target == null) {
                target = GameObject.FindGameObjectWithTag("Player").transform;
                EnemyPatrolState newState = new EnemyPatrolState();
                stateMachine.ChangeState(newState);
            }
            stateMachine.Update();
        }
        else {
            // Fade.
            if (rend.material.color.a > 0.0f) {
                Color newColor = rend.material.color;
                newColor.a -= Time.deltaTime * 5.0f;
                rend.material.color = newColor;
            }
            if (!deadParticles.IsAlive()) {
                Destroy(this.gameObject);
            }
        }

        // Gravity is always present.
        controller.Move(new Vector3(0.0f, -9.8f * Time.deltaTime, 0.0f));
	}

    protected override void Die() {
        deadParticles.Play();
    }

    public override void Attack() {
        RaycastHit hit;
        if (Physics.Raycast(attackSource.position, transform.forward, out hit, attackRange)) {
            if (hit.collider.tag == "Player") {
                hit.collider.GetComponent<Player>().TakeDamage(baseDamage);
            }
        }
    }

}