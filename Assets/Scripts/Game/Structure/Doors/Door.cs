using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

    public Color openedColor;
    public Color closedColor;
    public Renderer mesh;
    public GameObject playerBlocking;
    public TextMesh text;
    public Transform leftPart;
    public Transform rightPart;
    public DoorTrigger trigger;

    private bool closing = false;
    private float speed = 10.0f;

    private void Awake() {
        // By default is not shown.
        text.gameObject.SetActive(false);
        // Register the trigger's delegates.
        trigger.OnPlayerTouching = MoveParts;
        trigger.OnPlayerLeaving = StartClosing;
    }

    private void Update() {
        if (mesh.material.color == openedColor && closing) {
            if (leftPart.localPosition.x < 0.0f) {
                leftPart.Translate(speed * Time.deltaTime, 0.0f, 0.0f);
                rightPart.Translate(-speed * Time.deltaTime, 0.0f, 0.0f);
                if (leftPart.localPosition.x > 0.0f) {
                    leftPart.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                    rightPart.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                    closing = false;
                }
            }
        }
    }

    public void Open() {
        playerBlocking.SetActive(false);
        mesh.material.color = openedColor;
    }

    public void Close() {
        playerBlocking.SetActive(true);
        mesh.material.color = closedColor;
    }

    public void ShowRegionText(int region) {
        text.gameObject.SetActive(true);
        text.text = region.ToString();
    }

    private void MoveParts() {
        // One way of check if it is open...
        if (mesh.material.color == openedColor) {
            if (leftPart.localPosition.x > -2.0f) {
                leftPart.Translate(-speed * Time.deltaTime, 0.0f, 0.0f);
                rightPart.Translate(speed * Time.deltaTime, 0.0f, 0.0f);
                if (leftPart.localPosition.x < -2.0f) {
                    leftPart.localPosition = new Vector3(-2.0f, 0.0f, 0.0f);
                    rightPart.localPosition = new Vector3(2.0f, 0.0f, 0.0f);
                }
            }
        }
    }

    private void StartClosing() {
        closing = true;
    }

}