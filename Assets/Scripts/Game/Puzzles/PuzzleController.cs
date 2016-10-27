using UnityEngine;
using System.Collections;

public class PuzzleController : MonoBehaviour {

    private Door door;

    public void SetDoor(Door door) {
        this.door = door;
    }

    public void OpenDoor() {
        door.Open();
    }

}