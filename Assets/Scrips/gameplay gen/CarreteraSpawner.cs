using UnityEngine;
using System.Collections.Generic;

public class CarreteraSpawner : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador; // referencia al camión o jugador

    [Header("Configuración general")]
    public float largoChunk = 29f;
    public int chunksActivos = 4;
    public float offsetDespawn = 25f; // distancia tras el jugador para borrar

    [Header("Estado actual")]
    private readonly List<GameObject> carreteras = new();
    private float spawnZ = 0f;
    private int cantCarriles = 3;
    private bool enTransicion = false;

    void Start()
    {
        // 🔹 Spawnea tramos iniciales de carretera 3
        for (int i = 0; i < chunksActivos; i++)
        {
            SpawnCarretera($"carretera{cantCarriles}");
        }
    }

    void Update()
    {
        // 🔹 Si el jugador avanza, spawneamos nueva carretera y borramos la más vieja
        if (jugador.position.y > spawnZ - (chunksActivos * largoChunk))
        {
            SpawnCarretera($"carretera{cantCarriles}");
            BorrarCarretera();
        }
    }

    // =============================
    //  🟢 SPAWN
    // =============================
    private void SpawnCarretera(string nombre)
    {
        GameObject go = PoolManager.Instance.GetFromPool("carretera", nombre);
        if (go == null)
        {
            Debug.LogError($"❌ No se encontró el prefab '{nombre}' en el pool 'carretera'");
            return;
        }

        go.transform.position = new Vector3(0, spawnZ, 1f);
        go.transform.rotation = Quaternion.identity;
        go.transform.SetParent(transform);
        carreteras.Add(go);

        spawnZ += largoChunk;
    }

    // =============================
    //  🔴 DESPAWN
    // =============================
    private void BorrarCarretera()
    {
        if (carreteras.Count == 0) return;

        GameObject primero = carreteras[0];

        // Se borra solo si ya pasó suficientemente atrás del jugador
        if (jugador.position.y - offsetDespawn > primero.transform.position.y)
        {
            string nombrePool = ObtenerNombreBase(primero.name);
            PoolManager.Instance.ReturnToPool("carretera", nombrePool, primero);
            carreteras.RemoveAt(0);
            Debug.Log($"♻️ Devuelto al pool: {nombrePool}");
        }
    }

    // 🔹 Ayuda para quitar "(Clone)" o sufijos de Unity
    private string ObtenerNombreBase(string name)
    {
        string limpio = name.Replace("(Clone)", "").Trim();
        // Si el objeto tiene numeraciones tipo carretera3_0 etc
        int index = limpio.IndexOf("_");
        if (index > 0) limpio = limpio.Substring(0, index);
        return limpio;
    }

    // =============================
    //  🔁 CAMBIO DE TIPO DE CARRETERA
    // =============================
    public void CambiarTipoCarretera(int nuevoNumCarriles)
    {
        if (enTransicion || nuevoNumCarriles == cantCarriles) return;
        enTransicion = true;

        string nombreCambio = $"cambio{cantCarriles}{nuevoNumCarriles}";
        string nombreNueva = $"carretera{nuevoNumCarriles}";

        // Instanciamos transición y luego cambiamos
        GameObject trans = PoolManager.Instance.GetFromPool("carretera", nombreCambio);
        if (trans != null)
        {
            trans.transform.position = new Vector3(0, spawnZ, 1f);
            trans.transform.rotation = Quaternion.identity;
            trans.transform.SetParent(transform);
            carreteras.Add(trans);
            spawnZ += largoChunk;
        }
        else
        {
            Debug.LogWarning($"⚠️ No se encontró el prefab de transición {nombreCambio}");
        }

        cantCarriles = nuevoNumCarriles;
        enTransicion = false;

        Debug.Log($"✅ Cambio completado: ahora {cantCarriles} carriles");
    }
}
