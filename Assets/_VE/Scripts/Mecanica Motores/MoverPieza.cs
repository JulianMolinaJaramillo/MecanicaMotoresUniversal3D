using System.Collections;
using UnityEngine;

public class MoverPieza : MonoBehaviour
{
    [HideInInspector]
    public bool puedoValidar; // Para validar la colocacion de la pieza al momento de soltar el click y  no mientras arrastro
    [HideInInspector]
    public bool piezaColocada; // Para validar si la pieza ya fue colocada   

    [Header("CONFIGURACION INICIAL")]
    public bool piezaFinal; // Para validar si el prefab activa minijuego
    public Vector3 posicionObjetivo;  // La posicion en la cual dejaremos la pieza colocada
    public Material[] materialesSeleccion; // Para los materiales de seleccion verde y rojo
    public Material materialDisolver; // Para el material de disolucion
    public float tiempoDisolver; // Para controlar el tiempo de disolver 
    public GameObject[] snappsParaActivar; // Los puntos de contacto que se activan al momento de colocar una pieza
    public GameObject[] snappsParaDesactivar; // Los puntos de contacto que se desactivan al momento de colocar una pieza
    [TextArea(3, 4)]
    public string pista;

    [Header("PIEZAS QUE ACTIVAN MINIJUEGOS")]
    public MessageOnly mensaje7 = new MessageOnly("Para las piezas que activen minijuegos de atornillar", MessageTypeCustom.Info);
    public bool activaMinijuego; // Para validar si el prefab activa minijuego  
    public int sizeMinijuego; // Para indicar el tamaño de la llave para dicho minijuego
    public MessageOnly mensaje4 = new MessageOnly("Unicamente para las piezas con tornillos", MessageTypeCustom.Info);
    public AsignarTornillos asignarTornillos; // unicamente para las piezas que activen minijuego
    public MessageOnly mensaje6 = new MessageOnly("Para las piezas de lubricación", MessageTypeCustom.Info);
    public bool esLubricada; // unicamente para las piezas que activen minijuego de lubricación

    [Header("MODIFICACION DE PIEZAS QUE TENGAN HIJOS")]
    public MessageOnly mensaje3 = new MessageOnly("SI lo que deseas es modificarle el material a los hijos", MessageTypeCustom.Info);
    public MeshRenderer[] meshRendererHijos; // Para los mesh de los hijos de este objeto


    private Vector3 offset; // Para almacenar la diferencia entre la posicion del objeto y el punto de click
    private float coordinadaZ; // Para guardar la profundidad Z entre la camara y el objeto cuando se hace click
    private float valorDisolver;
    private bool noMover; // Para identificar si puedo o no mover la pieza
    private bool validarBrazo;
    private MeshRenderer meshRenderer;
    private Material[] materialesOriginales;
    private Collider collider;
    private Coroutine coroutine;

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

