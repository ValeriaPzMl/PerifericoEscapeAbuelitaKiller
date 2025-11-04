using UnityEngine;
using TMPro; // Asegúrate de tener TextMeshPro importado

public class DisplayData : MonoBehaviour
{
    public PositionManager tracker;           // Arrastra aquí el objeto que se mueve
    public TextMeshProUGUI differenceText;    // Arrastra el componente de texto UI
    public PlayerPhysicsController vidaGeter;
    public TextMeshProUGUI vida;
    public TextMeshProUGUI carrosM;
    private int carrosMuertos;


    void Update()
    {
        // Mostramos el valor en el UI (con dos decimales)
        differenceText.text = tracker.differenceX.ToString("F2")+"Km" ;
        vida.text = (vidaGeter.getLife()).ToString();
        carrosM.text =carrosMuertos.ToString();

    }
    public void CarroMuerto()
    {
        carrosMuertos++;
    }
    
}
