using UnityEngine;
using System.Collections;

public class MissionController : MonoBehaviour {

    // Accessible globally in all the scene.
    public static MissionController instance;

    public Mission[] missions;
    public Door[] doors;
    public Teleport exitTeleport;

    public delegate void MissionInfoChanged();
    public MissionInfoChanged OnMissionInfoChanged;

    public delegate void SimulationCompleted(bool completed);
    public SimulationCompleted OnSimulationCompleted;

    private int numberOfMissions;
    private int currentMission;  // [0, numberOfMissions - 1]

    private void Awake() {
        instance = this;
    }

    public void GenerateMissions(int amount) {
        numberOfMissions = amount;
        missions = new Mission[numberOfMissions];
        for (int i = 0; i < numberOfMissions; i++) {
            Mission.MissionTypes type = (Mission.MissionTypes)Random.Range(0, (int)Mission.MissionTypes.COUNT);
            switch (type) {
                case Mission.MissionTypes.ORB:
                    missions[i] = ScriptableObject.CreateInstance<OrbMission>();
                    break;
                case Mission.MissionTypes.MASSACRE:
                    missions[i] = ScriptableObject.CreateInstance<MassacreMission>();
                    break;
                case Mission.MissionTypes.GATHER:
                    missions[i] = ScriptableObject.CreateInstance<GatherMission>();
                    break;
            }
            missions[i].OnMissionInfoUpdated = CheckMissionStatus;
        }
        currentMission = 0;

        // Initialize the array of doors.
        doors = new Door[numberOfMissions - 1];

    }

    public void RegisterRegionDoor(Door door, int region) {
        doors[region - 1] = door;
    }

    public void RegisterExitTeleport(Teleport teleport) {
        exitTeleport = teleport;
        exitTeleport.OnTeleportEntered = FinishGame;
    }

    public Mission GetMissionByRegion(int region) {
        return missions[region - 1];
    }

    private void CheckMissionStatus(bool completed) {
        if (completed) {
            if (currentMission < numberOfMissions - 1) {
                doors[currentMission].Open();
            }
            else {
                // Activate the exit teleport.
                exitTeleport.Open();
            }
            currentMission++;
        }
        OnMissionInfoChanged();
    }

    private void FinishGame() {
        OnSimulationCompleted(true);    // Delegate. Implemented by Game.cs
    }

}