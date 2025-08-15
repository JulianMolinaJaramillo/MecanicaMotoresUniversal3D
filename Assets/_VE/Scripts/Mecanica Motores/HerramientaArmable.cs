using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Tipos posibles de herramientas.
/// Se puede ampliar según las necesidades.
/// </summary>
public enum ToolType
{
    Rachet,     // Base principal
    Head,       // Cabeza del rachet
    Socket,     // Copa
    Wrench,     // Llave fija
    Screwdriver // Destornillador
}

/// <summary>
/// Representa una herramienta que puede ser tomada o acoplada a otra.
/// Define su tipo, con qué es compatible y dónde se pueden acoplar otras piezas.
/// </summary>
public class HerramientaArmable : MonoBehaviour
{
    [Header("Tipo de esta herramienta")]
    public ToolType tipoHerramienta; // Tipo de herramienta (Rachet, Socket, etc.)

    [Header("Tipo que esta herramienta puede recibir")]
    public ToolType compatibleCon; // Lista de tipos que esta herramienta acepta como complemento

    [Header("Punto de unión para otra pieza")]
    public Transform puntoAcople; // Lugares físicos donde se pueden acoplar piezas adicionales

    [Header("Componente actualmente acoplado")]
    public HerramientaArmable componenteAcoplados; // Lista de herramientas ya acopladas

    public MessageOnly mensaje1 = new MessageOnly("Si lo que deseas es modificarle el material a los hijos", MessageTypeCustom.Info);
    public MeshRenderer[] meshRendererHijos; // Para los mesh de los hijos de este objeto

    public Material materialSeleccion; // El material que deseamos al momento de pararnos sobre la pieza  
    public string nombreHerramienta; // Nombre completo de la pieza para el titulo
    [TextArea(3, 10)]
    public string descripcionPieza; // Descripcion de para que sirve esta herramienta
    public bool piezaInicial; // Para indicar si es la pieza inicial
    public bool piezaFinal; // Para indicar si es la pieza final

    [Header("Solo para piezas finales")]
    public string nombreHerramientaImagen; // Nombre para asignarle a la imagen
    public Sprite icono; // Imagen para mostrar en el botón del inventario
    public int sizeHerramienta; // Para indicar el tamaño en milimetros

    // posición, rotación y parent originales
    private Vector3 actualPosition;
    private Quaternion actualRotation;
    private Transform originalParent;
    private MeshRenderer meshRenderer; // Referencia a nuestro mesh
    private Material[] materialesOriginales; // Para almacenar nuestros materiales
    [HideInInspector]
    public Collider collider;

    private void Awake()
    {
        // Obtenemos los componentes
        meshRenderer = GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();
    }

    private void Start()
    {
        if (meshRendererHijos.Length > 0)
        {
            // Guardamos el material original de los hijos
            materialesOriginales = meshRendererHijos[0].materials;
        }
        else
        {
            // Guardamos el material original
            materialesOriginales = meshRenderer.materials;
        }
    }

    /// <summary>
    /// Evento llamado automáticamente por Unity cuando se hace clic con el mouse sobre este objeto.
    /// Necesita un Collider en el objeto para funcionar.
    /// </summary>
    private void OnMouseDown()
    {
        // Si existe, le pasamos esta herramienta para que gestione la acción
        if (InventarioHerramientas.singleton != null && !ManagerCanvas.singleton.mensajeAlertaActivo)
        {
            // Guardar posición, rotación y padre originales
            actualPosition = transform.position;
            actualRotation = transform.rotation;
            originalParent = transform.parent;
            InventarioHerramientas.singleton.ClickHerramienta(this,collider);
        }
    }

    /// <summary>
    /// Metodo incovado al momento de posar el cursor sobre un objeto con collider
    /// </summary>
    void OnMouseEnter()
    {
        if (!ManagerCanvas.singleton.mensajeAlertaActivo)
        {
            AgregarMaterial(); // Asignamos el material secundario
            ManagerCanvas.singleton.ActualizarInformacionPieza(nombreHerramienta, descripcionPieza); // Actualizamos la informacion de la pieza en el canvas   
        }
        
    }

    /// <summary>
    /// Metodo incovado al momento de sacar el cursor de un objeto con collider
    /// </summary>
    void OnMouseExit()
    {
        QuitarMaterial(); // Quitamos el material secundario
        ManagerCanvas.singleton.BorrarInformacionPieza(); // Retiramos la informacion de la pieza del canvas    
    }

