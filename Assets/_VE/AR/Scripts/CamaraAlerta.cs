using UnityEngine;

public class CamaraAlerta : MonoBehaviour
{
    [Header("Configuración del Pulso")]
    public float tiempoPulso = 1f; // Tiempo en segundos para completar ida y vuelta

    [Header("Configuración de Color")]
    public Color colorInicialVerdad = Color.yellow;      // Color de inicio
    public Color colorInicial = Color.yellow;      // Color de inicio
    public Color colorObjetivo = Color.red;  // Color objetivo

    private float tiempo;
    private bool haciaArriba = true;

    public Material material; // Referencia al material
    private bool escalar;
    void Start()
    {
        material.color = colorInicialVerdad;
        escalar = true;
    }

    void Update()
    {
        if (escalar) return;

        // Calcular fracción de tiempo
        tiempo += (haciaArriba ? 1 : -1) * Time.deltaTime / (tiempoPulso / 2f);

        // Interpolación entre colores (Albedo)
        material.color = Color.Lerp(colorInicial, colorObjetivo, tiempo);

        // Invertir cuando llega a los límites
        if (tiempo >= 1f)
        {
            tiempo = 1f;
            haciaArriba = false;
        }
        else if (tiempo <= 0f)
        {
            tiempo = 0f;
            haciaArriba = true;
        }
    }

    [ContextMenu("restablecer")]
    public void RestablecerEscalaColor()
    {
        escalar = true;

        // Interpolación entre colores (Albedo)
        material.color = colorInicialVerdad;
    }

    [ContextMenu("iniciar")]
    public void IniciarAlerta()
    {
        escalar = false;
    }

}
