using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManagerCanvas : MonoBehaviour
{
    [Header("ESTA ES UNA CLASE SINGLETON")]
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public MovimientoJugador movimientoJugador; // Referencia al movimiento jugador principal
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public GameObject menuBienvenida; // Referencia al Menu de bienvenida del canvas principal
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public GameObject menuEleccionMotor; // Referencia al Menu de bienvenida del canvas principal
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public GameObject menuPausa; // Referencia al Menu Pausa del canvas principal
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public Button btnSalir; // Referencia al boton btnSalir del canvas principal
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public Button btnRotar; // Referencia al boton btnSalir del canvas principal
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public Button btnBajarPlataforma; // Referencia al boton btnSalir del canvas principal
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public GameObject btnReutilizarMotor; // Referencia al boton btnSalir del canvas principal
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public TextMeshProUGUI txtTituloPieza; // Referencia al texto titulo de la pieza
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public TextMeshProUGUI txtDescripcionPieza; // Referencia al texto descripcion para la pieza
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public GameObject mensajeAlerta;
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public TextMeshProUGUI txtMensaje; // Referencia al texto que nos indica si algo esta incorrecto o el inventario esta lleno

    //[HideInInspector]
    public bool btnReutilizableHabilitado;
    [HideInInspector]
    public bool juegoPausado;
    //[HideInInspector]
    public bool mensajeAlertaActivo;
    public bool activarTutorial;
    public static ManagerCanvas singleton;


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
        if (activarTutorial)
        {
            if (menuBienvenida != null)
            {
                menuBienvenida.SetActive(true);
                menuEleccionMotor.SetActive(true);
            }
        }    
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!juegoPausado)
            {
                if (AudioManager.singleton != null) AudioManager.singleton.PlayEfectString("Menu"); // Ejecutamos el efecto nombrado
                juegoPausado = true;
                menuPausa.SetActive(true);
                movimientoJugador.enabled = false;
                if (CamaraOrbital.singleton != null) CamaraOrbital.singleton.DeneterCamara();
            }
            else
            {
                if (AudioManager.singleton != null) AudioManager.singleton.PlayEfectString("Menu"); // Ejecutamos el efecto nombrado
                juegoPausado = false;
                menuPausa.SetActive(false);
                movimientoJugador.enabled = true;
                if (CamaraOrbital.singleton != null) CamaraOrbital.singleton.HabilitarCamara();
            }
        }
    }

    /// <summary>
    /// Metodo utilizado para actualizar la informacion de la pieza tomada de la mesa
    /// </summary>
    /// <param name="titulo"> Nombre tecnico de la pieza</param>
    /// <param name="descripcion"> Descripcion de para que sirve esa pieza </param>
    public void ActualizarInformacionPieza(string titulo, string descripcion)
    {
        txtTituloPieza.text = titulo;
        txtDescripcionPieza.text = descripcion;
    }

    /// <summary>
    /// Metodo utilizaod para borrar la informacion del titulo y descripcion
    /// </summary>
    public void BorrarInformacionPieza()
    {
        txtTituloPieza.text = "";
        txtDescripcionPieza.text = "";
    }

    /// <summary>
    /// Para habilitar la notificacion de inventario lleno o cualquier otro mensaje de alerta
    /// </summary>
    public void AlertarMensaje(string texto)
    {
        txtMensaje.text = texto;
        mensajeAlerta.SetActive(true);
        mensajeAlertaActivo = true;
    }

    /// <summary>
    /// Para deshabilitar la notificacion de inventario lleno o cualquier otro mensaje de alerta
    /// </summary>
    public void DesactivarAlertarMensaje()
    {
        mensajeAlerta.SetActive(false);
        mensajeAlertaActivo = false;
    }

    /// <summary>
    /// Para habilitar el interactuable del boton salir donde se requiera
    /// </summary>
    public void HabilitarBtnSalir()
    {
        btnSalir.interactable = true;
    }

    /// <summary>
    /// Para deshabilitar el interactuable del boton salir donde se requiera
    /// </summary>
    public void DeshabilitarBtnSalir()
    {
        btnSalir.interactable = false;
    }

    /// <summary>
    /// Para habilitar el interactuable del boton salir donde se requiera
    /// </summary>
    public void HabilitarBtnRotar()
    {
        btnRotar.interactable = true;
    }

    /// <summary>
    /// Para deshabilitar el interactuable del boton salir donde se requiera
    /// </summary>
    public void DeshabilitarBtnRotar()
    {
        btnRotar.interactable = false;
    }

    /// <summary>
    /// Para habilitar el interactuable del BtnBajarPlataforma salir donde se requiera
    /// </summary>
    public void HabilitarBtnBajarPlataforma()
    {
        btnBajarPlataforma.interactable = true;
    }

    /// <summary>
    /// Para deshabilitar el interactuable del BtnBajarPlataforma salir donde se requiera
    /// </summary>
    public void DeshabilitarBtnBajarPlataforma()
    {
        btnBajarPlataforma.interactable = false;
    }


    /// <summary>
    /// Para habilitar el boton btnReutilizarMotor donde se requiera
    /// </summary>
    public void HabilitarBtnReutilizarMotor()
    {
        btnReutilizarMotor.SetActive(true);
    }

    /// <summary>
    /// Para deshabilitar el boton btnReutilizarMotor donde se requiera
    /// </summary>
    public void DeshabilitarBtnReutilizarMotor()
    {
        btnReutilizarMotor.SetActive(false);
    }

    public void ActivarPausa()
    {
        if (!juegoPausado)
        {
            if (AudioManager.singleton != null) AudioManager.singleton.PlayEfectString("Menu"); // Ejecutamos el efecto nombrado
            juegoPausado = true;
            menuPausa.SetActive(true);
            movimientoJugador.enabled = false;
            if (CamaraOrbital.singleton != null) CamaraOrbital.singleton.DeneterCamara();
        }
        else
        {
            if (AudioManager.singleton != null) AudioManager.singleton.PlayEfectString("Menu"); // Ejecutamos el efecto nombrado
            juegoPausado = false;
            menuPausa.SetActive(false);
            movimientoJugador.enabled = true;
            if (CamaraOrbital.singleton != null) CamaraOrbital.singleton.HabilitarCamara();
        }
    }

    public void ActivarMenuEleccionMotor()
    {
        menuEleccionMotor.SetActive(true);
    }

    public void DesactivarMenuEleccionMotor()
    {
        menuEleccionMotor.SetActive(false);
    }
}
