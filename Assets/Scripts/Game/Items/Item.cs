using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

    public float movementRange = 0.2f;
    public float movementSpeed = 0.4f;
    public float rotationSpeed = 60.0f;

    private float originalY;
    private float direction;

	void Start () {
        direction = 1.0f;
        originalY = transform.localPosition.y;
	}
	
	void Update () {
        // Movement.
        transform.Translate(0.0f, direction * movementSpeed * Time.deltaTime, 0.0f);
        if (transform.localPosition.y > originalY + movementRange) {
            direction = -1.0f;
        }
        else if (transform.localPosition.y < originalY - movementRange) {
            direction = 1.0f;
        }
        // Rotation.
        transform.Rotate(0.0f, rotationSpeed * Time.deltaTime, 0.0f);
	}
}