        AgregarDisolver(tiempoDisolver,1); // Asegúrate de que esté seteado
    }

    private void Update()
    {
        // Verificamos si estamos validando el brazo, para saber dependiendo de la posicion si activo el derecho o izquierdo
        if (validarBrazo)
        {
            if (transform.localPosition.x > 0)
            {
                ManagerBrazos.singleton.AsignarTargetDerecho(this.transform); // Le asignamos este transform como target a los brazos
            }
            else
            {
                ManagerBrazos.singleton.AsignarTargetIzquierdo(this.transform); // Le asignamos este transform como target a los brazos
            }
        }
    }

    /// <summary>
    /// metodo invocado al momento de hacer click con el mouse sobre una pieza
    /// </summary>
    void OnMouseDown()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        if (MesaMotor.singleton.mesaMotorActiva) // Validamos que estamos interactuando en la mesa de armado para poder manipular las piezas
        {
            puedoValidar = false;
            validarBrazo = true;
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
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

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
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        if (MesaMotor.singleton.mesaMotorActiva) // Validamos que estamos interactuando en la mesa de armado para poder manipular las piezas
        {
            validarBrazo = false; // Indicamos que ya no estamos validando el brazo
            ManagerBrazos.singleton.RetornarBrazos(); // Le asignamos este transform como target a los brazis

            puedoValidar = true;
            QuitarMateriales();
            coroutine = StartCoroutine(GetPuedoValidar());
        }
    }

    /// <summary>
    /// Currutina que solo da un tiempo de espera para retornar la variable booleana a false
    /// </summary>
    IEnumerator GetPuedoValidar()
    {
        yield return new WaitForSeconds(0.2f);
        puedoValidar = false;
        coroutine = null;
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
        noMover = true;
        collider.enabled = false;  
        DesactivarSnapp();

        if (ManagerCanvas.singleton != null)
        {
            ManagerCanvas.singleton.DeshabilitarBtnSalir();
            ManagerCanvas.singleton.DeshabilitarBtnRotar();
            ManagerCanvas.singleton.DeshabilitarBtnBajarPlataforma();
        }
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(MoverPiezaSuavemente(2));
    }

    /// <summary>
    /// Currutina encargada del movimiento de la pieza suavizado
    /// </summary>
    /// <param name="duracion"> Tiempo del movimiento de la pieza</param>
    public IEnumerator MoverPiezaSuavemente(float duracion)
    {
        if (AudioManager.singleton != null) AudioManager.singleton.PlayEfectString("PiezaColocada"); // Ejecutamos el efecto nombrado
        ManagerCanvas.singleton.ActualizarInformacionPista(pista); // Actualizamos la pista de armado para el jugador

        Vector3 inicio = transform.localPosition; // Trabajamos en local
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            transform.localPosition = Vector3.Lerp(inicio, posicionObjetivo, tiempo / duracion); // Mover en local
            tiempo += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = posicionObjetivo; // Posición final también en local
        piezaColocada = true;
        ActivarSnapp();
        QuitarMateriales();

        // Validamos si la pieza a colocar activa minijuego, de ser asi enviamos los datos necesarios
        if (activaMinijuego)
        {
            if (ManagerMinijuego.singleton != null)
            {
                if (asignarTornillos != null)
                {
                    ManagerMinijuego.singleton.ActivarMinijuego(asignarTornillos);
                }
                else 
                {
                    ManagerMinijuego.singleton.ActivarMinijuego();
                }   
                ManagerMinijuego.singleton.sizeHerramienta = sizeMinijuego;
            }
        }

        // Validamos si la pieza a colocar es la ultima pieza de dicho motor para la validacion final
        if (piezaFinal)
        {
            if (ManagerMinijuego.singleton != null)
            {
                ManagerMinijuego.singleton.ValidarMiniJuego();
            }
        }

        if (ManagerCanvas.singleton != null)
        {
            ManagerCanvas.singleton.HabilitarBtnRotar();
            ManagerCanvas.singleton.HabilitarBtnSalir();         
            ManagerCanvas.singleton.HabilitarBtnBajarPlataforma();
        }
        
        coroutine = null;
    }

    /// <summary>
    /// Metodo encargado de desactivar los collider de los snapPoint
    /// </summary>
    void DesactivarSnapp()
    {
        if (snappsParaDesactivar.Length > 0)
        {
            for (int i = 0; i < snappsParaDesactivar.Length; i++)
            {
                snappsParaDesactivar[i].SetActive(false);
            }
        }    
    }

    /// <summary>
    /// Metodo encargado de desactivar los collider de los snapPoint
    /// </summary>
    void ActivarSnapp()
    {
        if (snappsParaActivar.Length > 0)
        {
            for (int i = 0; i < snappsParaActivar.Length; i++)
            {
                snappsParaActivar[i].SetActive(true);
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

    
    public void AgregarDisolver(float tiempo, int disolver)
    {
        tiempoDisolver = tiempo;
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(AgregarMaterialDisolver(disolver));
    }

    private IEnumerator AgregarMaterialDisolver(int disolverAdentro)
    {
        if (materialDisolver != null)
        {
            // Instanciar copia para que este objeto tenga su propio material
            Material instanciaMaterial = new Material(materialDisolver);
            instanciaMaterial.SetFloat("_Frecuencia", 1f);

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

                if (disolverAdentro == 1)
                {           
                    valorDisolver = Mathf.Lerp(1f, -1f, t);
                }
                else
                {
                    valorDisolver = Mathf.Lerp(-1f, 1f, t);
                }
                
                instanciaMaterial.SetFloat("_Frecuencia", valorDisolver);

                tiempo += Time.deltaTime;
                yield return null;
            }

            // Asegurar valor final
            instanciaMaterial.SetFloat("_Frecuencia", -1f);
            coroutine = null;
            QuitarMateriales();
        }
    }
}
