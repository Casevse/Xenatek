using UnityEngine;
using System.Collections;

public class PanelsPuzzlePiece : PuzzlePiece {

    public byte number;

    private bool activated = false;
    private Color originalColor;

    public void Start() {
        originalColor = GetComponent<Renderer>().material.color;
    }

    public override void Impact() {
        OnPieceImpacted(number);    // Delegate. Implemented by PanelsPuzzleController.cs
    }

    public bool IsActivated() {
        return activated;
    }

    public void Activate(Color color) {
        activated = true;
        GetComponent<Renderer>().material.color = color;
    }

    public void Reset() {
        activated = false;
        GetComponent<Renderer>().material.color = originalColor;
    }

}
