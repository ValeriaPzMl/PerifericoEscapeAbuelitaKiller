using System.Linq;
using UnityEngine;

public class TrafficSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public Transform camion;
    public Vector2[] medidasCarros;// referencia al camión

    [Header("Spawn config")]
    //private float distanciaSpawn = 20f; // qué tan lejos del camión aparecen
    public float tiempoSpawn = 0.5f;   // cada cuánto intenta spawnear
    public LayerMask trafficLayer;     // capa de tráfico
    public float zonaSegura = 15f;

    [Header("Carriles")]
    public float[] posicionesCarriles;

    [Header("Densidad global")]
    public int maxCoches = 30; // máximo en pantalla

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= tiempoSpawn)
        {
            TrySpawnVehiculo();
            timer = 0f;
        }
    }

    void TrySpawnVehiculo()
    {
        int cochesActuales = GameObject.FindGameObjectsWithTag("Traffic").Length;
        if (cochesActuales >= maxCoches) return;

        if (posicionesCarriles.Length == 0 || medidasCarros.Length == 0) return;

        // 📏 Buscar el auto más lejano hacia adelante
        float maxY = camion.position.y;
        var traficos = GameObject.FindGameObjectsWithTag("Traffic");
        if (traficos.Length > 0)
            maxY = traficos.Max(t => t.transform.position.y);

        // 🧮 Punto base del spawn: el más lejano o la distancia fija si no hay autos
        float spawnY = Mathf.Max(maxY + Random.Range(8f, 15f), camion.position.y + zonaSegura);

        // 🚗 Carril aleatorio
        float x = posicionesCarriles[Random.Range(0, posicionesCarriles.Length)];
        Vector3 pos = new Vector3(x, spawnY, 0);

        // 🧱 Verificar espacio libre con OverlapBox
        int numeroRandom = Random.Range(0, medidasCarros.Length);





        Vector2 detectionSize = medidasCarros[numeroRandom];
        Collider2D check = Physics2D.OverlapBox(pos, detectionSize, 0f, trafficLayer);

        if (check != null) return;

        GameObject prefab = PoolManager.Instance.GetFromPool("NPCs", $"carro{numeroRandom + 1}");
        if (prefab == null) return;
        else
        {
            prefab.transform.position = pos;
            prefab.transform.rotation = Quaternion.identity;
            prefab.tag = "Traffic"; // aseguramos el tag

        }
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

}
