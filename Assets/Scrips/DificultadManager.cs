using UnityEngine;

public class DificultadManager : MonoBehaviour
{
    [Header("Referencias")]
    public PositionManager positionManager;
    public CarreteraSpawner carreteraSpawner;
    public EnemySpawner enemySpawner;
    public TrafficSpawner trafficSpawner;

    [Header("Configuración de dificultad")]
    public float kmCambioA4 = 2f;
    public float kmCambioA5 = 5f;

    private int carrilesActuales = 3;
    private int dificultad;

    private void Start()
    {
        dificultad = 0;
    }
    void Update()
    {
        float km = positionManager.differenceX;

        if (carrilesActuales == 3 && km >= kmCambioA4)
        {
            CambiarACarriles(4);
        }
        else if (carrilesActuales == 4 && km >= kmCambioA5)
        {
            CambiarACarriles(5);
        }
        int kmInt =(int) Mathf.Floor(km);
        if (dificultad < kmInt&&dificultad<8)
        {
            
            dificultad = kmInt;
            enemySpawner.SubirDificultad(dificultad);
            Debug.Log($"subio la dificultada {dificultad}");
        }
        

    }

    void CambiarACarriles(int nuevosCarriles)
    {
        carrilesActuales = nuevosCarriles;
        Debug.Log($"Cambiando a carretera de {nuevosCarriles} carriles");

        // 🔹 Actualiza los prefabs de carretera del spawner
        carreteraSpawner.CambiarTipoCarretera(nuevosCarriles);

        // 🔹 Actualiza los carriles del tráfico
        trafficSpawner.ActualizarCarriles(nuevosCarriles);
        enemySpawner.ActualizarCarriles(nuevosCarriles);
    }
}
