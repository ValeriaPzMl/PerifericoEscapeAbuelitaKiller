using UnityEngine;

public class LookAtMouse2D : MonoBehaviour
{
    public int plus;
    void Update()
    {
        if (PauseMenu.GameIsPaused) return;
        // Obtener posición del mouse en coordenadas del mundo
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Asegura que el Z no afecte la rotación en 2D

        // Calcular dirección hacia el mouse
        Vector3 direction = mousePos - transform.position;

        // Calcular ángulo
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Aplicar rotación
        transform.rotation = Quaternion.Euler(0, 0, angle+plus);
    }
}
