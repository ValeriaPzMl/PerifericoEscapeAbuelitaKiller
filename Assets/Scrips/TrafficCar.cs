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
    public int distanciaMaxima = 20;
    private bool vivo;

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

    [Header("enemigos")]
    public string categoria;
    public bool enemigo;

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
        // Detecta vehículo enfrente con BoxCast más preciso
        RaycastHit2D hit = Physics2D.BoxCast(
            transform.position + transform.up * (altoDeteccion / 2),
            new Vector2(anchoDeteccion, altoDeteccion),
            0f,
            transform.up,
            distanciaDeteccion,
            trafficLayer
        );

        if (hit.collider != null)
        {
            TrafficCar otroCarro = hit.collider.GetComponent<TrafficCar>();

            if (otroCarro != null)
            {
                // suavizar cambio de velocidad
                velocidadActual = Mathf.Lerp(
                    velocidadActual,
                    otroCarro.velocidadActual,
                    Time.deltaTime * 3f
                );
            }
        }
        else if (!estaChocando)
        {
            // Regresa a velocidad base
            velocidadActual = Mathf.Lerp(
                velocidadActual,
                velocidad,
                Time.deltaTime * 0.8f
            );
        }
    }
    void FixedUpdate()
    {
        // movimiento estable
        rb.linearVelocity = transform.up * velocidadActual;

        if (player != null)
        {
            float distancia = Vector2.Distance(transform.position, player.position);
            Vector2 haciaCarro = (transform.position - player.position).normalized;

            float punto = Vector2.Dot(player.up, haciaCarro);

            if (distancia > distanciaMaxima && punto < 0)
            {
                quitarHits();
                DevolverAlPool();
            }
        }
    }


    // 🔹 Funciones de colisión
    private void OnCollisionEnter2D(Collision2D col)
    {
        float impacto = col.relativeVelocity.magnitude;
        Vector2 normal = col.GetContact(0).normal;

        // CHOQUE ENTRE AUTOS
        TrafficCar otro = col.collider.GetComponent<TrafficCar>();
        if (otro != null)
        {
            int dmg = Mathf.RoundToInt(impacto);

            TakeDamage(dmg);
            otro.TakeDamage(dmg);

            rb.AddForce(normal * impacto * 2f, ForceMode2D.Impulse);

            Rigidbody2D otroRB = col.collider.attachedRigidbody;
            if (otroRB != null)
                otroRB.AddForce(-normal * impacto * 2f, ForceMode2D.Impulse);

            estaChocando = true;
            if (pitar != null) pitar.Play();
            return;
        }

        // CHOQUE CON PLAYER
        if (col.collider.CompareTag("Player"))
        {
            int dmg = Mathf.RoundToInt(impacto);
            TakeDamage(dmg);

            rb.AddForce(normal * impacto * 3f, ForceMode2D.Impulse);
            return;
        }

        // CHOQUE CON PARED
        if (col.collider.CompareTag("limites"))
        {
            DevolverAlPool();
            return;
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
        Debug.Log($"golpe de {dmg} dejo la vida en {health}");
        if (atacador != null) atacador.EstaSiendoAtacado();
        if (health <= 0&&vivo)
        {
            vivo = false;
            if (enemigo)
            {
                GameObject pickup = PoolManager.Instance.GetFromPool(categoria, "Taker");
                if (pickup != null)
                {
                    pickup.transform.position = transform.position;
                }

            }
            muerte();
        }
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
        vivo = true;
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
