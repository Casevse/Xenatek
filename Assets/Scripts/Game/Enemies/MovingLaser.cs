using UnityEngine;
using System.Collections;

public class MovingLaser : Enemy {

    // Movement.
    public float horizontalRange = 5.0f;
    public float horizontalSpeed = 6.0f;
    public float verticalRange = 2.0f;
    public float verticalSpeed = 4.0f;
    public LineRenderer line;
    public ParticleSystem particleSystem;

    // Attack.
    public Transform attackSpawnPoint;

    // Movement again.
    private Transform parentTransform;
    private Vector3 middleHorizontal;
    private Vector3 maxHorizontal;
    private Vector3 minHorizontal;
    private Vector3 middleVertical;
    private Vector3 maxVertical;
    private Vector3 minVertical;
    private float movingRight = 1.0f;
    private float movingUp = 1.0f;
    private Vector3 right;
    private Vector3 up;
    private float horizontalDistance;
    private float verticalDistance;
    private bool middleXVisited = true;
    private bool middleYVisited = true;
    private float lastXDistance = 0.0f;
    private float lastYDistance = 0.0f;

    private void Start() {
        parentTransform = transform;    // NOTE: Before it was parent.transform
        right = parentTransform.TransformDirection(Vector3.right);
        up = parentTransform.TransformDirection(Vector3.up);

        // Horizontal axis.
        maxHorizontal = parentTransform.position + parentTransform.TransformDirection(Vector3.right) * horizontalRange;
        minHorizontal = parentTransform.position - parentTransform.TransformDirection(Vector3.right) * horizontalRange;
        // Limit the horizontal axis.
        middleHorizontal = Utils.NullifyVector3Axes(parentTransform.position, right);
        maxHorizontal = Utils.NullifyVector3Axes(maxHorizontal, right);
        minHorizontal = Utils.NullifyVector3Axes(minHorizontal, right);
        // Calculate the distance to the ends.
        horizontalDistance = Vector3.Distance(middleHorizontal, maxHorizontal);

        // Vertical axis.
        maxVertical = parentTransform.position + parentTransform.TransformDirection(Vector3.up) * verticalRange;
        minVertical = parentTransform.position - parentTransform.TransformDirection(Vector3.up) * verticalRange;
        // Limit the vertical axis.
        middleVertical = Utils.NullifyVector3Axes(parentTransform.position, up);
        maxVertical = Utils.NullifyVector3Axes(maxVertical, up);
        minVertical = Utils.NullifyVector3Axes(minVertical, up);
        // Calculate the distance to the ends.
        verticalDistance = Vector3.Distance(middleVertical, maxVertical);

        // Configure the particle system.
        particleSystem.startSpeed = attackRange * 0.5f;
        particleSystem.emissionRate = particleSystem.startSpeed * 40.0f;
    }

    private void Update() {

        // Movement.
        if (horizontalSpeed > 0.0f) {
            parentTransform.Translate(movingRight * right * horizontalSpeed * Time.deltaTime, Space.World);

            float xDistance = Vector3.Distance(Utils.NullifyVector3Axes(parentTransform.position, right), middleHorizontal);

            if (xDistance > lastXDistance) {
                middleXVisited = true;
            }
            lastXDistance = xDistance;

            if (middleXVisited && xDistance > horizontalDistance) {
                movingRight = movingRight * -1.0f;
                middleXVisited = false;
            }
        }

        if (verticalSpeed > 0.0f) {
            parentTransform.Translate(movingUp * up * verticalSpeed * Time.deltaTime, Space.World);

            float yDistance = Vector3.Distance(Utils.NullifyVector3Axes(parentTransform.position, up), middleVertical);

            if (yDistance > lastYDistance) {
                middleYVisited = true;
            }
            lastYDistance = yDistance;

            if (middleYVisited && yDistance > verticalDistance) {
                movingUp = movingUp * -1.0f;
                middleYVisited = false;
            }
        }

        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position + transform.forward * attackRange);

        // Attack.
        if (Time.time > nextAttack) {
            RaycastHit hit;
            if (Physics.Raycast(attackSpawnPoint.position, parentTransform.TransformDirection(Vector3.forward), out hit, attackRange)) {
                if (hit.collider.tag == "Player") {
                    hit.collider.GetComponent<Player>().TakeDamage(baseDamage);
                }
            }
            nextAttack = Time.time + attackRate;
        }

    }

    protected override void Die() {
        Destroy(parentTransform.gameObject);
    }

    public override void Alert(Vector3 alertSource) {
        // I don't remember the reason of this...
        while (true) {
            break;
        }
    }

}
