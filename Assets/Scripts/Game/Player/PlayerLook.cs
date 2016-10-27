using UnityEngine;
using System.Collections;

public class PlayerLook : MonoBehaviour {

    public float xSensitivity = 7.0f;
    public float ySensitivity = 7.0f;
    public float smoothing = 14.0f;
    public float minDegrees = -85.0f;
    public float maxDegrees = 85.0f;
    public bool showCursor = false;

    private float xTargetRotation = 0.0f;
    private float yTargetRotation = 0.0f;
    private float xAxisMove;
    private float yAxisMove;

    void Start() {
        Cursor.visible = showCursor;
    }

    void LateUpdate() {
        xAxisMove = Input.GetAxis("Look X") * xSensitivity;
        xTargetRotation += xAxisMove;
        xTargetRotation = xTargetRotation % 360;

        yAxisMove = Input.GetAxis("Look Y") * ySensitivity;
        yTargetRotation -= yAxisMove;
        yTargetRotation = yTargetRotation % 360;
        yTargetRotation = Mathf.Clamp(yTargetRotation, minDegrees, maxDegrees);

        transform.parent.rotation = Quaternion.Lerp(transform.parent.rotation, Quaternion.Euler(0.0f, xTargetRotation, 0.0f), Time.deltaTime * smoothing);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(yTargetRotation, 0.0f, 0.0f), Time.deltaTime * smoothing);
    }

}