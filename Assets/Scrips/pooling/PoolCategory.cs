using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolCategory
{
    public string categoryName;               // Ej: "Armas", "Enemigos", "Carreteras"
    public List<ObjectPool> pools;            // Lista de pools dentro de esta categoría

    private Dictionary<string, ObjectPool> poolDictionary;

    public void Initialize(Transform parent)
    {
        poolDictionary = new Dictionary<string, ObjectPool>();

        Transform catParent = new GameObject($"Category_{categoryName}").transform;
        catParent.SetParent(parent);

        foreach (ObjectPool pool in pools)
        {
            pool.Initialize(catParent);
            poolDictionary.Add(pool.poolName, pool);
        }
    }

    public ObjectPool GetPool(string poolName)
    {
        if (poolDictionary.TryGetValue(poolName, out ObjectPool pool))
            return pool;

        Debug.LogWarning($"[PoolCategory] Pool '{poolName}' no encontrado en categoría '{categoryName}'");
        return null;
    }
}
