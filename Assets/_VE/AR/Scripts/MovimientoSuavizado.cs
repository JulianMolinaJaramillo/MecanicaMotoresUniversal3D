using UnityEngine;

public class MovimientoSuavizado : MonoBehaviour
{
    [Header("Objetivo hacia donde moverse")]
    public Transform objetivo;
    public Transform objetivo2;

    [Header("Configuración")]
    public float velocidad = 0.2f;       // Velocidad del movimiento

    private Vector3 posicionInicial;
    private bool iniciar;

    void Start()
    {
        // Guardamos la posición inicial al comenzar
        posicionInicial = transform.localPosition;
    }

    void Update()
    {
        if (objetivo == null) return;

        if (!iniciar) return;

        // Movimiento suavizado con Lerp
        transform.localPosition = Vector3.Lerp(transform.localPosition, objetivo.localPosition, Time.deltaTime * velocidad);
    }

    /// <summary>
    /// Reinicia la posición al punto inicial
    /// </summary>
    [ContextMenu("reiniciar")]
    public void ReiniciarPosicion()
    {
        iniciar = false;
        transform.localPosition = posicionInicial;
    }

    [ContextMenu("iniciar")]
    public void IniciarDesplazamiento()
    {
        iniciar = true;
    }

    [ContextMenu("cambiarObjetivo")]
    public void CambiarObjetivoSecundario()
    {
        objetivo = objetivo2;
    }
}
