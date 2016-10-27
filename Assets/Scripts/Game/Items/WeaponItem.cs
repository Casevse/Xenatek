using UnityEngine;
using System.Collections;

public class WeaponItem : Item {

    public Weapon.WeaponTypes type;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            other.GetComponent<Player>().PickWeapon(type);
            Destroy(this.gameObject);
        }
    }

}
