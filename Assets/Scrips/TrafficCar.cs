using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficCar : MonoBehaviour, IPooledObject
{
    private float distanciaMaxima = 80f; // Distancia máxima para destruirse
    private Transform player;
    public float velocidad = 5f;
    public float tiempoVida = 15f; // para destruirlo si se pasa
    public int vidaInicial;
    private int health;

    [Header("Área de detección del carro")]
    public float anchoDeteccion;
    public float altoDeteccion;
    void Start()
    {
        // Busca al objeto con tag "Player"
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            player = jugador.transform;
        }
    }
    void Update()
    {
        // Si prefieres que se muevan hacia abajo:
        transform.Translate(Vector3.up * velocidad * Time.deltaTime);

        if (player == null) return;

        // Calcula la distancia entre este objeto y el jugador
        float distancia = Vector3.Distance(transform.position, player.position);

        if (distancia >= distanciaMaxima)
        {

            DevolverAlPool();
        }
    }
    private string ObtenerNombreBase(string name)
    {
        string limpio = name.Replace("(Clone)", "").Trim();
        // Si el objeto tiene numeraciones tipo carretera3_0 etc
        int index = limpio.IndexOf("_");
        if (index > 0) limpio = limpio.Substring(0, index);
        return limpio;
    }
    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
            DevolverAlPool();
    }

    private void DevolverAlPool()
    {
        string nombrePool = ObtenerNombreBase(gameObject.name);
        PoolManager.Instance.ReturnToPool("NPCs", nombrePool, gameObject);
        Debug.Log($"♻️ Devuelto al pool: {nombrePool}");
    }
    public Vector2 GetDetectionSize()
    {
        return new Vector2(anchoDeteccion, altoDeteccion);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan; // azulito para distinguirlos
        Gizmos.DrawWireCube(transform.position, new Vector3(anchoDeteccion, altoDeteccion, 0));
    }

    public void OnSpawn()
    {
        health = vidaInicial;
    }

    public void OnDespawn()
    {
        
        // 🔹 Copiar referencias a los hijos
        Debug.Log($"transform.childCount = {transform.childCount}");
        List<GameObject> hijos = new List<GameObject>();
        foreach (Transform child in transform)
        {
            hijos.Add(child.gameObject);
            Debug.Log("-> child directo: " + child.name);
        }
        Debug.Log($"hijos detectados {hijos.Count}");
        // 🔹 Desconectarlos primero (esto evita el bug del hijo que queda)
        transform.DetachChildren();

        // 🔹 Ahora despawnear/limpiar cada uno
        foreach (GameObject childGO in hijos)
        {

            Hiteados hitCS = childGO.GetComponent<Hiteados>();

            if (hitCS != null)
            {
                string cat = hitCS.categoryName;
                PoolManager.Instance.ReturnToPool(cat, "hit", childGO);
                Debug.Log($"♻️ Devuelto al pool: {childGO.name} → {cat}/hit");

            }
            
        }
    }
    IEnumerator Esperar()
    {
        
        yield return new WaitForSeconds(10f);  // espera 3 segundos
        
    }


}
