using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target a seguir")]
    public Transform target; // El personaje principal

    [Header("Ajustes de seguimiento")]
    public float smoothSpeed = 0.125f; // Velocidad del suavizado
    public Vector3 offset; // Desplazamiento de la cámara respecto al personaje

    private void LateUpdate()
    {
        if (target == null) return; // Si no hay objetivo, no hacer nada

        // Posición deseada con offset
        Vector3 desiredPosition = target.position + offset;

        // Interpolación suave entre la posición actual y la deseada
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Aplicar posición final, manteniendo Z fijo (importante en 2D)
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }
}
