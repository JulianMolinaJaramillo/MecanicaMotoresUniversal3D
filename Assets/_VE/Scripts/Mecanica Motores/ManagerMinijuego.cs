using UnityEngine;
using UnityEngine.UI;

public class ManagerMinijuego : MonoBehaviour
{
    public bool minijuegoActivo;
    public int sizeHerramienta;
    public string motorActivo;
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public GameObject miniJuegoAtornillar;
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public Button btnAplicarTorque;
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public Button btnEncenderMotor;
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public GameObject sliderVelocidadMotor;

    public Transform[] posicionesMinijuego1;
    public int[] torquesTornillosBancada;

    public Transform[] posicionesMinijuego2;
    public int[] torquesTornillosBielas;

    public bool[] minijuegos;

    [HideInInspector]
    public Transform posicionMonijuegoActual;

    [HideInInspector]
    public bool aplicandoTorque;
    

    private int contador = 0;
    private int puntaje = 0;
    public static ManagerMinijuego singleton;

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
        posicionMonijuegoActual = posicionesMinijuego1[0];
        btnAplicarTorque.onClick.AddListener(TorqueAplicadoTornillosBancada);
    }

    /// <summary>
    /// Para habilitar el interactuable del boton btnEncenderMotor donde se requiera
    /// </summary>
    public void HabilitarBtnEnceder()
    {
        btnEncenderMotor.interactable = true;
    }

    /// <summary>
    /// Para deshabilitar el interactuable del boton btnEncenderMotor donde se requiera
    /// </summary>
    public void DeshabilitarBtnEnceder()
    {
        btnEncenderMotor.interactable = false;
    }

    /// <summary>
    /// Metodo invocado desde los botones de Eleccion Motor en el canvas principal
    /// </summary>
    /// <param name="nombreMotor"> Nombre del motor que se va a proceder con el armado</param>
    public void AsignarMotorActivo(string nombreMotor)
    {
        motorActivo = nombreMotor;
    }

    public void ActivarMinijuego()
    {
        minijuegoActivo = true;
        miniJuegoAtornillar.SetActive(true);
        PosicionInicialCamaraMinijuego();
    }

    public void PosicionInicialCamaraMinijuego()
    {
        if (minijuegos[0])
        {
            ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuego1[0], 1);
        }
        else if (minijuegos[1])
        {
            ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuego2[0], 1);
        }
    }

    public void DesactivarMinijuego()
    {
        Atornillar.singleton.ReiniciarValorSlider();
        ControlCamaraMotor.singleton.IniciarMovimientoCamara(ControlCamaraMotor.singleton.posicionFrontal, 1);
        contador = 0;
        minijuegoActivo = false;
        miniJuegoAtornillar.SetActive(false);
    }

    public void ValidarMiniJuego()
    {
        for (int i = 0; i < torquesTornillosBancada.Length; i++)
        {
            if (torquesTornillosBancada[i] > 49 && torquesTornillosBancada[i] < 61)
            {
                puntaje += 1;
            }
        }

        for (int i = 0; i < torquesTornillosBielas.Length; i++)
        {
            if (torquesTornillosBielas[i] > 59 && torquesTornillosBielas[i] < 71)
            {
                puntaje += 1;
            }
        }

        btnEncenderMotor.gameObject.SetActive(true);
        
        if (puntaje == 4)
        {
            Debug.Log("todo good");
            if (ManagerCanvas.singleton != null)
            {
                ManagerCanvas.singleton.btnReutilizableHabilitado = true;
            }
            sliderVelocidadMotor.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("todo mal");
            btnEncenderMotor.onClick.AddListener(MesaMotor.singleton.DetenerInteraccionesMotor);
        }

        puntaje = 0;
    }

    public void TorqueAplicadoTornillosBancada()
    {
        if (!aplicandoTorque)
        {
            Debug.Log("TorqueAplicadoTornillosBancada");
            aplicandoTorque = true;

            torquesTornillosBancada[contador] = Mathf.RoundToInt(Atornillar.singleton.AsignarValorTorque());
            contador += 1;
            if (contador < 4)
            {
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuego1[contador], 1);
                Atornillar.singleton.ReiniciarValorSlider();
                posicionMonijuegoActual = posicionesMinijuego1[contador];                
            }
            else
            {
                btnAplicarTorque.onClick.RemoveListener(TorqueAplicadoTornillosBancada);
                btnAplicarTorque.onClick.AddListener(TorqueAplicadoTornillosBielas);
                minijuegos[0] = false;
                minijuegos[1] = true;
                aplicandoTorque = false;

                DesactivarMinijuego();
                posicionMonijuegoActual = posicionesMinijuego2[contador];
            }
        }    
    }

    public void TorqueAplicadoTornillosBielas()
    {
        if (!aplicandoTorque)
        {
            Debug.Log("TorqueAplicadoTornillosBielas");
            aplicandoTorque = true;

            torquesTornillosBielas[contador] = Mathf.RoundToInt(Atornillar.singleton.AsignarValorTorque());
            contador += 1;
            if (contador < 4)
            {
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuego2[contador], 1);
                Atornillar.singleton.ReiniciarValorSlider();
                posicionMonijuegoActual = posicionesMinijuego2[contador];
            }
            else
            {     
                DesactivarMinijuego();
                //posicionMonijuegoActual = posicionesMinijuego3[contador];
            }
        }       
    }
}