    /// <summary>
    /// Comprueba si esta herramienta puede recibir otra herramienta específica.
    /// </summary>
    /// <param name="other">Herramienta que se quiere acoplar</param>
    /// <returns>True si es compatible, False si no</returns>
    public bool puedoUnir(HerramientaArmable other)
    {
        if (puntoAcople != null)
        {
            return compatibleCon == other.tipoHerramienta;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Intenta acoplar otra herramienta a esta.
    /// </summary>
    /// <param name="other">Herramienta que se quiere acoplar</param>
    /// <returns>True si se acopló correctamente, False si no</returns>
    public bool Unir(HerramientaArmable other)
    {
        // Verifica compatibilidad y que haya espacio en los puntos de unión
        if (puedoUnir(other) && puntoAcople != null)
        {
            // Añadimos la herramienta a la lista de acoplados
            componenteAcoplados = other;

            // Establecemos la relación de jerarquía en el punto de unión
            other.transform.SetParent(puntoAcople);

            // Llamamos a la corrutina para moverlo suavemente al punto
            StartCoroutine(UnirSuavemente(other, puntoAcople));
            return true;
        }
        return false;
    }

    /// <summary>
    /// Mueve la herramienta a su punto de acople con una animación suave.
    /// </summary>
    private IEnumerator UnirSuavemente(HerramientaArmable other, Transform targetPoint)
    {
        // Guardamos posición y rotación inicial
        Vector3 startPos = other.transform.position;
        Quaternion startRot = other.transform.rotation;

        // Posición y rotación final (punto de acople)
        Vector3 targetPos = targetPoint.position;
        Quaternion targetRot = targetPoint.rotation;

        float duration = 0.5f; // Tiempo de animación
        float t = 0;

        // Interpolamos durante el tiempo indicado
        while (t < duration)
        {
            t += Time.deltaTime;
            float factor = t / duration;

            other.transform.position = Vector3.Lerp(startPos, targetPos, factor);
            other.transform.rotation = Quaternion.Lerp(startRot, targetRot, factor);

            yield return null;
        }

        // Aseguramos que termine exactamente en el punto objetivo
        other.transform.position = targetPos;
        other.transform.rotation = targetRot;

        if (other.piezaFinal)
        {
            if (AudioManager.singleton != null) AudioManager.singleton.PlayEfectString("Pop"); // Ejecutamos el efecto nombrado  
        }
    }

    public void RestaurarPosicionActual()
    {
        StartCoroutine(RestaurarSuamenteActual());
    }

    private IEnumerator RestaurarSuamenteActual()
    {
        transform.SetParent(originalParent);

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.position = Vector3.Lerp(startPos, actualPosition, t);
            transform.rotation = Quaternion.Lerp(startRot, actualRotation, t);

            yield return null;
        }

        // Asegurar que termine en la posición y rotación exacta
        transform.position = actualPosition;
        transform.rotation = actualRotation;

        componenteAcoplados = null;
    }

    /// <summary>
    /// Metodo utilizado para asignarle el material de seleccion al momento de mover las piezas del motor
    /// </summary>
    public void AgregarMaterial()
    {
        if (materialSeleccion != null)
        {
            Material[] nuevosMateriales = new Material[2]; // Creamos los nuevos materiales
            nuevosMateriales[0] = materialesOriginales[0]; // mantener el original
            nuevosMateriales[1] = materialSeleccion; // añadir el segundo

            if (meshRendererHijos.Length > 0) // Validamos si tengo renders hijos o no
            {
                for (int i = 0; i < meshRendererHijos.Length; i++)
                {
                    meshRendererHijos[i].materials = nuevosMateriales; // Agrego los materiales a los hijos
                }
            }
            else
            {
                meshRenderer.materials = nuevosMateriales; // Agrego el material a nuestro objeto padre
            }
        }
    }

    /// <summary>
    /// Para quitar el materiale de seleccion y solo dejar el material por defecto
    /// </summary>
    public void QuitarMaterial()
    {
        if (meshRendererHijos.Length > 0)
        {
            for (int i = 0; i < meshRendererHijos.Length; i++)
            {
                meshRendererHijos[i].materials = new Material[] { materialesOriginales[0] };
            }
        }
        else
        {
            meshRenderer.materials = new Material[] { materialesOriginales[0] };
        }
    }

}

