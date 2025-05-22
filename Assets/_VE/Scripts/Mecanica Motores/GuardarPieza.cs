
using UnityEngine;

public class GuardarPieza : MonoBehaviour
{
    public string nombrePiezaBoton; // Nombre para asignarle al boton de la piza
    public string nombrePieza; // Nombre completo de la pieza para el titulo
    [TextArea(3, 10)]
    public string descripcionPieza; // Descripcion de para que sirve esa pieza

    public Material materialSeleccion; // El material que deseamos al momento de pararnos sobre la pieza
    public GameObject prefabInstancia; // El prefab que posteriormente instanciará esa pieza
    public Sprite icono; // Imagen para mostrar en el botón del inventario

    private MeshRenderer meshRenderer; // Referencia a nuestro mesh
    private Material[] materialesOriginales; // Para almacenar nuestros materiales

    private void Awake()
    {
        // Obtenemos los componentes
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        // Guardamos el material original
        materialesOriginales = meshRenderer.materials;
    }

    /// <summary>
    /// Metodo incovado al momento de posar el cursor sobre un objeto con collider
    /// </summary>
    void OnMouseEnter()
    {
        AgregarMaterial(); // Asignamos el material secundario
        InformacionUI.singleton.ActualizarInformacionPieza(nombrePieza, descripcionPieza); // Actualizamos la informacion de la pieza en el canvas
    }

    /// <summary>
    /// Metodo incovado al momento de sacar el cursor de un objeto con collider
    /// </summary>
    void OnMouseExit()
    {
        QuitarMaterial(); // Quitamos el material secundario
        InformacionUI.singleton.BorrarInformacionPieza(); // Retiramos la informacion de la pieza del canvas
    }

    /// <summary>
    /// Metodo incovado al momento de darle click sobre un objeto con collider
    /// </summary>
    void OnMouseDown()
    {
        InventarioUI inventario = FindObjectOfType<InventarioUI>();  // Encontramos y referenciamos nuestro inventario
        if (inventario != null)
        {
            inventario.AgregarAlInventario(icono, prefabInstancia, nombrePiezaBoton, descripcionPieza); // Agregamos el objeto a nuestro inventario
        }

        InformacionUI.singleton.BorrarInformacionPieza(); // Retiramos la informacion de la pieza del canvas
        Destroy(this.gameObject); // Destruimos el objeto
    }

    /// <summary>
    /// Metodo utilizado para asignarle el material de seleccion al momento de mover las piezas del motor
    /// </summary>
    /// <param name="id"> Para identificar si el material debe ser el verde o rojo </param>
    public void AgregarMaterial()
    {
        Material[] nuevosMateriales = new Material[2]; // Obtener material
        nuevosMateriales[0] = materialesOriginales[0]; // mantener el original
        nuevosMateriales[1] = materialSeleccion; // añadir el segundo
        meshRenderer.materials = nuevosMateriales; // Asignar material
    }

    /// <summary>
    /// Para quitar el materiale de seleccion y solo dejar el material por defecto
    /// </summary>
    public void QuitarMaterial()
    {
        meshRenderer.materials = new Material[] { materialesOriginales[0] }; // Reestablecemos el material original
    }
}
