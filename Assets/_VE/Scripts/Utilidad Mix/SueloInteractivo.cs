using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SueloInteractivo : MonoBehaviour
{
    [Header("Referencias Obligatorias")]
    public GameObject canvasWorldSpace; // Hace referencia al canvas que nos indica que tecla oprimir
    public GameObject canvasPrincipal; // Hace referencia al canvas principal del escenario
    public Button btnSalir; // Referencia al btnSalir del canvas
    public CamaraOrbital camaraPrincipal; // Camara orbital / principal
    public Transform posicionObjetivoCamara; // Posicion a la que deseamos llevar la camara
    public float velocidadPosCamara = 1; // Velocidad de desplazamiento de la camara
    

    [Header("Referencias Opcionales")]
    public MoverObjeto moverObjeto;
    public Button btnBajarPlataforma; // Referencia al btnBajarPlataforma del canvas
    public Collider[] piezasMeson; // Piezas sobre la mesa
    public bool mesaArmadoMotor; // Para validar si es el suelo interactivo de la mesa de armado, deberia ir activa en el sueloInteractivoArmadoMotor
    public bool mesaHerramientas; // Para validar si es el suelo interactivo de la mesa de herramientas, deberia ir activa en el SueloInteractivo Porta Herramientas

    private MovimientoJugador movimientoJugador; // Para guardar la referencia del movimiento del jugador
    private Camera camera; // Para guardar referencia a nuestra camara
    private int playerLayer; // Para guardar el numero de layer
    private Vector3 posicionOriginal; // para guardar la posicion original
    private Quaternion rotacionOriginal; // para guardar la rotacion original
    private bool interactuar; // Para validar si estoy interactuando
    public bool salirInteraccion; // Para validar si salgo de la interaccion
    private bool plataformaAbajo; // Para validar si salgo bajando la plataforma
    private Coroutine coroutine;
    
    private void Awake()
    {
        camera = camaraPrincipal.gameObject.GetComponent<Camera>(); // Obtenemos el componenete de la camara
    }

    private void Start()
    {
        StartCoroutine(CargaFantasmaCanvas()); // Cargamos los componentes de canvas rapidamente al inicio
        playerLayer = LayerMask.NameToLayer("Player"); // Obtener el número de layer correspondiente al nombre "Player"
        plataformaAbajo = true; // indicamos que inicialmente la plataforma se encuentra abajo
    }

    private void Update()
    {
        if (interactuar)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (AudioManager.singleton != null) AudioManager.singleton.PlayEfectString("SueloInteractivo2");

                DesactivarMovimientoJugador(movimientoJugador); // Desactivamos el movimiento del jugador que interactua

                posicionOriginal = camaraPrincipal.transform.position; // Guardamos la posicion original de mi camara orbital antes de interactuar
                rotacionOriginal = camaraPrincipal.transform.rotation; // Guardamos la rotacion original de mi camara orbital antes de interactuar

                camaraPrincipal.CursorVisible(); // Habilitamos la vista del cursor
                camaraPrincipal.enabled = false; // Deshabilitamos el script de la camara orbital

                if (mesaArmadoMotor && ManagerMinijuego.singleton.minijuegoActivo)
                {
                    if (plataformaAbajo)
                    {
                        EntornoMecanica.singleton.AbrirCompuerta(ManagerMinijuego.singleton.posicionMonijuegoActual);
                        plataformaAbajo = false;
                    }
                    else
                    {
                        InicializarMovimientoCamara(ManagerMinijuego.singleton.posicionMonijuegoActual);
                    }
                }
                else if (mesaArmadoMotor)
                {
                    if (plataformaAbajo)
                    {
                        EntornoMecanica.singleton.AbrirCompuerta(posicionObjetivoCamara);
                        plataformaAbajo = false;
                    }
                    else
                    {
                        InicializarMovimientoCamara(posicionObjetivoCamara);
                    }            
                }
                else
                {
                    InicializarMovimientoCamara(posicionObjetivoCamara);
                }

                if (!mesaArmadoMotor && !mesaHerramientas)
                {
                    if (ManagerCanvas.singleton != null && ManagerCanvas.singleton.btnReutilizableHabilitado == true && MesaMotor.singleton.interaccionEjecutada == true)
                    {
                        ManagerCanvas.singleton.HabilitarBtnReutilizarMotor();
                    }
                }

                camera.cullingMask &= ~(1 << playerLayer); // Desactivamos la layer "PLayer" de la camara para que no se vea nuestro personaje               
                canvasWorldSpace.SetActive(false);  // Desactivamos canvas visual       
                btnSalir.onClick.AddListener(SalirInteraccion); // Agregamos el evento actual al boton

                // Si tenemos referenciado el boton lo activamos
                if (btnBajarPlataforma != null) btnBajarPlataforma.onClick.AddListener(BajarPlataforma);

                // Si tenemos referenciado el script, ejecutamos
                if (moverObjeto != null) moverObjeto.IniciarDesplazamientoObjeto();

                // Si tenemos almenos una pieza para interactuar
                if (piezasMeson.Length > 0) ActivarPiezas();

                interactuar = false; // indicamos que ya no podemos interactuar
            }
        }
    }

    public void InicializarMovimientoCamara(Transform posicionObjetivo)
    {
        if (coroutine != null) StopCoroutine(coroutine);

        coroutine = StartCoroutine(MoverCamara(posicionObjetivo.position, posicionObjetivo.rotation, velocidadPosCamara)); // Movemos la camara 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactuar = true; // Indicamos que podemos interactuar
            canvasWorldSpace.SetActive(true); // Activamos canvas visual

            movimientoJugador = other.GetComponent<MovimientoJugador>();  // Obtenemos una referencia al movimiento del jugador que interactua       
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactuar = false; // Indicamos que no podemos interactuar
            canvasWorldSpace.SetActive(false);  // Desactivamos canvas visual

            movimientoJugador = null;  // Eliminamos la referencia al movimiento del jugador que interactua    
        }
    }

    /// <summary>
    /// Currutina encargada del movimiento de la pieza suavizado
    /// </summary>
    /// <param name="posicionDeseada"> La posicion a la cual queremos moder la camara </param>
    /// <param name="duracion"> Tiempo del movimiento de la pieza </param>

    IEnumerator MoverCamara(Vector3 destinoPos, Quaternion destinoRot, float duracion)
    {
        Vector3 posicionInicio = camaraPrincipal.transform.position; //  Guardamos la posicion de inicio
        Quaternion rotacionInicio = camaraPrincipal.transform.rotation; //  Guardamos la rotacion de inicio

        float tiempo = 0f; // Damos un tiempo para la interpolacion

        while (tiempo < duracion)
        {
            // Asignamos la posicion y rotacion de la camara, con interpolacion lineal
            camaraPrincipal.transform.position = Vector3.Lerp(posicionInicio, destinoPos, tiempo / duracion);
            camaraPrincipal.transform.rotation = Quaternion.Lerp(rotacionInicio, destinoRot, tiempo / duracion);

            tiempo += Time.deltaTime;
            yield return null;
        }

        camaraPrincipal.transform.position = destinoPos; // Aseguramos la posición final
        camaraPrincipal.transform.rotation = destinoRot; // Aseguramos la rotacion final

        SaliendoInteraccion();
    }

    public void SaliendoInteraccion()
    {
        if (salirInteraccion) // Si salimos de interaccion
        {
            camaraPrincipal.enabled = true; // Habilitamos nuevamente la camara orbital
            camaraPrincipal.CursorInvisible(); // Habilitamos la vista del cursor
            ActivarMovimientoJugador(movimientoJugador); // Activamos el movimiento del jugador que interactua
            salirInteraccion = false; // Indicamos que ya no estamos interactuando
            interactuar = true; // Indicamos que nuevamente puede interactura aun sin salir del trigger
        }
        else
        {
            btnSalir.gameObject.SetActive(true); // Habilitamos el boton de salir
            canvasPrincipal.SetActive(true);  // Activamos canvas informativo

            if (btnBajarPlataforma != null) btnBajarPlataforma.gameObject.SetActive(true);// Si tenemos referenciado el boton lo activamos

            if (mesaArmadoMotor && !plataformaAbajo)
            {
                // Si el miijuego esta activo lo desactivamos al momento de salir de la interaccion de la mesa de armado
                if (ManagerMinijuego.singleton.minijuegoActivo) ManagerMinijuego.singleton.miniJuegoAtornillar.SetActive(true);

                if (EntornoMecanica.singleton != null) EntornoMecanica.singleton.BajarIntensidadLuzPrincipal();

                if (ControlCamaraMotor.singleton != null) ControlCamaraMotor.singleton.enabled = true;

                MesaMotor.singleton.mesaMotorActiva = true;
            }      
        }
    }

    /// <summary>
    /// Metodo utilizado al momento de salir de la interacccion
    /// </summary>
    public void SalirInteraccion()
    {
        if (AudioManager.singleton != null) AudioManager.singleton.PlayEfectString("SueloInteractivo2");

        // Indicamos que estamos saliendo de la interacion
        salirInteraccion = true; 

        if (moverObjeto != null) moverObjeto.RetornarPosicionOriginal();

        // Si tenemos almenos una pieza para interactuar
        if (piezasMeson.Length > 0) DesactivarPiezas();

        if (!plataformaAbajo && mesaArmadoMotor)
        {
            if (ManagerMinijuego.singleton.minijuegoActivo) ManagerMinijuego.singleton.miniJuegoAtornillar.SetActive(false);

            if (EntornoMecanica.singleton != null) EntornoMecanica.singleton.SubirIntensidadLuzPrincipal();

            if (ControlCamaraMotor.singleton != null) ControlCamaraMotor.singleton.enabled = false;

            MesaMotor.singleton.mesaMotorActiva = false;

            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(MoverCamara(posicionOriginal, rotacionOriginal, velocidadPosCamara)); // Retornamos la camara principal a la posicion original 
            HabilitarInfoMesaArmado();      
        }
        else if(!mesaArmadoMotor)
        {
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(MoverCamara(posicionOriginal, rotacionOriginal, velocidadPosCamara)); // Retornamos la camara principal a la posicion original 
            HabilitarInfoMesaArmado();
        }

        camera.cullingMask |= (1 << playerLayer); // Activamos de nuevo la layer "Player" para que nuestro personaje se vea     
        canvasPrincipal.SetActive(false);  // Desactivamos canvas informativo   
        btnSalir.gameObject.SetActive(false); // Habilitamos el boton de salir 
        btnSalir.onClick.RemoveListener(SalirInteraccion); // Retiramos el evento actual del boton

        if (!mesaArmadoMotor && !mesaHerramientas)
        {
            if (ManagerCanvas.singleton != null && ManagerCanvas.singleton.btnReutilizableHabilitado == true)
            {
                ManagerCanvas.singleton.DeshabilitarBtnReutilizarMotor();
            }
        }

        if (btnBajarPlataforma != null)
        {
            btnBajarPlataforma.onClick.RemoveListener(BajarPlataforma); // Retiramos el evento actual del boton
            btnBajarPlataforma.gameObject.SetActive(false);
        }
    }

    public void BajarPlataforma()
    {
        if (EntornoMecanica.singleton != null)
        {
            plataformaAbajo = true;
            SalirInteraccion();
            EntornoMecanica.singleton.CerrarCompuerta();       
            btnBajarPlataforma.gameObject.SetActive(false);       
        }  
    }

    public void HabilitarInfoMesaArmado()
    {
        canvasWorldSpace.SetActive(true); // Activamos canvas visual
    }

    /// <summary>
    /// Metodo utilizado para activar el movimiento del jugador que interactura
    /// </summary>
    /// <param name="movimiento"> script de movimiento </param>
    public void ActivarMovimientoJugador(MovimientoJugador movimiento)
    {
        movimiento.enabled = true;
    }

    /// <summary>
    /// Metodo utilizado para desactivar el movimiento del jugador que interactura
    /// </summary>
    /// <param name="movimiento"> script de movimiento </param>
    public void DesactivarMovimientoJugador(MovimientoJugador movimiento)
    {
        movimiento.enabled = false;
    }

    /// <summary>
    /// Metodo para habilitar los collider de las piezas
    /// </summary>
    public void ActivarPiezas()
    {
        for (int i = 0; i < piezasMeson.Length; i++)
        {
            // Validamos que las piezas no sean nulas y procedemos a activar los colliders
            if (piezasMeson[i] != null)
            {
                piezasMeson[i].enabled = true;
            }           
        }
    }

    /// <summary>
    /// Metodo para inhabilitar los collider de las piezas
    /// </summary>
    public void DesactivarPiezas()
    {
        for (int i = 0; i < piezasMeson.Length; i++)
        {
            // Validamos que las piezas no sean nulas y procedemos a desactivar los colliders
            if (piezasMeson[i] != null)
            {
                piezasMeson[i].enabled = false;
            }          
        }
    }

    IEnumerator CargaFantasmaCanvas()
    {
        canvasWorldSpace.SetActive(true);
        canvasPrincipal.SetActive(true);
        btnSalir.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        canvasWorldSpace.SetActive(false);
        canvasPrincipal.SetActive(false);
        btnSalir.gameObject.SetActive(false);
    }
}
