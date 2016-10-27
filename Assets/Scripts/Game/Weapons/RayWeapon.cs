using UnityEngine;
using System.Collections;

public class RayWeapon : Weapon {

    public int initAmmo = 50;
    public float maxRange = 50.0f;
    public int powerPerShot = 1;
    public float dischargeRate = 0.02f;
    public Transform cameraTransform;
    public GameObject ray;
    public LineRenderer line;
    public EllipsoidParticleEmitter emitter;    // NOTE: Not used for the moment.

    private bool shooting = false;
    private Color rayColor = new Color(1.0f, 0.0f, 0.0f);
    private bool darkenColor = true;
    private float nextDischarge;

    private void Start() {
        // NOTE: For the moment, particles are not used.
        emitter.gameObject.SetActive(false);
    }

    public override bool Shoot(bool isVampire = false, bool isQuick = false) {
        if (ammo > 0) {
            ray.SetActive(true);
            shooting = true;
            if (Time.time > nextFire) {

                // Increase the power each time a raycast is made.
                power += powerPerShot;
                if (power > maxPower) {
                    power = maxPower;
                }

                // Add dispersion to the direction of the ray.
                Vector3 forward = cameraTransform.forward;
                forward.x += Random.Range(-0.03f + accuracyCapacity * 0.02f, 0.03f - accuracyCapacity * 0.02f);
                forward.y += Random.Range(-0.03f + accuracyCapacity * 0.02f, 0.03f - accuracyCapacity * 0.02f);
                forward.z += Random.Range(-0.03f + accuracyCapacity * 0.02f, 0.03f - accuracyCapacity * 0.02f);

                // Adjust the longitude and the direction of the ray. Check if there is an impact with an enemy.
                RaycastHit hit;
                if (Physics.Raycast(cameraTransform.position, forward, out hit, maxRange)) {
                    // If the raycast impacts, shoot exactly in that direction.
                    line.SetPosition(0, ray.transform.position);
                    line.SetPosition(1, hit.point);

                    // If we hit the enemy, damage him.
                    if (hit.collider.tag == "Enemy") {
                        // Damage the enemy. More power = more damage.
                        hit.collider.GetComponent<Enemy>().TakeDamage(baseDamage + power * 0.025f);
                        if (isVampire) {
                            OnVampibotEffect(baseDamage + power * 0.025f);
                        }
                    }
                    else if (hit.collider.tag == "Puzzle") {
                        hit.collider.GetComponent<PuzzlePiece>().Impact();
                    }
                }
                else {
                    // If the raycast does not impact, shoot to the point in the perpendicular direction of the camera.
                    line.SetPosition(0, ray.transform.position);
                    line.SetPosition(1, ray.transform.position + forward * maxRange);
                }

                if (isQuick) {
                    nextFire = Time.time + fireRate * 0.5f;
                }
                else {
                    nextFire = Time.time + fireRate;
                }

                ammo--;
                return true;
            }
        }
        return false;
    }

    public override bool UpdatePassiveEffect(bool isVampire = false, bool isQuick = false) {
        // Modify the color if the ray is showing.
        if (shooting) {
            if (darkenColor) {
                rayColor = new Color(rayColor.r - 0.04f, rayColor.g + 0.04f, 0.0f);
                if (rayColor.r < 0.7f) {
                    darkenColor = false;
                }
            }
            else {
                rayColor = new Color(rayColor.r + 0.04f, rayColor.g - 0.04f, 0.0f);
                if (rayColor.r > 1.0f) {
                    darkenColor = true;
                }
            }
            line.SetColors(rayColor, new Color(rayColor.g + 0.5f, 0.0f, 0.0f));
            shooting = false;
        }
        else {
            ray.SetActive(false);
        }
        // Decrease the power as the time passes.
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
