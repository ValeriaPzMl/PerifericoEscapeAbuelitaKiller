using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento hacia adelante")]
    public float velocidad = 5f;
    public float aceleracion = 0.2f;
    public float freno = 0.3f;
    public float velMin = 2f;
    public float velMax = 15f;

    [Header("Carriles")]
    public float[] posicionesCarriles;
    private int carrilActual = 1;
    public float velocidadCambioCarril = 10f;

    private void Start()
    {
        transform.position = new Vector3(posicionesCarriles[carrilActual], 0f, 0f);
    }

    private void Update()
    {
        // Acelerar / frenar
        if (UnityEngine.Input.GetKey(KeyCode.UpArrow))
            velocidad += aceleracion;

        if (UnityEngine.Input.GetKey(KeyCode.DownArrow))
            velocidad -= freno;

        velocidad = Mathf.Clamp(velocidad, velMin, velMax);

        // Avanzar hacia arriba en el mundo
        transform.Translate(Vector3.up * velocidad * Time.deltaTime);

        // Cambio de carril
        if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow) && carrilActual > 0)
            carrilActual--;

        if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow) && carrilActual < posicionesCarriles.Length - 1)
            carrilActual++;

        Vector3 posDeseada = new Vector3(posicionesCarriles[carrilActual], transform.position.y, 0);
        transform.position = Vector3.Lerp(transform.position, posDeseada, velocidadCambioCarril * Time.deltaTime);
    }
}
