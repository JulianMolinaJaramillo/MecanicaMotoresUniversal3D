using UnityEngine;

public class Roca : MonoBehaviour
{
    [Header("Configuración del Recorrido")]
    public Transform[] puntos;      // Array de posiciones y rotaciones
    public float velocidad = 0.1f;    // Velocidad de movimiento
    public float rotVelocidad = 2f; // Velocidad de rotación
    public float distanciaMin = 0.01f; // Umbral para considerar que llegó al punto

    private int indiceActual = 0;   // Punto actual del recorrido
    private bool puedoIniciar;   // Punto actual del recorrido
    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;
    private void Start()
    {
        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;
    }
    void Update()
    {
        if (puntos.Length == 0) return;
        if (!puedoIniciar) return;

        Transform destino = puntos[indiceActual];

        // Movimiento hacia el punto
        transform.position = Vector3.MoveTowards(transform.position, destino.position, velocidad * Time.deltaTime);

        // Rotación suave hacia la rotación del punto
        transform.rotation = Quaternion.Lerp(transform.rotation, destino.rotation, rotVelocidad * Time.deltaTime);

        // Verificar si ya llegó al destino
        if (Vector3.Distance(transform.position, destino.position) <= distanciaMin)
        {
            indiceActual++;

            // Si llegó al final, desactivar este script
            if (indiceActual >= puntos.Length)
            {
                enabled = false;
            }
        }
    }

    // Método opcional para reiniciar el recorrido
    [ContextMenu("reiniciar")]
    public void ReiniciarRecorrido()
    {
        transform.position = posicionInicial;
        transform.rotation = rotacionInicial;
        indiceActual = 0;
        enabled = true;
    }

    [ContextMenu("iniciar")]
    public void IniciarRecorrido()
    {
        puedoIniciar = true;
    }
}
