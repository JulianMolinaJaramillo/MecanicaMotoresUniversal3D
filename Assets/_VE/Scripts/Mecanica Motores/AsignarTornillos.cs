using UnityEngine;

/// <summary>
/// Clase utilizada para recolectar los tornillos de la pieza que tenga este script
/// </summary>
public class AsignarTornillos : MonoBehaviour
{
    public Material[] materialesSeleccion; // Para los materiales de seleccion verde y rojo
    public ApretarTornillos[] apretarTornillos;
    public MeshRenderer[] meshRendererHijos; // Para los mesh de los hijos de este objeto

    private Material[] materialesOriginales;

    // Start is called before the first frame update
    void Start()
    {
        if (meshRendererHijos.Length > 0)
        {
            // Guardamos el material original de los hijos
            materialesOriginales = meshRendererHijos[0].materials;
        }
    }

    /// <summary>
    /// Metodo utilizado para asignarle el material de seleccion al momento de mover las piezas del motor
    /// </summary>
    /// <param name="id"> Para identificar si el material debe ser el verde o rojo </param>
    [ContextMenu("activar")]
    public void InicializarTornillosMinijuego()
    {
        if (ManagerMinijuego.singleton != null)
        {
            for (int i = 0; i < apretarTornillos.Length; i++)
            {
                ManagerMinijuego.singleton.tornillosParaApretar.Add(apretarTornillos[i]);
            }
        }

        if (materialesSeleccion.Length > 0)
        {
            Material[] nuevosMateriales = new Material[2]; // Creamos los nuevos materiales
            nuevosMateriales[0] = materialesOriginales[0]; // mantener el original
            nuevosMateriales[1] = materialesSeleccion[0]; // añadir el segundo

            if (meshRendererHijos.Length > 0) // Validamos si tengo renders hijos o no
            {
                for (int i = 0; i < meshRendererHijos.Length; i++)
                {
                    meshRendererHijos[i].materials = nuevosMateriales; // Agrego los materiales a los hijos
                }
            }
        }
    }
}
