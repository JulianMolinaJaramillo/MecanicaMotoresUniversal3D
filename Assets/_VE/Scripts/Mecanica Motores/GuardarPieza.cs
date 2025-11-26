using System.Collections;
using UnityEngine;

public class GuardarPieza : MonoBehaviour
{
    public string nombrePiezaBoton; // Nombre para asignarle al boton de la piza
    public string nombrePieza; // Nombre completo de la pieza para el titulo
    [TextArea(3, 10)]
    public string descripcionPieza; // Descripcion de para que sirve esa pieza

    public Material materialSeleccion; // El material que deseamos al momento de pararnos sobre la pieza
    public Material materialDisolver; // El material que deseamos poner al momento de tomar la pieza
    public float tiempoDisolver; // Para controlar el tiempo de disolver 
    private string frecuencia = "_Frecuencia"; // Nombre exacto de la propiedad en el Shader Graph

    public GameObject prefabInstancia; // El prefab que posteriormente instanciará esa pieza
    public Sprite icono; // Imagen para mostrar en el botón del inventario
    public bool piezaExterna; // Para identificar si instanciamos en el punto de instancia interno o externo

    public MessageOnly mensaje1 = new MessageOnly("Si lo que deseas es modificarle el material a los hijos", MessageTypeCustom.Info);
    public MeshRenderer[] meshRendererHijos; // Para los mesh de los hijos de este objeto

    private MeshRenderer meshRenderer; // Referencia a nuestro mesh
    private Material[] materialesOriginales; // Para almacenar nuestros materiales

    private bool puedoInteractuar;
    private float valorDisolver;
    private Coroutine coroutine;

    private void Awake()
    {
        // Obtenemos los componentes
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
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
    /// Metodo incovado al momento de posar el cursor sobre un objeto con collider
    /// </summary>
    void OnMouseEnter()
    {
        if (!puedoInteractuar && !ManagerCanvas.singleton.mensajeAlertaActivo)
        {
            AgregarMaterialSeleccion(); // Asignamos el material secundario
            ManagerCanvas.singleton.ActualizarInformacionPieza(nombrePieza, descripcionPieza); // Actualizamos la informacion de la pieza en el canvas 
        }     
    }

    /// <summary>
    /// Metodo incovado al momento de sacar el cursor de un objeto con collider
    /// </summary>
    void OnMouseExit()
    {
        if (!puedoInteractuar)
        {
            QuitarMaterial(); // Quitamos el material secundario
            ManagerCanvas.singleton.BorrarInformacionPieza(); // Retiramos la informacion de la pieza del canvas
        }    
    }

    /// <summary>
    /// Metodo incovado al momento de darle click sobre un objeto con collider
    /// </summary>
    void OnMouseDown()
    {
        if (!puedoInteractuar && !ManagerCanvas.singleton.mensajeAlertaActivo)
        {
            InteractuarPieza();
        }    
    }

    /// <summary>
    /// Metodo utilizado para instanciar la pieza en el canvas, en el inventario y ser guardada
    /// </summary>
    public void InteractuarPieza()
    {
        if (InventarioUI.singleton != null)
        {
            if (InventarioUI.singleton.contadorInstancias < 12) // Si todavia tengo capacidad en el inventario
            {
                if (AudioManager.singleton != null) AudioManager.singleton.PlayEfectString("TomarPieza2"); // Ejecutamos el efecto nombrado

                puedoInteractuar = true;

                InventarioUI.singleton.AgregarAlInventario(icono, prefabInstancia, nombrePiezaBoton, nombrePieza, descripcionPieza, piezaExterna); // instanciamos en el inventario

                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
                coroutine = StartCoroutine(MaterialDisolver(1));

                ManagerCanvas.singleton.BorrarInformacionPieza(); // Retiramos la informacion de la pieza del canvas
            }
            else
            {
                string texto = "El Inventario Se Encuentra LLeno, Debes Liberar Espacio Primero";
                ManagerCanvas.singleton.AlertarMensaje(texto);
            }
        }
        
    }

    public void AgregarMaterialDisolver(int id)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(MaterialDisolver(id));
    }
    private IEnumerator MaterialDisolver(int id)
    {
        if (materialDisolver != null)
        {
            // Instanciar copia para que este objeto tenga su propio material
            Material instanciaMaterial = new Material(materialDisolver);
            instanciaMaterial.SetFloat("_Frecuencia", -1f);

            Material[] nuevosMateriales = new Material[1];
            nuevosMateriales[0] = instanciaMaterial;

            // Asignar material a los renderers
            if (meshRendererHijos.Length > 0)
            {
                for (int i = 0; i < meshRendererHijos.Length; i++)
                {
                    meshRendererHijos[i].materials = nuevosMateriales;
                }
            }
            else
            {
                meshRenderer.materials = nuevosMateriales;
            }

            // Interpolar la propiedad "_Frecuencia"
            float tiempo = 0f;
            while (tiempo < tiempoDisolver)
            {
                float t = tiempo / tiempoDisolver;
                if (id == 1)
                {
                    valorDisolver = Mathf.Lerp(-1f, 1f, t);
                }
                else
                {
                    valorDisolver = Mathf.Lerp(1f, -1f, t);
                }
                
                instanciaMaterial.SetFloat("_Frecuencia", valorDisolver);

                tiempo += Time.deltaTime;
                yield return null;
            }

            // Asegurar valor final
            instanciaMaterial.SetFloat("_Frecuencia", 1f);

            if (id == 1)
            {
                this.gameObject.SetActive(false);
            }

            puedoInteractuar = false;
            QuitarMaterial();
            coroutine = null;
        }
    }

    /// <summary>
    /// Metodo utilizado para asignarle el material de seleccion al momento de mover sobre las piezas del motor
    /// </summary>
    /// <param name="id"> Para identificar si el material debe ser el verde o rojo </param>
    public void AgregarMaterialSeleccion()
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
