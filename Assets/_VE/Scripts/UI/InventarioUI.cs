using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class InventarioUI : MonoBehaviour
{
    [Header("ESTA ES UNA CLASE SINGLETON")]
    [Header("")]
    [Header("REFERENCIAS PARA EL INVENTARIO PRINCIPAL")]
    [InfoMessage("Este es una referencia importante, asegúrate de configurarlo correctamente.", MessageTypeCustom.Warning)]
    public GameObject buttonPrefab; // Prefab del botón
    [InfoMessage("Este es una referencia importante, asegúrate de configurarlo correctamente.", MessageTypeCustom.Warning)]
    public Transform contentPanel;  // Contenedor de los botones (dentro del panel del inventario)
    [InfoMessage("Este es una referencia importante, asegúrate de configurarlo correctamente.", MessageTypeCustom.Warning)]
    public Transform puntoInstanciaInterno; // Punto de instancia de las piezas internas
    [InfoMessage("Este es una referencia importante, asegúrate de configurarlo correctamente.", MessageTypeCustom.Warning)]
    public Transform puntoInstanciaExterno; // Punto de instancia de las piezas externas

    [Header("REFERENCIAS PARA EL INVENTARIO DE HERRAMIENTAS")]
    [InfoMessage("Este es una referencia importante, asegúrate de configurarlo correctamente.", MessageTypeCustom.Warning)]
    public Image imgHerramienta; // Prefab del botón
    [InfoMessage("Este es una referencia importante, asegúrate de configurarlo correctamente.", MessageTypeCustom.Warning)]
    public TextMeshProUGUI txtHerramienta; // Prefab del botón

    [HideInInspector]
    public int contadorInstancias; // Para limitar la cantidad de objetos en el inventario
    private GameObject prefabSeleccionado; // El prefab seleccionado actualmente 

    public static InventarioUI singleton;
    private void Awake()
    {
        // Configurar Singleton
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// Metodo implementado al momento de agregar nuevos objetos al inventario
    /// </summary>
    /// <param name="icono"> El icono que tendrá el boton</param>
    /// <param name="prefab"> El prefab que instanciará ese boton </param>
    /// <param name="nombreBoton"> El nombre del objeto que tendrá el boton </param>
    public void AgregarAlInventario(Sprite icono, GameObject prefab, string nombreBoton, string nombrePieza, string descripcionPieza, bool piezaExterna)
    {
        if (contadorInstancias < 12) // Si hay menos de 13 piezas en el inventario
        {
            prefabSeleccionado = prefab; // Asignamos el prefab

            GameObject nuevoBoton = Instantiate(buttonPrefab, contentPanel);// Instanciamos el boton en el inventario
            Image iamgenIcono = nuevoBoton.GetComponentInChildren<Image>(); // Obtenemos el componenete imagen
            iamgenIcono.sprite = icono; // Asignamos la imagen al boton

            TextMeshProUGUI textoBoton = nuevoBoton.GetComponentInChildren<TextMeshProUGUI>(); // Obtenemos el componente texto
            textoBoton.text = nombreBoton; // Asignamos el texto al boton

            btnInventario btnInventario = nuevoBoton.GetComponent<btnInventario>(); // Obtenemos el componenete inventario
            btnInventario.prebafInstancia = prefabSeleccionado; // Agregamos el prefab seleccionado

            if (piezaExterna)
            {
                btnInventario.posicionInstancia = puntoInstanciaExterno; // Le Asignamos el punto de instancia 
            }
            else
            {
                btnInventario.posicionInstancia = puntoInstanciaInterno; // Le Asignamos el punto de instancia 
            }
            
            btnInventario.descripcion = descripcionPieza; // Agregamos la descripcion de la pieza
            btnInventario.nombre = nombrePieza; // Agregamos la descripcion de la pieza
   
            Button btn = nuevoBoton.GetComponent<Button>(); // Obtenemos el componenete button
            btn.onClick.AddListener(btnInventario.InstanciarPiezaMotor); // Agregamos la acción al botón
        }
        contadorInstancias += 1; // Aumentamos el contador
    }    

    public void AgregarHerramientaInventario(Sprite imagenHerramienta, string textoHerramienta)
    {
        imgHerramienta.sprite = imagenHerramienta;
        txtHerramienta.text = textoHerramienta;
    }
}
