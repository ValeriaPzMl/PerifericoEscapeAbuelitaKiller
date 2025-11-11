using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerPhysicsController : MonoBehaviour
{
    private float aceleracion = 2f;
    private float freno = 4f;
    private float velMax = 15f;
    private float anguloMaxLlantas = 7f;
    private float distanciaEjes = 5f;

    private Rigidbody2D rb;
    private float velocidadActual = 0f;
    private float anguloVolante = 0f;
    private float vida = 800;
    private int carrosMuertos;
    private float Shield = 1;

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
        {
            anguloVolante = Mathf.Lerp(anguloVolante, anguloMaxLlantas, Time.deltaTime * 6f);
            if (velocidadActual <= 0) velocidadActual = 1;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            anguloVolante = Mathf.Lerp(anguloVolante, -anguloMaxLlantas, Time.deltaTime * 6f);
            if (velocidadActual <= 0) velocidadActual = 1;
        }
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
            Debug.Log($"Chocaste con un carro! {vida}");
            takeDamage(10);
            // Aquí restas pasajeros / vida, reproducir sonido, etc.
            // Ej: GameManager.Instance.LosePassenger();
        }
    }
    public void takeDamage(float damage)
    {
        damage *= Shield;
        vida -= damage;
        Debug.Log($"se ataco con {damage}");
        if (vida <= 0) SceneManager.LoadScene(1);

    }
    public void Proteger(float sh)
    {
        Shield = sh;
    }
    public void DesProteger(bool nada, bool medias)
    {
        if(medias && !nada)
        {
            Shield = 0.5f;
        }else if(nada && !medias)
        {
            Shield = 0;
        }else if(!nada && !medias)
        {
            Shield = 1;
        }
        else
        {
            Shield = 0;
        }
    }
    
    public int getLife()
    {
        return (int)vida/20;
    }
    public float MasShoot()
    {
        if (velocidadActual < 5) return 1f;
        else if(velocidadActual >= 5)return 3f;
        else return 5f;
    }
    public void MasVida(float x)
    {
        vida += x;
    }
    public void CompletarVida()
    {
        vida = (vida<800)?800 :vida;
    }
}
