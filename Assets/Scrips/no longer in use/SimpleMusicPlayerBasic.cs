using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Asegúrate de tener esto arriba

public class SimpleMusicPlayerBasic : MonoBehaviour
{
    public AudioSource audioSource;            // Arrastra aquí tu AudioSource
    public List<AudioClip> tracks = new List<AudioClip>();
    public Button playPauseButton;   // aquí arrastras TU BOTÓN
    public Sprite playSprite;        // sprite de Play
    public Sprite pauseSprite;       // sprite de Pause


    private int currentIndex = 0;
    private bool wasPlayingBeforeEndCheck = false;

    void Start()
    {
        if (audioSource == null)
        {
            Debug.LogError("SimpleMusicPlayerBasic: asigna un AudioSource en el inspector.");
            enabled = false;
            return;
        }

        if (tracks == null || tracks.Count == 0)
        {
            Debug.LogWarning("SimpleMusicPlayerBasic: no hay pistas en la lista.");
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false; // controlamos avance manualmente
    }

    void Update()
    {
        // Detectar fin de pista para avanzar (solo si estaba reproduciéndose)
        if (audioSource.clip != null)
        {
            if (!audioSource.isPlaying && wasPlayingBeforeEndCheck)
            {
                // If clip finished (time at/near end), avanzar a siguiente
                if (audioSource.time >= audioSource.clip.length - 0.05f)
                {
                    NextTrack();
                }
                wasPlayingBeforeEndCheck = false;
            }
            else if (audioSource.isPlaying)
            {
                wasPlayingBeforeEndCheck = true;
            }
        }
    }

    // Botón Play/Pausa
    public void PlayPauseToggle()
    {
        if (audioSource == null) return;

        if (audioSource.clip == null && tracks.Count > 0)
        {
            currentIndex = Mathf.Clamp(currentIndex, 0, tracks.Count - 1);
            audioSource.clip = tracks[currentIndex];
            audioSource.time = 0f;
        }

        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            UpdatePlayPauseSprite(false);
        }
        else
        {
            audioSource.Play();
            UpdatePlayPauseSprite(true);
        }
    }

    private void UpdatePlayPauseSprite(bool isPlaying)
    {
        if (playPauseButton == null) return;

        Image img = playPauseButton.GetComponent<Image>(); // saca la imagen del botón

        if (img == null) return;

        img.sprite = isPlaying ? pauseSprite : playSprite;
    }



    // Botón Siguiente
    public void NextTrack()
    {
        if (tracks == null || tracks.Count == 0) return;
        currentIndex = (currentIndex + 1) % tracks.Count;
        PlayCurrentIndex();
    }

    // Botón Anterior
    public void PreviousTrack()
    {
        if (tracks == null || tracks.Count == 0) return;

        // Si la canción tiene más de 2 segundos, volver al inicio; si no, ir a la anterior
        if (audioSource.clip != null && audioSource.time > 2f)
        {
            audioSource.time = 0f;
            if (!audioSource.isPlaying) audioSource.Play();
            return;
        }

        currentIndex = (currentIndex - 1 + tracks.Count) % tracks.Count;
        PlayCurrentIndex();
    }

    private void PlayCurrentIndex()
    {
        audioSource.clip = tracks[currentIndex];
        audioSource.time = 0f;
        audioSource.Play();
    }
}
