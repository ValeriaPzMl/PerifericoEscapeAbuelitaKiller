using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;   // ← NECESARIO para IEnumerator

public class GameOverController : MonoBehaviour
{
    public GameObject gameOverPanel;

    private bool waitingForInput;
    private float partidaKm;
    private int partidaCarros;
    public AudioSource final;
    public TextMeshProUGUI textContinue;
    public void GameOver(float kmRecorridos, int carrosDestruidos)
    {
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
        ResetCursorToDefault();

        partidaKm = kmRecorridos;
        partidaCarros = carrosDestruidos;

        StartCoroutine(EsperarAudioYActivarInput());
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
        ResetCursorToDefault();

        partidaKm = 0;
        partidaCarros = 0;

        StartCoroutine(EsperarAudioYActivarInput());
    }

    private IEnumerator EsperarAudioYActivarInput()
    {
        float waitTime = (final != null && final.clip != null)
            ? final.clip.length : 0.1f;

        if (final != null)
            final.Play();

        yield return new WaitForSecondsRealtime(waitTime);

        waitingForInput = true;
        textContinue.gameObject.SetActive(true);
    }


    private void Update()
    {
        if (!waitingForInput) return;

        if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
            Continuar();
    }

    private void Continuar()
    {
        waitingForInput = false;
        textContinue.gameObject.SetActive(false);
        Time.timeScale = 1f;

        StatsManager.Instance.ProcesarStats(partidaKm, partidaCarros);

        EndOfRunData.km = partidaKm;
        EndOfRunData.carros = partidaCarros;

        SceneManager.LoadScene("GameOver");
    }

    public void ResetCursorToDefault()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
