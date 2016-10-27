using UnityEngine;
using System.Collections;

public class PowerUpItem : Item {

    public PowerUp.PowerUpTypes type;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            other.GetComponent<Player>().PickPowerUp(type);
            Destroy(this.gameObject);
        }
    }

}
