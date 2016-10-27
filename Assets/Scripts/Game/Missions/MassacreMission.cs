using UnityEngine;
using System.Collections;

public class MassacreMission : Mission {

    private int totalEnemies = 0;
    private int enemiesLeft = 0;

    public MassacreMission() {
        name = "Destroy all enemies";
        progress = "Enemies: 0 / X";
    }

    public override void RegisterMissionElement(GameObject element) {
        element.GetComponent<Enemy>().OnEnemyKilled = UpdateProgress;
        requirementReached = true;  // With one enemy it is achieved.
        totalEnemies++;
        enemiesLeft++;
        progress = "Enemies: " + enemiesLeft.ToString() + " / " + totalEnemies.ToString();
    }

    private void UpdateProgress() {
        enemiesLeft--;
        if (enemiesLeft > 0) {
            progress = "Enemies: " + enemiesLeft.ToString() + " / " + totalEnemies.ToString();
            OnMissionInfoUpdated(false); // Delegate. Implemented by MissionController.cs
        }
        else {
            progress = "Completed";
            OnMissionInfoUpdated(true); // Delegate. Implemented by MissionController.cs
        }
    }

}