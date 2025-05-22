using System.Collections;
using UnityEngine;

public class IluminarMaterial : MonoBehaviour
{
    public Material material; // Material que queremos alterar en intensidad
    public float maxIntensidad = 10f;  // Máximo de intensidad permitido
    public float velocidadTransicion = 2f; // Velocidad de la iluminacion

    private Color emisionOriginal;
    private float intensidadOriginal;
    private readonly Color colorFijo = new Color(0.749f, 0.749f, 0.749f); // 191/255

    void Start()
    {
        // Iniciamos la emision del material inicialmente en 191/191/191
        material.SetColor("_EmissionColor", colorFijo);

        // Guardamos la emision e intensidad original del material en cuestion
        emisionOriginal = material.GetColor("_EmissionColor");
        intensidadOriginal = emisionOriginal.maxColorComponent;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopCoroutine(BajarEmission()); // Detenemos currutina actual
            StartCoroutine(SubirEmission()); // Iniciamos nueva currutina
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopCoroutine(SubirEmission()); // Detenemos currutina actual
            StartCoroutine(BajarEmission()); // Iniciamos nueva currutina
        }
    }

    IEnumerator SubirEmission()
    {
        float tiempoTranscurrido = 0f;

        // Aumentar la intensidad hasta el máximo permitido
        while (tiempoTranscurrido < 1f)
        {
            tiempoTranscurrido += Time.deltaTime * velocidadTransicion;
            float intensidad = Mathf.Lerp(intensidadOriginal, maxIntensidad, tiempoTranscurrido);
            material.SetColor("_EmissionColor", emisionOriginal * Mathf.Min(intensidad, maxIntensidad));
            yield return null;
        }
    }

    IEnumerator BajarEmission()
    {
        float tiempoTranscurrido = 0f;
        // Disminuir la intensidad de vuelta al original
        tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < 1f)
        {
            tiempoTranscurrido += Time.deltaTime * velocidadTransicion;
            float intensidad = Mathf.Lerp(maxIntensidad, intensidadOriginal, tiempoTranscurrido);
            material.SetColor("_EmissionColor", emisionOriginal * intensidad);
            yield return null;
        }

        // Forzar el color final al fijo (191,191,191)
        material.SetColor("_EmissionColor", colorFijo);
    }
}
