using UnityEngine;
using System.Collections;

public class PanelsPuzzleController : PuzzleController {

    public PanelsPuzzlePiece[] pieces;
    public Color activatedColor;
    public Color completedColor;

    private bool solved = false;
    private byte[] combination = { 0, 1, 2, 3, 4 };
    private byte piecesActivated = 0;

	void Start () {
        // Generate the combination.
        Utils.Shuffle<byte>(combination);
        // Assign the delegates.
        for (int i = 0; i < pieces.Length; i++) {
            pieces[i].OnPieceImpacted = CheckPiece;
        }
	}

    private void CheckPiece(byte number) {
        if (!solved && !pieces[number].IsActivated()) {
            if (number == combination[piecesActivated]) {
                pieces[number].Activate(activatedColor);
                piecesActivated++;
                if (piecesActivated == pieces.Length) {
                    foreach (PanelsPuzzlePiece piece in pieces) {
                        piece.Activate(completedColor);
                    }
                    solved = true;
                    OpenDoor();
                }
            }
            else {
                // Reset all the pieces.
                foreach (PanelsPuzzlePiece piece in pieces) {
                    piece.Reset();
                }
                piecesActivated = 0;
            }
        }
    }

}
