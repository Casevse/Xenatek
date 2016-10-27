using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    public Button playButton;
    public Text currentProfileName;
    public Text currentProfileProgress;
    public Transform profilesListContentTransform;
    public GameObject profileButton;
    public GameObject newProfileButton;
    public Scrollbar profilesScrollbar;

    private void Awake() {
        
    }

    private void Start() {
        Cursor.visible = true;

        playButton.Select();    // NOTA: This works?

        // 1 - Update the current profile info.
        UpdateCurrentProfileInfo();

        // Deactivate the profile list GameObjects, because we only want them to clone them.
        profileButton.SetActive(false);
        newProfileButton.SetActive(false);

    }

    public void PlayGame() {
        Application.LoadLevel("Game");
    }

    public void ExitGame() {
        XenatekManager.xenatek.SaveProfiles();
        Application.Quit();
    }

    public void UpdateCurrentProfileInfo() {
        currentProfileName.text = string.Format("XTK-{0:000}", XenatekManager.xenatek.currentProfile + 1);
        currentProfileProgress.text = string.Format("Progress: {0} %", Mathf.CeilToInt(XenatekManager.xenatek.GetCurrentProfileProgress() * 100.0f));
    }

    // Add all the profiles to the button list.
    public void ShowProfiles() {
        // NOTE: If the canvas rendering is not represented in World Space, the position can't be changed.

        // First erase the content of the list (the clones).
        for (int i = profilesListContentTransform.childCount - 1; i >= 0; i--) {
            if (profilesListContentTransform.GetChild(i).gameObject.activeSelf) {
                // Only the active ones are clones.
                Destroy(profilesListContentTransform.GetChild(i).gameObject);
            }
        }

        // Resize the list according to the number of profiles.
        float[] profiles = XenatekManager.xenatek.GetProfiles();
        profilesListContentTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(300.0f, Mathf.Max(400.0f, 100.0f + 85.0f * profiles.Length));

        profilesScrollbar.value = 0.0f; // Necessary after the resizing.

        // Create the button for each profile.
        GameObject newClone;
        float currentY = -50.0f;    // Increments of -85.0f
        for (int i = 0; i < profiles.Length; i++) {
            newClone = Instantiate(profileButton) as GameObject;
            newClone.SetActive(true);
            newClone.GetComponent<RectTransform>().position = new Vector3(-10.0f, currentY, 0.0f);
            newClone.transform.SetParent(profilesListContentTransform, false);

            // Modify the text.
            newClone.transform.GetChild(0).GetComponent<Text>().text = string.Format("XTK-{0:000}", i + 1);
            newClone.transform.GetChild(1).GetComponent<Text>().text = string.Format("Progress: {0} %", Mathf.CeilToInt(profiles[i] * 100.0f));

            // Add a listener to the OnClick() event.
            int index = i;
            newClone.GetComponent<Button>().onClick.AddListener(() => XenatekManager.xenatek.ChangeCurrentProfile(index));
            // This has to be after, because the current profile is not updated yet.
            newClone.GetComponent<Button>().onClick.AddListener(() => UpdateCurrentProfileInfo());

            currentY += -85.0f;
        }

        // Finally, create the new profile's button.
        newClone = Instantiate(newProfileButton) as GameObject;
        newClone.SetActive(true);
        newClone.GetComponent<RectTransform>().position = new Vector3(-10.0f, currentY, 0.0f);
        newClone.transform.SetParent(profilesListContentTransform, false);
        newClone.GetComponent<Button>().onClick.AddListener(() => XenatekManager.xenatek.AddNewProfile());
        newClone.GetComponent<Button>().onClick.AddListener(() => UpdateCurrentProfileInfo());
    }

}