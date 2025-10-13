using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Config Arma")]
    public Animator animator;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public int damage = 50;
    public LayerMask carLayer; // capa de coches

    private Vector3 targetPos;
    private TrafficCar targetCar; // referencia al coche clickeado

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;

            // raycast para ver si pegamos un coche
            RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero, 0f, carLayer);

            if (hit.collider != null)
            {
                // tocamos un coche
                targetCar = hit.collider.GetComponent<TrafficCar>();
                targetPos = hit.point; // punto exacto dentro del coche
            }
            else
            {
                // click libre en el piso
                targetCar = null;
                targetPos = mouseWorld;
            }

            animator.SetTrigger("Shoot");
        }
    }

    // llamado desde el evento de animación
    public void LaunchProjectile()
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        proj.GetComponent<Projectile>().Init(targetPos, damage, targetCar);
    }
}
