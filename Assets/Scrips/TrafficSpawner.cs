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
    public float[] posicionesCarriles; // posiciones fijas en X (ej: -4, -2, 0, 2, 4)

    [Header("Densidad global")]
    public int maxCoches = 15; // máximo en pantalla

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

        // 3. Elegir prefab
        GameObject prefab = vehiculos[Random.Range(0, vehiculos.Length)];
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
        GameObject nuevoCarro = Instantiate(prefab, pos, Quaternion.identity);
        nuevoCarro.tag = "Traffic"; // aseguramos el tag
    }  
}
