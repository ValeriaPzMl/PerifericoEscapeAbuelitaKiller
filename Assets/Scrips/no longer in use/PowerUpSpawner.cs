using System.Linq;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public Transform camion;
  

    [Header("Spawn config")]
    //private float distanciaSpawn = 20f; // qué tan lejos del camión aparecen
    public float tiempoSpawn = 0.5f;   // cada cuánto intenta spawnear
    public float zonaSegura = 15f;

    [Header("Carriles")]
    public float[] posicionesCarriles;


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


        if (posicionesCarriles.Length == 0 ) return;

        // 📏 Buscar el auto más lejano hacia adelante
        float maxY = camion.position.y;
        

        // 🧮 Punto base del spawn: el más lejano o la distancia fija si no hay autos
        float spawnY = Mathf.Max(maxY + Random.Range(8f, 15f), camion.position.y + zonaSegura);

        // 🚗 Carril aleatorio
        float x = posicionesCarriles[Random.Range(0, posicionesCarriles.Length)];
        Vector3 pos = new Vector3(x, spawnY, 0);

        // 🧱 Verificar espacio libre con OverlapBox
        int numeroRandom = Random.Range(0, 7);



        GameObject prefab = PoolManager.Instance.GetFromPool("PowerUps", $"Power{numeroRandom + 1}");
        if (prefab == null) return;
        else
        {
            prefab.transform.position = pos;
            prefab.transform.rotation = Quaternion.identity;

        }
    }

    

}
