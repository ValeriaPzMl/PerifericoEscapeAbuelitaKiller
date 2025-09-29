using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Config Arma")]
    public Animator animator;        // animador de la catapulta / arma
    public GameObject projectilePrefab;
    public Transform firePoint;
    public int damage = 50;

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
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        proj.GetComponent<Projectile>().Init(targetPos, damage);
    }
}
