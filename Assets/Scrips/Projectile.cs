using UnityEngine;
using UnityEngine.InputSystem.XR;

public class ProjectileDemo : MonoBehaviour,IPooledObject
{
    public float speed = 15f;
    private int damage;
    private Vector3 targetPos;
    public GameObject impactPrefab; // sandía aplastada
    public string categoryName;
    private AudioSource audio;



    public void Init(Vector3 target, int dmg)
    {
        targetPos = target;
        damage = dmg;
        Vector3 direction = targetPos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

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

        Debug.Log($"Hit() llamado por {gameObject.name}");

        // instanciar sandía aplastada en el punto del impacto
        if (impactPrefab != null)
        {
            Vector3 euler = transform.rotation.eulerAngles; // Obtener la rotación en ángulos
            euler.z += 180f; // Sumar 180 grados al eje Z
            Quaternion rotacion = Quaternion.Euler(euler); // Convertir de nuevo a Quaternion

            //GameObject impact = Instantiate(impactPrefab, targetPos, rotacion);
            GameObject impact = PoolManager.Instance.GetFromPool(categoryName, "hit");
            if (impact != null)
            {
                impact.transform.position = targetPos;
                impact.transform.rotation = rotacion;
            }


            // revisar si hay algún coche
            Collider2D[] hits = Physics2D.OverlapCircleAll(targetPos, 0.5f);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Traffic"))
                {
                    // aplicar daño
                    
                    // hacer que la sandía sea hija del coche
                    Vector3 localPoint = hit.transform.InverseTransformPoint(targetPos);
                    impact.transform.SetParent(hit.transform);
                    impact.transform.localPosition = localPoint;
                    SpriteRenderer sr = impact.GetComponent<SpriteRenderer>();
                    
                    if (sr != null)
                    {
                        sr.sortingLayerName = "Proyectiles";
                    }
                    hit.GetComponent<TrafficCar>().TakeDamage(damage);
                    break; // solo uno
                }
            }
        }

        // destruir proyectil
        PoolManager.Instance.ReturnToPool(categoryName, "proyectil", gameObject);

    }

    public void OnSpawn()
    {
        if (audio == null)
            audio = GetComponent<AudioSource>();

        if (audio != null)
            audio.Play();
    }

    public void OnDespawn()
    {
        
    }
}
