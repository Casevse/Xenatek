using UnityEngine;
using System.Collections;

public class RotatoryPuzzleController : PuzzleController {

    public RotatoryPuzzlePiece[] pieces;
    public Renderer[] targetRenderers;
    public Color targetActivatedColor;
    public Color targetSolvedColor;

    private bool solved = false;
    private byte targetsActivated = 0;
    private Color targetDeactivatedColor;

	void Start () {
        // Assign delegates.
        for (int i = 0; i < pieces.Length; i++) {
            pieces[i].OnPieceImpacted = CheckPiece;
        }
        targetDeactivatedColor = targetRenderers[0].material.color;
	}

    private void CheckPiece(byte number) {
        if (!solved && pieces[number].IsActivated()) {
            float yRotation = pieces[number].transform.localRotation.y;
            if (yRotation > -0.21f && yRotation < 0.21f) {
                pieces[number].SetActivated(false);
                targetRenderers[number].material.color = targetActivatedColor;
                targetsActivated++;
                if (targetsActivated == pieces.Length) {
                    solved = true;
                    foreach (Renderer renderer in targetRenderers) {
                        renderer.material.color = targetSolvedColor;
                    }
                    OpenDoor();
                }
            }
            else {
                // Reset all the pieces.
                foreach (RotatoryPuzzlePiece piece in pieces) {
                    piece.SetActivated(true);
                }
                // Reset the targets.
                foreach (Renderer renderer in targetRenderers) {
                    renderer.material.color = targetDeactivatedColor;
                }
                targetsActivated = 0;
            }
        }
    }

}