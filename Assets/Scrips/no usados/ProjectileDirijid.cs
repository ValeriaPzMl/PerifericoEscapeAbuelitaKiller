using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public GameObject impactPrefab; // sandía aplastada

    private Vector3 impactPoint; // punto exacto del click
    private int damage;
    private TrafficCar targetCar;

    private bool initialized = false;

    public void Init(Vector3 point, int dmg, TrafficCar car)
    {
        impactPoint = point; // este es el lugar donde va la sandía
        damage = dmg;
        targetCar = car;
        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

        // destino actual: si hay coche, lo seguimos, si no, el punto fijo
        Vector3 currentTarget = targetCar != null ? targetCar.transform.position : impactPoint;

        // mover hacia el destino
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);

        // si llegó
        if (Vector3.Distance(transform.position, currentTarget) < 0.05f)
        {
            if (targetCar != null)
            {
                targetCar.TakeDamage(damage);
            }

            if (impactPrefab != null)
            {
                GameObject impact = Instantiate(impactPrefab, impactPoint, Quaternion.identity);

                if (targetCar != null)
                {
                    // convertir punto de impacto a local del carro
                    Vector3 localPoint = targetCar.transform.InverseTransformPoint(impactPoint);

                    // hacerlo hijo
                    impact.transform.SetParent(targetCar.transform);

                    // ajustar posición al local point dentro del carro
                    impact.transform.localPosition = localPoint;
                }
            }


            Destroy(gameObject);
        }
    }
}
