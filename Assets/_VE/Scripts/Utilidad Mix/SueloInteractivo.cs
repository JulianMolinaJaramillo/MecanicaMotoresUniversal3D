using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SueloInteractivo : MonoBehaviour
{
    [Header("Referencias Obligatorias")]
    public GameObject canvasWorldSpace; // Hace referencia al canvas que nos indica que tecla oprimir
    public GameObject canvasPrincipal; // Hace referencia al canvas principal del escenario
    public Button botonSalir; // Referencia al boton del canvas
    public CamaraOrbital camaraPrincipal; // Camara orbital / principal
    public Transform posicionObjetivoCamara; // Posicion a la que deseamos llevar la camara
    public float velocidadPosCamara = 1; // Velocidad de desplazamiento de la camara
    public bool mesaArmadoMotor; // Para validar si es el suelo interactivo de la mesa de armado, deberia ir activa en el sueloInteractivoArmadoMotor

    [Header("Referencias Opcionales")]
    public GameObject canvasInformativo; // Hace referencia al canvas de informacion
    public ControlCamaraMotor controlCamaraMotor; // Referencia al script que controla las camaras en el armado
    public Collider[] piezasMeson; // Piezas sobre la mesa

    private MovimientoJugador movimientoJugador; // Para guardar la referencia del movimiento del jugador
    private Camera camera; // Para guardar referencia a nuestra camara
    private int playerLayer; // Para guardar el numero de layer
    private Vector3 posicionOriginal; // para guardar la posicion original
    private Quaternion rotacionOriginal; // para guardar la rotacion original
    private bool interactuar; // Para validar si estoy interactuando
    private bool salirInteraccion; // Para validar si salgo de la interaccion
    
    private void Awake()
    {
        camera = camaraPrincipal.gameObject.GetComponent<Camera>(); // Obtenemos el componenete de la camara
    }

    private void Start()
    {    
        playerLayer = LayerMask.NameToLayer("Player"); // Obtener el número de layer correspondiente al nombre "Player"
    }
    private void Update()
    {
        if (interactuar)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                posicionOriginal = camaraPrincipal.transform.position; // Guardamos la posicion original de mi camara orbital antes de interactuar
                rotacionOriginal = camaraPrincipal.transform.rotation; // Guardamos la rotacion original de mi camara orbital antes de interactuar

                camaraPrincipal.enabled = false; // Deshabilitamos el script de la camara orbital
                camaraPrincipal.tag = "MainCamera"; // Cambiamos el tag de la camara orbital para que interactue con las piezas armables
                StartCoroutine(MoverCamara(posicionObjetivoCamara.position, posicionObjetivoCamara.rotation, velocidadPosCamara)); // Movemos la camara

                camera.cullingMask &= ~(1 << playerLayer); // Desactivamos la layer "PLayer" de la camara para que no se vea nuestro personaje         
                DesactivarMovimientoJugador(movimientoJugador); // Desactivamos el movimiento del jugador que interactua

                canvasWorldSpace.SetActive(false);  // Desactivamos canvas visual
                canvasPrincipal.SetActive(true);  // Activamos canvas informativo
                
                botonSalir.onClick.AddListener(SalirInteraccion); // Agregamos el evento actual al boton

                if (canvasInformativo != null) // Si es diferente de null habilitamos el canvas informativo
                {
                    canvasInformativo.SetActive(true);
                }

                if (controlCamaraMotor != null) // Si es diferente de null habilitamos el script del movimiento de camaras
                {
                    controlCamaraMotor.enabled = true;
                }

                if (piezasMeson.Length > 0) // Si tenemos almenos una pieza para interactuar
                {
                    ActivarPiezas();
                }

                if (mesaArmadoMotor)
                {
                    MesaMotor.singleton.mesaMotorActiva = true;
                }

                interactuar = false; // indicamos que ya no podemos interactuar
            }
        }
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

        if (salirInteraccion) // Si salimos de interaccion
        {
            camaraPrincipal.enabled = true; // Habilitamos nuevamente la camara orbital
            salirInteraccion = false; // Indicamos que ya no estamos interactuando
            interactuar = true; // Indicamos que nuevamente puede interactura aun sin salir del trigger
        }
    }

    /// <summary>
    /// Metodo utilizado al momento de salir de la interacccion
    /// </summary>
    public void SalirInteraccion()
    {
        salirInteraccion = true; // Indicamos que estamos saliendo de la interacion

        if (canvasInformativo != null) // Si es diferente de null deshabilitamos el canvas informativo
        {
            canvasInformativo.SetActive(false);
        }

        if (controlCamaraMotor != null) // Si es diferente de null deshabilitamos el script
        {
            controlCamaraMotor.enabled = false;
        }

        if (piezasMeson.Length > 0) // Si tenemos almenos una pieza para interactuar
        {
            DesactivarPiezas();
        }

        if (mesaArmadoMotor)
        {
            MesaMotor.singleton.mesaMotorActiva = false;
        }

       StartCoroutine(MoverCamara(posicionOriginal,rotacionOriginal,velocidadPosCamara)); // Retornamos la camara principal a la posicion original
       camera.cullingMask |= (1 << playerLayer); // Activamos de nuevo la layer "Player" para que nuestro personaje se vea
       ActivarMovimientoJugador(movimientoJugador); // Activamos el movimiento del jugador que interactua
       canvasWorldSpace.SetActive(true); // Activamos canvas visual
       canvasPrincipal.SetActive(false);  // Desactivamos canvas informativo
       camaraPrincipal.tag = "Untagged"; // Volvemos el tag de la camara al por defecto para que no interactue con las piezas armables     
       botonSalir.onClick.RemoveListener(SalirInteraccion); // Retiramos el evento actual del boton
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
}
