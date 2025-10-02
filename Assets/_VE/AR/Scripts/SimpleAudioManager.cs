using UnityEngine;

public class SimpleAudioManager : MonoBehaviour
{
    public static SimpleAudioManager singleton;

    [Header("Clips de audio")]
    public AudioClip[] clips; // arrastras tus audios aquí

    public AudioSource audioSource;
    public AudioSource audioSourceSecundario;
    public AudioSource audioSourceFondo;
    public AudioSource audioSourceGritos;
    public AudioSource audioSourceAlarma;
    public AudioSource audioSourcePersonas;

    private void Awake()
    {
        // Singleton simple
        if (singleton != null && singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        singleton = this;
    }

    /// <summary>
    /// Reproduce un clip por índice (0,1,2,3)
    /// </summary>
    public void PlaySound(int index)
    {
        if (clips == null || clips.Length == 0 || index < 0 || index >= clips.Length)
            return;

        audioSource.clip = clips[index];
        audioSource.Play();
    }

    /// <summary>
    /// Reproduce un clip por índice (0,1,2,3)
    /// </summary>
    public void PlaySound2(int index)
    {
        if (clips == null || clips.Length == 0 || index < 0 || index >= clips.Length)
            return;

        audioSourceSecundario.clip = clips[index];
        audioSourceSecundario.Play();
    }

    /// <summary>
    /// Detiene el audio actual
    /// </summary>
    public void StopSound()
    {
        audioSource.Stop();
    }

    public void DetenerAudioFondo()
    {
        audioSourceFondo.Stop();
        audioSourceGritos.volume = 0f;
        audioSourceAlarma.volume = 0f;
    }

    public void RestaurarAudioFondo()
    {
        audioSourceFondo.Play();
        audioSourceGritos.volume = 0.5f;
        audioSourceAlarma.volume = 0.4f;
        audioSourcePersonas.volume = 0.5f;
    }

    public void Alarma()
    {
        audioSourceAlarma.Play();
    }

    public void DesactivarAlarma()
    {
        audioSourceAlarma.Stop();
    }
    [ContextMenu("gritar")]
    public void Gritos()
    {
        audioSourceGritos.Play();
    }
    [ContextMenu("callar")]
    public void DesactivarGritos()
    {
        audioSourceGritos.Stop();
    }

    [ContextMenu("gritar")]
    public void Hablando()
    {
        audioSourcePersonas.Play();
    }
    [ContextMenu("callar")]
    public void DesactivarHablando()
    {
        audioSourcePersonas.Stop();
    }
}
