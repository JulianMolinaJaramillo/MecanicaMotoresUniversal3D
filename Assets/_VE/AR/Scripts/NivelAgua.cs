using System.Collections;
using UnityEngine;

public class NivelAgua : MonoBehaviour
{
    [Header("Referencia")]
    public Renderer objetoRenderer; // El objeto que usa el shader

    [Header("Configuración")]
    public float velocidad = 0.2f;  // Qué tan rápido sube
    public float tope = 0.5f;       // Valor máximo permitido
    public float topeGain = 0.5f;       // Valor máximo permitido
    public bool rioActivo;

    [Header("Valores de reinicio")]
    public float resetDisplace = 0f; // Valor objetivo al reiniciar _DisplaceAmp
    public float resetGain = 0f;     // Valor objetivo al reiniciar _NoiseGain

    private Material mat;

    void Start()
    {
        mat = objetoRenderer.material;
        mat.SetFloat("_DisplaceAmp", 0f);
        mat.SetFloat("_NoiseGain", resetGain);
    }

    // Método público para iniciar el aumento escalado
    [ContextMenu("subir")]
    public void SubirDisplace()
    {
        StopAllCoroutines(); // por si ya estaba corriendo
        StartCoroutine(AumentarDisplace());
    }

    private IEnumerator AumentarDisplace()
    {
        float valorActual = mat.GetFloat("_DisplaceAmp");

        while (valorActual < tope)
        {
            valorActual += Time.deltaTime * velocidad;
            mat.SetFloat("_DisplaceAmp", Mathf.Min(valorActual, tope));
            yield return null;
        }
    }

    // Método público para iniciar el aumento escalado
    [ContextMenu("subirGain")]
    public void SubirGain()
    {
        StopAllCoroutines(); // por si ya estaba corriendo
        StartCoroutine(AumentarGain());
    }

    private IEnumerator AumentarGain()
    {
        float valorActual = mat.GetFloat("_NoiseGain");

        while (valorActual < topeGain)
        {
            valorActual += Time.deltaTime * velocidad;
            mat.SetFloat("_NoiseGain", Mathf.Min(valorActual, topeGain));
            yield return null;
        }
    }

    // ===== RESTABLECER DISPLACE =====
    [ContextMenu("Reset Displace")]
    public void ResetDisplace()
    {
        StopAllCoroutines();
        StartCoroutine(RestaurarDisplace());
    }

    private IEnumerator RestaurarDisplace()
    {
        float valorActual = mat.GetFloat("_DisplaceAmp");

        while (valorActual > resetDisplace)
        {
            valorActual -= Time.deltaTime * velocidad;
            mat.SetFloat("_DisplaceAmp", Mathf.Max(valorActual, resetDisplace));
            yield return null;
        }
    }

    // ===== RESTABLECER GAIN =====
    [ContextMenu("Reset Gain")]
    public void ResetGain()
    {
        StopAllCoroutines();
        StartCoroutine(RestaurarGain());
    }

    private IEnumerator RestaurarGain()
    {
        float valorActual = mat.GetFloat("_NoiseGain");

        while (valorActual > resetGain)
        {
            valorActual -= Time.deltaTime * velocidad;
            mat.SetFloat("_NoiseGain", Mathf.Max(valorActual, resetGain));
            yield return null;
        }
    }

    private void OnEnable()
    {
        
        if (rioActivo)
        {
            SubirDisplace();       
        }
    }
}
