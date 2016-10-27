using UnityEngine;
using System.Collections;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class XenatekManager : MonoBehaviour {

    // This class is the director of the video game.
    // It contains data shared between scenes.

    public static XenatekManager xenatek;

    // The 3 main stats. The value is normalized to make calculations easier.
    public float toughnessCapacity = 0.5f;
    public float speedCapacity = 0.5f;
    public float accuracyCapacity = 0.5f;

    // This properties have impact in the way the level is generated. For DEBUG.
    public int levelWidth    = 4;
    public int levelHeight   = 4;
    public int zoneWidth     = 4;
    public int zoneHeight    = 4;
    public GenerateMode generateMode = GenerateMode.NEWEST;

    // Profiles. 0..n
    public int currentProfile;
    private float[] profiles;
    private string path;

    private void Awake() {
        // This is the first that the video game executes.
        path = Application.persistentDataPath + "/save.dat";
        LoadProfiles();

        // Singleton pattern.
        if (xenatek == null) {
            DontDestroyOnLoad(gameObject);
            xenatek = this;
        }
        else if (xenatek != this) {
            Destroy(gameObject);
        }
    }

    private void Start() {

    }

    public void SaveProfiles() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path);

        // 1 - Save the number of profiles.
        bf.Serialize(file, profiles.Length);

        // 2 - Save each profile percentage.
        for (int i = 0; i < profiles.Length; i++) {
            bf.Serialize(file, profiles[i]);
        }

        // 3 - Save last profile used.
        bf.Serialize(file, currentProfile);

        file.Close();
    }

    public void LoadProfiles() {
        if (File.Exists(path)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);

            // 1 - Read the number of profiles.
            profiles = new float[(int)bf.Deserialize(file)];

            // 2 - Read each profile percentage.
            for (int i = 0; i < profiles.Length; i++) {
                profiles[i] = (float)bf.Deserialize(file);
            }

            // 3 - Read the last used profile.
            currentProfile = (int)bf.Deserialize(file);

            file.Close();
        }
        else {
            // If it does not exist, create a first profile.
            profiles = new float[1];
            currentProfile = 0;
            profiles[currentProfile] = 0.0f;
            SaveProfiles();
        }
    }

    public int GetCurrentProfile() {
        return currentProfile;
    }

    public float GetCurrentProfileProgress() {
        return profiles[currentProfile];
    }

    public void AddNewProfile() {
        // Save the profiles in a temporal array.
        float[] tempProfiles = profiles;

        // Resize the array.
        profiles = new float[profiles.Length + 1];

        // Copy the previous elements in the new array.
        for (int i = 0; i < profiles.Length - 1; i++) {
            profiles[i] = tempProfiles[i];
        }

        // Initialize the new profile.
        profiles[profiles.Length - 1] = 0.0f;
        currentProfile = profiles.Length - 1;

        // Save again all the profiles.
        SaveProfiles();
    }

    public float[] GetProfiles() {
        return profiles;
    }

    public void ChangeCurrentProfile(int newProfile) {
        currentProfile = newProfile;

        // Save again all the profiles.
        SaveProfiles();
    }

    public void IncrementCurrentProfileProgress(float amount) {
        profiles[currentProfile] += amount;
        SaveProfiles();
    }

}