using UnityEngine;
using System.Collections;

public class Bot : SmartEnemy {

    public Transform attackSource;
    public LaserBullet bullet;
    public Renderer mesh;
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
        LaserBullet bulletClone = Instantiate(bullet, attackSource.position, attackSource.rotation) as LaserBullet;
        bulletClone.SetDamage(baseDamage, false);
        bulletClone.isPlayerBullet = false;
    }

}