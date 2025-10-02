using UnityEngine;

public class Temblor : MonoBehaviour
{
    [Header("Configuración de la vibración")]
    public float intensidad = 0.001f;   // Qué tanto se mueve (magnitud)
    public float velocidad = 20f;       // Qué tan rápido vibra

    private Vector3 posicionInicial;
    private bool vibrando = false;

    void Start()
    {
        posicionInicial = transform.localPosition;
    }

    void Update()
    {
        if (vibrando)
        {
            // Movimiento aleatorio en X y Y (puedes agregar Z si quieres)
            float offsetX = Mathf.Sin(Time.time * velocidad) * intensidad;
            float offsetY = Mathf.Cos(Time.time * velocidad) * intensidad;

            transform.localPosition = posicionInicial + new Vector3(offsetX, offsetY, 0);
        }
    }

    /// <summary>
    /// Activa la vibración indefinida
    /// </summary>
    public void IniciarVibracion(float _intensidad)
    {
        intensidad = _intensidad;
        posicionInicial = transform.localPosition;
        vibrando = true;
    }

    [ContextMenu("vibrar")]
    public void Vibrar()
    {
        IniciarVibracion(intensidad);
    }

    [ContextMenu("detener Vibrar")]
    public void DetenerVibracion()
    {
        vibrando = false;
        transform.localPosition = posicionInicial; // vuelve a su posición original
    }
}
