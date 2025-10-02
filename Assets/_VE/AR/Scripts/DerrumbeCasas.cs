using UnityEngine;

public class DerrumbeCasas : MonoBehaviour
{
    [Header("Configuración de Waypoints")]
    public Temblor vibracion;
    public Transform[] puntos;        // Lista de puntos a recorrer
    public float velocidad = 0.01f;      // Velocidad de movimiento
    public float distanciaMin = 0.1f; // Umbral para considerar que llegó al punto
    public float velocidadRotacion = 0.03f; // Qué tan rápido gira
    public bool activarDerrumbe;
    private int indiceActual = 0;

    void Update()
    {
        if (puntos.Length == 0) return;
        if (!activarDerrumbe) return;

        // Posición actual y destino
        Transform destino = puntos[indiceActual];
        Vector3 direccion = destino.position - transform.position;

        // Si aún no está en el destino, avanza
        if (direccion.magnitude > distanciaMin)
        {
            // Movimiento interpolado (suave)
            transform.position = Vector3.MoveTowards(
                transform.position,
                destino.position,
                velocidad * Time.deltaTime
            );

            // 🔹 Rotación solo en el eje Y
            if (direccion != Vector3.zero)
            {
                // Tomamos solo la componente X y Z (plano horizontal)
                Vector3 direccionPlano = new Vector3(direccion.x, 0, direccion.z);

                if (direccionPlano.sqrMagnitude > 0.001f)
                {
                    Quaternion rotacion = Quaternion.LookRotation(direccionPlano);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotacion, Time.deltaTime * velocidadRotacion);
                }
            }
        }
        else
        {
            // Cuando llega al punto, pasa al siguiente
            indiceActual++;
            if (indiceActual >= puntos.Length)
            {
                // Opciones: detenerse o reiniciar recorrido
                // Detenerse:
                if (vibracion != null)
                {
                    vibracion.Vibrar();
                }
                
                enabled = false;

                // O reiniciar ciclo:
                // indiceActual = 0;
            }
        }
    }

    [ContextMenu("iniciar")]
    public void IniciarDerrumbe()
    {
        activarDerrumbe = true;
    }

    [ContextMenu("reiniciar")]
    public void ReiniciarDerrumbe()
    {
        indiceActual = 0;
        enabled = true;
    }
}
