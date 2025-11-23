using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AchievementsWindowController : MonoBehaviour
{
    public Text kmText;
    public Text carrosText;

    private void Start()
    {
        kmText.text = $"Km Totales: {StatsManager.Instance.maxKm:F1}";
        carrosText.text = $"Carros Destruidos Totales: {StatsManager.Instance.maxCarros}";
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
