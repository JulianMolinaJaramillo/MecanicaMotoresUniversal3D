using UnityEngine;

public class PulsoEscala : MonoBehaviour
{
    [Header("Configuración del Pulso")]
    public Vector3 escalaInicial = Vector3.one;
    public Vector3 escalaObjetivo = new Vector3(1200f, 1200f, 1200f);
    public float tiempoPulso = 1f;

    [Header("Configuración de Color")]
    public Color colorInicial = Color.yellow;
    public Color colorObjetivo = Color.red;

    [Header("Configuración Emission")]
    private float intensidadEmission = 5f; // Intensidad fija de la emission

    private float tiempo;
    private bool haciaArriba = true;

    public Material material;
    private bool escalar;
    public bool escalarAlIniciar;

    void Start()
    {
        transform.localScale = escalaInicial;
        material.color = colorInicial;

        // 🔹 Aseguramos que la emisión esté activa
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", colorInicial * intensidadEmission);

        escalar = true;
    }

    void Update()
    {
        if (escalar) return;

        // Calcular fracción de tiempo
        tiempo += (haciaArriba ? 1 : -1) * Time.deltaTime / (tiempoPulso / 2f);

        // Interpolación entre escalas
        transform.localScale = Vector3.Lerp(escalaInicial, escalaObjetivo, tiempo);

        // Interpolación entre colores (Albedo)
        Color nuevoColor = Color.Lerp(colorInicial, colorObjetivo, tiempo);
        material.color = nuevoColor;

        // Interpolación entre colores (Emission) con intensidad fija
        material.SetColor("_EmissionColor", nuevoColor * intensidadEmission);

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
        transform.localScale = escalaInicial;

        material.color = colorInicial;
        material.SetColor("_EmissionColor", colorInicial * intensidadEmission);
    }

    [ContextMenu("iniciar")]
    public void IniciarAlerta()
    {
        escalar = false;
    }

    private void OnEnable()
    {
        if (escalarAlIniciar) escalar = false;
    }
}
