using UnityEngine;
using System.Collections;

abstract public class PuzzlePiece : MonoBehaviour {

    // Delegates.
    public delegate void PieceImpacted(byte number = 0);
    public PieceImpacted OnPieceImpacted;

    abstract public void Impact();
	
}
