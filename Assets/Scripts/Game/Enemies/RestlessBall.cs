using UnityEngine;
using System.Collections;

public class RestlessBall : Enemy {

    public float speed;
    public Renderer mesh;
    public ParticleSystem deadParticles;

    private CharacterController controller;
    private Vector3 direction;

    private void Start() {
        controller = GetComponent<CharacterController>();

        direction = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), Vector3.up) * transform.forward;
        mesh.transform.LookAt(transform.position + direction);
    }

    private void Update() {
        if (isDead) {
            // Fade.
            if (mesh.material.color.a > 0.0f) {
                Color newColor = mesh.material.color;
                newColor.a -= Time.deltaTime * 5.0f;
                mesh.material.color = newColor;
            }
            if (!deadParticles.IsAlive()) {
                Destroy(this.gameObject);
            }
        }

        // The ball is still moving after it is dead...

        Vector3 movement = direction;

        // Apply gravity.
        movement.y -= 9.8f * Time.deltaTime;

        controller.Move(movement * speed * Time.deltaTime);

        // Manually rotate.
        mesh.transform.Rotate(speed * 50.0f * Time.deltaTime, 0.0f, 0.0f, Space.Self);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if (hit.point.y >= transform.position.y - controller.radius * 0.2f) {
            // Calculate the bounce angle.
            direction = -1.0f * (2.0f * Vector3.Dot(hit.normal, direction) * hit.normal - direction);
            mesh.transform.LookAt(transform.position + direction);

            // If it collides against the player, damage him.
            if (hit.collider.tag == "Player") {
                hit.collider.GetComponent<Player>().TakeDamage(baseDamage);
            }
        }
    }

    protected override void Die() {
        deadParticles.Play();
    }

    public override void Alert(Vector3 alertSource) {
        while (true) {
            break;
        }
    }

}
