using UnityEngine;

public class WeaponDemo : MonoBehaviour,IPooledObject
{
    [Header("Config Arma")]
    public Animator animator;        // animador de la catapulta / arma
    public GameObject projectilePrefab;
    public Transform firePoint;
    public int damageFijo;
    private int damage;
    public string categoryName;
    private GameObject jugador;
    private Vector3 targetPos;
    private PlayerPhysicsController playerPhysicsController;

    private void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player");
        playerPhysicsController = jugador.GetComponent<PlayerPhysicsController>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // click izquierdo
        {
            targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            if (playerPhysicsController != null)
            {
                targetPos.y += playerPhysicsController.MasShoot();
            }
            targetPos.z = 0f;
                animator.SetTrigger("Shoot"); // dispara animación
        }
    }

    // Este lo llamas desde el último frame del animador
    public void LaunchProjectile()
    {
        GameObject proj = PoolManager.Instance.GetFromPool(categoryName, "proyectil");
        if (proj != null)
        {
            proj.transform.position = firePoint.position;
            proj.transform.rotation = firePoint.rotation;
            proj.GetComponent<ProjectileDemo>().Init(targetPos, damage,true);
        }

    }
    public void ChangeDmage(int dam) {
        damage *= dam;
        Debug.Log($"se cambio el daño a{damage}");
    }
    public void RestaurarDamge(int ex)
    {
        damage /= ex;
        Debug.Log($"se cambio el daño a{damage}");
    }

    public void OnSpawn()
    {
        damage = damageFijo;
    }

    public void OnDespawn()
    {
        damage = damageFijo;
    }
}
