using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EndStatsUI : MonoBehaviour
{
    public TextMeshProUGUI kmPartidaText;
    public TextMeshProUGUI carrosPartidaText;

    public TextMeshProUGUI kmRecordText;
    public TextMeshProUGUI carrosRecordText;

    public void Start()
    {
        kmPartidaText.text = $"Km esta partida: {EndOfRunData.km:F2}";
        carrosPartidaText.text = $"Carros destruidos: {EndOfRunData.carros}";

        kmRecordText.text = $"Récord Km: {StatsManager.Instance.maxKm:F2}";
        carrosRecordText.text = $"Récord Carros: {StatsManager.Instance.maxCarros}";
    }

    public void VolverMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
