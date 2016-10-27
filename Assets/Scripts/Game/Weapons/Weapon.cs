using UnityEngine;
using System.Collections;

abstract public class Weapon : MonoBehaviour {

    public enum WeaponTypes {
        LASER = 0, SHOTGUN = 1, BAZOOKA = 2, RAY = 3, CLEANER = 4
    }

    public bool picked;
    public int maxAmmo;
    public int ammo;
    public int maxPower;
    public int power;
    public float baseDamage;
    public float fireRate;
    public float accuracyCapacity;

    // Delegates.
    public delegate void VampibotEffect(float amount);
    public VampibotEffect OnVampibotEffect;

    protected float nextFire;

    abstract public bool Shoot(bool isVampire = false, bool isQuick = false);
    abstract public bool UpdatePassiveEffect(bool isVampire = false, bool isQuick = false);

    public void SetPicked(bool picked) {
        this.picked = picked;
    }

    public bool IsPicked() {
        return picked;
    }

    public void AddAmmo(int amount) {
        ammo += amount;
        if (ammo > maxAmmo) {
            ammo = maxAmmo;
        }
    }

    // For the vampire power up.
    protected void HealPlayer(float amount) {
        OnVampibotEffect(amount);   // Delegate. Implemented by Player.cs
    }

    abstract public void AddInitAmmo(); // Each weapon has an initial quantity of ammo.

}
