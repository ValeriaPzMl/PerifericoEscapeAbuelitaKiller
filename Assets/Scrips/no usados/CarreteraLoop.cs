using UnityEngine;

public class CarreteraLoop : MonoBehaviour
{
    public float velocidad = 5f;        // Velocidad de scroll
    public float alturaSprite = 20f;    // Altura del sprite en unidades

    private void Update()
    {
        // Mover hacia abajo
        transform.Translate(Vector3.down * velocidad * Time.deltaTime);

        // Si salió de la cámara por abajo -> lo reposiciono arriba
        if (transform.position.y <= -alturaSprite)
        {
            ReposicionarArriba();
        }
    }

    void ReposicionarArriba()
    {
        // Buscar el otro tile de carretera
        CarreteraLoop[] carreteras = FindObjectsOfType<CarreteraLoop>();

        foreach (CarreteraLoop c in carreteras)
        {
            if (c != this)
            {
                // Poner esta carretera justo arriba de la otra
                float nuevaY = c.transform.position.y + alturaSprite;
                transform.position = new Vector3(transform.position.x, nuevaY, transform.position.z);
                break;
            }
        }
    }
}
