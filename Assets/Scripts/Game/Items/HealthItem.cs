using UnityEngine;
using System.Collections;

public class HealthItem : Item {

    public float amount = 20.0f;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            other.GetComponent<Player>().Heal(amount);
            Destroy(this.gameObject);
        }
    }

}
