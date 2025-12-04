using System;
using System.Linq;
using UnityEngine;


public class UnifiedSpawner : MonoBehaviour
{
    [Header("General Config")]
    public Transform camion;
    public float tiempoSpawn = 0.15f;
    public float zonaSegura = 15f;
    public float[] posicionesCarriles;

    [Header("Traffic Config")]
    public LayerMask trafficLayer;
    public Vector2[] medidasCarros;
    public int maxCoches = 30;
    private float ultimoSpawnY;

    [Header("Enemy Config")]
    public int dificultad = 0;
    public Vector2[] medidasEnemigos;

    [Header("Spawn Probabilities (0-1)")]
    [Range(0f, 1f)] public float probabilidadPowerUp = 0.15f;
    [Range(0f, 1f)] public float probabilidadEnemy = 0.3f;
    // El resto del tiempo genera tráfico normal

    [Header("Pick ups")]
    public string[] armas;

    private float timer;

    private void Start()
    {
        ultimoSpawnY = camion.position.y; // o 0
        timer = 0f;
        TrafficCounter.TotalTraffic = 0;

    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= tiempoSpawn)
        {
            TrySpawn();
            timer = 0f;
        }
    }

    void TrySpawn()
    {
        if (posicionesCarriles.Length == 0) return;

        // Revisar cantidad de tráfico actual
        
        if (TrafficCounter.TotalTraffic >= maxCoches) return;
        // Buscar el objeto más adelantado


        // Calcular punto de spawn
        float spawnY = Mathf.Max(ultimoSpawnY + UnityEngine.Random.Range(8f, 15f),camion.position.y + zonaSegura);
        float x = posicionesCarriles[UnityEngine.Random.Range(0, posicionesCarriles.Length)];
        Vector3 pos = new Vector3(x, spawnY, 0);

        // Decidir tipo de spawn
        float rand = UnityEngine.Random.value;
        if (rand < probabilidadPowerUp)
            SpawnPowerUp(pos);
        else if (rand < probabilidadPowerUp + probabilidadEnemy)
            SpawnEnemy(pos);
        else
            SpawnTraffic(pos);
    }

    void SpawnTraffic(Vector3 pos)
    {
        if (medidasCarros.Length == 0) return;

        int numeroRandom = UnityEngine.Random.Range(0, medidasCarros.Length);
        Vector2 detectionSize = medidasCarros[numeroRandom];

        Collider2D check = Physics2D.OverlapBox(pos, detectionSize, 0f, trafficLayer);
        if (check != null) return;

        GameObject prefab = PoolManager.Instance.GetFromPool("NPCs", $"carro{numeroRandom + 1}");
        if (prefab == null) return;

        prefab.transform.position = pos;
        prefab.transform.rotation = Quaternion.identity;
        prefab.tag = "Traffic";
        ultimoSpawnY = pos.y;

    }

    void SpawnEnemy(Vector3 pos)
    {
        if (dificultad <= 0 || medidasEnemigos.Length == 0) return;

        int numeroRandom = UnityEngine.Random.Range(0, dificultad);
        numeroRandom = Mathf.Clamp(numeroRandom, 0, medidasEnemigos.Length - 1);

        Vector2 detectionSize = medidasEnemigos[numeroRandom];
        Collider2D check = Physics2D.OverlapBox(pos, detectionSize, 0f, trafficLayer);
        if (check != null) return;

        GameObject prefab = PoolManager.Instance.GetFromPool("enemigos", $"enemigo{numeroRandom + 1}");
        if (prefab == null) return;

        prefab.transform.position = pos;
        prefab.transform.rotation = Quaternion.identity;
        prefab.tag = "Traffic";
        ultimoSpawnY = pos.y;

    }

    void SpawnPowerUp(Vector3 pos)
    {

        int numeroRandom = UnityEngine.Random.Range(0, 8);
        GameObject prefab = PoolManager.Instance.GetFromPool("PowerUps", $"Power{numeroRandom + 1}");
        if (prefab == null) return;

        prefab.transform.position = pos;
        prefab.transform.rotation = Quaternion.identity;
        ultimoSpawnY = pos.y;
    }

    public void ActualizarCarriles(int numCarriles)
    {
        switch (numCarriles)
        {
            case 3:
                posicionesCarriles = new float[] { -4.5f, 0f, 4.5f };
                break;
            case 4:
                posicionesCarriles = new float[] { -4.5f, 0f, 4.5f, 9f };
                break;
            case 5:
                posicionesCarriles = new float[] { -9f, -4.5f, 0f, 4.5f, 9f };
                break;
        }
        Debug.Log("Carriles actualizados a: " + numCarriles);
    }

    public void SubirDificultad(int dif)
    {
        dificultad = dif;
        Debug.Log($"subio la dificultad {dificultad} ");

    }

    public void SpawnArma(string arma)
    {

        GameObject prefab = PoolManager.Instance.GetFromPool(arma, "Taker");
        if (prefab == null) return;
        Vector3 pos = new Vector3(0, ultimoSpawnY, 0);

        prefab.transform.position = pos;
        prefab.transform.rotation = Quaternion.identity;
        ultimoSpawnY = pos.y;
    }
}
