using UnityEngine;
using System.Collections;

public class LaserWeapon : Weapon {

    public float rechargeRate;
    public LaserBullet bullet;
    public Transform spawnPoint;
    public Transform cameraTransform;

    private float nextRecharge;

    public override bool Shoot(bool isVampire = false, bool isQuick = false) {
        if (power > 0 && Time.time > nextFire) {
            power--;
            
            // Create the bullet.
            LaserBullet bulletClone = Instantiate(bullet, spawnPoint.position, transform.rotation) as LaserBullet;
            bulletClone.SetDamage(baseDamage, isVampire);
            bulletClone.OnTargetReached = HealPlayer;   // Register the vampibot delegate.

            // Adjust the bullet direction.
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

            if (isQuick) {
                nextFire = Time.time + fireRate * 0.5f;
            }
            else {
                nextFire = Time.time + fireRate;
            }

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
        // In this case, don't add anything.
    }

}
