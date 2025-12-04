using UnityEngine;
using UnityEngine.UIElements;

public class EnemyWeapon : MonoBehaviour
{
    [Header("Config Arma")]
    public Animator animator;
    public Transform firePoint;
    public int damage;
    public float coolDown = 2f;
    public string categoryName;

    [Header("Detección")]
    public float distanciaAtaque = 10f; // distancia para detectar al jugador
    public string tagJugador = "Player";

    //private bool estaEnPantalla = false;
    private bool estaSiendoAtacado = false;
    private bool jugadorCerca = false;
    private bool puedeAtacar = false;

    private float tiempoDisparo = 0f;
    private Transform jugador;
    private int plus = -90;
    private Camera cam;
    private bool enPantallaReal = false;

    void Start()
    {
        jugador = GameObject.FindGameObjectWithTag(tagJugador)?.transform;
        cam = Camera.main;
    }


    void Update()
    {
        enPantallaReal = IsVisibleByCamera();
        if (!enPantallaReal) return;   // 🟢 Solo actúa si REALMENTE está visible

        if (jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);
        jugadorCerca = distancia <= distanciaAtaque;

        puedeAtacar = enPantallaReal && (estaSiendoAtacado || jugadorCerca);

        if (puedeAtacar)
        {
            tiempoDisparo -= Time.deltaTime;
            if (tiempoDisparo <= 0f)
            {
                LookAtMe();
                Disparar();
                tiempoDisparo = coolDown;
            }
        }
    }



    void Disparar()
    {
        animator.SetTrigger("Shoot");
    }

    // Llamado desde el evento de animación
    public void LaunchProjectile()
    {
        if (firePoint == null) return;

        Vector3 targetPos = jugador != null ? jugador.position : transform.position + Vector3.down;

        GameObject proj = PoolManager.Instance.GetFromPool(categoryName, "proyectil");
        if (proj != null)
        {
            proj.transform.position = firePoint.position;
            proj.transform.rotation = firePoint.rotation;
            proj.GetComponent<ProjectileDemo>().Init(targetPos, damage,false);
        }
    }

    // --- Detectar si está visible en cámara ---
    bool IsVisibleByCamera()
    {
        if (cam == null) return false;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        Bounds bounds = GetComponentInParent<Renderer>().bounds;

        return GeometryUtility.TestPlanesAABB(planes, bounds);
    }




    // --- Llamar externamente cuando el jugador lo ataque ---
    public void EstaSiendoAtacado()
    {
        estaSiendoAtacado = true;
        Invoke(nameof(ResetAtaque), 5f); // vuelve a false después de 5 segundos
    }

    void ResetAtaque()
    {
        estaSiendoAtacado = false;
    }
    void LookAtMe()
    {
        Vector3 direction = jugador.position - transform.position;

        // Calcular ángulo
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Aplicar rotación
        transform.rotation = Quaternion.Euler(0, 0, angle + plus);
    }
    private void OnDrawGizmos()
    {
        // Color rojo para el rango de ataque
        Gizmos.color = Color.red;

        // Dibuja un círculo para visualizar el área de ataque
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);

        // Si existe el jugador, dibuja una línea hacia él
        if (jugador != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, jugador.position);
        }
    }

}
