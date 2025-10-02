using System.Collections;
using TMPro;
using UnityEngine;

public class TextoFadeIn : MonoBehaviour
{
    [Header("Configuración")]
    public float duracion = 1.5f; // Tiempo que dura el fade
    public float delay = 0f;      // Retraso antes de empezar el fade

    private TextMeshProUGUI textoTMP;

    void Awake()
    {
        textoTMP = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        // Asegurar que empiece invisible
        Color c = textoTMP.color;
        c.a = 0f;
        textoTMP.color = c;

        // Iniciar corrutina
        StartCoroutine(FadeInTexto());
    }

    private IEnumerator FadeInTexto()
    {
        yield return new WaitForSeconds(delay);

        float tiempo = 0f;
        Color c = textoTMP.color;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float alpha = Mathf.Clamp01(tiempo / duracion);
            c.a = alpha;
            textoTMP.color = c;
            yield return null;
        }
    }
}
