using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public float maxHealth = 100.0f;
    public float health = 100.0f;
    public float maxShield = 100.0f;
    public float shield = 100.0f;
    public float shieldRechangeTime = 3.0f;
    public float shieldRechargeRate = 0.25f;
    public float shieldRechangeAmount = 1.0f;
    public float regenerationPowerUpRate = 0.25f;
    public Weapon[] weapons;

    // Delegates.
    public delegate void HealthChanged(bool showFeedback = true);
    public HealthChanged OnHealthChanged;
    public delegate void ShieldChanged();
    public ShieldChanged OnShieldChanged;
    public delegate void WeaponSwitched(int newWeapon, Weapon[] weapons);
    public WeaponSwitched OnWeaponSwitched;
    public delegate void WeaponAmmoChanged(int currentAmmo, int maxPower, int currentPower);
    public WeaponAmmoChanged OnWeaponAmmoChanged;
    public delegate void ItemPicked();
    public ItemPicked OnItemPicked;
    public delegate void PowerUpChanged(PowerUp.PowerUpTypes type, bool isActive);
    public PowerUpChanged OnPowerUpChanged;
    public delegate void PlayerDead(bool completed);
    public PlayerDead OnPlayerDead;

    private float toughnessCapacity;
    private PlayerMotor motor;
    private float nextShieldRecharge = 0.0f;
    private int currentWeapon = 0;
    private bool canChangeWeapon = true;
    private bool isDead = false;

    // Power Ups
    private bool[] powerUpsPicked = new bool[(int)PowerUp.PowerUpTypes.COUNT];
    private float[] powerUpsTime = new float[(int)PowerUp.PowerUpTypes.COUNT];
    private float nextRegeneration = 0.0f;

	void Start () {
        // Anular las capacidades.
        XenatekManager.xenatek.toughnessCapacity = 0.0f;
        XenatekManager.xenatek.speedCapacity = 1.0f;
        XenatekManager.xenatek.accuracyCapacity = 1.0f;

        toughnessCapacity = XenatekManager.xenatek.toughnessCapacity;
        motor = GetComponent<PlayerMotor>();
        OnWeaponSwitched(currentWeapon, weapons); // Delegate. Implemented by Game.cs
        OnWeaponAmmoChanged(weapons[currentWeapon].ammo, weapons[currentWeapon].maxPower, weapons[currentWeapon].power); // Delegate.Implemented by Game.cs
        PickWeapon(Weapon.WeaponTypes.LASER);   // The player always has the laser.
        // Tests.
        // PickWeapon(Weapon.WeaponTypes.SHOTGUN);
        // PickWeapon(Weapon.WeaponTypes.BAZOOKA);
        // PickWeapon(Weapon.WeaponTypes.RAY);
        // PickWeapon(Weapon.WeaponTypes.CLEANER);
    }
	
	void Update () {

        if (isDead) {
            return;
        }

        // Check the power ups.
        for (int i = 0; i < (int)PowerUp.PowerUpTypes.COUNT; i++) {
            if (powerUpsPicked[i]) {
                if (i == (int)PowerUp.PowerUpTypes.REGENERATION && Time.time > nextRegeneration) {
                    Heal(2.0f, false);
                    nextRegeneration = Time.time + regenerationPowerUpRate;
                }
                if (Time.time > powerUpsTime[i]) {
                    if (i == (int)PowerUp.PowerUpTypes.TURBO) {
                        motor.SetSpeedPowerUp(false);
                    }
                    powerUpsPicked[i] = false;
                    OnPowerUpChanged((PowerUp.PowerUpTypes)i, false);   // Delegate. Implemented by Game.cs
                }
            }
        }

        // Recharges the shield.
        if (shield < maxShield && Time.time > nextShieldRecharge) {
            shield += shieldRechangeAmount;
            if (shield > maxShield) {
                shield = maxShield;
            }
            nextShieldRecharge = Time.time + shieldRechargeRate;
            OnShieldChanged();  // Delegate. Implemented by Game.cs
        }

        // Shot with the weapon.
        if (Input.GetAxis("Fire") > 0.0f) {
            if (weapons[currentWeapon].Shoot(powerUpsPicked[(int)PowerUp.PowerUpTypes.VAMPIBOT], powerUpsPicked[(int)PowerUp.PowerUpTypes.QUICKFIRE])) {
                // Update the HUD.
                OnWeaponAmmoChanged(weapons[currentWeapon].ammo, weapons[currentWeapon].maxPower, weapons[currentWeapon].power);  // Delegate. Implemented by Game.cs
                // Alert the nearby enemies. Only check the enemies layer (NOTE: This can be used for the explosions).
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, 20.0f, 0x200);  // 0x200 = 512 = enemies layer.
                for (int i = 0; i < hitColliders.Length; i++) {
                    hitColliders[i].GetComponent<Enemy>().Alert(transform.position);
                }
            }
        }

        // Some weapons keep updating even if they are not equiped.
        for (int i = 0; i < weapons.Length; i++) {
            if (weapons[i].IsPicked()) {
                if (weapons[i].UpdatePassiveEffect(powerUpsPicked[(int)PowerUp.PowerUpTypes.VAMPIBOT], powerUpsPicked[(int)PowerUp.PowerUpTypes.QUICKFIRE])) {
                    OnWeaponAmmoChanged(weapons[currentWeapon].ammo, weapons[currentWeapon].maxPower, weapons[currentWeapon].power);  // Delegate. Implemented by Game.cs
                }
            }
        }

        // Change the weapon.
        if (Input.GetAxis("Switch Weapon") > 0.0f && canChangeWeapon) {
            ChangeWeapon(true);
            canChangeWeapon = false ;
        }
        else if (Input.GetAxis("Switch Weapon") < 0.0f && canChangeWeapon) {
            ChangeWeapon(false);
            canChangeWeapon = false;
        }
        else if (Input.GetAxis("Switch Weapon") == 0.0f && !canChangeWeapon) {
            canChangeWeapon = true;
        }

        // Change the weapon with numbers, only for platforms with keyboard.
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            ChangeWeapon(false, 0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            ChangeWeapon(false, 1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            ChangeWeapon(false, 2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) {
            ChangeWeapon(false, 3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) {
            ChangeWeapon(false, 4);
        }

	}

    // Returns the max health.
    public float GetMaxHealth() {
        return maxHealth;
    }

    // Returns the current health.
    public float GetHealth() {
        return health;
    }

    // Returns the max shield.
    public float GetMaxShield() {
        return maxShield;
    }

    // Returns the current shield.
    public float GetShield() {
        return shield;
    }

    // Damage the player.
    public void TakeDamage(float damage) {
        if (!isDead) {
            if (!powerUpsPicked[(int)PowerUp.PowerUpTypes.MEGAPROTECTION]) {
                shield -= damage;
                OnShieldChanged();  // Delegate. Implemented by Game.cs
                if (shield < 0.0f) {
                    shield -= shield * 0.5f * toughnessCapacity;    // Reduce the damage taken taking in count the toughness capacity.
                    health += shield;
                    if (health <= 0.0f) {
                        health = 0.0f;
                        isDead = true;
                        OnPlayerDead(false); // Delegate. Implemented by Game.cs
                        motor.Freeze(true);
                        weapons[currentWeapon].gameObject.SetActive(false);
                    }
                    OnHealthChanged();  // Delegate. Implemented by Game.cs
                    shield = 0.0f;
                }
                nextShieldRecharge = Time.time + shieldRechangeTime;
            }
        }
    }

    // Heals the player.
    public void Heal(float heal, bool showFeedback = true) {
        health += heal;
        OnHealthChanged(showFeedback);
        if (health > maxHealth) {
            health = maxHealth;
        }
    }

    // Changes the weapon.
    public void ChangeWeapon(bool isNext, int concreteWeapon = -1) {
        weapons[currentWeapon].gameObject.SetActive(false);
        if (concreteWeapon != -1) { // In case we choose the weapon with the keyboard.
            if (weapons[concreteWeapon].IsPicked()) {
                currentWeapon = concreteWeapon;
            }
        }
        else {
            do {
                if (isNext) {
                    currentWeapon++;
                    if (currentWeapon == weapons.Length) {
                        currentWeapon = 0;
                    }
                }
                else {
                    currentWeapon--;
                    if (currentWeapon < 0) {
                        currentWeapon = weapons.Length - 1;
                    }
                }
            } while (!weapons[currentWeapon].IsPicked());
        }
        weapons[currentWeapon].gameObject.SetActive(true);
        OnWeaponSwitched(currentWeapon, weapons); // Delegate. Implemented by Game.cs
        OnWeaponAmmoChanged(weapons[currentWeapon].ammo, weapons[currentWeapon].maxPower, weapons[currentWeapon].power);   // Delegate. Implemented by Game.cs
    }

    // Picks a weapon.
    public void PickWeapon(Weapon.WeaponTypes type) {
        // If the weapon was not picked before, equip the weapon.
        if (!weapons[(int)type].IsPicked()) {
            weapons[currentWeapon].gameObject.SetActive(false);
            currentWeapon = (int)type;
            weapons[currentWeapon].SetPicked(true);
            weapons[currentWeapon].gameObject.SetActive(true);
            weapons[currentWeapon].OnVampibotEffect = DoVampibotEffect;    // For the vampire power up.
            weapons[currentWeapon].accuracyCapacity = XenatekManager.xenatek.accuracyCapacity;  // Set the accuracy capacity.
            OnWeaponSwitched(currentWeapon, weapons); // Delegate. Implemented by Game.cs
        }
        // In any case, add some ammo when the weapon is picked.
        weapons[(int)type].AddInitAmmo();
        OnWeaponAmmoChanged(weapons[currentWeapon].ammo, weapons[currentWeapon].maxPower, weapons[currentWeapon].power);   // Delegate. Implemented by Game.cs
        OnItemPicked();
    }

    // Pick the ammo of a weapon.
    public void PickAmmo(Weapon.WeaponTypes type, int amount) {
        weapons[(int)type].AddAmmo(amount);
        if (currentWeapon == (int)type) {
            OnWeaponAmmoChanged(weapons[currentWeapon].ammo, weapons[currentWeapon].maxPower, weapons[currentWeapon].power);   // Delegate. Implemented by Game.cs
        }
        OnItemPicked();
    }

    // Add a power up to the player. The durations are directly placed here.
    public void PickPowerUp(PowerUp.PowerUpTypes type) {
        powerUpsPicked[(int)type] = true;
        switch (type) {
            case PowerUp.PowerUpTypes.REGENERATION:
                powerUpsTime[(int)type] = Time.time + 20.0f;
                break;
            case PowerUp.PowerUpTypes.TURBO:
                motor.SetSpeedPowerUp(true);
                powerUpsTime[(int)type] = Time.time + 40.0f;
                break;
            case PowerUp.PowerUpTypes.MEGAPROTECTION:
                shield = maxShield;
                OnShieldChanged();
                powerUpsTime[(int)type] = Time.time + 10.0f;
                break;
            case PowerUp.PowerUpTypes.VAMPIBOT:
                powerUpsTime[(int)type] = Time.time + 30.0f;
                break;
            case PowerUp.PowerUpTypes.QUICKFIRE:
                powerUpsTime[(int)type] = Time.time + 25.0f;
                break;
        }
        OnPowerUpChanged(type, true);   // Delegate. Implemented by Game.cs
    }

    // Restores the player's life in case of not have the vampire power up.
    private void DoVampibotEffect(float amount) {
        Heal(amount);
    }

    // Blocks the movement capacity.
    public void FreezeMotor() {
        motor.Freeze(false);
    }

}