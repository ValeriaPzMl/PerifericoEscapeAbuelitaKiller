using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerPhysicsController : MonoBehaviour
{
    private float aceleracion = 2f;
    private float freno = 3f;
    private float velMax = 25f;
    private float anguloMaxLlantas = 5f;
    private float distanciaEjes = 5f;

    private Rigidbody2D rb;
    private float velocidadActual = 0f;
    private float anguloVolante = 0f;
    private float vida = 400;
    private int carrosMuertos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Opcional: mejorar colisiones si vas muy rápido
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Update()
    {
        // Input de volante suavizado
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            anguloVolante = Mathf.Lerp(anguloVolante, anguloMaxLlantas, Time.deltaTime * 6f);
        else if (Input.GetKey(KeyCode.RightArrow)|| Input.GetKey(KeyCode.D))
            anguloVolante = Mathf.Lerp(anguloVolante, -anguloMaxLlantas, Time.deltaTime * 6f);
        else
            anguloVolante = Mathf.Lerp(anguloVolante, 0f, Time.deltaTime * 6f);

        // Acelerar / frenar
        if (Input.GetKey(KeyCode.UpArrow)|| Input.GetKey(KeyCode.W))
            velocidadActual += aceleracion * Time.deltaTime;
        else if (Input.GetKey(KeyCode.DownArrow)|| Input.GetKey(KeyCode.S))
            velocidadActual -= freno * Time.deltaTime; // frena si no aceleras (ajusta a tu gusto)

        velocidadActual = Mathf.Clamp(velocidadActual, 0f, velMax);
    }

    void FixedUpdate()
    {
        // Movimiento tipo coche simple (Ackermann aproximado)
        float rad = anguloVolante * Mathf.Deg2Rad;
        float giroAngular = Mathf.Tan(rad) * velocidadActual / Mathf.Max(0.001f, distanciaEjes);

        // Actualizamos rotación y posición con la física
        float anguloDeg = giroAngular * Mathf.Rad2Deg * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation + anguloDeg);

        // Mover hacia adelante según la orientación actual
        Vector2 dir = transform.up; // assuming sprite's up is forward
        rb.MovePosition(rb.position + dir * velocidadActual * Time.fixedDeltaTime);
    }

    // Manejo de colisiones: se dispara cuando el Rigidbody dinámico toca un collider estático
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Traffic"))
        {
            Debug.Log("Chocaste con un carro!");
            // Aquí restas pasajeros / vida, reproducir sonido, etc.
            // Ej: GameManager.Instance.LosePassenger();
        }
    }
    public void takeDamage(int damage)
    {
        vida += damage;
    }
    public int getLife()
    {
        return (int)vida/10;
    }
    
}
