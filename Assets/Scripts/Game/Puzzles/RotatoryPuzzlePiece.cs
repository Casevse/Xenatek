using UnityEngine;
using System.Collections;

public class RotatoryPuzzlePiece : PuzzlePiece {

    public byte number;

    private bool activated = true;

    private void Update() {
        if (activated) {
            transform.Rotate(0.0f, Time.deltaTime * 200.0f, 0.0f);
            // The Y rotation has to be between -0.20 and 0.20.
        }
    }

    public override void Impact() {
        OnPieceImpacted(number);    // Delegate. Implemented by RotatoryPuzzleController.cs
    }

    public bool IsActivated() {
        return activated;
    }

    public void SetActivated(bool value) {
        activated = value;
    }

}