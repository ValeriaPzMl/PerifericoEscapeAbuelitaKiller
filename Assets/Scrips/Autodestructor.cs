using UnityEngine;

public class Autodestructor : MonoBehaviour

    
{
    private Transform player;
    private int distanciaMaxima=50;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Busca al objeto con tag "Player"
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            player = jugador.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        // Calcula la distancia entre este objeto y el jugador
        float distancia = Vector3.Distance(transform.position, player.position);

        if (distancia >= distanciaMaxima)
        {
            Debug.Log("Objeto destruido por alejarse demasiado del jugador.");
            Destroy(gameObject);
        }
    }
}
