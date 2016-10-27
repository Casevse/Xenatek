using UnityEngine;
using System.Collections;

public class GatherMission : Mission {

    private int totalFragments = 0;
    private int fragmentsLeft = 0;
    private int minFragments = 5;   // Minimal fragments per mission.

    public GatherMission() {
        name = "Gather the fragments";
        progress = "Fragments: 0 / X";
    }

    public override void RegisterMissionElement(GameObject element) {
        element.GetComponent<MissionItem>().OnMissionItemPicked = PickFragment;
        totalFragments++;
        fragmentsLeft++;
        if (totalFragments == minFragments) {
            requirementReached = true;
        }
        progress = progress = "Fragments: " + fragmentsLeft.ToString() + " / " + totalFragments.ToString();
    }

    private void PickFragment() {
        fragmentsLeft--;
        if (fragmentsLeft > 0) {
            progress = "Fragments: " + fragmentsLeft.ToString() + " / " + totalFragments.ToString();
            OnMissionInfoUpdated(false); // Delegate. Implemented by MissionController.cs
        }
        else {
            progress = "Completed";
            OnMissionInfoUpdated(true); // Delegate. Implemented by MissionController.cs
        }
    }

}