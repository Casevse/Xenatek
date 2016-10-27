using UnityEngine;
using System.Collections;

public class BazookaWeapon : Weapon {

    public int initAmmo = 10;
    public float rechargeRate = 0.075f;
    public int powerLossPerShot = 30;
    public float defaultSpeed = 50.0f;
    public BazookaBullet bullet;
    public Transform spawnPoint;
    public Transform cameraTransform;

    private float nextRecharge;

    public override bool Shoot(bool isVampire = false, bool isQuick = false) {
        if (ammo > 0 && Time.time > nextFire) {
            // Create a bullet.
            BazookaBullet bulletClone = Instantiate(bullet, spawnPoint.position, transform.rotation) as BazookaBullet;
            bulletClone.SetDamage(baseDamage, isVampire);
            bulletClone.OnTargetReached = HealPlayer;   // Register the vampibot delegate.

            // Adjust the direction of the bullet.
            RaycastHit hit;
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 500.0f)) {
                // If the raycast impacts, shoot exactly in that direction.
                bulletClone.transform.LookAt(hit.point);
            }
            else {
                // If the raycast does not impact, shoot to the point in the perpendicular direction of the camera.
                bulletClone.transform.LookAt(spawnPoint.position + cameraTransform.forward * 500.0f);
            }

            // Adjust the dispersion.
            Vector3 euler = bulletClone.transform.localRotation.eulerAngles;
            euler.x += Random.Range(-5.0f + accuracyCapacity * 4.0f, 5.0f - accuracyCapacity * 4.0f);
            euler.y += Random.Range(-5.0f + accuracyCapacity * 4.0f, 5.0f - accuracyCapacity * 4.0f);
            bulletClone.transform.localRotation = Quaternion.Euler(euler);

            // Adjust the bullet speed taking in count the power.
            bulletClone.SetSpeed(defaultSpeed + power * 0.5f);

            // We lose power each time we shoot.
            power -= powerLossPerShot;
            if (power < 0) {
                power = 0;
            }

            if (isQuick) {
                nextFire = Time.time + fireRate * 0.5f;
            }
            else {
                nextFire = Time.time + fireRate;
            }

            ammo--;
            nextRecharge = Time.time + rechargeRate;
            return true;
        }
        return false;
    }

    public override bool UpdatePassiveEffect(bool isVampire = false, bool isQuick = false) {
        // Recharge the power.
        if (power < maxPower && Time.time > nextRecharge) {
            power++;
            nextRecharge = Time.time + rechargeRate;
            return true;
        }
        return false;
    }

    public override void AddInitAmmo() {
        AddAmmo(initAmmo);
    }

}
