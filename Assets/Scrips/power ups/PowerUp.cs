using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private PowerUpManager Manager;
    public int PUnumber;
    private Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            
            if (Manager != null)
            {
                Manager.ActivePower(PUnumber);
            }

            PoolManager.Instance.ReturnToPool("PowerUps", $"Power{PUnumber}", gameObject);

        }
    }
    void Start()
    {
        Manager = FindFirstObjectByType<PowerUpManager>();
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
                PoolManager.Instance.ReturnToPool("PowerUps", $"Power{PUnumber}", gameObject);
            }
        }
    }

}
