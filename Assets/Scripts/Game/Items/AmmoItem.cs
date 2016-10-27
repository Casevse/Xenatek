using UnityEngine;
using System.Collections;

public class AmmoItem : Item {

    public Weapon.WeaponTypes type;
    public int amount;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            other.GetComponent<Player>().PickAmmo(type, amount);
            Destroy(this.gameObject);
        }
    }

}
