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
        currentWeapon = Instantiate(weaponPrefab, weaponHolder);

        // Aseguramos que quede alineada al holder
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.Euler(0f,0f,0f);
        Debug.Log("Equipada arma: " + weaponPrefab.name);
    }
}
