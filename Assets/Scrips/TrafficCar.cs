using System;
using UnityEngine;

public class TrafficCar : MonoBehaviour
{
    private float distanciaMaxima = 80f; // Distancia máxima para destruirse
    private Transform player;
    public float velocidad = 5f;
    public float tiempoVida = 15f; // para destruirlo si se pasa
    public int health;

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
            Debug.Log("Objeto destruido por alejarse demasiado del jugador.");
            Destroy(gameObject);
        }
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
    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Destroy(gameObject); // más adelante explosión o animación
        }
    }
}
