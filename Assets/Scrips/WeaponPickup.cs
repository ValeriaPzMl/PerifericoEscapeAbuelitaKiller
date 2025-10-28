using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject weaponPrefab; // la catapulta (un prefab con Weapon.cs)

   
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // tu camión
        {
            PlayerWeapons pw = other.GetComponent<PlayerWeapons>();
            if (pw != null)
            {
                pw.EquipWeapon(weaponPrefab);
            }

            Destroy(gameObject); // desaparece el ítem de la carretera
        }
    }
}
