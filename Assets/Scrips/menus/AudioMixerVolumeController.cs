using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class DualAudioMixerController : MonoBehaviour
{
    [Header("Mixer 1 (ej. Música)")]
    public AudioMixer mixer1;
    public string exposedParam1 = "MasterVolumeMusic";
    public Slider slider1;
    public string prefKey1 = "music_volume";

    [Header("Mixer 2 (ej. SFX)")]
    public AudioMixer mixer2;
    public string exposedParam2 = "MasterVolume";
    public Slider slider2;
    public string prefKey2 = "sfx_volume";

    private const float MIN_DB = -80f;

    void Start()
    {
        // Inicializar ambos sliders
        InitSlider(slider1, prefKey1, (value) => SetVolume(mixer1, exposedParam1, value));
        InitSlider(slider2, prefKey2, (value) => SetVolume(mixer2, exposedParam2, value));
    }

    private void InitSlider(Slider slider, string prefKey, System.Action<float> onChange)
    {
        if (slider == null) return;

        // Cargar valor guardado o default 1
        float saved = PlayerPrefs.GetFloat(prefKey, 1f);

        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = saved;

        // Aplicar inmediatamente
        onChange(saved);

        // Listener
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener((v) =>
        {
            onChange(v);
            PlayerPrefs.SetFloat(prefKey, v);
        });
    }

    private void SetVolume(AudioMixer mixer, string exposedParam, float sliderValue)
    {
        if (mixer == null) return;

        float dB = (sliderValue <= 0.0001f)
            ? MIN_DB
            : Mathf.Clamp(20f * Mathf.Log10(sliderValue), MIN_DB, 0f);

        mixer.SetFloat(exposedParam, dB);
    }
}
