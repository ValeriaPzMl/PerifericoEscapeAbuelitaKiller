using UnityEngine;
using TMPro; // Asegúrate de tener TextMeshPro importado

public class DisplayData : MonoBehaviour
{
    public PositionManager tracker;           // Arrastra aquí el objeto que se mueve
    public TextMeshProUGUI differenceText;    // Arrastra el componente de texto UI

    void Update()
    {
        // Mostramos el valor en el UI (con dos decimales)
        differenceText.text = tracker.differenceX.ToString("F2")+"Km" ;
    }
}
