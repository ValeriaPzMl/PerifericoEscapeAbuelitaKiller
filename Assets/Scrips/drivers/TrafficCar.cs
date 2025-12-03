using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro; // Asegúrate de tener TextMeshPro importado
using UnityEngine.SceneManagement;
using System.Linq;
using System.Text.RegularExpressions;

public class TrafficCar : MonoBehaviour, IPooledObject
{
    private Transform player;
    private Rigidbody2D rb;
    private Animator explotar;
    public string cat;
    private float checkTimer = 0f;


    [Header("Parámetros generales")]
    public float velocidad = 5f;
    private float velocidadActual;
    public int vidaInicial;
    private int health;
    public int distanciaMaxima = 20;
    private bool vivo;
    private bool agresivo;
    private bool visible;
    [Range(0f, 1f)] public float queTanAgresivo;

    [Header("Sensores de tráfico")]
    public float distanciaDeteccion = 3f;   // distancia para detectar autos al frente
    public LayerMask trafficLayer;

    //private bool estaChocando = false;

    [Header("Área de detección del carro")]
    public float anchoDeteccion = 1f;
    public float altoDeteccion = 2f;

    public DisplayData matador;
    public EnemyWeapon atacador;

    public AudioSource pitar;
    public AudioSource explocion;

    [Header("enemigos")]
    public string categoria;
    public bool enemigo;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        velocidadActual = velocidad;
        health = vidaInicial;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        //protaCS = player.GetComponent<PlayerPhysicsController>();

    }

    void FixedUpdate()
    {
        // Movimiento constante
        rb.linearVelocity = transform.up * velocidadActual;

        // Revisar poca frecuencia para ahorrar CPU
        checkTimer += Time.deltaTime;
        if (checkTimer >= 0.2f)   // cada 0.2s
        {
            checkTimer = 0f;
            RevisarDistancias();
        }

        // Movimiento lateral de enemigos agresivos
        if (agresivo && visible)
            SeguirLineaXAgresivo();
    }

    private void OnBecameVisible()
    {
        visible = true;
    }
    private void OnBecameInvisible()
    {
        visible = false;
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

            
            return;
        }

        // CHOQUE CON PLAYER
        if (col.collider.CompareTag("Player"))
        {
            int dmg = Mathf.RoundToInt(impacto);
            TakeDamage(dmg);
            if (pitar != null) pitar.Play();
            rb.AddForce(normal * impacto * 3f, ForceMode2D.Impulse);
            return;
        }

        // CHOQUE CON PARED
        if (col.collider.CompareTag("limites"))
        {
            muerte(false);
            return;
        }
    }

    void RevisarDistancias()
    {
        if (player == null) return;

        float distancia = Vector2.Distance(transform.position, player.position);
        Vector2 haciaCarro = (transform.position - player.position).normalized;
        float punto = Vector2.Dot(player.up, haciaCarro);

        if (distancia > distanciaMaxima && punto < 0)
        {
            quitarHits();
            DevolverAlPool();
        }
    }


    // 🔹 Gizmos para depuración
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(anchoDeteccion, altoDeteccion, 0));

        
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
            muerte(true);
            if (enemigo)
            {
                GameObject pickup = PoolManager.Instance.GetFromPool(categoria, "Taker");
                if (pickup != null)
                {
                    pickup.transform.position = transform.position;
                }

            }
           
        }
    }

    public void muerte(bool matado)
    {
        vivo = false;
        quitarHits();
        if (matado)matador.CarroMuerto();
        explocion.Play();
        explotar.SetTrigger("explosion");
    }

    public void OnSpawn()
    {


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
        float rand = UnityEngine.Random.value;
        agresivo = (rand < queTanAgresivo);
        TrafficCounter.TotalTraffic++;

    }

    public void OnDespawn()
    {
        if (explotar != null)
        {
            explotar.Rebind();
            explotar.Update(0f);
        }
        TrafficCounter.TotalTraffic--;

    }
    private void SeguirLineaXAgresivo()
    {
        if (!agresivo || player == null) return;

        // Solo mover en eje X, manteniendo Y igual
        float objetivoX = player.position.x;

        // Velocidad del desvío horizontal
        float velocidadLateral = 3f;

        // Interpolación suave hacia la misma X
        Vector3 pos = transform.position;
        pos.x = Mathf.Lerp(pos.x, objetivoX, Time.deltaTime * velocidadLateral);

        transform.position = pos;
    }

    public Vector2 GetDetectionSize()
    {
        return new Vector2(anchoDeteccion, altoDeteccion);
    }
}
