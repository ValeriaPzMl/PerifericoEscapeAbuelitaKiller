using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    [Header("Categorías de Pools")]
    public List<PoolCategory> categories;

    private Dictionary<string, PoolCategory> categoryDictionary;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        //DontDestroyOnLoad(gameObject);

        categoryDictionary = new Dictionary<string, PoolCategory>();

        foreach (PoolCategory cat in categories)
        {
            cat.Initialize(transform);
            categoryDictionary.Add(cat.categoryName, cat);
        }
    }

    public GameObject GetFromPool(string categoryName, string poolName)
    {
        if (categoryDictionary.TryGetValue(categoryName, out PoolCategory cat))
        {
            ObjectPool pool = cat.GetPool(poolName);
            if (pool != null)
                return pool.GetObject();
        }

        Debug.LogWarning($"[PoolManager] No se encontró el pool '{poolName}' en categoría '{categoryName}'");
        return null;
    }

    public void ReturnToPool(string categoryName, string poolName, GameObject obj)
    {
        if (categoryDictionary.TryGetValue(categoryName, out PoolCategory cat))
        {
            ObjectPool pool = cat.GetPool(poolName);
            if (pool != null)
                pool.ReturnObject(obj);
        }
    }
}
