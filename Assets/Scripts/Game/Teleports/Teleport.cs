using UnityEngine;
using System.Collections;

public class Teleport : MonoBehaviour {

    public ParticleSystem particles;
    public Color openedColor;
    public Color closedColor;

    public delegate void TeleportEntered();
    public TeleportEntered OnTeleportEntered;

    private bool opened = false;

    public void Close() {
        opened = false;
        particles.startColor = closedColor;
    }

    public void Open() {
        opened = true;
        particles.startColor = openedColor;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            // NOTE. If more teleports would be needed, specialize this method.
            if (opened) {
                OnTeleportEntered();    // Delegate. Implemented by MissionController.cs
            }
        }
    }

}