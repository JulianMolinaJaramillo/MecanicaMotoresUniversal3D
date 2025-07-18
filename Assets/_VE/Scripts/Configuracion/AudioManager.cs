using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager singleton; // Singleton

    [Header("Audio Sources")]
    public AudioSource musica; // AudioSource que controlará la música de fondo
    public AudioSource efectos;  // AudioSource que controlará los efectos de sonido

    [Header("Clips de Audio")]
    public AudioClip[] musicaClips;  // Array de música
    public AudioClip[] efectosClips;    // Array de efectos de sonido

    [Header("Audio Mixer")]
    public AudioMixer audioMixer; // Referencia al MainAudioMixer

    [Header("UI Sliders")]
    public Slider musicSlider; // Slider para musica
    public Slider efectosSlider; // Slider para efectos

    [Header("Musica de fondo Inicial")]
    public int index;
    public bool iniciarConMusica;

    private Dictionary<string, AudioClip> efectosDict;

    private void Awake()
    {
        // Configurar Singleton
        if (singleton == null)
        {
            singleton = this;
        }
    }

    private void Start()
    {
        if (musicSlider != null || efectosSlider!= null)
        {
            // Configurar los valores iniciales de los sliders
            float musicVolume;
            audioMixer.GetFloat("MusicVolume", out musicVolume); // Obtenemos el valor que tenemos por defecto en el Audio Mixer
            musicSlider.value = musicVolume; // Asignamos el valor por defecto al slider

            float sfxVolume;
            audioMixer.GetFloat("SFXVolume", out sfxVolume); // Obtenemos el valor que tenemos por defecto en el Audio Mixer
            efectosSlider.value = sfxVolume; // Asignamos el valor por defecto al slider

            // Añadir listeners a los sliders
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            efectosSlider.onValueChanged.AddListener(SetSFXVolume);

            if (iniciarConMusica)
            {
                PlayMusic(index); // Reproducimos la musica de fondo para la escena en cuestion  
            }
        }

        efectosDict = new Dictionary<string, AudioClip>();

        foreach (var clip in efectosClips)
        {
            if (clip != null && !efectosDict.ContainsKey(clip.name))
            {
                efectosDict.Add(clip.name, clip);
            }
        }
    }

    /// <summary>
    /// Método para ajustar el volumen de la musica de fondo
    /// </summary>
    /// <param name="volumen"> Valor enviado por el slider musicSlider </param>
    public void SetMusicVolume(float volumen)
    {
        audioMixer.SetFloat("MusicVolume", volumen);
    }

    /// <summary>
    /// Método para ajustar el volumen de los efectos
    /// </summary>
    /// <param name="volumen"> Valor enviado por el slider sfxSlider </param>
    public void SetSFXVolume(float volumen)
    {
        audioMixer.SetFloat("SFXVolume", volumen);
    }

    /// <summary>
    /// Reproducir música de fondo
    /// </summary>
    /// <param name="index"> La posicion del clip del sonido que queremos reproducir </param>
    public void PlayMusic(int index)
    {
        if (index >= 0 && index < musicaClips.Length)
        {
            musica.clip = musicaClips[index];
            musica.Play();
        }
        else
        {
            print("Índice de música fuera de rango.");
        }
    }

    /// <summary>
    /// Reproducir efecto de sonido por indice
    /// </summary>
    /// <param name="index"> La posicion del clip del sonido que queremos reproducir</param>
    public void PlayEfectInt(int index)
    {
        if (index >= 0 && index < efectosClips.Length)
        {
            efectos.PlayOneShot(efectosClips[index]);
        }
        else
        {
            print("Índice de SFX fuera de rango.");
        }
    }

    /// <summary>
    /// Reproducir efecto de sonido por nombre
    /// </summary>
    /// <param name="index"> EL nombre del clip del sonido que queremos reproducir</param>
    public void PlayEfectString(string nombre)
    {
        if (efectosDict != null && efectosDict.TryGetValue(nombre, out AudioClip clip))
        {
            efectos.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("Efecto de sonido no encontrado: " + nombre);
        }
    }

    /// <summary>
    /// Detener la música actual
    /// </summary>
    public void StopMusic()
    {
        musica.Stop();
    }
}
