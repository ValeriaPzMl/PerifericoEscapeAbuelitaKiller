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

    [Header("Enemy Config")]
    public int dificultad = 0;
    public Vector2[] medidasEnemigos;

    [Header("Spawn Probabilities (0-1)")]
    [Range(0f, 1f)] public float probabilidadPowerUp = 0.15f;
    [Range(0f, 1f)] public float probabilidadEnemy = 0.3f;
    // El resto del tiempo genera tráfico normal

    [Header("Pick ups")]
    public string armas;

    private float timer;

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
        int cochesActuales = GameObject.FindGameObjectsWithTag("Traffic").Length;
        if (cochesActuales >= maxCoches) return;

        // Buscar el objeto más adelantado
        float maxY = camion.position.y;
        var traficos = GameObject.FindGameObjectsWithTag("Traffic");
        if (traficos.Length > 0)
            maxY = traficos.Max(t => t.transform.position.y);

        // Calcular punto de spawn
        float spawnY = Mathf.Max(maxY + Random.Range(8f, 15f), camion.position.y + zonaSegura);
        float x = posicionesCarriles[Random.Range(0, posicionesCarriles.Length)];
        Vector3 pos = new Vector3(x, spawnY, 0);

        // Decidir tipo de spawn
        float rand = Random.value;
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

        int numeroRandom = Random.Range(0, medidasCarros.Length);
        Vector2 detectionSize = medidasCarros[numeroRandom];

        Collider2D check = Physics2D.OverlapBox(pos, detectionSize, 0f, trafficLayer);
        if (check != null) return;

        GameObject prefab = PoolManager.Instance.GetFromPool("NPCs", $"carro{numeroRandom + 1}");
        if (prefab == null) return;

        prefab.transform.position = pos;
        prefab.transform.rotation = Quaternion.identity;
        prefab.tag = "Traffic";
    }

    void SpawnEnemy(Vector3 pos)
    {
        if (dificultad <= 0 || medidasEnemigos.Length == 0) return;

        int numeroRandom = Random.Range(0, dificultad);
        numeroRandom = Mathf.Clamp(numeroRandom, 0, medidasEnemigos.Length - 1);

        Vector2 detectionSize = medidasEnemigos[numeroRandom];
        Collider2D check = Physics2D.OverlapBox(pos, detectionSize, 0f, trafficLayer);
        if (check != null) return;

        GameObject prefab = PoolManager.Instance.GetFromPool("enemigos", $"enemigo{numeroRandom + 1}");
        if (prefab == null) return;

        prefab.transform.position = pos;
        prefab.transform.rotation = Quaternion.identity;
        prefab.tag = "Traffic";
    }

    void SpawnPowerUp(Vector3 pos)
    {

        int numeroRandom = Random.Range(0, 8);
        GameObject prefab = PoolManager.Instance.GetFromPool("PowerUps", $"Power{numeroRandom + 1}");
        if (prefab == null) return;

        prefab.transform.position = pos;
        prefab.transform.rotation = Quaternion.identity;
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
    }

    public void SpawnArma(int num)
    {
        int numeroRandom = Random.Range(0, posicionesCarriles.Length);

    }
}
