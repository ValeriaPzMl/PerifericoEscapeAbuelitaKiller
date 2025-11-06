using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro; // Asegúrate de tener TextMeshPro importado
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class TrafficCar : MonoBehaviour, IPooledObject
{
    private Transform player;
    private Rigidbody2D rb;
    private Animator explotar;
    public string cat;

    [Header("Parámetros generales")]
    public float velocidad = 5f;
    private float velocidadActual;
    public int vidaInicial;
    private int health;
    public int distanciaMaxima = 70;

    [Header("Sensores de tráfico")]
    public float distanciaDeteccion = 3f;   // distancia para detectar autos al frente
    public LayerMask trafficLayer;

    private bool estaChocando = false;

    [Header("Área de detección del carro")]
    public float anchoDeteccion = 1f;
    public float altoDeteccion = 2f;

    public DisplayData matador;
    public EnemyWeapon atacador;

    private AudioSource pitar;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        velocidadActual = velocidad;
        health = vidaInicial;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        pitar = GetComponent<AudioSource>();
        //protaCS = player.GetComponent<PlayerPhysicsController>();

    }

    void Update()
    {
        // 🔹 Raycast para detectar carros enfrente
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.up,
            distanciaDeteccion,
            trafficLayer
        );

        if (hit.collider != null)
        {
            TrafficCar otroCarro = hit.collider.GetComponent<TrafficCar>();
            if (otroCarro != null)
            {
                // Si el de enfrente va más lento, igualar velocidad
                if (otroCarro.velocidadActual < velocidadActual)
                {
                    velocidadActual = otroCarro.velocidadActual * 0.9f;
                }
            }
        }
        else
        {
            // Nadie enfrente → vuelve a su velocidad normal
            if (!estaChocando)
                velocidadActual =  velocidad;
        }

        // 🔹 Movimiento hacia adelante
        Vector2 nuevaPos = rb.position + Vector2.up * velocidadActual * Time.fixedDeltaTime;
        rb.MovePosition(nuevaPos);

        // 🔹 Revisa si se alejó demasiado del jugador
        if (player != null)
        {
            // El jugador avanza hacia arriba (eje Y)
            Vector2 direccionJugador = player.up;
            Vector2 direccionAlObjeto = (transform.position - player.position).normalized;

            float punto = Vector2.Dot(direccionJugador, direccionAlObjeto);
            float distancia = Vector2.Distance(transform.position, player.position);

            // Solo devolver al pool si está lejos y detrás del jugador
            if (distancia >= distanciaMaxima && punto < 0f)
            {
                quitarHits();
                DevolverAlPool();
            }
        }
    }

    // 🔹 Funciones de colisión
    private void OnCollisionEnter2D(Collision2D collision)
    {
        TrafficCar otroCarro = collision.gameObject.GetComponent<TrafficCar>();
        if (otroCarro != null)
        {
            // Si choca con otro auto, desacelera un poco
            velocidadActual = otroCarro.velocidadActual * 0.8f;
            estaChocando = true;
            pitar.Play();
        }
        else
        {
            if (collision.gameObject.tag == "limites") DevolverAlPool();
            // Si choca con algo que no es auto (jugador, obstáculo, etc.)
            //rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            velocidadActual = 0f;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        estaChocando = false;
        //velocidadActual = velocidad;
        // Al dejar de chocar, libera movimiento lateral otra vez
        //rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // 🔹 Gizmos para depuración
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(anchoDeteccion, altoDeteccion, 0));

        Gizmos.color = Physics2D.Raycast(transform.position, Vector2.up, distanciaDeteccion, trafficLayer)
                ? Color.green
                : Color.red; Vector3 origen = transform.position;
        Vector3 direccion = Vector3.up * distanciaDeteccion;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * distanciaDeteccion);
        Gizmos.DrawSphere(origen + direccion, 0.1f);
    }

    // 🔹 Pooling y limpieza
    private void quitarHits()
    {
        List<GameObject> hijos = new List<GameObject>();
        foreach (Transform child in transform)
            hijos.Add(child.gameObject);

        //transform.DetachChildren();

        foreach (GameObject childGO in hijos)
        {
            Hiteados hitCS = childGO.GetComponent<Hiteados>();
            if (hitCS != null)
            {
                string cat = hitCS.categoryName;
                PoolManager.Instance.ReturnToPool(cat, "hit", childGO);
                Debug.Log($"♻️ Devuelto al pool: {childGO.name} → {cat}/hit");
            }
        }
    }

    private string ObtenerNombreBase(string name)
{
    string limpio = name.Replace("(Clone)", "").Trim();

    // Quitar todas las 'Z' o 'z'
    limpio = Regex.Replace(limpio, "[Zz]", "");

    int index = limpio.IndexOf("_");
    if (index > 0)
        limpio = limpio.Substring(0, index);

    return limpio;
}

    public void DevolverAlPool()
    {
        string nombrePool = ObtenerNombreBase(gameObject.name);
        PoolManager.Instance.ReturnToPool(cat, nombrePool, gameObject);
        Debug.Log($"♻️ Devuelto al pool: {nombrePool}");
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (atacador != null) atacador.EstaSiendoAtacado();
        if (health <= 0)
            muerte();
    }

    public void muerte()
    {
        quitarHits();
        matador.CarroMuerto();
        explotar.SetTrigger("explosion");
    }

    public void OnSpawn()
    {
        if(pitar==null)
            pitar=GetComponent<AudioSource>();

        if (matador == null)
            matador = FindFirstObjectByType<DisplayData>();

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        health = vidaInicial;
        velocidadActual = velocidad;

        if (explotar == null)
            explotar = GetComponent<Animator>();

        explotar.Rebind();
        explotar.Update(0f);
    }

    public void OnDespawn()
    {
        if (explotar != null)
        {
            explotar.Rebind();
            explotar.Update(0f);
        }
    }
    public Vector2 GetDetectionSize()
    {
        return new Vector2(anchoDeteccion, altoDeteccion);
    }
}
