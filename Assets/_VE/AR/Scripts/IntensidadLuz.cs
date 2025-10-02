using System.Collections;
using UnityEngine;

public class IntensidadLuz : MonoBehaviour
{
    private Light luz;
    public float duracion;
    public float nuevoValor;
    private float intensidadOriginal;

    public static IntensidadLuz singleton;
    private void Awake()
    {
        // Implementación Singleton
        if (singleton != null && singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        singleton = this;

        // Si quieres que persista entre escenas:
        // DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        luz = GetComponent<Light>();
        intensidadOriginal = luz.intensity; // Guardamos el valor inicial
    }

    /// <summary>
    /// Aumenta la intensidad de la luz hasta un valor objetivo de forma suavizada.
    /// </summary>
    /// <param name="nuevaIntensidad">Valor objetivo de intensidad</param>
    /// <param name="duracion">Tiempo que tarda en llegar</param>
    [ContextMenu("aument")]
    public void AumentarIntensidad()
    {
        StopAllCoroutines();
        StartCoroutine(InterpolarIntensidad(nuevoValor, duracion));
    }

    /// <summary>
    /// Devuelve la intensidad de la luz a su valor original de forma suavizada.
    /// </summary>
    /// <param name="duracion">Tiempo que tarda en volver</param>
    [ContextMenu("restaurar")]
    public void RestaurarIntensidad()
    {
        StopAllCoroutines();
        StartCoroutine(InterpolarIntensidad(intensidadOriginal, duracion));
    }

    private IEnumerator InterpolarIntensidad(float objetivo, float duracion)
    {
        float tiempo = 0f;
        float inicio = luz.intensity;

        while (tiempo < duracion)
        {
            luz.intensity = Mathf.Lerp(inicio, objetivo, tiempo / duracion);
            tiempo += Time.deltaTime;
            yield return null;
        }

        luz.intensity = objetivo; // Aseguramos el valor final exacto
    }
}
