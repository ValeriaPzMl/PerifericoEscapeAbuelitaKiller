using UnityEngine;

[ExecuteInEditMode]
public class GridDrawer : MonoBehaviour
{
    public float cellSize = 1f;       // Tamaño de cada celda en Unity units
    public int gridExtent = 20;       // Cuántas celdas alrededor de la cámara
    public Color gridColor = Color.gray;
    public Camera targetCamera;

    private void OnDrawGizmos()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null)
            return;

        Gizmos.color = gridColor;

        // Tomar la posición de la cámara
        Vector3 camPos = targetCamera.transform.position;

        // Redondear al múltiplo más cercano de cellSize (para que la grilla se alinee)
        float startX = Mathf.Floor(camPos.x / cellSize - gridExtent) * cellSize;
        float endX = Mathf.Floor(camPos.x / cellSize + gridExtent) * cellSize;
        float startY = Mathf.Floor(camPos.y / cellSize - gridExtent) * cellSize;
        float endY = Mathf.Floor(camPos.y / cellSize + gridExtent) * cellSize;

        // Dibujar líneas verticales
        for (float x = startX; x <= endX; x += cellSize)
        {
            Gizmos.DrawLine(new Vector3(x, startY, 0), new Vector3(x, endY, 0));
        }

        // Dibujar líneas horizontales
        for (float y = startY; y <= endY; y += cellSize)
        {
            Gizmos.DrawLine(new Vector3(startX, y, 0), new Vector3(endX, y, 0));
        }
    }
}
