using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject weaponPrefab; // Prefab del arma con WeaponDemo.cs

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerWeapons pw = other.GetComponent<PlayerWeapons>();
            if (pw != null)
            {
                // 🔹 Sacamos la categoría directamente del prefab
                WeaponDemo wd = weaponPrefab.GetComponent<WeaponDemo>();
                if (wd != null)
                {
                    pw.EquipWeapon(weaponPrefab, wd.categoryName);
                    Debug.Log($"⚙️ Pickup: Equipando arma categoría [{wd.categoryName}]");
                }
                else
                {
                    Debug.LogWarning("⚠️ El prefab del arma no tiene WeaponDemo con categoryName");
                }
                PoolManager.Instance.ReturnToPool(wd.categoryName, "Taker", gameObject);
            }

  
            
        }
    }
}
