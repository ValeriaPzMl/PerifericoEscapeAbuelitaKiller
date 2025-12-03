using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject pausePanel;

    [Header("Cursor")]
    public Texture2D gameCursorTexture;       // asigna tu textura aquí
    public bool autoCenterHotspot = true;     // centra hotspot automáticamente
    public Vector2 manualHotspot = new Vector2(0, 0);
    public int maxCursorSize = 64;            // tamaño máximo en píxeles del cursor (64 es buen valor)
    public bool hideCursorDuringPlay = false; // si true oculta el cursor en play (ej. FPS); si false se muestra siempre

    [Header("Audio")]
    public bool keepMusicPlaying = true;

    private float previousTimeScale = 1f;
    private float defaultFixedDelta;
    private bool isPaused = false;
    public static bool GameIsPaused { get; private set; } = false;

    // textura escalada que realmente usaremos
    private Texture2D runtimeCursorTexture;
    private Vector2 runtimeHotspot;

    void Awake()
    {
        defaultFixedDelta = Time.fixedDeltaTime;
        if (pausePanel != null) pausePanel.SetActive(false);
        PrepareCursor();
        ApplyCursor(); // aplica al iniciar
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
    }
    public void Quit()
    {
        Time.timeScale = 1f; // MUY IMPORTANTE
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("Start");
    }
    // Prepara runtimeCursorTexture (posible reescalado) y hotspot
    private void PrepareCursor()
    {
        if (gameCursorTexture == null)
        {
            runtimeCursorTexture = null;
            runtimeHotspot = Vector2.zero;
            Debug.LogWarning("[PauseMenu] No gameCursorTexture assigned.");
            return;
        }

        // Si ya es pequeño, lo usamos tal cual
        if (gameCursorTexture.width <= maxCursorSize && gameCursorTexture.height <= maxCursorSize)
        {
            runtimeCursorTexture = gameCursorTexture;
        }
        else
        {
            // reescala a maxCursorSize manteniendo ratio
            float ratio = Mathf.Min((float)maxCursorSize / gameCursorTexture.width, (float)maxCursorSize / gameCursorTexture.height);
            int newW = Mathf.Max(1, Mathf.RoundToInt(gameCursorTexture.width * ratio));
            int newH = Mathf.Max(1, Mathf.RoundToInt(gameCursorTexture.height * ratio));
            runtimeCursorTexture = ScaleTexture(gameCursorTexture, newW, newH);
            Debug.Log("[PauseMenu] Cursor scaled from " + gameCursorTexture.width + "x" + gameCursorTexture.height + " to " + newW + "x" + newH);
        }

        // hotspot auto o manual
        if (autoCenterHotspot && runtimeCursorTexture != null)
        {
            runtimeHotspot = new Vector2(runtimeCursorTexture.width / 2f, runtimeCursorTexture.height / 2f);
        }
        else
        {
            runtimeHotspot = manualHotspot;
        }
    }

    private void ApplyCursor()
    {
        if (runtimeCursorTexture != null)
        {
            Cursor.SetCursor(runtimeCursorTexture, runtimeHotspot, CursorMode.ForceSoftware);
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        // visibilidad según preferencia
        Cursor.visible = !hideCursorDuringPlay ? true : false;
        Cursor.lockState = hideCursorDuringPlay ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void TogglePause()
    {
        if (isPaused) Unpause(); else Pause();
    }

    public void Pause()
    {
        if (isPaused) return;

        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        Time.fixedDeltaTime = defaultFixedDelta * Time.timeScale;
        isPaused = true;
        GameIsPaused = true;

        if (pausePanel != null) pausePanel.SetActive(true);

        // en pausa: queremos cursor normal del sistema (no tu sprite)
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (!keepMusicPlaying) AudioListener.pause = true;
    }

    public void Resume()
    {
        if (!isPaused) return;
        Unpause();
    }

    private void Unpause()
    {
        Time.timeScale = previousTimeScale == 0f ? 1f : previousTimeScale;
        Time.fixedDeltaTime = defaultFixedDelta * Time.timeScale;
        isPaused = false;
        GameIsPaused = false;

        if (pausePanel != null) pausePanel.SetActive(false);

        // reinstaurar TU cursor personalizado
        ApplyCursor();

        if (!keepMusicPlaying) AudioListener.pause = false;
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = defaultFixedDelta;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    // ---------------- Helper: escalado de textura via RenderTexture ----------------
    // Esta función crea una copia escalada de la textura original (mantiene transparencia)
    private Texture2D ScaleTexture(Texture2D src, int targetWidth, int targetHeight)
    {
        // crear RenderTexture temporal
        RenderTexture rt = RenderTexture.GetTemporary(targetWidth, targetHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        rt.filterMode = FilterMode.Bilinear;

        // copiar la textura al RT
        RenderTexture activeRT = RenderTexture.active;
        Graphics.Blit(src, rt);

        // leer los píxeles del RT
        RenderTexture.active = rt;
        Texture2D result = new Texture2D(targetWidth, targetHeight, TextureFormat.ARGB32, false);
        result.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        result.Apply();

        // restaurar y liberar
        RenderTexture.active = activeRT;
        RenderTexture.ReleaseTemporary(rt);

        // importante: marcar como no 'hideFlags' ni Read/Write específicos; es un texture runtime.
        return result;
    }

}
