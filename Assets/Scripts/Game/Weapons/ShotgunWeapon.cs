using UnityEngine;
using System.Collections;

public class ShotgunWeapon : Weapon {

    public int initAmmo = 10;
    public int bulletsPerShot = 15;
    public int powerPerShot = 20;
    public float dischargeRate = 0.05f;
    public ShotgunBullet bullet;
    public Transform spawnPoint;

    private float nextDischarge;

    public override bool Shoot(bool isVampire = false, bool isQuick = false) {
        if (ammo > 0 && Time.time > nextFire) {

            // Increase the power each time we shoot.
            power += powerPerShot;
            if (power > maxPower) {
                power = maxPower;
            }

            // Increase the bullet number taking in count the power.
            int amount = bulletsPerShot + (int)(power * 0.1f);

            // Create some bullets.
            for (int i = 0; i < amount; i++) {
                ShotgunBullet bulletClone = Instantiate(bullet, spawnPoint.position, transform.rotation) as ShotgunBullet;
                bulletClone.SetDamage(baseDamage, isVampire);
                bulletClone.OnTargetReached = HealPlayer;   // Register the vampibot delegate.
                Vector3 euler = bulletClone.transform.localRotation.eulerAngles;
                euler.x += Random.Range(-10.0f + accuracyCapacity * 4.0f, 10.0f - accuracyCapacity * 4.0f);
                euler.y += Random.Range(-10.0f + accuracyCapacity * 4.0f, 10.0f - accuracyCapacity * 4.0f);
                bulletClone.transform.localRotation = Quaternion.Euler(euler);
            }

            if (isQuick) {
                nextFire = Time.time + fireRate * 0.5f;
            }
            else {
                nextFire = Time.time + fireRate;
            }

            ammo--;
            nextDischarge = Time.time + dischargeRate;
            return true;
        }

        return false;
    }

    public override bool UpdatePassiveEffect(bool isVampire = false, bool isQuick = false) {
        if (power > 0 && Time.time > nextDischarge) {
            power--;
            nextDischarge = Time.time + dischargeRate;
            return true;
        }
        return false;
    }

    public override void AddInitAmmo() {
        AddAmmo(initAmmo);
    }

}
