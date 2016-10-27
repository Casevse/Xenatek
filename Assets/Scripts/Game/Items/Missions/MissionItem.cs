using UnityEngine;
using System.Collections;

public class MissionItem : MonoBehaviour {

    public GameObject item;

    public delegate void MissionItemPicked();
    public MissionItemPicked OnMissionItemPicked;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            OnMissionItemPicked();  // Delegate. Implemented by the mission type class.
            Destroy(item);
        }
    }

}