using UnityEngine;

public class DificultadManager : MonoBehaviour
{
    [Header("Referencias")]
    public PositionManager positionManager;
    public CarreteraSpawner carreteraSpawner;
    public UnifiedSpawner unifiedSpawner;

    [Header("Configuración de dificultad")]
    public float kmCambioA4 = 1f;
    public float kmCambioA5 = 2f;

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
        int kmInt =(int) Mathf.Round(km*10);
        
        if (dificultad < kmInt&&dificultad<10)
        {
            Debug.Log($"km int {kmInt}  dificultad {dificultad}");
            dificultad=kmInt;
            switch (kmInt)
            {
                case 1: 
                    //piña
                    unifiedSpawner.SubirDificultad(1);
                    unifiedSpawner.SpawnArma("catapultaPina");
                    break;
                case 2: 
                    //ropa
                    unifiedSpawner.SubirDificultad(2);
                    unifiedSpawner.SpawnArma("lanzaCamisas");
                    break;
                case 5: 
                    //clavos
                    unifiedSpawner.SubirDificultad(3);
                    unifiedSpawner.SpawnArma("pistolaClavos");

                    break;
                case 6:
                    unifiedSpawner.SpawnArma("bazookaConfetti");
                    break;
                case 8: 
                    //helado
                    unifiedSpawner.SubirDificultad(4);
                    unifiedSpawner.SpawnArma("helado");

                    break;
                case 10: 
                    // energia
                    unifiedSpawner.SubirDificultad(5);
                    unifiedSpawner.SpawnArma("canonEnergia");

                    break;
                case 13: 
                    //bazooka
                    unifiedSpawner.SubirDificultad(6);
                    unifiedSpawner.SpawnArma("bazooka");

                    break;
                case 16: 
                    //atomica 
                    unifiedSpawner.SubirDificultad(7);
                    unifiedSpawner.SpawnArma("bombaAtomica");

                    break;

            }
        }
        

    }

    void CambiarACarriles(int nuevosCarriles)
    {
        carrilesActuales = nuevosCarriles;
        Debug.Log($"Cambiando a carretera de {nuevosCarriles} carriles");

        // 🔹 Actualiza los prefabs de carretera del spawner
        carreteraSpawner.CambiarTipoCarretera(nuevosCarriles);

        // 🔹 Actualiza los carriles del tráfico
        //trafficSpawner.ActualizarCarriles(nuevosCarriles);
        //enemySpawner.ActualizarCarriles(nuevosCarriles);
        unifiedSpawner.ActualizarCarriles(nuevosCarriles);
    }
}
