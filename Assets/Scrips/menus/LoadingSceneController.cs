using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class LoadingSceneController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI mensajeTMP;

    [Header("Mensajes Aleatorios")]
    public List<string> mensajes = new List<string>();

    [Header("Tiempos")]
    public float tiempoAntesDeCargar = 4f;
    public float tiempoCambioMensaje = 2f;

    private void Start()
    {
        Time.timeScale = 1f;
        StartCoroutine(CambiarMensajes());
        StartCoroutine(CargarGameplay());
    }

    IEnumerator CargarGameplay()
    {
        // Espera antes de iniciar la carga real
        yield return new WaitForSeconds(tiempoAntesDeCargar);

        AsyncOperation load = SceneManager.LoadSceneAsync("GamePlay");

        // Esperar que termine
        while (!load.isDone)
        {
            yield return null;
        }
    }

    IEnumerator CambiarMensajes()
    {
        // Si no hay mensajes, no hacemos nada
        if (mensajes.Count == 0 || mensajeTMP == null)
            yield break;

        while (true)
        {
            int randomIndex = Random.Range(0, mensajes.Count);
            mensajeTMP.text = mensajes[randomIndex];

            yield return new WaitForSeconds(tiempoCambioMensaje);
        }
    }
}
