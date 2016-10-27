using UnityEngine;
using System.Collections;

public class OrbMission : Mission {

    public OrbMission() {
        name = "Pick the orb";
        progress = "Orb: 0 / 1";
    }

    public override void RegisterMissionElement(GameObject element) {
        element.GetComponent<MissionItem>().OnMissionItemPicked = PickOrb;
    }

    private void PickOrb() {
        // If it has been picked, the mission is completed. Open the door.
        progress = "Completed";

        OnMissionInfoUpdated(true); // Delegate. Implemented by MissionController.cs
    }

}