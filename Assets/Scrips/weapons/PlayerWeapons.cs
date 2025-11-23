using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
    [Header("Referencias")]
    public Transform weaponHolder; // Donde se coloca el arma
    public PowerUpManager pum;

    private GameObject currentWeapon;
    private string currentCategory;

    public void EquipWeapon(GameObject weaponPrefab, string category)
    {
        // 🔹 Si ya hay un arma equipada, la devolvemos a su pool
        if (currentWeapon != null)
        {
            PoolManager.Instance.ReturnToPool(currentCategory, "Prefab", currentWeapon);
            currentWeapon = null;
        }

        // 🔹 Guardamos la nueva categoría
        currentCategory = category;

        // 🔹 Obtenemos el arma del pool correcto
        currentWeapon = PoolManager.Instance.GetFromPool(category, "Prefab");
        if (currentWeapon == null)
        {
            Debug.LogError($"❌ No se pudo obtener arma del pool {category}/Prefab");
            return;
        }

        // 🔹 Colocamos el arma en el holder
        currentWeapon.transform.SetParent(weaponHolder,false);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;

        Debug.Log($"✅ Arma equipada desde pool: {category}/Prefab");

        pum.NewWeapon(currentWeapon);
    }
}
