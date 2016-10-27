using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {
    
    // Create a global access point for the parameters of the level. The ideal would be do not use this method.
    public static Game game;

    public Level levelPrefab;
    public Player playerPrefab;
    public HUD hud;
    public MissionController missionController;
    public Transform minimapTransform;
    public Transform minimapCamera;
    public Transform minimapArrow;

    // NOTE: Maybe sometime this will need to be public.
    private Level level;
    private Player player;
    private float gameTime = 0.0f;

    // For optimizations.
    Vec2i currentZone;
    Vec2i lastZone;
    Vec2i currentRoom;
    Vec2i lastRoom;

    void Awake() {
        game = this;
    }

	void Start() {
        // Calculate what type of resources are generables depending on the profile's progress.
        LevelResources.instance.CalculateLimits(XenatekManager.xenatek.GetCurrentProfileProgress());

        // Create an instante for the level.
        level = Instantiate(levelPrefab) as Level;

        // Initialize the variables.
        level.name = "Level";
        level.SetMinimapTransform(minimapTransform);

        // For DEBUG...
        // level.width = XenatekManager.xenatek.levelWidth;
        // level.height = XenatekManager.xenatek.levelHeight;
        // level.generateMode = XenatekManager.xenatek.generateMode;

        GenerateLevelAndZoneSize();

        // Generate the level.
        level.Generate();

        // Asign the initial position of the player.
        Zone startZone = level.GetStartZone();
        Vector3 initialPosition = new Vector3(0.0f, 1.0f, 0.0f);
        initialPosition.x = startZone.x * startZone.width * Globals.ROOM_SIZE + startZone.centralRoom.x * Globals.ROOM_SIZE;
        initialPosition.z = startZone.y * startZone.height * Globals.ROOM_SIZE + startZone.centralRoom.y * Globals.ROOM_SIZE;

        player = Instantiate(playerPrefab, initialPosition, Quaternion.identity) as Player;
        player.name = "Player";

        // Initialize the current zone and current room variables.
        lastZone.x = Mathf.FloorToInt((transform.position.x + Globals.HALF_ROOM_SIZE) / Globals.ROOM_SIZE / XenatekManager.xenatek.zoneWidth);
        lastZone.y = Mathf.FloorToInt((transform.position.z + Globals.HALF_ROOM_SIZE) / Globals.ROOM_SIZE / XenatekManager.xenatek.zoneHeight);
        lastRoom.x = Mathf.FloorToInt((transform.position.x + Globals.HALF_ROOM_SIZE) / Globals.ROOM_SIZE) % XenatekManager.xenatek.zoneWidth;
        lastRoom.y = Mathf.FloorToInt((transform.position.z + Globals.HALF_ROOM_SIZE) / Globals.ROOM_SIZE) % XenatekManager.xenatek.zoneHeight;

        // Register the delegates for the player's health and update it for first time.
        player.OnHealthChanged = UpdateHealth;
        player.OnShieldChanged = UpdateShield;
        UpdateHealth();
        UpdateShield();

        // Register the weapon's delegates.
        player.OnWeaponSwitched = UpdateWeaponSlots;
        player.OnWeaponAmmoChanged = UpdateWeaponAmmo;

        // Register the item picking delegate.
        player.OnItemPicked = ShowItemPickedFeedback;

        // Register the power ups delegates (picking and updating).
        player.OnPowerUpChanged = UpdatePowerUpIcons;

        // Register the player's death delegate.
        player.OnPlayerDead = FinishGame;

        // Always start in the first region.
        hud.ChangeCurrentRegion(1);
        hud.ChangeCurrentMission(MissionController.instance.GetMissionByRegion(1).GetName());
        hud.ChangeCurrentProgress(MissionController.instance.GetMissionByRegion(1).GetProgress());

        // Register the progress mission update delegate.
        MissionController.instance.OnMissionInfoChanged = UpdateMissionProgress;

        // Register the level completed delegate.
        MissionController.instance.OnSimulationCompleted = FinishGame;

	}

    void Update() {
        // Count the game time.
        gameTime += Time.deltaTime;

        // Calculate in which zone is the player.
        currentZone.x = Mathf.FloorToInt((player.transform.position.x + Globals.HALF_ROOM_SIZE) / Globals.ROOM_SIZE / XenatekManager.xenatek.zoneWidth);
        currentZone.y = Mathf.FloorToInt((player.transform.position.z + Globals.HALF_ROOM_SIZE) / Globals.ROOM_SIZE / XenatekManager.xenatek.zoneHeight);
        if (currentZone != lastZone) {
            // Change the HUD info.
            int currentRegion = level.GetZoneByCoords(currentZone).region;
            if (currentRegion != level.GetZoneByCoords(lastZone).region) {
                hud.ChangeCurrentRegion(currentRegion);
                hud.ChangeCurrentMission(MissionController.instance.GetMissionByRegion(currentRegion).GetName());
                hud.ChangeCurrentProgress(MissionController.instance.GetMissionByRegion(currentRegion).GetProgress());
            }

            // Execute the optimization per zones.
            level.ChangeZone(currentZone, lastZone);
            hud.ChangeCurrentZone(currentZone);

            // Change the minimap's camera position.
            minimapCamera.localPosition = new Vector3(currentZone.x * Globals.ROOM_SIZE, 50.0f, currentZone.y * Globals.ROOM_SIZE);
            minimapArrow.localPosition = new Vector3(currentZone.x * Globals.ROOM_SIZE, 20.0f, currentZone.y * Globals.ROOM_SIZE);

            lastZone = currentZone;
        }

        // The minimap's arrow rotation always changes with the rotation of the player.
        minimapArrow.rotation = Quaternion.LookRotation(player.transform.forward);

        // Show the pause menu.
        if (Input.GetButtonDown("Menu")) {
            if (Time.timeScale == 0.0f) {
                ResumeGame();
            }
            else {
                PauseGame();
            }
        }
    }

    // Generates the level and the zones size.
    private void GenerateLevelAndZoneSize() {
        float progress = XenatekManager.xenatek.GetCurrentProfileProgress();
        float rest;
        Vec2i range;    // x = min, y = max

        // Reset the seeds.
        int profile = XenatekManager.xenatek.GetCurrentProfile();
        int seed = profile * 101 + Mathf.CeilToInt(progress * 100.0f);    // This can't be null.
        Random.seed = seed;
        Utils.randomGenerator = new System.Random(seed);

        // Level.
        if (progress < 0.2f) {
            range.x = 2;
            range.y = 3;
            rest = progress;
        }
        else if (progress < 0.4f) {
            range.x = 3;
            range.y = 4;
            rest = progress - 0.2f;
        }
        else if (progress < 0.6f) {
            range.x = 4;
            range.y = 5;
            rest = progress - 0.4f;
        }
        else if (progress < 0.8f) {
            range.x = 6;
            range.y = 7;
            rest = progress - 0.6f;
        }
        else {
            range.x = 7;
            range.y = 8;
            rest = progress - 0.8f;
        }

        XenatekManager.xenatek.levelWidth = Random.Range(range.x, range.y + 1);
        XenatekManager.xenatek.levelHeight = Random.Range(range.x, range.y + 1);

        // Zone.
        if (rest < 0.04f) {
            range.x = 1;
            range.y = 1;
        }
        else if (rest < 0.08f) {
            range.x = 2;
            range.y = 2;
        }
        else if (rest < 0.12f) {
            range.x = 3;
            range.y = 3;
        }
        else if (rest < 0.16f) {
            range.x = 4;
            range.y = 4;
        }
        else {
            range.x = 5;
            range.y = 5;
        }
        XenatekManager.xenatek.zoneWidth = Random.Range(range.x, range.y + 1);
        XenatekManager.xenatek.zoneHeight = Random.Range(range.x, range.y + 1);
    }

    // Update the HUD's player health.
    private void UpdateHealth(bool showFeedback = true) {
        hud.ChangeHealth(player.GetHealth(), player.GetMaxHealth(), showFeedback);
    }

    // Update the HUD's player shield.
    private void UpdateShield() {
        hud.ChangeShield(player.GetShield(), player.GetMaxShield());
    }

    // Update the HUD's weapon slots.
    private void UpdateWeaponSlots(int newWeapon, Weapon[] weapons) {
        hud.UpdateWeaponSlots(newWeapon, weapons);
    }

    // Update the HUD's current weapon ammo.
    private void UpdateWeaponAmmo(int currentAmmo, int maxPower, int currentPower) {
        hud.UpdateWeaponAmmo(currentAmmo, maxPower, currentPower);
    }

    // Shows feedback in the HUD when the player picks an object.
    private void ShowItemPickedFeedback() {
        hud.ShowItemPickedFeedback();
    }

    // Updates the HUD's power ups icons.
    private void UpdatePowerUpIcons(PowerUp.PowerUpTypes type, bool isActive) {
        hud.SetPowerUpIconActive(type, isActive);
    }

    // Updates the HUD's mission info.
    private void UpdateMissionProgress() {
        int currentRegion = level.GetZoneByCoords(currentZone).region;
        hud.ChangeCurrentProgress(MissionController.instance.GetMissionByRegion(currentRegion).GetProgress());
    }

    // Ends the game by player's death. Don't save the progress!
    private void FinishGame(bool completed) {
        if (completed) {
            // We have completed the game.
            hud.ShowEndEffect(true);
            player.FreezeMotor();
            XenatekManager.xenatek.IncrementCurrentProfileProgress(0.04f);  // Increate the progress a 4 %.
        }
        else {
            // We have died.
            hud.ShowEndEffect(false);
        }
        StartCoroutine(WaitAndExit(5.0f));
    }

    private IEnumerator WaitAndExit(float seconds) {
        yield return new WaitForSeconds(seconds);
        ExitGame();
    }

    private void PauseGame() {
        Time.timeScale = 0.0f;
        hud.SetPausePanelActive(true);
        hud.UpdateGameTime(gameTime);
        Cursor.visible = true;
    }

    public void ResumeGame() {
        Time.timeScale = 1.0f;
        hud.SetPausePanelActive(false);
        Cursor.visible = false;
    }

    public void ExitGame() {
        Application.LoadLevel("Menu");
    }

}