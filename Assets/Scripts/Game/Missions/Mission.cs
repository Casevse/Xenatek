using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class Mission : ScriptableObject {

    public delegate void MissionInfoUpdated(bool completed);
    public MissionInfoUpdated OnMissionInfoUpdated;

    public enum MissionTypes {
        ORB = 0, MASSACRE = 1, GATHER = 2, COUNT
    }

    protected string name;
    protected string progress;
    protected bool requirementReached = false;

    public abstract void RegisterMissionElement(GameObject element);

    public string GetName() {
        return name;
    }

    public string GetProgress() {
        return progress;
    }

    public bool RequirementIsReached() {
        return requirementReached;
    }

}