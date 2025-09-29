using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Projectile : MonoBehaviour
{
    public float speed = 15f;
    private int damage;
    private Vector3 targetPos;

    public void Init(Vector3 target, int dmg)
    {
        targetPos = target;
        damage = dmg;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            Hit();
        }
    }

    void Hit()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(targetPos, 0.5f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Traffic"))
            {
                hit.GetComponent<TrafficCar>().TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}
