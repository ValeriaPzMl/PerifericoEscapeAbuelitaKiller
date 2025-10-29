using UnityEngine;
using System.Collections.Generic;

public class CarreteraSpawner : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador; // referencia al camión

    [Header("Tipos específicos")]
    public GameObject carretera3;
    public GameObject cambio34;
    public GameObject carretera4;
    public GameObject cambio45;
    public GameObject carretera5;

    [Header("Configuración")]
    public float largoChunk = 29f; // largo de cada tramo de carretera
    public int chunksActivos = 4;  // cuántos mantenemos en escena

    private List<GameObject> carreteras = new List<GameObject>();
    private float spawnZ = 0f;
    private GameObject[] prefabsCarretera;
    private int ultimoIndice = -1;
    private bool enTransicion = false;

    void Start()
    {
        // 🔹 Empieza solo con carretera de 3 carriles
        prefabsCarretera = new GameObject[] { carretera3 };

        for (int i = 0; i < chunksActivos; i++)
        {
            SpawnCarretera();
        }
    }

    void Update()
    {
        if (jugador.position.y - 20f > spawnZ - (chunksActivos * largoChunk))
        {
            SpawnCarretera();
            BorrarCarretera();
        }
    }

    void SpawnCarretera()
    {
        if (prefabsCarretera == null || prefabsCarretera.Length == 0) return;

        int indice = RandomPrefabIndex();
        GameObject prefab = prefabsCarretera[indice];

        GameObject go = Instantiate(prefab, new Vector3(0, spawnZ, 1f), Quaternion.identity);
        go.transform.SetParent(transform);

        carreteras.Add(go);
        spawnZ += largoChunk;
    }

    void BorrarCarretera()
    {
        if (carreteras.Count > 0)
        {
            Destroy(carreteras[0]);
            carreteras.RemoveAt(0);
        }
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

    // 🔹 Llamado desde DificultadManager
    public void CambiarTipoCarretera(int numCarriles)
    {
        if (enTransicion) return; // evita que se dispare doble
        enTransicion = true;

        switch (numCarriles)
        {
            case 4:
                // Instancia una vez el prefab de transición cambio34
                InstanciarTransicion(cambio34, new GameObject[] { carretera4 });
                break;

            case 5:
                // Instancia una vez el prefab de transición cambio45
                InstanciarTransicion(cambio45, new GameObject[] { carretera5 });
                break;
        }
    }

    void InstanciarTransicion(GameObject prefabTransicion, GameObject[] nuevoSet)
    {
        // Instanciamos un tramo de cambio especial
        GameObject trans = Instantiate(prefabTransicion, new Vector3(0, spawnZ, 1f), Quaternion.identity);
        trans.transform.SetParent(transform);
        carreteras.Add(trans);
        spawnZ += largoChunk;

        // Después de la transición, actualizamos el set principal
        prefabsCarretera = nuevoSet;
        enTransicion = false;

        Debug.Log($"Cambiado a carretera de {nuevoSet[0].name}");
    }
}
