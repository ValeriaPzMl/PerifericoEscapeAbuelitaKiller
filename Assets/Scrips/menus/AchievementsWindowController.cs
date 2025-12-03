using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class AchievementsWindowController : MonoBehaviour
{
    public TextMeshProUGUI kmText;
    public TextMeshProUGUI carrosText;

    [Header("medallas km")]
    public Image km1;
    public Image km2;
    public Image km3;
    public Image km1D;
    public Image km2D;
    public Image km3D;

    [Header("medallas carros")]
    public Image carros1;
    public Image carros2;
    public Image carros3;
    public Image carros1D;
    public Image carros2D;
    public Image carros3D;



    private void Start()
    {
        float recorkm = StatsManager.Instance.maxKm;
        int recordCarros = StatsManager.Instance.maxCarros;
        kmText.text = $"Record: {recorkm:F2}";
        carrosText.text = $"Record: {recordCarros}";
        if (recordCarros > 50) Existe(carros1, carros1D, 1);
        if (recordCarros > 100) Existe(carros2, carros2D, 2);
        if (recordCarros > 200) Existe(carros3, carros3D, 3);
        if (recorkm >= 2) Existe(km1, km1D, 1);
        if (recorkm >= 4) Existe(km2, km2D, 2);
        if (recorkm >= 6) Existe(km3, km3D, 3);

    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Start");
    }

    private void Existe(Image fondo, Image dibujo, int caso)
    {
        Color color=new Color(0, 0, 0);


        switch (caso)
        {
            case 1: color = RGB(120, 93, 49); break;
            case 2: color = RGB(105, 104, 104); break;
            case 3: color = RGB(196, 171, 4); break;
        }
        Debug.Log($"entro debug caso {caso} el nuevo color es {color}");
        fondo.color = color;
        dibujo.gameObject.SetActive(true);


    }
    private Color RGB(float r, float g, float b)
    {
        return new Color(r / 255f, g / 255f, b / 255f);
    }

}
