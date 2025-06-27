using System.Collections;
using UnityEngine;

public class MoverPieza : MonoBehaviour
{
    [HideInInspector]
    public bool puedoValidar; // Para validar la colocacion de la pieza al momento de soltar el click y  no mientras arrastro
    [HideInInspector]
    public bool piezaColocada; // Para validar si la pieza ya fue colocada
    public bool activaMinijuego; // Para validar si el prefab activa minijuego
    public int sizeMinijuego; // Para indicar el tamaño de la llave para dicho minijuego
    public Vector3 posicionObjetivo;  // La posicion en la cual dejaremos la pieza colocada
    public Material[] materialesSeleccion; // Para los materiales de seleccion verde y rojo
    public Collider[] snappsParaActivar; // Los puntos de contacto que se activan al momento de colocar una pieza
    public Collider[] snappsParaDesactivar; // Los puntos de contacto que se desactivan al momento de colocar una pieza

    private Vector3 offset; // Para almacenar la diferencia entre la posicion del objeto y el punto de click
    private float coordinadaZ; // Para guardar la profundidad Z entre la camara y el objeto cuando se hace click
    private bool noMover; // Para identificar si puedo o no mover la pieza

    public MessageOnly mensaje1 = new MessageOnly("SI lo que deseas es modificarle el material a los hijos", MessageTypeCustom.Info);
    public MeshRenderer[] meshRendererHijos; // Para los mesh de los hijos de este objeto

    private MeshRenderer meshRenderer;
    private Material[] materialesOriginales;
    private Collider collider;

    private void Awake()
    {
        // Obtenemos los componentes
        collider = GetComponent<Collider>();
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
    /// metodo invocado al momento de hacer click con el mouse sobre una pieza
    /// </summary>
    void OnMouseDown()
    {
        if (MesaMotor.singleton.mesaMotorActiva) // Validamos que estamos interactuando en la mesa de armado para poder manipular las piezas
        {
            puedoValidar = false;
            if (!noMover)
            {
                AgregarSegundoMaterial(0);
                coordinadaZ = Camera.main.WorldToScreenPoint(transform.position).z; // Convertimos la posicion del objeto en coordenadas de la pantalla
                offset = transform.position - ObtenerPosicionMouse(); // Calcula la diferencia entre la posición real del objeto y la posición del mouse en el mundo 3D
            }
        }    
    }

    /// <summary>
    /// metodo invocado al momento de arrastrar con el click presionado una pieza
    /// </summary>
    void OnMouseDrag()
    {
        if (MesaMotor.singleton.mesaMotorActiva) // Validamos que estamos interactuando en la mesa de armado para poder manipular las piezas
        {
            if (!noMover)
            {
                // Actualiza la posición del objeto a la nueva posición del mouse en el mundo, manteniendo el offset inicial
                transform.position = ObtenerPosicionMouse() + offset;
            }
        }   
    }

    /// <summary>
    /// Metodo incovado al momento de soltar el click sostenido de una pieza
    /// </summary>
    void OnMouseUp()
    {
        if (MesaMotor.singleton.mesaMotorActiva) // Validamos que estamos interactuando en la mesa de armado para poder manipular las piezas
        {
            puedoValidar = true;
            QuitarMateriales();
            StartCoroutine(GetPuedoValidar());
        }     
    }

    /// <summary>
    /// Currutina que solo da un tiempo de espera para retornar la variable booleana a false
    /// </summary>
    IEnumerator GetPuedoValidar()
    {
        yield return new WaitForSeconds(0.2f);
        puedoValidar = false;
    }

    /// <summary>
    /// Metodo para obtener la posicion del objeto mientras lo mvoemos
    /// </summary>
    /// <returns> La posicion del mouse en el mundo </returns>
    Vector3 ObtenerPosicionMouse()
    {
        Vector3 puntoCursor = Input.mousePosition; // Toma la posición del mouse en píxeles de la pantalla
        puntoCursor.z = coordinadaZ; // Le asignamos la misma profundidad Z que tenía el objeto al hacer clic, para que coincidan los planos
        return Camera.main.ScreenToWorldPoint(puntoCursor); // Devuelve la posición del mouse en el mundo 3D usando la cámara principal
    }

    /// <summary>
    /// Metodo invocado desde Script SnapPoint encargado de iniciar la currutina de desplazamiento del objeto y de desactivar los colliders de los sanpPoint
    /// </summary>
    public void IniciarMovimiento()
    {
        piezaColocada = true;
        noMover = true;
        collider.enabled = false;
        QuitarMateriales();
        DesactivarSnappColliders();
        ActivarSnappColliders();
        StartCoroutine(MoverPiezaSuavemente(2));
    }

    /// <summary>
    /// Currutina encargada del movimiento de la pieza suavizado
    /// </summary>
    /// <param name="duracion"> Tiempo del movimiento de la pieza</param>
    public IEnumerator MoverPiezaSuavemente(float duracion)
    {
        Vector3 inicio = transform.position; //  Guardamos la posicion de inicio
        float tiempo = 0f; // Damos un tiempo para la interpolacion
        
        while (tiempo < duracion)
        {
            transform.position = Vector3.Lerp(inicio, posicionObjetivo, tiempo / duracion);
            tiempo += Time.deltaTime;
            yield return null;
        }

        transform.position = posicionObjetivo; // Asegura posición final

        // Validamos si la pieza a colocar activa minijuego, de ser asi enviamos los datos necesarios
        if (activaMinijuego)
        {
            if (ManagerCanvas.singleton != null)
            {
                ManagerMinijuego.singleton.ActivarMinijuego();
            }
            if (ManagerMinijuego.singleton != null)
            {
                ManagerMinijuego.singleton.sizeHerramienta = sizeMinijuego;
            }
        }
    }

    /// <summary>
    /// Metodo encargado de desactivar los collider de los snapPoint
    /// </summary>
    void DesactivarSnappColliders()
    {
        if (snappsParaDesactivar.Length > 0)
        {
            for (int i = 0; i < snappsParaDesactivar.Length; i++)
            {
                snappsParaDesactivar[i].enabled = false;
            }
        }    
    }

    /// <summary>
    /// Metodo encargado de desactivar los collider de los snapPoint
    /// </summary>
    void ActivarSnappColliders()
    {
        if (snappsParaActivar.Length > 0)
        {
            for (int i = 0; i < snappsParaActivar.Length; i++)
            {
                snappsParaActivar[i].enabled = true;
            }
        }    
    }

    /// <summary>
    /// Metodo utilizado para asignarle el material de seleccion al momento de mover las piezas del motor
    /// </summary>
    /// <param name="id"> Para identificar si el material debe ser el verde o rojo </param>
    public void AgregarSegundoMaterial(int id)
    {
        if (materialesSeleccion.Length > 0)
        {
            Material[] nuevosMateriales = new Material[2]; // Creamos los nuevos materiales
            nuevosMateriales[0] = materialesOriginales[0]; // mantener el original
            nuevosMateriales[1] = materialesSeleccion[id]; // añadir el segundo

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
    /// Para quitar los materiales de seleccion y solo dejar el material por defecto
    /// </summary>
    public void QuitarMateriales()
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
