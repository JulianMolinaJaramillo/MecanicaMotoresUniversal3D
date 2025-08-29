using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ManagerMinijuego : MonoBehaviour
{
    public bool minijuegoActivo; // Para validar si hay un minijuego activo
    public int sizeHerramienta; // Tamaño de herramienta tomada
    public string motorActivo; // Para controlar el motor activo
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public GameObject miniJuegoAtornillar; // Referencia al objeto de minujuegoTorque del canvas
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public GameObject herramientasRotatorias; // Referencia al objeto de minujuegoTorque del canvas
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public Button btnAplicarTorque; // Referencia al bt que aplica torque
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public Button btnEncenderMotor; // Boton que enciende el motor despues de colocar la ultima pieza
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public GameObject sliderVelocidadMotor; // Slider que controla la velocidad de la animacion del motor activo
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public Slider sliderTorqueMinijuego; // Slider que controla la velocidad de la animacion del motor activo
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public GameObject[] motoresAnimados; // Motores internos animados

    public Collider[] sueloInteractivoNissan; // Para activar o desactivar segun el motor activo
    public Collider[] sueloInteractivoDiesel; // Para activar o desactivar segun el motor activo

    public Transform[] posicionesMinijuego1; // Posiciones minijuego
    public int[] torquesTornillosBancada; // Guarda el torque aplicado de dicho minijuego

    public Transform[] posicionesMinijuego2; // Posiciones minijuego
    public int[] torquesTornillosBielas; // Guarda el torque aplicado de dicho minijuego

    public bool[] minijuegos;

    public List<ApretarTornillos> tornillosParaApretar;
    public List<AsignarTornillos> asignarTornillos;

    [HideInInspector]
    public Transform posicionMonijuegoActual;
    [HideInInspector]
    public bool aplicandoTorque;
    [HideInInspector]
    public GameObject motorAnimadoActivo;

    private int contador = 0;
    private int puntaje = 0;
    private Coroutine coroutine;
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
        if (nombreMotor == "Diesel")
        {
            for (int i = 0; i < sueloInteractivoDiesel.Length; i++)
            {
                sueloInteractivoDiesel[i].enabled = true;
                sueloInteractivoNissan[i].enabled = false;
                motorAnimadoActivo = motoresAnimados[0]; // Es igual al motor animado Diesel
            }
        }
        else if (nombreMotor == "Nissan")
        {
            for (int i = 0; i < sueloInteractivoNissan.Length; i++)
            {
                sueloInteractivoNissan[i].enabled = true;
                sueloInteractivoDiesel[i].enabled = false;
                motorAnimadoActivo = motoresAnimados[1]; // Es igual al motor animado Nissan
            }
        }
        else
        {
            
        }
    }

    public void ActivarMinijuego(AsignarTornillos asignar)
    {
        asignarTornillos.Add(asignar);
        minijuegoActivo = true;
        miniJuegoAtornillar.SetActive(true);
        herramientasRotatorias.SetActive(true);
        asignarTornillos[0].InicializarTornillosMinijuego();
        PosicionInicialCamaraMinijuego();
        HabilitarTornilloApretar();   
    }

    public void DesactivarMinijuego()
    {
        Atornillar.singleton.ReiniciarValorSlider();
        ControlCamaraMotor.singleton.IniciarMovimientoCamara(ControlCamaraMotor.singleton.posicionFrontal, 1);
        contador = 0;
        minijuegoActivo = false;
        miniJuegoAtornillar.SetActive(false);
        herramientasRotatorias.SetActive(false);
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

    public void HabilitarTornilloApretar()
    {
        if (tornillosParaApretar.Count > 0)
        {
            tornillosParaApretar[0].HabilitarSlider(sliderTorqueMinijuego);
        }    
    }

    

    public void ValidarMiniJuego()
    {
        for (int i = 0; i < torquesTornillosBancada.Length; i++)
        {
            if (torquesTornillosBancada[i] > 87 && torquesTornillosBancada[i] < 96)
            {
                puntaje += 1;
                Debug.Log(puntaje);
            }
        }

        for (int i = 0; i < torquesTornillosBielas.Length; i++)
        {
            if (torquesTornillosBielas[i] > 51 && torquesTornillosBielas[i] < 59)
            {
                puntaje += 1;
                Debug.Log(puntaje);
            }
        }

        btnEncenderMotor.gameObject.SetActive(true);
        Debug.Log(puntaje);

        if (puntaje == 8)
        {
            Debug.Log("todo good");
            if (ManagerCanvas.singleton != null)
            {
                ManagerCanvas.singleton.btnReutilizableHabilitado = true;
            }
            sliderVelocidadMotor.gameObject.SetActive(true);
            motorAnimadoActivo.SetActive(true);
            ExplosionObjetosHijos.singleton.DestruirHijos(ExplosionObjetosHijos.singleton.objetosPadres[1]);
            ExplosionObjetosHijos.singleton.DestruirHijos(ExplosionObjetosHijos.singleton.objetosPadres[3]);
        }
        else
        {
            Debug.Log("todo mal");
            btnEncenderMotor.onClick.AddListener(MesaMotor.singleton.DetenerInteraccionesMotor);
        }

        puntaje = 0;
    }

    public void ConfigurarTornilloActivo()
    {
        aplicandoTorque = true;
        tornillosParaApretar[0].DeshabilitarSlider(sliderTorqueMinijuego);
        tornillosParaApretar[0].QuitarMaterial();
        tornillosParaApretar.RemoveAt(0);
        HabilitarTornilloApretar();
    }

    public void TorqueAplicadoTornillosBancada()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(TorqueAplicadoTornillosBancadaCorrutina());
    }

    IEnumerator TorqueAplicadoTornillosBancadaCorrutina()
    {
        if (!aplicandoTorque)
        {
            Debug.Log("TorqueAplicadoTornillosBancada");         
            ConfigurarTornilloActivo();
            torquesTornillosBancada[contador] = Mathf.RoundToInt(Atornillar.singleton.AsignarValorTorque());

            yield return new WaitForSeconds(0.1f);
        
            contador += 1;
            if (contador < 4)
            {
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuego1[contador], 1);
                Atornillar.singleton.ReiniciarValorSlider();
                posicionMonijuegoActual = posicionesMinijuego1[contador];
            }
            else
            {
                if (asignarTornillos.Count > 0)
                {
                    asignarTornillos.RemoveAt(0);
                }

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
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(TorqueAplicadoTornillosBielasCorrutina());
    }

    IEnumerator TorqueAplicadoTornillosBielasCorrutina()
    {
        if (!aplicandoTorque)
        {
            Debug.Log("TorqueAplicadoTornillosBielas");
            ConfigurarTornilloActivo();
            torquesTornillosBielas[contador] = Mathf.RoundToInt(Atornillar.singleton.AsignarValorTorque());

            yield return new WaitForSeconds(0.1f);
        
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
