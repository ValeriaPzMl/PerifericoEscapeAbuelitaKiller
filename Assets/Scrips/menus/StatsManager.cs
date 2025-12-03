using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

    public float maxKm;
    public int maxCarros;
    public bool cambio;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        CargarStats();
    }

    public void ProcesarStats(float km, int carros)
    {
        cambio = false;

        if (km > maxKm)
        {
            maxKm = km;
            cambio = true;
        }

        if (carros > maxCarros)
        {
            maxCarros = carros;
            cambio = true;
        }

        if (cambio) GuardarStats();
    }

    public void GuardarStats()
    {
        PlayerPrefs.SetFloat("maxKm", maxKm);
        PlayerPrefs.SetInt("maxCarros", maxCarros);
        PlayerPrefs.Save();
    }

    public void CargarStats()
    {
        maxKm = PlayerPrefs.GetFloat("maxKm", 0f);
        maxCarros = PlayerPrefs.GetInt("maxCarros", 0);
    }
}
