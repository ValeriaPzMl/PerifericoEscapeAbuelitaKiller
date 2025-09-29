using UnityEngine;
using System.Collections.Generic;

public class CarreteraSpawner : MonoBehaviour
{
    [Header("Prefabs de carretera")]
    public GameObject[] prefabsCarretera;   // diferentes tipos de carretera
    public Transform jugador;               // referencia al camión

    [Header("Configuración")]
    public float largoChunk = 29f;          // largo de cada tramo de carretera
    public int chunksActivos = 4;           // cuántos mantenemos en escena

    private List<GameObject> carreteras = new List<GameObject>();
    private float spawnZ = 0f;              // posición Z/Y hasta dónde hemos instanciado
    private int ultimoIndice = -1;          // para evitar repetir siempre el mismo prefab

    void Start()
    {
        // generar los primeros chunks
        for (int i = 0; i < chunksActivos; i++)
        {
            if (i == 0)
                SpawnCarretera(0); // primer tramo siempre igual
            else
                SpawnCarretera();
        }
    }

    void Update()
    {
        // cuando el jugador esté cerca del final del último chunk, instanciamos otro
        if (jugador.position.y - 20f > spawnZ - (chunksActivos * largoChunk))
        {
            SpawnCarretera();
            BorrarCarretera();
        }
    }

    void SpawnCarretera(int indicePrefab = -1)
    {
        GameObject go;
        if (indicePrefab == -1)
        {
            indicePrefab = RandomPrefabIndex();
        }

        go = Instantiate(prefabsCarretera[indicePrefab],
            new Vector3(0, spawnZ, 1f), Quaternion.identity);

        go.transform.SetParent(transform);
        carreteras.Add(go);
        spawnZ += largoChunk;
    }

    void BorrarCarretera()
    {
        Destroy(carreteras[0]);
        carreteras.RemoveAt(0);
    }

    int RandomPrefabIndex()
    {
        if (prefabsCarretera.Length <= 1)
            return 0;

        int indice;
        do
        {
            indice = Random.Range(0, prefabsCarretera.Length);
        } while (indice == ultimoIndice);

        ultimoIndice = indice;
        return indice;
    }
}
