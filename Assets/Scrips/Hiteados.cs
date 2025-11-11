using UnityEngine;

public class Hiteados : MonoBehaviour, IPooledObject
{
    private Transform player;
    private int distanciaMaxima = 50;
    public string categoryName;
    private AudioSource plack;

    void Awake()
    {
        plack = GetComponent<AudioSource>();
    }

    void Start()
    {
        // Busca al objeto con tag "Player"
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
            player = jugador.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distancia = Vector3.Distance(transform.position, player.position);
        if (transform.parent == null && distancia >= distanciaMaxima)
        {
            Debug.Log("Objeto destruido por alejarse demasiado del jugador.");
            PoolManager.Instance.ReturnToPool(categoryName, "hit", gameObject);
            // ❌ Quita Destroy(gameObject)
        }
    }

    public void OnSpawn()
    {
        if (plack == null)
            plack = GetComponent<AudioSource>();

        if (plack != null)
            plack.Play();
    }


    public void OnDespawn()
    {
        // Limpieza visual
        transform.SetParent(null);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.sortingLayerName = "Default";
    }
}
