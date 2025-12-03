using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EndStatsUI : MonoBehaviour
{
    public TextMeshProUGUI kmPartidaText;
    public TextMeshProUGUI carrosPartidaText;
    public TextMeshProUGUI kmPartidaSombra;
    public TextMeshProUGUI carrosPartidaSombra;
    public TextMeshProUGUI nRecord;


    private bool record;

    public void Start()
    {
        kmPartidaText.text = $"{EndOfRunData.km:F2}";
        carrosPartidaText.text = $"{EndOfRunData.carros}";
        kmPartidaSombra.text = $"{EndOfRunData.km:F2}";
        carrosPartidaSombra.text = $"{EndOfRunData.carros}";

        record = StatsManager.Instance.cambio;
        nRecord.gameObject.SetActive(record);

        // kmRecordText.text = $"{:F2}";
        // carrosRecordText.text = $"{StatsManager.Instance.maxCarros}";
    }

    public void VolverMenu()
    {
        SceneManager.LoadScene("Start");
    }
    public void VentanaAch() { 
        SceneManager.LoadScene("AchievementsScene");
    }
}
