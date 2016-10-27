using UnityEngine;
using System.Collections;

public class CleanerWeapon : Weapon {

    public int initAmmo = 100;
    public int minPowerToShoot = 10;
    public int ammoPerShot = 25;
    public float chargeRate = 0.05f;
    public CleanerBullet bullet;
    public Transform[] spawnPoints;

    private bool holding = false;
    private float nextCharge = 0.0f;

    public override bool Shoot(bool isVampire = false, bool isQuick = false) {
        holding = true;
        return false;
    }

    public override bool UpdatePassiveEffect(bool isVampire = false, bool isQuick = false) {
        if (holding) {
            if (power < maxPower && ammo >= ammoPerShot && Time.time > nextCharge) {
                power++;

                if (isQuick) {
                    nextCharge = Time.time + chargeRate * 0.5f;
                }
                else {
                    nextCharge = Time.time + chargeRate;
                }
            }
            holding = false;
            return true;
        }
        else if (power > minPowerToShoot && ammo >= ammoPerShot) {
            // NOTE: This weapon does not take in count the accuracy.
            // Shoot multiple bullets.
            for (int i = 0; i < spawnPoints.Length; i++) {
                CleanerBullet bulletClone = Instantiate(bullet, spawnPoints[i].position, transform.rotation) as CleanerBullet;
                bulletClone.SetDamage(baseDamage + power / 10.0f, isVampire);
                bulletClone.OnTargetReached = HealPlayer;   // Register the vampibot delegate.
            }
            power = 0;
            ammo -= ammoPerShot;
            holding = false;
            return true;
        }
        else {
            holding = false;
            if (power > 0) {
                power = 0;
                return true;
            }
        }
        return false;
    }

    public override void AddInitAmmo() {
        AddAmmo(initAmmo);
    }

}
