using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerPhysicsController : MonoBehaviour
{
    private float aceleracion = 6f;
    private float velMax = 15f;
    private float anguloMaxLlantas = 10f;
    private float distanciaEjes = 6f;

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
        // Input volante
        float inputTurn = 0f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            inputTurn = 1f;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            inputTurn = -1f;

        // Suavizar ángulo del volante
        anguloVolante = Mathf.Lerp(anguloVolante, inputTurn * anguloMaxLlantas, Time.deltaTime * 8f);

        // Acelerar
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            velocidadActual += aceleracion * Time.deltaTime;

        // Reversa
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            velocidadActual -= aceleracion * Time.deltaTime;

        // Fricción cuando no tocas nada
        if (!Input.anyKey)
            velocidadActual = Mathf.Lerp(velocidadActual, 0f, Time.deltaTime * 1.5f);

        velocidadActual = Mathf.Clamp(velocidadActual, -velMax * 0.5f, velMax);
    }

    void FixedUpdate()
    {
        float rad = anguloVolante * Mathf.Deg2Rad;

        // Giro basado en velocidad actual
        float giroAngular = Mathf.Tan(rad) * velocidadActual / Mathf.Max(0.01f, distanciaEjes);

        // Aplicar giro real
        rb.angularVelocity = giroAngular * Mathf.Rad2Deg;

        // Mover según velocidad actual
        rb.linearVelocity = transform.up * velocidadActual;
    }



    // Manejo de colisiones: se dispara cuando el Rigidbody dinámico toca un collider estático
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Traffic"))
        {
            // --- VELOCIDAD DEL IMPACTO ---
            float impacto = col.relativeVelocity.magnitude;

            // --- DAÑO REALISTA ---
            float daño = impacto;   // Ajusta multiplicador según se sienta
            takeDamage(daño*2);

            // --- KNOCKBACK DEL JUGADOR ---
            Vector2 normal = col.GetContact(0).normal;
            rb.AddForce(normal * impacto * 0.2f, ForceMode2D.Impulse);

            // --- EMPUJE AL OTRO CARRO ---
            Rigidbody2D otroRB = col.collider.GetComponent<Rigidbody2D>();
            if (otroRB != null)
                otroRB.AddForce(-normal * impacto*2f, ForceMode2D.Impulse);

            Debug.Log($"💥 Impacto={impacto} | Daño={daño}");
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
