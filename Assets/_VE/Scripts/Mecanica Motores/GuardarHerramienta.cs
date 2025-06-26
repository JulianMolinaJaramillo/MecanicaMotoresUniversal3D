using UnityEngine;

public class GuardarHerramienta : MonoBehaviour
{
    public string nombreHerramientaImagen; // Nombre para asignarle a la imagen
    public string nombreHerramienta; // Nombre completo de la pieza para el titulo
    [TextArea(3, 10)]
    public string descripcionPieza; // Descripcion de para que sirve esta herramienta

    public Material materialSeleccion; // El material que deseamos al momento de pararnos sobre la pieza
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
        ManagerCanvas.singleton.ActualizarInformacionPieza(nombreHerramienta, descripcionPieza); // Actualizamos la informacion de la pieza en el canvas
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
    /// Metodo incovado al momento de darle click sobre un objeto con collider
    /// </summary>
    void OnMouseDown()
    {
        if (InventarioUI.singleton != null)
        {
            InventarioUI.singleton.AgregarHerramientaInventario(icono, nombreHerramientaImagen);
            ManagerCanvas.singleton.BorrarInformacionPieza(); // Retiramos la informacion de la pieza del canvas

            if (AdministrarHerramientas.singleton != null)
            {
                AdministrarHerramientas.singleton.ReactivarHerramientas(); // Reactivamos la herramienta antes desactivada
                AdministrarHerramientas.singleton.herramientas.Add(this.gameObject); // Agregamos la herramienta a nuestro administrador
            }
            QuitarMaterial(); // Quitamos el material secundario
            this.gameObject.SetActive(false); // Desactivamos la herramienta seleccionada
        }
    }

    /// <summary>
    /// Metodo utilizado para asignarle el material de seleccion al momento de mover las piezas del motor
    /// </summary>
    /// <param name="id"> Para identificar si el material debe ser el verde o rojo </param>
    public void AgregarMaterial()
    {
        if (materialSeleccion != null)
        {
            Material[] nuevosMateriales = new Material[2]; // Creamos los nuevos materiales
            nuevosMateriales[0] = materialesOriginales[0]; // mantener el original
            nuevosMateriales[1] = materialSeleccion; // añadir el segundo
            meshRenderer.materials = nuevosMateriales; // Agrego el material a nuestro objeto objeto
        }
    }

    /// <summary>
    /// Para quitar el materiale de seleccion y solo dejar el material por defecto
    /// </summary>
    public void QuitarMaterial()
    {
        meshRenderer.materials = new Material[] { materialesOriginales[0] };
    }
}
