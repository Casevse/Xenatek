using UnityEngine;
using System.Collections;

public class DartboardPuzzlePiece : PuzzlePiece {

    public PuzzleController controller;
    public Color[] colors;
    public Color hitColor;
    public float changeRate = 1.0f;
    public Renderer[] hitRenderers;

    private bool solved = false;
    private int currentColor = 0;
    private int correctColorIndex = 0;
    private int hits = 0;
    private float nextChange = 0.0f;
    private float startChangeRate = 0.0f;
    private Renderer rend;
    private Color startHitColor;
    private Color correctColor;

    private void Start() {
        correctColor = colors[0]; // The correct color is always the first.

        // Shuffle the colors array and pick the correct one position.
        ShuffleColors();

        rend = GetComponent<Renderer>();
        startHitColor = hitRenderers[0].material.color;
        startChangeRate = changeRate;
    }

    private void Update() {
        if (!solved && Time.time > nextChange) {
            ChangeColor();
        }
    }

    public override void Impact() {
        if (!solved) {
            // Check if it was the correct color.
            if (currentColor == correctColorIndex) {
                hitRenderers[hits].material.color = hitColor;
                hits++;
                if (hits == hitRenderers.Length) {
                    solved = true;
                    controller.OpenDoor();
                    rend.material.color = colors[correctColorIndex];
                }
                else {
                    ChangeColor();
                }
                ShuffleColors();    // Shuffle the colors again.
            }
            else {
                // Reset the impacts.
                hits = 0;
                foreach (Renderer r in hitRenderers) {
                    r.material.color = startHitColor;
                }
                ChangeColor();
            }
        }

    }

    private void ChangeColor() {
        currentColor++;
        if (currentColor == colors.Length) {
            currentColor = 0;
        }
        rend.material.color = colors[currentColor];
        changeRate = startChangeRate - (startChangeRate * Random.value) * 0.5f; // Vary the change velocity for variety.
        nextChange = Time.time + changeRate;
    }

    private void ShuffleColors() {
        Utils.Shuffle<Color>(colors);

        int i = 0;
        while (colors[i] != correctColor) {
            i++;
        }
        correctColorIndex = i;
    }

}