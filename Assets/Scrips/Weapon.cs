using UnityEngine;

public class WeaponDemo : MonoBehaviour
{
    [Header("Config Arma")]
    public Animator animator;        // animador de la catapulta / arma
    public GameObject projectilePrefab;
    public Transform firePoint;
    public int damage = 50;
    public string categoryName;

    private Vector3 targetPos;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // click izquierdo
        {
            targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
            proj.GetComponent<ProjectileDemo>().Init(targetPos, damage);
        }

    }
}
