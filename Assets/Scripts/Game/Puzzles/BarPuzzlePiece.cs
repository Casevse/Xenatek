using UnityEngine;
using System.Collections;

public class BarPuzzlePiece : PuzzlePiece {

    public PuzzleController controller;
    public Transform barTransform;
    public Color[] colors;
    public float changeRate = 1.0f;
    public float chargeAmount = 0.075f;
    public float dischargeAmount = 0.001f;
    public float dischargeRate = 0.025f;

    private bool solved = false;
    private float value = 0.0f;
    private float lastValue = 0.0f;
    private float nextDischarge = 0.0f;
    private float startChangeRate;
    private int currentColorIndex = 0;
    private float nextChange = 0.0f;
    private Renderer rend;

	void Start () {
	    startChangeRate = changeRate;
        rend = GetComponent<Renderer>();
	}
	
	void Update () {
        if (!solved) {
            if (Time.time > nextDischarge) {
                value -= dischargeAmount;
                if (value < 0.01f) {
                    value = 0.01f;
                }
                nextDischarge = Time.time + dischargeRate;
            }
            if (Time.time > nextChange) {
                currentColorIndex++;
                if (currentColorIndex == colors.Length) {
                    currentColorIndex = 0;
                }
                rend.material.color = colors[currentColorIndex];
                changeRate = startChangeRate + startChangeRate * Random.value;  // Vary a little the color change frecuence.
                nextChange = Time.time + changeRate;
            }
            if (value != lastValue) {
                ChangeBarSize();
            }
        }
	}

    public override void Impact() {
        if (currentColorIndex == 0) {   // The 0 is the correct color.
            value += chargeAmount;
            if (value >= 1.0f) {
                value = 1.0f;
                solved = true;
                ChangeBarSize();
                controller.OpenDoor();
            }
        }
        else {
            value -= chargeAmount * 2.0f;
            if (value < 0.01f) {
                value = 0.01f;
            }
        }
    }

    private void ChangeBarSize() {
        barTransform.localPosition = new Vector3(value * -1.0f, 0.0f, 0.0f);
        barTransform.localScale = new Vector3(value * 2.0f, 0.35f, 0.1f);
        lastValue = value;
    }

}
