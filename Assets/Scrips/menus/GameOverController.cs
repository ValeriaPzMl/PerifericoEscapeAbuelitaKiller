using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public GameObject gameOverPanel;

    private bool waitingForInput;
    private float partidaKm;
    private int partidaCarros;

    public void GameOver(float kmRecorridos, int carrosDestruidos)
    {
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
        ResetCursorToDefault();
        partidaKm = kmRecorridos;
        partidaCarros = carrosDestruidos;

        waitingForInput = true;
    }
    public void GameOver()
    {
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
        ResetCursorToDefault();
        partidaKm = 0;
        partidaCarros = 0;

        waitingForInput = true;
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
        Time.timeScale = 1f;

        // solo guarda máximos
        StatsManager.Instance.ProcesarStats(partidaKm, partidaCarros);

        // pasa también stats de esta partida para la pantalla final
        EndOfRunData.km = partidaKm;
        EndOfRunData.carros = partidaCarros;
        
        SceneManager.LoadScene("GameOver");
    }
    public void ResetCursorToDefault()
    {
        // restaurar textura por defecto
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        // volver a mostrar cursor
        Cursor.visible = true;

        // desbloquear para que pueda moverse normal
        Cursor.lockState = CursorLockMode.None;
    }

}
