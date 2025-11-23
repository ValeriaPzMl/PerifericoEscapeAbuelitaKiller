using UnityEngine;

public class PositionManager : MonoBehaviour
{
    [HideInInspector] public float differenceX; // Será accesible desde otros scripts
    private float initialy;

    void Start()
    {
        // Guardamos la posición inicial
        initialy = transform.position.y;
    }

    void Update()
    {
        // Calculamos la diferencia actual
        differenceX = (transform.position.y - initialy)/1000;
    }
    
}