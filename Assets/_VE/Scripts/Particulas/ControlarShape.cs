using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ControlarShape : MonoBehaviour
{
    [Header("Escala en eje X (Box)")]
    public float escalaXObjetivo = 5f;

    [Header("Velocidad de transición")]
    public float velocidadLerp = 0.5f;

    private ParticleSystem sistema;
    private ParticleSystem.ShapeModule shape;
    private Vector3 escalaInicial;
    private Vector3 escalaActual;
    private float objetivoX;
    private bool enTransicion = false;

    private const float umbral = 0.01f; // Para detener cuando se alcanza el valor objetivo

    void Start()
    {
        sistema = GetComponent<ParticleSystem>();
        shape = sistema.shape;

        escalaInicial = shape.scale;         // Guardar la escala original
        escalaActual = escalaInicial;
        objetivoX = escalaInicial.x;
        shape.scale = escalaActual;
    }

    void Update()
    {
        if (!enTransicion) return;
        // Interpolación suavizada
        float nuevaX = Mathf.Lerp(escalaActual.x, objetivoX, Time.deltaTime * velocidadLerp);
        escalaActual = new Vector3(nuevaX, escalaActual.y, escalaActual.z);
        shape.scale = escalaActual;

        // Verificar si ya llegamos (umbral)
        if (Mathf.Abs(escalaActual.x - objetivoX) < umbral)
        {
            escalaActual = new Vector3(objetivoX, escalaActual.y, escalaActual.z);
            shape.scale = escalaActual;
            enTransicion = false;
        }
    }

    // 🔼 Aumentar hacia la escala objetivo
    public void AumentarEscala()
    {
        objetivoX = escalaXObjetivo;
        enTransicion = true;
    }

    // 🔽 Restaurar a la escala inicial
    public void RestaurarEscala()
    {
        objetivoX = escalaInicial.x;
        enTransicion = true;
    }
}
