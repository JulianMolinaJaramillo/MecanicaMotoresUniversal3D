using System.Collections;
using TMPro;
using UnityEngine;

public class TextoEscalonado : MonoBehaviour
{
    [Header("Referencia del Texto")]
    public TextMeshProUGUI textoTMP;
    public string textoAlmacenado;
    public bool mostrarInicio;
    [Header("Configuración del texto")]
    public float velocidad = 0.05f; // Tiempo entre cada letra

    private Coroutine corrutinaEscribir;

    [Header("Configuración del pulso")]
    public Vector3 escalaInicial = Vector3.one; // Escala base
    public Vector3 escalaMaxima = new Vector3(1.2f, 1.2f, 1.2f); // Escala máxima
    public float duracionPulso = 0.3f; // Tiempo en segundos que dura el pulso

    private void Awake()
    {
        if (mostrarInicio)
        {
            textoAlmacenado = textoTMP.text;
        }
    }

    /// <summary>
    /// Llama este método para iniciar el efecto escribiendo un texto.
    /// </summary>
    public void MostrarTexto(string nuevoTexto)
    {
        if (gameObject.activeInHierarchy)
        {
            if (corrutinaEscribir != null)
            {
                StopCoroutine(corrutinaEscribir);
            }

            corrutinaEscribir = StartCoroutine(EscribirTexto(nuevoTexto));
        }     
    }

    private IEnumerator EscribirTexto(string texto)
    {
        textoTMP.text = "";
        foreach (char letra in texto)
        {
            textoTMP.text += letra;
            yield return new WaitForSeconds(velocidad);
        }
        corrutinaEscribir = null;
    }

    private IEnumerator Pulso()
    {
        float t = 0f;

        // Escalar hacia arriba
        while (t < 1f)
        {
            t += Time.deltaTime / (duracionPulso / 2f);
            transform.localScale = Vector3.Lerp(escalaInicial, escalaMaxima, t);
            yield return null;
        }

        t = 0f;

        // Volver a escala inicial
        while (t < 1f)
        {
            t += Time.deltaTime / (duracionPulso / 2f);
            transform.localScale = Vector3.Lerp(escalaMaxima, escalaInicial, t);
            yield return null;
        }
    }

    [ContextMenu("es")]
    public void Escribir()
    {
        MostrarTexto("Aqui podemos apreciar el estado normal de la cuenca y las personas en su día a día");
    }

    private void OnEnable()
    {
        if (!mostrarInicio)
        {
            // Reiniciar escala al activarse
            transform.localScale = escalaInicial;
            StartCoroutine(Pulso());

            if (corrutinaEscribir != null)
            {
                StopCoroutine(corrutinaEscribir);
            }
            corrutinaEscribir = StartCoroutine(EscribirTexto(textoAlmacenado));       
        }
        mostrarInicio = false;
    }
}
