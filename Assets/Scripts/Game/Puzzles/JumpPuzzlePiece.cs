using UnityEngine;
using System.Collections;

public class JumpPuzzlePiece : MonoBehaviour {

    // NOTE: This class is not a child of PuzzlePiece.

    public PuzzleController controller;
    public Transform baseTransform;

    private bool solved = false;
    private Vector3 startForward;

    private void Start() {
        startForward = transform.parent.forward;
        // Put the platform from where the player can jump.
        float x = Random.Range(3.0f, 4.0f);
        if (Random.value > 0.5f) {
            x = -x;
        }
        baseTransform.localPosition = new Vector3(x, 0.5f, -0.75f);
    }

    private void OnTriggerEnter(Collider other) {
        if (!solved && other.gameObject.tag == "Player") {
            solved = true;
            Vector3 directorVector = other.transform.position - transform.position;
            if (Vector3.Cross(startForward, directorVector).y < 0.0f) {
                transform.parent.Rotate(0.0f, 30.0f, 0.0f);
            }
            else {
                transform.parent.Rotate(0.0f, -30.0f, 0.0f);
            }
            controller.OpenDoor();
        }
    }

}