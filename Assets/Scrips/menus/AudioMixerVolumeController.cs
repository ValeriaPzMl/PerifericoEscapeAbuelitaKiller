using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMixerVolumeController : MonoBehaviour
{
    [Header("Mixer")]
    public AudioMixer mixer;                 // arrastra aquí tu AudioMixer
    public string exposedParam = "MasterVolume"; // nombre exacto del parametro expuesto

    [Header("UI (opcional)")]
    public Slider volumeSlider;              // arrastra el slider (opcional)
    public float defaultVolume = 1f;         // valor default 0..1

    // PlayerPrefs key
    private const string PREF_KEY = "master_volume";

    void Start()
    {
        // cargar valor guardado (si existe)
        float saved = PlayerPrefs.GetFloat(PREF_KEY, defaultVolume);

        // aplicar al mixer
        SetMasterVolume(saved, save: false);

        // si hay slider, inicializarlo y conectarlo
        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = saved;

            // remover listeners previos y añadir el nuestro
            volumeSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.onValueChanged.AddListener((v) => SetMasterVolume(v, save: true));
        }
    }

    /// <summary>
    /// Ajusta el volumen del AudioMixer.
    /// sliderValue es 0..1 (0=mute, 1=full)
    /// save=true guardará en PlayerPrefs
    /// </summary>
    public void SetMasterVolume(float sliderValue, bool save = true)
    {
        if (mixer == null)
        {
            Debug.LogWarning("[AudioMixerVolumeController] mixer no asignado.");
            return;
        }

        // evitar log(0). si es 0 usamos valor muy bajo en dB (mute)
        const float minDb = -80f; // valor de "mute" para la mayoría de mixers
        float dB;

        if (sliderValue <= 0.0001f)
        {
            dB = minDb;
        }
        else
        {
            // conversión logarítmica: 20 * log10(slider)
            dB = Mathf.Clamp(20f * Mathf.Log10(sliderValue), minDb, 0f);
        }

        mixer.SetFloat(exposedParam, dB);

        if (save)
            PlayerPrefs.SetFloat(PREF_KEY, sliderValue);
    }
}
