using UnityEngine;
using System.Collections;

public class DoorTrigger : MonoBehaviour {

    // NOTE: In the Physics settings, deactivate the ray impact with the triggers.

    public delegate void PlayerTouching();
    public PlayerTouching OnPlayerTouching;

    public delegate void PlayerLeaving();
    public PlayerLeaving OnPlayerLeaving;

    private void OnTriggerStay(Collider other) {
        if (other.tag == "Player") {
            OnPlayerTouching(); // Delegate. Implemented by Door.cs
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            OnPlayerLeaving(); // Delegate. Implemented by Door.cs
        }
    }

}