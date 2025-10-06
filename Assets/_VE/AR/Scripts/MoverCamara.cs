using UnityEngine;

public class MoverCamara : MonoBehaviour
{
    [Header("Configuración de movimiento")]
    public float velocidadMovimiento = 5f;   // Qué tan rápido se mueve la cámara
    public float velocidadRotacion = 5f;     // Qué tan rápido rota hacia el objetivo

    private Transform objetivoActual = null;
    public static MoverCamara singleton;
    private void Awake()
    {
        // Implementación Singleton
        if (singleton != null && singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        singleton = this;

        // Si quieres que persista entre escenas:
        // DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (objetivoActual != null)
        {
            // Movimiento suave con Lerp
            transform.position = Vector3.Lerp(transform.position, objetivoActual.position, Time.deltaTime * velocidadMovimiento);

            // Rotación suave con Slerp
            transform.rotation = Quaternion.Slerp(transform.rotation, objetivoActual.rotation, Time.deltaTime * velocidadRotacion);
        }
    }

    /// <summary>
    /// Llama a este método para mover la cámara hacia un transform.
    /// </summary>
    /// <param name="nuevoObjetivo">El transform al cual la cámara debe moverse</param>
    public void MoverHacia(Transform nuevoObjetivo)
    {
        objetivoActual = nuevoObjetivo;
    }

    /// <summary>
    /// Llama a este método si quieres que la cámara deje de seguir.
    /// </summary>
    public void DetenerMovimiento()
    {
        objetivoActual = null;
    }
}
