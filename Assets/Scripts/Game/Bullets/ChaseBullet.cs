using UnityEngine;
using System.Collections;

public class ChaseBullet : Bullet {

    private Transform target;

    // NOTE: This method replaces the Bullet one.
	private void Update () {
        if (Time.time > destroyTime) {
            Destroy(this.gameObject);
        }
        else {
            Vector3 direction = target.position - transform.position;
            float distance = direction.magnitude;
            Quaternion rotation = Quaternion.LookRotation(direction);

            if (distance < 2.0f) {
                transform.LookAt(target);
            }
            else {
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, speed * 0.5f * Time.deltaTime);
            }
            transform.Translate(0.0f, 0.0f, speed * Time.deltaTime);
        }
	}

    public void SetTarget(Transform target) {
        this.target = target;
    }

}