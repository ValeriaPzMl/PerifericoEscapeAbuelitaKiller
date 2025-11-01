using UnityEngine;

public class TrafficSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] vehiculos; // coches posibles
    public Transform camion;       // referencia al camión

    [Header("Spawn config")]
    private float distanciaSpawn = 20f; // qué tan lejos del camión aparecen
    public float tiempoSpawn = 1.5f;   // cada cuánto intenta spawnear
    public LayerMask trafficLayer;     // capa de tráfico

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
        // 1. Revisar densidad global
        int cochesActuales = GameObject.FindGameObjectsWithTag("Traffic").Length;
        if (cochesActuales >= maxCoches) return;

        // 2. Elegir carril aleatorio
        if (posicionesCarriles.Length == 0 || vehiculos.Length == 0) return;
        float x = posicionesCarriles[Random.Range(0, posicionesCarriles.Length)];
        Vector3 pos = new Vector3(x, camion.position.y + distanciaSpawn, 0);

        if (pos.y <= camion.position.y + 2f) return;

        // 3. Elegir prefab

        int numeroRandom = Random.Range(0, vehiculos.Length);
        GameObject prefab = PoolManager.Instance.GetFromPool("NPCs", $"carro{numeroRandom+1}");
        TrafficCar carData = prefab.GetComponent<TrafficCar>();
        if (carData == null)
        {
            Debug.LogWarning("El prefab " + prefab.name + " no tiene el script TrafficCar");
            return;
        }

        // 4. Revisar espacio libre con OverlapBox usando el tamaño del prefab
        Vector2 detectionSize = carData.GetDetectionSize();
        Collider2D check = Physics2D.OverlapBox(
            pos,
            detectionSize,
            0f,
            trafficLayer
        );
        if (check != null) return; // ya hay un carro muy cerca

        // 5. Instanciar
        if (prefab != null)
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
