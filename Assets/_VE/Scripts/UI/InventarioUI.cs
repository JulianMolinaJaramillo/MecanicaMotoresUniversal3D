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
    [InfoMessage("Este es una referencia importante, asegúrate de configurarlo correctamente.", MessageTypeCustom.Warning)]
    public Image imgManoSuelta; // imagen de la mano
    [InfoMessage("Este es una referencia importante, asegúrate de configurarlo correctamente.", MessageTypeCustom.Warning)]
    public GameObject btnSoltarHerramienta; // referencia al btnSoltarHerramienta
    //[HideInInspector]
    public int tamanoHerramienta;

    [HideInInspector]
    public int contadorInstancias; // Para limitar la cantidad de objetos en el inventario
    private GameObject prefabSeleccionado; // El prefab seleccionado actualmente 
    private Sprite spriteActualHerramienta;
    private Color alfaActual;

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

    private void Start()
    {
        // Obtenemos una referencia a el sprite inicial de la herramienta
        spriteActualHerramienta = imgHerramienta.sprite;
        alfaActual = imgHerramienta.color;
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
            //Image iamgenIcono = nuevoBoton.GetComponentInChildren<Image>(); // Obtenemos el componenete imagen
            //iamgenIcono.sprite = icono; 

            TextMeshProUGUI textoBoton = nuevoBoton.GetComponentInChildren<TextMeshProUGUI>(); // Obtenemos el componente texto
            textoBoton.text = nombreBoton; // Asignamos el texto al boton

            btnInventario btnInventario = nuevoBoton.GetComponent<btnInventario>(); // Obtenemos el componenete inventario
            btnInventario.imagenIcono.sprite = icono; // Asignamos la imagen al boton
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

    public void LimpiarInventario()
    {
        contadorInstancias = 0;
        foreach (Transform child in contentPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Metodo invocado para agregar una herramienta a mi inventario de tools
    /// </summary>
    /// <param name="imagenHerramienta"> Imagen de la herramienta a colocar, tomada del prefab de la herramienta</param>
    /// <param name="textoHerramienta"> Texto de la herramienta a colocar, tomada del prefab de la herramienta</param>
    public void AgregarHerramientaInventario(Sprite imagenHerramienta, string textoHerramienta, int size)
    {
        tamanoHerramienta = size;
        imgHerramienta.sprite = imagenHerramienta;
        Color nuevoAlfa = imgHerramienta.color;
        nuevoAlfa.a = 255f;
        imgHerramienta.color = nuevoAlfa;
        txtHerramienta.fontSize = 26f;
        txtHerramienta.text = textoHerramienta;
        imgManoSuelta.enabled = false;
        btnSoltarHerramienta.SetActive(true);
    }

    /// <summary>
    /// Metodo invocado desde btnSoltarHerramienta para reestablecer la imagen de mi herramienta inicial
    /// </summary>
    public void ReestablecerHerramientaInventario()
    {
        if (InventarioHerramientas.singleton != null)
        {
            InventarioHerramientas.singleton.herramientasIndividuales.Clear(); // Limpiamos la lista de herramientas
        }

        if (ManagerMinijuego.singleton.minijuegoActivo)
        {
            ManagerMinijuego.singleton.herramientasRotatorias.SetActive(false);
            ManagerMinijuego.singleton.prensaValvulas.SetActive(false);
        }

        imgHerramienta.sprite = spriteActualHerramienta;
        imgHerramienta.color = alfaActual;
        txtHerramienta.fontSize = 18f;
        txtHerramienta.text = "Sin Herramienta";
        tamanoHerramienta = 0;
        btnSoltarHerramienta.SetActive(false);
        imgManoSuelta.enabled = true;
    }
}
