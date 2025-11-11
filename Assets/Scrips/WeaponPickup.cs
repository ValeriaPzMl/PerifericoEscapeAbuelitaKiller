using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject weaponPrefab; // Prefab del arma con WeaponDemo.cs
    private Transform player;
    private WeaponDemo wd;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerWeapons pw = other.GetComponent<PlayerWeapons>();
            if (pw != null)
            {
                // 🔹 Sacamos la categoría directamente del prefab
                wd = weaponPrefab.GetComponent<WeaponDemo>();
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
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

    }
    private void Update()
    {
        if (player != null)
        {
            // El jugador avanza hacia arriba (eje Y)
            Vector2 direccionJugador = player.up;
            Vector2 direccionAlObjeto = (transform.position - player.position).normalized;

            float punto = Vector2.Dot(direccionJugador, direccionAlObjeto);
            float distancia = Vector2.Distance(transform.position, player.position);

            // Solo devolver al pool si está lejos y detrás del jugador
            if (distancia >= 20 && punto < 0f)
            {
                PoolManager.Instance.ReturnToPool(wd.categoryName, "Taker", gameObject);
            }
        }
    }
}
