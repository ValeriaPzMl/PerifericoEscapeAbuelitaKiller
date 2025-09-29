using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
    public Transform weaponHolder; // un empty en el camión donde se montan armas
    private GameObject currentWeapon;

    public void EquipWeapon(GameObject weaponPrefab)
    {
        // Si ya hay un arma, la borramos
        if (currentWeapon != null) Destroy(currentWeapon);

        // Instanciamos la nueva
        currentWeapon = Instantiate(weaponPrefab, weaponHolder.position, Quaternion.Euler(180f, 0f, 0f), weaponHolder);

        Debug.Log("Equipada arma: " + weaponPrefab.name);
    }
}
