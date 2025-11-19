using System.Collections;
using System.Collections.Generic;
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
    public GameObject herramientasRotatorias; // Referencia al objeto dentro de la camara
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public GameObject prensaValvulas; // Referencia al objeto dentro de la camara
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public Button btnAplicarTorque; // Referencia al bt que aplica torque
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public Button btnEncenderMotor; // Boton que enciende el motor despues de colocar la ultima pieza
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public Button btnAplicarAceite; // Boton que se habilita al momento de colocar una pieza aceitada
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public GameObject controlVelocidadMotor; // Slider que controla la velocidad de la animacion del motor activo
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public Slider sliderTorqueMinijuego; // Slider que controla la velocidad de la animacion del motor activo
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public GameObject[] motoresAnimados; // Motores internos animados

    public SueloInteractivo[] sueloInteractivoNissan; // Para activar o desactivar segun el motor activo
    public ActivarDesactivarHijos[] partesNissan; // Para reactivar las partes del motor
    public SueloInteractivo[] sueloInteractivoDiesel; // Para activar o desactivar segun el motor activo
    public ActivarDesactivarHijos[] partesDiesel; // Para reactivar las partes del motor

    [Header("CONFIGURACION INICIAL")]
    public bool[] cantidadMinijuegosMotorDiesel; // Cantidad de minijuegos disponibles
    public bool[] cantidadMinijuegosMotorNissan; // Cantidad de minijuegos disponibles

    [Header("MINIJUEGO 1 MOTOR DIESEL")]
    public Transform[] posicionesMinijuegoBielas; // Posiciones minijuego
    public int[] torquesTornillosBielas; // Guarda el torque aplicado de dicho minijuego

    [Header("MINIJUEGO 2 MOTOR DIESEL")]
    public Transform[] posicionesMinijuegoValvulas; // Posiciones minijuego
    public int[] torquesValvulas; // Guarda el torque aplicado de dicho minijuego

    [Header("MINIJUEGO 3 MOTOR DIESEL")]
    public Transform[] posicionesMinijuegoBombaAgua; // Posiciones minijuego
    public int[] torquesTornillosBombaAgua; // Guarda el torque aplicado de dicho minijuego

    [Header("MINIJUEGO 1 MOTOR NISSAN")]
    public Transform[] posicionesMinijuegoCarterInferior; // Posiciones minijuego
    public int[] torquesTornillosCarterInferior; // Guarda el torque aplicado de dicho minijuego

    [Header("MINIJUEGO 2 MOTOR NISSAN")]
    public Transform[] posicionesMinijuegoBancadasLevas; // Posiciones minijuego
    public int[] torquesTornillosBancadasLevas; // Guarda el torque aplicado de dicho minijuego

    [Header("MINIJUEGOS ACEITES")]
    public Transform[] posicionesMinijuegoAceiteDiesel; // Posiciones minijuego
    public ExpansionRadial piezasInternas;
    public MoverObjeto botellaAceite;
    public ParticleSystem aceite;
    private bool desactivarExternas;

    [HideInInspector]
    public List<ApretarTornillos> tornillosParaApretar;
    [HideInInspector]
    public List<AsignarTornillos> asignarTornillos;
    [HideInInspector]
    public bool minijuegoTerminado; // Para validar en el script de rotacion y de expansion cuando terminan estas
    [HideInInspector]
    public Transform posicionMinijuegoActual;
    [HideInInspector]
    public bool aplicandoTorque;
    [HideInInspector]
    public GameObject motorAnimadoActivo;

    private int contador = 0;
    private int piezaAceitadaActual = 0;
    public int puntaje = 0;
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
        ExplosionObjetosHijos.singleton.DestruirTodosLosHijos(); // Destruimos todas las piezas que se hayan colocado
        btnAplicarTorque.onClick.RemoveAllListeners(); // Removemos todos los listener
        InventarioUI.singleton.LimpiarInventario(); // Limpiamos inventario
        minijuegoActivo = false;
        contador = 0;
        puntaje = 0;

        // Los que involucren tornillos
        if (asignarTornillos.Count > 0)
        {
            asignarTornillos.Clear();
        }

        // Los que involucren tornillos
        if (tornillosParaApretar.Count > 0)
        {
            tornillosParaApretar.Clear();
        }

        if (nombreMotor == "Diesel")
        {
            ManagerCanvas.singleton.ActualizarInformacionPista("Antes de comenzar cualquier armado, asegúrate de tener la base sólida que soportará todo el conjunto interno del motor. Esta pieza es el punto de anclaje donde descansan los componentes principales, y sobre ella se construirá toda la estructura.");

            for (int i = 0; i < sueloInteractivoDiesel.Length; i++)
            {
                //Habilitamos piezas en cuestion
                sueloInteractivoDiesel[i].puedoInteractuarInicialmente = true; 
                partesDiesel[i].ActivarTodosLosHijos();

                //Deshabilitamos lo demas
                sueloInteractivoNissan[i].puedoInteractuarInicialmente = false;          
                sueloInteractivoNissan[i].TrigerExit();                         
            }

            // habilitamos el minijuego cero del Diesel
            for (int i = 0; i < cantidadMinijuegosMotorDiesel.Length; i++)
            {
                cantidadMinijuegosMotorDiesel[i] = false;
            }
            cantidadMinijuegosMotorDiesel[0] = true;
            posicionMinijuegoActual = posicionesMinijuegoBielas[0];
            btnAplicarTorque.onClick.AddListener(TorqueAplicadoTornillosBancada);

            motorAnimadoActivo = motoresAnimados[0]; // Es igual al motor animado Diesel
        
        }
        else if (nombreMotor == "Nissan")
        {
            ManagerCanvas.singleton.ActualizarInformacionPista("Antes de comenzar lo primero es asegurar la base donde descansarán los mecanismos internos. Este componente actúa como recipiente para el aceite y como soporte inferior del bloque, garantizando la lubricación y rigidez estructural del conjunto.");

            for (int i = 0; i < sueloInteractivoNissan.Length; i++)
            {
                //Habilitamos piezas en cuestion
                sueloInteractivoNissan[i].puedoInteractuarInicialmente = true; 
                partesNissan[i].ActivarTodosLosHijos();

                //Deshabilitamos lo demas
                sueloInteractivoDiesel[i].puedoInteractuarInicialmente = false;
                sueloInteractivoDiesel[i].TrigerExit();         
            }

            // habilitamos el minijuego cero del Nissan
            for (int i = 0; i < cantidadMinijuegosMotorNissan.Length; i++)
            {
                cantidadMinijuegosMotorNissan[i] = false;
            }
            cantidadMinijuegosMotorNissan[0] = true;
            posicionMinijuegoActual = posicionesMinijuegoCarterInferior[0];
            btnAplicarTorque.onClick.AddListener(TorqueAplicadoTornillosCarterInferior);

            motorAnimadoActivo = motoresAnimados[1]; // Es igual al motor animado Nissan
        }
        else
        {
            // Sino selecciona ningun motor para armar
            for (int i = 0; i < sueloInteractivoDiesel.Length; i++)
            {
                sueloInteractivoDiesel[i].enabled = false;
                sueloInteractivoNissan[i].enabled = false;
            }
        }
    }

    /// <summary>
    /// Metodo invocado al momento de colocar una pieza que active minijuego con tornillos
    /// </summary>
    /// <param name="asignar"> Script para el manejo de los tornillos</param>
    public void ActivarMinijuego(AsignarTornillos asignar)
    {
        // Se activa minijuego
        minijuegoActivo = true;
        miniJuegoAtornillar.SetActive(true);

        // Activamos herramienta
        herramientasRotatorias.SetActive(true);

        if (InventarioUI.singleton.tamanoHerramienta == 1)
        {
            prensaValvulas.SetActive(true);
        }

        // Configuración de los tornillos
        asignarTornillos.Add(asignar);                 
        asignarTornillos[0].InicializarTornillosMinijuego();
        HabilitarTornilloApretar();

        // Posicionamos
        PosicionInicialCamaraMinijuego();
          
    }

    /// <summary>
    /// Metodo invocado al momento de colocar una pieza que active minijuego sin tornillos
    /// </summary>
    public void ActivarMinijuego()
    {
        // Se activa minijuego
        minijuegoActivo = true;
        miniJuegoAtornillar.SetActive(true);

        // Activamos herramientas
        herramientasRotatorias.SetActive(true);

        if (InventarioUI.singleton.tamanoHerramienta == 1)
        {
            prensaValvulas.SetActive(true);
        }
        
        // Posicionamos
        PosicionInicialCamaraMinijuego();
    }

    /// <summary>
    /// Metodo invocado al momento de colocar una pieza que sea aceitable
    /// </summary>
    /// <param name="numeroPieza"> Numero de la pieza a aceitar </param>
    public void ActivarMinijuegoAceite(int numeroPieza , bool desExternas)
    {
        piezaAceitadaActual = numeroPieza; // Guardamos la pieza aceitable actual
        btnAplicarAceite.gameObject.SetActive(true); // Activamos el boton para aplicar aceite
        desactivarExternas = desExternas;
    }

    /// <summary>
    /// Metodo invocado desde btnAceitar en informacion de motor en el canvas principal
    /// </summary>
    /// <param name="numeroPieza"></param>
    public void AplicarAceite()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(AplicarAceiteCorrutine());
    }

    IEnumerator AplicarAceiteCorrutine()
    {
        btnAplicarAceite.gameObject.SetActive(false); // Desactivamos el boton para aplicar aceite
        puntaje += 1; // Damos un punto por aplicar aceite

        piezasInternas.Contraer();// Contraemos las piezas internas si estan expandidas

        if (desactivarExternas)
        {
            ExplosionObjetosHijos.singleton.DesactivarHijos(ExplosionObjetosHijos.singleton.objetosPadres[0]); // Desactivamos las piezas externas mientras aplicamos aceite
            ExplosionObjetosHijos.singleton.DesactivarHijos(ExplosionObjetosHijos.singleton.objetosPadres[2]); // Desactivamos las piezas externas mientras aplicamos aceite
        }

        ControlCamaraMotor.singleton.noMover = true; // Indicamos que no podemos mover la camara
        ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuegoAceiteDiesel[piezaAceitadaActual], 1);
        ControlCamaraMotor.singleton.ReestablecerPosicionCamara(); // Reiniciamos el indice para que la posicion de la camara sea correcta

        //Desactivamos momentaneamente los botones que no necesitamos
        ManagerCanvas.singleton.DeshabilitarBtnSalir(); 
        ManagerCanvas.singleton.DeshabilitarBtnBajarPlataforma();
        ManagerCanvas.singleton.DeshabilitarBtnExpandir();

        yield return new WaitForSeconds(1f);

        // Activamos la botella de aceite y la rotamos
        botellaAceite.gameObject.SetActive(true);
        botellaAceite.IniciarDesplazamientoObjeto();

        yield return new WaitForSeconds(0.5f);
        aceite.Play(); // Aplicamos aceite en particulas

        yield return new WaitForSeconds(1f);

        botellaAceite.RetornarPosicionOriginal(); // Regresamos a la posicion original

        yield return new WaitForSeconds(1f);

        botellaAceite.gameObject.SetActive(false);
        ControlCamaraMotor.singleton.noMover = false;
        ControlCamaraMotor.singleton.IniciarMovimientoCamara(ControlCamaraMotor.singleton.posicionFrontal, 1);

        //Activamos nuevamente los botones de salir 
        ManagerCanvas.singleton.HabilitarBtnSalir();
        ManagerCanvas.singleton.HabilitarBtnBajarPlataforma();
        ManagerCanvas.singleton.HabilitarBtnExpandir();

        if (desactivarExternas)
        {
            ExplosionObjetosHijos.singleton.ActivarHijos(ExplosionObjetosHijos.singleton.objetosPadres[0]); // Desactivamos las piezas externas mientras aplicamos aceite
            ExplosionObjetosHijos.singleton.ActivarHijos(ExplosionObjetosHijos.singleton.objetosPadres[2]); // Desactivamos las piezas externas mientras aplicamos aceite
        }
    }

    public void DesactivarMinijuego()
    {
        Atornillar.singleton.ReiniciarValorSlider();
        ControlCamaraMotor.singleton.ReestablecerPosicionCamara(); // Reiniciamos el indice para que la posicion de la camara sea correcta
        ControlCamaraMotor.singleton.IniciarMovimientoCamara(ControlCamaraMotor.singleton.posicionFrontal, 1);
        contador = 0;
        aplicandoTorque = false;
        minijuegoActivo = false;
        miniJuegoAtornillar.SetActive(false);

        if (InventarioUI.singleton.tamanoHerramienta == 1)
        {
            prensaValvulas.SetActive(false);
        }
        else
        {
            herramientasRotatorias.SetActive(false);
        }       
    }

    public void PosicionInicialCamaraMinijuego()
    {
        if (motorActivo == "Diesel")
        {
            if (cantidadMinijuegosMotorDiesel[0])
            {
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuegoBielas[0], 1);
            }
            else if (cantidadMinijuegosMotorDiesel[1])
            {
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuegoValvulas[0], 1);
            }
            else if (cantidadMinijuegosMotorDiesel[2])
            {
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuegoBombaAgua[0], 1);
            }
        }
        else if (motorActivo == "Nissan")
        {
            if (cantidadMinijuegosMotorNissan[0])
            {
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuegoCarterInferior[0], 1);
            }
        }   
    }

    public void ConfigurarTornilloActivo()
    {
        aplicandoTorque = true;
        tornillosParaApretar[0].DeshabilitarSlider(sliderTorqueMinijuego);
        tornillosParaApretar[0].QuitarMaterial();
        tornillosParaApretar.RemoveAt(0);
        HabilitarTornilloApretar();
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
        minijuegoTerminado = true; // indicamos que ya terminaron los minijuegos

        for (int i = 0; i < torquesTornillosBielas.Length; i++)
        {
            if (torquesTornillosBielas[i] >= 88 && torquesTornillosBielas[i] <= 95) // los torques Entre 88 y 95
            {
                puntaje += 1;
                Debug.Log(puntaje);
            }
        }

        for (int i = 0; i < torquesValvulas.Length; i++)
        {
            if (torquesValvulas[i] >= 70 && torquesValvulas[i] <= 80) // los torques Entre 70 y 80
            {
                puntaje += 1;
                Debug.Log(puntaje);
            }
        }

        for (int i = 0; i < torquesTornillosBombaAgua.Length; i++)
        {
            if (torquesTornillosBombaAgua[i] >= 52 && torquesTornillosBombaAgua[i] <= 58) // los torques Entre 52 y 58
            {
                puntaje += 1;
                Debug.Log(puntaje);
            }
        }

        btnEncenderMotor.gameObject.SetActive(true);
        
        puntaje = 8;
        Debug.Log("Puntaje " + puntaje);
        if (puntaje == 8)
        {
            Debug.Log("todo good");
            if (ManagerCanvas.singleton != null)
            {
                ManagerCanvas.singleton.btnReutilizableHabilitado = true;
            }

            controlVelocidadMotor.gameObject.SetActive(true);
            motorAnimadoActivo.SetActive(true);

            // Desactivamos piezas internas
            ExplosionObjetosHijos.singleton.DesactivarHijos(ExplosionObjetosHijos.singleton.objetosPadres[1]); 
            ExplosionObjetosHijos.singleton.DesactivarHijos(ExplosionObjetosHijos.singleton.objetosPadres[3]);
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
            ConfigurarTornilloActivo(); // Los que involucren tornillos

            torquesTornillosBielas[contador] = Mathf.RoundToInt(Atornillar.singleton.AsignarValorTorque()); // Asignamos el valor del torque

            yield return new WaitForSeconds(0.1f);
        
            contador += 1;
            if (contador < 4)
            {
                // Nos movemos a la siguiente posicion del minijuego
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuegoBielas[contador], 1);
                Atornillar.singleton.ReiniciarValorSlider();
                posicionMinijuegoActual = posicionesMinijuegoBielas[contador];
            }
            else
            {
                // Los que involucren tornillos
                if (asignarTornillos.Count > 0)
                {
                    asignarTornillos.RemoveAt(0);
                }

                // Reestablecemos valores minijuego
                btnAplicarTorque.onClick.RemoveListener(TorqueAplicadoTornillosBancada);
                btnAplicarTorque.onClick.AddListener(TorqueAplicadoValvulas);
                cantidadMinijuegosMotorDiesel[0] = false;
                cantidadMinijuegosMotorDiesel[1] = true;
                DesactivarMinijuego();
                posicionMinijuegoActual = posicionesMinijuegoValvulas[contador];
            }
        }       
    }
    

    public void TorqueAplicadoValvulas()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(TorqueAplicadoValvulasCorrutina());
    }

    IEnumerator TorqueAplicadoValvulasCorrutina()
    {
        if (!aplicandoTorque)
        {
            Debug.Log("TorqueAplicadoTornillosValvulas");
            torquesValvulas[contador] = Mathf.RoundToInt(Atornillar.singleton.AsignarValorTorque()); // Asignamos el valor del torque

            yield return new WaitForSeconds(0.1f);
        
            contador += 1;
            if (contador < 4)
            {
                // Nos movemos a la siguiente posicion del minijuego
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuegoValvulas[contador], 1);
                Atornillar.singleton.ReiniciarValorSlider();
                posicionMinijuegoActual = posicionesMinijuegoValvulas[contador];
            }
            else
            {
                // Reestablecemos valores minijuego
                btnAplicarTorque.onClick.RemoveListener(TorqueAplicadoValvulas);
                btnAplicarTorque.onClick.AddListener(TorqueAplicadoTornillosBombaAgua);
                cantidadMinijuegosMotorDiesel[1] = false;
                cantidadMinijuegosMotorDiesel[2] = true;
                DesactivarMinijuego();
                posicionMinijuegoActual = posicionesMinijuegoBombaAgua[contador];
            }
        }
    }

    public void TorqueAplicadoTornillosBombaAgua()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(TorqueAplicadoTornillosBombaAguaCorrutina());
    }

    IEnumerator TorqueAplicadoTornillosBombaAguaCorrutina()
    {
        if (!aplicandoTorque)
        {
            ConfigurarTornilloActivo(); // Los que involucren tornillos

            torquesTornillosBombaAgua[contador] = Mathf.RoundToInt(Atornillar.singleton.AsignarValorTorque()); // Asignamos el valor del torque

            yield return new WaitForSeconds(0.1f);

            contador += 1;
            if (contador < 4)
            {
                // Nos movemos a la siguiente posicion del minijuego
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuegoBombaAgua[contador], 1);
                Atornillar.singleton.ReiniciarValorSlider();
                posicionMinijuegoActual = posicionesMinijuegoBombaAgua[contador];
            }
            else
            {
                // Los que involucren tornillos
                if (asignarTornillos.Count > 0)
                {
                    asignarTornillos.RemoveAt(0);
                }

                // Reestablecemos valores minijuego
                btnAplicarTorque.onClick.RemoveListener(TorqueAplicadoTornillosBombaAgua);
                cantidadMinijuegosMotorDiesel[2] = false;
                DesactivarMinijuego();
            }
        }
    }


    // Minijuegos Motor NISSAN

    public void TorqueAplicadoTornillosCarterInferior()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(TorqueAplicadoTornillosCarterInferiorCorrutina());
    }

    IEnumerator TorqueAplicadoTornillosCarterInferiorCorrutina()
    {
        if (!aplicandoTorque)
        {
            ConfigurarTornilloActivo(); // Los que involucren tornillos

            torquesTornillosCarterInferior[contador] = Mathf.RoundToInt(Atornillar.singleton.AsignarValorTorque()); // Asignamos el valor del torque

            yield return new WaitForSeconds(0.1f);

            contador += 1;
            if (contador < 4)
            {
                // Nos movemos a la siguiente posicion del minijuego
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuegoCarterInferior[contador], 1);
                Atornillar.singleton.ReiniciarValorSlider();
                posicionMinijuegoActual = posicionesMinijuegoCarterInferior[contador];
            }
            else
            {
                // Los que involucren tornillos
                if (asignarTornillos.Count > 0)
                {
                    asignarTornillos.RemoveAt(0);
                }

                // Reestablecemos valores minijuego
                btnAplicarTorque.onClick.RemoveListener(TorqueAplicadoTornillosCarterInferior);
                btnAplicarTorque.onClick.AddListener(TorqueAplicadoTornillosBancadaLevas);
                cantidadMinijuegosMotorNissan[0] = false;
                cantidadMinijuegosMotorNissan[1] = true;
                DesactivarMinijuego();
                posicionMinijuegoActual = posicionesMinijuegoBancadasLevas[contador];
            }
        }
    }


    public void TorqueAplicadoTornillosBancadaLevas()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(TorqueAplicadoTornillosBancadaLevasCorrutina());
    }

    IEnumerator TorqueAplicadoTornillosBancadaLevasCorrutina()
    {
        if (!aplicandoTorque)
        {
            ConfigurarTornilloActivo(); // Los que involucren tornillos

            torquesTornillosCarterInferior[contador] = Mathf.RoundToInt(Atornillar.singleton.AsignarValorTorque()); // Asignamos el valor del torque

            yield return new WaitForSeconds(0.1f);

            contador += 1;
            if (contador < 4)
            {
                // Nos movemos a la siguiente posicion del minijuego
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuegoCarterInferior[contador], 1);
                Atornillar.singleton.ReiniciarValorSlider();
                posicionMinijuegoActual = posicionesMinijuegoCarterInferior[contador];
            }
            else
            {
                // Los que involucren tornillos
                if (asignarTornillos.Count > 0)
                {
                    asignarTornillos.RemoveAt(0);
                }

                // Reestablecemos valores minijuego
                //btnAplicarTorque.onClick.RemoveListener(TorqueAplicadoTornillosBancadaLevas);
                //btnAplicarTorque.onClick.AddListener();
                //cantidadMinijuegosMotorNissan[1] = false;
                //cantidadMinijuegosMotorNissan[2] = true;
                //DesactivarMinijuego();
                //posicionMinijuegoActual = posicionesMinijuegoValvulas[contador];
            }
        }
    }
}