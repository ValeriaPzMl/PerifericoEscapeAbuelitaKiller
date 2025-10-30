using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPool
{
    public string poolName;           // Nombre del pool (ej: "Projectile_Sandia")
    public GameObject prefab;         // Prefab base
    public int initialAmount = 10;    // Cuántos instanciar al inicio
    public bool expandable = true;    // Si puede crear más si se acaban

    private Queue<GameObject> poolObjects;
    private Transform parent;         // Para mantener la jerarquía limpia

    public void Initialize(Transform parentTransform)
    {
        parent = new GameObject($"Pool_{poolName}").transform;
        parent.SetParent(parentTransform);
        poolObjects = new Queue<GameObject>();

        for (int i = 0; i < initialAmount; i++)
        {
            GameObject obj = GameObject.Instantiate(prefab, parent);
            obj.SetActive(false);
            poolObjects.Enqueue(obj);
        }
    }

    public GameObject GetObject()
    {
        if (poolObjects.Count > 0)
        {
            GameObject obj = poolObjects.Dequeue();
            obj.SetActive(true);
            IPooledObject pooledObj = obj.GetComponent<IPooledObject>();
            if (pooledObj != null)
            {
                pooledObj.OnSpawn();
            }
            return obj;
        }
        else if (expandable)
        {
            GameObject obj = GameObject.Instantiate(prefab, parent);
            obj.SetActive(true);
            Debug.Log($"expandido {poolName}");
            IPooledObject pooledObj = obj.GetComponent<IPooledObject>();
            if (pooledObj != null)
            {
                pooledObj.OnSpawn();
            }
            return obj;
        }

        return null;
    }

    public void ReturnObject(GameObject obj)
    {
        IPooledObject pooledObj = obj.GetComponent<IPooledObject>();
        if (pooledObj != null)
        {
            pooledObj.OnDespawn();
        }

        obj.SetActive(false);
        obj.transform.SetParent(parent);
        poolObjects.Enqueue(obj);
    }
}
