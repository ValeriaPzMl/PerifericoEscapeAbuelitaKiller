using UnityEngine;

public class FollowForwardLimit : MonoBehaviour
{
    public Transform target;      // El jugador
    public float distancia = 10f;
    public float suavizado = 15f;

    void LateUpdate()
    {
        if (target == null) return;

        // Posición deseada detrás del jugador
        Vector3 objetivo = target.position - target.up * distancia;

        // 🛑 NO seguir si el jugador está igual o más abajo en Y
        if (objetivo.y <= transform.position.y)
            return;

        // Sí seguir si el jugador está arriba
        transform.position = Vector3.Lerp(transform.position, objetivo, Time.deltaTime * suavizado);
    }
}
