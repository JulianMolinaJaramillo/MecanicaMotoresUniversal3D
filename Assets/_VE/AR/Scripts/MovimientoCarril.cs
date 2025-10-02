using UnityEngine;

public class MovimientoCarril : MonoBehaviour
{
    [Header("Configuración del carril")]
    public Transform[] destinos;   // Lista de destinos (en orden)
    public float velocidad = 0.2f; // Velocidad de movimiento
    public float rotacionVel = 1f; // Velocidad de rotación
    public float distanciaMinima = 0.08f; // Distancia para considerar que llegó
    public bool soyTaxi;
    public bool soyCarro;
    public bool soyBus;

    private int indiceActual = 0;

    void Update()
    {
        if (destinos.Length == 0) return;

        Transform destinoActual = destinos[indiceActual];

        // Interpolación de posición local
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, destinoActual.localPosition, Time.deltaTime * velocidad);

        // Interpolación de rotación local
        transform.localRotation = Quaternion.Lerp(transform.localRotation, destinoActual.localRotation, Time.deltaTime * rotacionVel);

        // Verificar si ya llegó al destino
        float distancia = Vector3.Distance(transform.localPosition, destinoActual.localPosition);
        if (distancia < distanciaMinima)
        {
            // Pasar al siguiente destino
            indiceActual++;

            // Reiniciar ciclo si terminó la lista
            if (indiceActual >= destinos.Length)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void AsignarDestinos(Transform[] destinosEnviados)
    {
        destinos = new Transform[destinosEnviados.Length];

        for (int i = 0; i < destinosEnviados.Length; i++)
        {
            destinos[i] = destinosEnviados[i];
        }
    }
}
