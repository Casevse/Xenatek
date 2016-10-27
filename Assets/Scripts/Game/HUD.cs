using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    public Slider healthSlider;
    public Image healthSliderFill;
    public Slider shieldSlider;
    public Text regionText;
    public Text missionText;
    public Text progressText;
    public Text zoneText;
    public Text roomText;
    public Image screenFeedback;
    public float feedbackTime = 0.5f;   // NOTE: This can be done in relation to the amount.
    public Image[] weaponImages;
    public Text ammoText;
    public Slider powerSlider;
    public Image[] powerUpImages;
    public Text endText;
    public Image pausePanel;
    public Text gameTimeText;

    private float feedbackCurrent = 0.0f;
    private float feedbackHalf = 0.0f;
    private float feedbackEnd = 0.0f;
    private bool showingFeedback = false;
    private bool showingEndEffect = false;

    private void Update() {

        // Show the feedback if it is necessary.
        if (showingEndEffect) {
            // This is for when we die or win the game.
            feedbackCurrent += Time.deltaTime;
            screenFeedback.color = new Color(screenFeedback.color.r, screenFeedback.color.g, screenFeedback.color.b, feedbackCurrent / feedbackEnd);
        }
        else if (showingFeedback) {
            float alpha;
            feedbackCurrent += Time.deltaTime;
            if (feedbackCurrent < feedbackHalf) {
                alpha = feedbackCurrent / feedbackHalf;
            }
            else if (feedbackCurrent < feedbackEnd) {
                alpha = 1.0f - (feedbackCurrent - feedbackHalf) / feedbackHalf;
            }
            else {
                alpha = 0.0f;
                showingFeedback = false;
            }
            Color newColor = new Color(screenFeedback.color.r, screenFeedback.color.g, screenFeedback.color.b, alpha * 0.25f);
            screenFeedback.color = newColor;
        }

    }

    public void ChangeCurrentRegion(int region) {
        regionText.text = "Region " + region;
    }

    public void ChangeCurrentMission(string name) {
        missionText.text = name;
    }

    public void ChangeCurrentProgress(string progress) {
        progressText.text = progress;
    }

    public void ChangeCurrentZone(Vec2i zone) {
        zoneText.text = "Zone: " + zone.x + " / " + zone.y;
    }

    public void ChangeCurrentRoom(Vec2i room) {
        roomText.text = "Room: " + room.x + " / " + room.y;
    }

    public void ChangeHealth(float current, float max, bool showFeedback = true) {
        float percentage = current / max;
        if (showFeedback && percentage < healthSlider.value) {
            // Show the loss health feedback.
            ShowFeedback(new Color(1.0f, 0.0f, 0.0f, 0.0f));
        }
        else if (showFeedback && percentage > healthSlider.value) {
            // Show the restore health feedback.
            ShowFeedback(new Color(0.0f, 0.5f, 0.0f, 0.0f));
        }
        healthSlider.value = percentage;
        healthSliderFill.color = new Color((1.0f - percentage), percentage * 2.0f, 0.0f);
    }

    public void ChangeShield(float current, float max) {
        float percentage = current / max;
        if (percentage < shieldSlider.value) {
            // Show the shield damaged feedback.
            ShowFeedback(new Color(0.0f, 0.0f, 1.0f, 0.0f));
        }
        shieldSlider.value = current / max;
    }

    private void ShowFeedback(Color color) {
        if (!showingEndEffect) {
            feedbackCurrent = 0.0f;
            feedbackHalf = feedbackTime / 2.0f;
            feedbackEnd = feedbackTime;
            screenFeedback.color = color;
            showingFeedback = true;
        }
    }

    // Updates the weapon slots of the HUD.
    public void UpdateWeaponSlots(int newWeapon, Weapon[] weapons) {
        for (int i = 0; i < weaponImages.Length; i++) {
            if (i == newWeapon) {
                weaponImages[i].color = new Color(1.0f, 1.0f, 1.0f);
            }
            else if (weapons[i].IsPicked()) {
                weaponImages[i].color = new Color(0.3f, 0.3f, 0.3f);
            }
            else {
                weaponImages[i].color = new Color(0.1f, 0.1f, 0.1f);
            }
        }
    }

    // Updates the current weapon ammo in the HUD.
    public void UpdateWeaponAmmo(int currentAmmo, int maxPower, int currentPower) {
        if (currentAmmo == -1) {    // Infinite ammo.
            ammoText.text = "-";
        }
        else {
            ammoText.text = currentAmmo.ToString();
        }
        powerSlider.value = currentPower / (float)maxPower;
    }

    // Shows the pick object feedback.
    public void ShowItemPickedFeedback() {
        ShowFeedback(new Color(1.0f, 1.0f, 0.0f, 0.0f));
    }

    // Shows/hides a power up icon.
    public void SetPowerUpIconActive(PowerUp.PowerUpTypes type, bool isActive) {
        powerUpImages[(int)type].gameObject.SetActive(isActive);
    }

    // Shows the defeat effect.
    public void ShowEndEffect(bool completed) {
        feedbackCurrent = 0.0f;
        feedbackEnd = 4.0f;
        showingEndEffect = true;
        endText.gameObject.SetActive(true);
        if (completed) {
            screenFeedback.color = new Color(0.0f, 0.5f, 1.0f, 0.0f);
            endText.text = "Completed";
        }
        else {
            screenFeedback.color = new Color(1.0f, 0.0f, 0.0f, 0.0f);
            endText.text = "Defeated";
        }
    }

    // Activates or deactivates the pause panel.
    public void SetPausePanelActive(bool active) {
        pausePanel.gameObject.SetActive(active);
    }

    // Updates the game time in the HUD.
    public void UpdateGameTime(float gameTime) {
        string time = System.TimeSpan.FromSeconds(gameTime).ToString();
        time = time.Substring(0, time.LastIndexOf('.'));
        gameTimeText.text = "Time elapsed: " + time; 
    }

}