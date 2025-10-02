using UnityEngine;

public class CrecimientoNubes : MonoBehaviour
{
    [Header("Configuración de Escala")]
    public float escalaObjetivoX = 2f;   // Escala deseada en X
    public float velocidad = 2f;         // Velocidad de cambio de escala

    private bool puedoEscalar;
    private Vector3 escalaInicial;

    void Start()
    {
        // Guardamos la escala inicial
        escalaInicial = transform.localScale;
    }

    void Update()
    {
        if (!puedoEscalar) return;

        // Escala actual
        Vector3 escalaActual = transform.localScale;

        // Nueva escala interpolada solo en X
        float nuevaX = Mathf.Lerp(escalaActual.x, escalaObjetivoX, Time.deltaTime * velocidad);

        // Aplicamos la nueva escala manteniendo Y y Z igual
        transform.localScale = new Vector3(nuevaX, escalaActual.y, escalaActual.z);
    }

    public void ActivarCrecimiento()
    {
        puedoEscalar = true;
    }

    public void RestablecerCrecimiento()
    {
        escalaObjetivoX = escalaInicial.x;
    }
}
