using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManagerMinijuego : MonoBehaviour
{
    public bool minijuegoActivo; // Para validar si hay un minijuego activo
    public int sizeHerramienta; // Tamaño de herramienta tomada
    public string motorActivo; // Para controlar el motor activo

    [Header(" REFERENCIAS ")]
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public GameObject infoTorquesDiesel; // Referencia al objeto de minujuegoTorque del canvas
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public GameObject infoTorquesNissan; // Referencia al objeto de minujuegoTorque del canvas
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public TextMeshProUGUI txtTorques; // Referencia al objeto de minujuegoTorque del canvas
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public TextMeshProUGUI txtTorques2; // Referencia al objeto de minujuegoTorque del canvas
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
    public Button btnDesplazarMotor; // Boton que enciende el motor despues de colocar la ultima pieza
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
    public int[] torquesDieselBielas; // Guarda el torque aplicado de dicho minijuego

    [Header("MINIJUEGO 2 MOTOR DIESEL")]
    public Transform[] posicionesMinijuegoValvulas; // Posiciones minijuego
    public int[] torquesDieselValvulas; // Guarda el torque aplicado de dicho minijuego

    [Header("MINIJUEGO 3 MOTOR DIESEL")]
    public Transform[] posicionesMinijuegoBombaAgua; // Posiciones minijuego
    public int[] torquesDieselBombaAgua; // Guarda el torque aplicado de dicho minijuego

    [Header("MINIJUEGO 1 MOTOR NISSAN")]
    public Transform[] posicionesMinijuegoCarterInferior; // Posiciones minijuego
    public int[] torquesNissanCarterInferior; // Guarda el torque aplicado de dicho minijuego

    [Header("MINIJUEGO 2 MOTOR NISSAN")]
    public Transform[] posicionesMinijuegoBancadasCiguenal; // Posiciones minijuego
    public int[] torquesNissanBancadasCiguenal; // Guarda el torque aplicado de dicho minijuego

    [Header("MINIJUEGO 3 MOTOR NISSAN")]
    public Transform[] posicionesMinijuegoBloque; // Posiciones minijuego
    public int[] torquesNissanBloque; // Guarda el torque aplicado de dicho minijuego

    [Header("MINIJUEGO 4 MOTOR NISSAN")]
    public Transform[] posicionesMinijuegoEmpaqueCulata; // Posiciones minijuego
    public int[] torquesNissanEmpaqueCulata; // Guarda el torque aplicado de dicho minijuego

    [Header("MINIJUEGO 5 MOTOR NISSAN")]
    public Transform[] posicionesMinijuegoNissanValvulas; // Posiciones minijuego
    public int[] torquesNissanValvulas; // Guarda el torque aplicado de dicho minijuego

    [Header("MINIJUEGO 6 MOTOR NISSAN")]
    public Transform[] posicionesMinijuegoBancadaLevas; // Posiciones minijuego
    public int[] torquesNissanBancadaLevas; // Guarda el torque aplicado de dicho minijuego

    [Header("MINIJUEGO 7 MOTOR NISSAN")]
    public Transform[] posicionesMinijuegoTapaCulata; // Posiciones minijuego
    public int[] torquesNissanTapaCulata; // Guarda el torque aplicado de dicho minijuego

    [Header("MINIJUEGOS ACEITES")]
    public Transform[] posicionesMinijuegoAceiteDiesel; // Posiciones minijuego
    public Transform[] posicionesMinijuegoAceiteNissan; // Posiciones minijuego
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
    public bool minijuegoValidadoCorrectamente;
    [HideInInspector]
    public bool minijuegoValidadoAceiteMal;
    [HideInInspector]
    public Transform posicionMinijuegoActual;
    [HideInInspector]
    public bool aplicandoTorque;
    [HideInInspector]
    public GameObject motorAnimadoActivo;

    private int contador = 0;
    private int piezaAceitadaActual = 0;
    public int puntajeTorque = 0;
    public int puntajeAceite = 0;
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
        controlVelocidadMotor.gameObject.SetActive(false);
        btnEncenderMotor.gameObject.SetActive(true);
        btnDesplazarMotor.gameObject.SetActive(false);
        if (motorAnimadoActivo != null) motorAnimadoActivo.SetActive(false);


        MesaMotor.singleton.interaccionEjecutada = false;
        ExplosionObjetosHijos.singleton.DestruirTodosLosHijos(); // Destruimos todas las piezas que se hayan colocado
        ExplosionObjetosHijos.singleton.vibrarHasta = false;
        MesaMotor.singleton.DesactivarHumo();
        ManagerDesplazamientoMotor.singleton.ReinicioMotores();

        btnAplicarTorque.onClick.RemoveAllListeners(); // Removemos todos los listener
        btnEncenderMotor.onClick.RemoveAllListeners(); // Removemos todos los listener

        minijuegoValidadoCorrectamente = false;
        minijuegoValidadoAceiteMal = false;
        minijuegoTerminado = false;

        ManagerCanvas.singleton.HabilitarBtnExpandir();
        ManagerCanvas.singleton.HabilitarBtnRotar();

        InventarioUI.singleton.LimpiarInventario(); // Limpiamos inventario
        minijuegoActivo = false;
        contador = 0;
        puntajeTorque = 0;
        puntajeAceite = 0;

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

            // Actualizamos el panel de torques
            txtTorques.text = "Información Torques \r\n Motor Diesel";
            txtTorques2.text = "Prueba Motor Diesel";
            infoTorquesDiesel.SetActive(true);
            infoTorquesNissan.SetActive(false);

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

            // Actualizamos el panel de torques
            txtTorques.text = "Información Torques \r\n Motor Nissan";
            txtTorques2.text = "Prueba Motor Nissan";
            infoTorquesDiesel.SetActive(false);
            infoTorquesNissan.SetActive(true);

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
            btnAplicarTorque.onClick.AddListener(TorqueNissanCarterInferior);

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
        puntajeAceite += 1; // Damos un punto por aplicar aceite

        piezasInternas.Contraer();// Contraemos las piezas internas si estan expandidas

        if (desactivarExternas)
        {
            ExplosionObjetosHijos.singleton.DesactivarHijos(ExplosionObjetosHijos.singleton.objetosPadres[0]); // Desactivamos las piezas externas mientras aplicamos aceite
            ExplosionObjetosHijos.singleton.DesactivarHijos(ExplosionObjetosHijos.singleton.objetosPadres[2]); // Desactivamos las piezas externas mientras aplicamos aceite
        }

        ControlCamaraMotor.singleton.noMover = true; // Indicamos que no podemos mover la camara

        if (motorActivo == "Diesel")
        {
            ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuegoAceiteDiesel[piezaAceitadaActual], 1);
        }
        else if (motorActivo == "Nissan")
        {
            ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuegoAceiteNissan[piezaAceitadaActual], 1);
        }
        
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
            else if (cantidadMinijuegosMotorNissan[1])
            {
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuegoBancadasCiguenal[0], 1);
            }
            else if (cantidadMinijuegosMotorNissan[2])
            {
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuegoBloque[0], 1);
            }
            else if (cantidadMinijuegosMotorNissan[3])
            {
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuegoEmpaqueCulata[0], 1);
            }
            else if (cantidadMinijuegosMotorNissan[4])
            {
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuegoNissanValvulas[0], 1);
            }
            else if (cantidadMinijuegosMotorNissan[5])
            {
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuegoBancadaLevas[0], 1);
            }
            else if (cantidadMinijuegosMotorNissan[6])
            {
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuegoTapaCulata[0], 1);
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

        // VALIDACION: Motor Diesel
        for (int i = 0; i < torquesDieselBielas.Length; i++)
        {
            if (torquesDieselBielas[i] >= 88 && torquesDieselBielas[i] <= 95) // los torques Entre 88 y 95
            {
                puntajeTorque += 1;
            }
        }

        for (int i = 0; i < torquesDieselValvulas.Length; i++)
        {
            if (torquesDieselValvulas[i] >= 70 && torquesDieselValvulas[i] <= 80) // los torques Entre 70 y 80
            {
                puntajeTorque += 1;
            }
        }

        for (int i = 0; i < torquesDieselBombaAgua.Length; i++)
        {
            if (torquesDieselBombaAgua[i] >= 52 && torquesDieselBombaAgua[i] <= 58) // los torques Entre 52 y 58
            {
                puntajeTorque += 1;
            }
        }


        // VALIDACION: Motor Nissan
        for (int i = 0; i < torquesNissanCarterInferior.Length; i++)
        {
            if (torquesNissanCarterInferior[i] >= 65 && torquesNissanCarterInferior[i] <= 70) 
            {
                puntajeTorque += 1;
            }
        }

        for (int i = 0; i < torquesNissanBancadasCiguenal.Length; i++)
        {
            if (torquesNissanBancadasCiguenal[i] >= 52 && torquesNissanBancadasCiguenal[i] <= 57) 
            {
                puntajeTorque += 1;
            }
        }

        for (int i = 0; i < torquesNissanBloque.Length; i++)
        {
            if (torquesNissanBloque[i] >= 42 && torquesNissanBloque[i] <= 47) 
            {
                puntajeTorque += 1;
            }
        }

        for (int i = 0; i < torquesNissanEmpaqueCulata.Length; i++)
        {
            if (torquesNissanEmpaqueCulata[i] >= 88 && torquesNissanEmpaqueCulata[i] <= 93) 
            {
                puntajeTorque += 1;
            }
        }

        for (int i = 0; i < torquesNissanValvulas.Length; i++)
        {
            if (torquesNissanValvulas[i] >= 50 && torquesNissanValvulas[i] <= 60) 
            {
                puntajeTorque += 1;
            }
        }

        for (int i = 0; i < torquesNissanBancadaLevas.Length; i++)
        {
            if (torquesNissanBancadaLevas[i] >= 73 && torquesNissanBancadaLevas[i] <= 78) 
            {
                puntajeTorque += 1;
            }
        }

        for (int i = 0; i < torquesNissanTapaCulata.Length; i++)
        {
            if (torquesNissanTapaCulata[i] >= 28 && torquesNissanTapaCulata[i] <= 33)
            {
                puntajeTorque += 1;
            }
        }


        Debug.Log(puntajeTorque);

        // RESULTADO

        if (motorActivo == "Diesel")
        {
            if (puntajeTorque == 12 && puntajeAceite == 13)
            {
                Debug.Log("todo good");
                MinijuegosSuperados();
            }
            else if(puntajeTorque == 12 && puntajeAceite < 13)
            {
                Debug.Log("aceite mal");
                btnEncenderMotor.onClick.AddListener(MesaMotor.singleton.ActivarParticulasHumo);
                minijuegoValidadoAceiteMal = true;
            }
            else if (puntajeTorque < 12 && puntajeAceite == 13)
            {
                Debug.Log("torque mal");
                btnEncenderMotor.onClick.AddListener(MesaMotor.singleton.DetenerInteraccionesMotor);
            }
            else
            {
                Debug.Log("todo mal");
                btnEncenderMotor.onClick.AddListener(MesaMotor.singleton.DetenerInteraccionesMotor);
                btnEncenderMotor.onClick.AddListener(MesaMotor.singleton.ActivarParticulasHumo);
            }
            controlVelocidadMotor.gameObject.SetActive(true);
        }
        else if (motorActivo == "Nissan")
        {
            if (puntajeTorque == 28 && puntajeAceite == 8)
            {
                Debug.Log("todo good");
                MinijuegosSuperados();
            }
            else if (puntajeTorque == 28 && puntajeAceite < 8)
            {
                Debug.Log("aceite mal");
                btnEncenderMotor.onClick.AddListener(MesaMotor.singleton.ActivarParticulasHumo);
                minijuegoValidadoAceiteMal = true;
            }
            else if (puntajeTorque < 28 && puntajeAceite == 8)
            {
                Debug.Log("torque mal");
                btnEncenderMotor.onClick.AddListener(MesaMotor.singleton.DetenerInteraccionesMotor);
            }
            else
            {
                Debug.Log("todo mal");
                btnEncenderMotor.onClick.AddListener(MesaMotor.singleton.DetenerInteraccionesMotor);
                btnEncenderMotor.onClick.AddListener(MesaMotor.singleton.ActivarParticulasHumo);
            }
            controlVelocidadMotor.gameObject.SetActive(true);
        }       
    }

    public void MinijuegosSuperados()
    {
        minijuegoValidadoCorrectamente = true;

        btnDesplazarMotor.gameObject.SetActive(true);
        motorAnimadoActivo.SetActive(true);

        // Desactivamos piezas internas
        ExplosionObjetosHijos.singleton.DesactivarHijos(ExplosionObjetosHijos.singleton.objetosPadres[1]);
        ExplosionObjetosHijos.singleton.DesactivarHijos(ExplosionObjetosHijos.singleton.objetosPadres[3]);
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

            torquesDieselBielas[contador] = Mathf.RoundToInt(Atornillar.singleton.AsignarValorTorque()); // Asignamos el valor del torque

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
            torquesDieselValvulas[contador] = Mathf.RoundToInt(Atornillar.singleton.AsignarValorTorque()); // Asignamos el valor del torque

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

            torquesDieselBombaAgua[contador] = Mathf.RoundToInt(Atornillar.singleton.AsignarValorTorque()); // Asignamos el valor del torque

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

    public void TorqueNissanCarterInferior()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(TorqueNissanCarterInferiorCorrutina());
    }

    IEnumerator TorqueNissanCarterInferiorCorrutina()
    {
        if (!aplicandoTorque)
        {
            ConfigurarTornilloActivo(); // Los que involucren tornillos

            torquesNissanCarterInferior[contador] = Mathf.RoundToInt(Atornillar.singleton.AsignarValorTorque()); // Asignamos el valor del torque

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
                btnAplicarTorque.onClick.RemoveListener(TorqueNissanCarterInferior);
                btnAplicarTorque.onClick.AddListener(TorqueNissanBancadaCiguenal);
                cantidadMinijuegosMotorNissan[0] = false;
                cantidadMinijuegosMotorNissan[1] = true;
                DesactivarMinijuego();
                posicionMinijuegoActual = posicionesMinijuegoBancadasCiguenal[contador];
            }
        }
    }


    public void TorqueNissanBancadaCiguenal()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(TorqueNissanBancadaCiguenalCorrutina());
    }

    IEnumerator TorqueNissanBancadaCiguenalCorrutina()
    {
        if (!aplicandoTorque)
        {
            ConfigurarTornilloActivo(); // Los que involucren tornillos

            torquesNissanBancadasCiguenal[contador] = Mathf.RoundToInt(Atornillar.singleton.AsignarValorTorque()); // Asignamos el valor del torque

            yield return new WaitForSeconds(0.1f);

            contador += 1;
            if (contador < 4)
            {
                // Nos movemos a la siguiente posicion del minijuego
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuegoBancadasCiguenal[contador], 1);
                Atornillar.singleton.ReiniciarValorSlider();
                posicionMinijuegoActual = posicionesMinijuegoBancadasCiguenal[contador];
            }
            else
            {
                // Los que involucren tornillos
                if (asignarTornillos.Count > 0)
                {
                    asignarTornillos.RemoveAt(0);
                }

                // Reestablecemos valores minijuego
                btnAplicarTorque.onClick.RemoveListener(TorqueNissanBancadaCiguenal);
                btnAplicarTorque.onClick.AddListener(TorqueNissanBloque);
                cantidadMinijuegosMotorNissan[1] = false;
                cantidadMinijuegosMotorNissan[2] = true;
                DesactivarMinijuego();
                posicionMinijuegoActual = posicionesMinijuegoBloque[contador];
            }
        }
    }

    public void TorqueNissanBloque()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(TorqueNissanBloqueCorrutina());
    }

    IEnumerator TorqueNissanBloqueCorrutina()
    {
        if (!aplicandoTorque)
        {
            ConfigurarTornilloActivo(); // Los que involucren tornillos

            torquesNissanBloque[contador] = Mathf.RoundToInt(Atornillar.singleton.AsignarValorTorque()); // Asignamos el valor del torque

            yield return new WaitForSeconds(0.1f);

            contador += 1;
            if (contador < 4)
            {
                // Nos movemos a la siguiente posicion del minijuego
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuegoBloque[contador], 1);
                Atornillar.singleton.ReiniciarValorSlider();
                posicionMinijuegoActual = posicionesMinijuegoBloque[contador];
            }
            else
            {
                // Los que involucren tornillos
                if (asignarTornillos.Count > 0)
                {
                    asignarTornillos.RemoveAt(0);
                }

                // Reestablecemos valores minijuego
                btnAplicarTorque.onClick.RemoveListener(TorqueNissanBloque);
                btnAplicarTorque.onClick.AddListener(TorqueNissanEmpaqueCulata);
                cantidadMinijuegosMotorNissan[2] = false;
                cantidadMinijuegosMotorNissan[3] = true;
                DesactivarMinijuego();
                posicionMinijuegoActual = posicionesMinijuegoEmpaqueCulata[contador];
            }
        }
    }

    public void TorqueNissanEmpaqueCulata()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(TorqueNissanEmpaqueCulataCorrutina());
    }

    IEnumerator TorqueNissanEmpaqueCulataCorrutina()
    {
        if (!aplicandoTorque)
        {
            ConfigurarTornilloActivo(); // Los que involucren tornillos

            torquesNissanEmpaqueCulata[contador] = Mathf.RoundToInt(Atornillar.singleton.AsignarValorTorque()); // Asignamos el valor del torque

            yield return new WaitForSeconds(0.1f);

            contador += 1;
            if (contador < 4)
            {
                // Nos movemos a la siguiente posicion del minijuego
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuegoEmpaqueCulata[contador], 1);
                Atornillar.singleton.ReiniciarValorSlider();
                posicionMinijuegoActual = posicionesMinijuegoEmpaqueCulata[contador];
            }
            else
            {
                // Los que involucren tornillos
                if (asignarTornillos.Count > 0)
                {
                    asignarTornillos.RemoveAt(0);
                }

                // Reestablecemos valores minijuego
                btnAplicarTorque.onClick.RemoveListener(TorqueNissanEmpaqueCulata);
                btnAplicarTorque.onClick.AddListener(TorqueAplicadoValvulasNissan);
                cantidadMinijuegosMotorNissan[3] = false;
                cantidadMinijuegosMotorNissan[4] = true;
                DesactivarMinijuego();
                posicionMinijuegoActual = posicionesMinijuegoNissanValvulas[contador];
            }
        }
    }

    public void TorqueAplicadoValvulasNissan()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(TorqueAplicadoValvulasNissanCorrutina());
    }

    IEnumerator TorqueAplicadoValvulasNissanCorrutina()
    {
        if (!aplicandoTorque)
        {
            Debug.Log("TorqueAplicadoTornillosValvulas");
            torquesNissanValvulas[contador] = Mathf.RoundToInt(Atornillar.singleton.AsignarValorTorque()); // Asignamos el valor del torque

            yield return new WaitForSeconds(0.1f);

            contador += 1;
            if (contador < 4)
            {
                // Nos movemos a la siguiente posicion del minijuego
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuegoNissanValvulas[contador], 1);
                Atornillar.singleton.ReiniciarValorSlider();
                posicionMinijuegoActual = posicionesMinijuegoNissanValvulas[contador];
            }
            else
            {
                // Reestablecemos valores minijuego
                btnAplicarTorque.onClick.RemoveListener(TorqueAplicadoValvulasNissan);
                btnAplicarTorque.onClick.AddListener(TorqueNissanBancadaLevas);
                cantidadMinijuegosMotorNissan[4] = false;
                cantidadMinijuegosMotorNissan[5] = true;
                DesactivarMinijuego();
                posicionMinijuegoActual = posicionesMinijuegoBancadaLevas[contador];
            }
        }
    }

    public void TorqueNissanBancadaLevas()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(TorqueNissanBancadaLevasCorrutina());
    }

    IEnumerator TorqueNissanBancadaLevasCorrutina()
    {
        if (!aplicandoTorque)
        {
            ConfigurarTornilloActivo(); // Los que involucren tornillos

            torquesNissanBancadaLevas[contador] = Mathf.RoundToInt(Atornillar.singleton.AsignarValorTorque()); // Asignamos el valor del torque

            yield return new WaitForSeconds(0.1f);

            contador += 1;
            if (contador < 4)
            {
                // Nos movemos a la siguiente posicion del minijuego
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuegoBancadaLevas[contador], 1);
                Atornillar.singleton.ReiniciarValorSlider();
                posicionMinijuegoActual = posicionesMinijuegoBancadaLevas[contador];
            }
            else
            {
                // Los que involucren tornillos
                if (asignarTornillos.Count > 0)
                {
                    asignarTornillos.RemoveAt(0);
                }

                // Reestablecemos valores minijuego
                btnAplicarTorque.onClick.RemoveListener(TorqueNissanBancadaLevas);
                btnAplicarTorque.onClick.AddListener(TorqueNissanTapaCulata);
                cantidadMinijuegosMotorNissan[5] = false;
                cantidadMinijuegosMotorNissan[6] = true;
                DesactivarMinijuego();
                posicionMinijuegoActual = posicionesMinijuegoTapaCulata[contador];
            }
        }
    }

    public void TorqueNissanTapaCulata()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(TorqueNissanTapaCulataCorrutina());
    }

    IEnumerator TorqueNissanTapaCulataCorrutina()
    {
        if (!aplicandoTorque)
        {
            ConfigurarTornilloActivo(); // Los que involucren tornillos

            torquesNissanTapaCulata[contador] = Mathf.RoundToInt(Atornillar.singleton.AsignarValorTorque()); // Asignamos el valor del torque

            yield return new WaitForSeconds(0.1f);

            contador += 1;
            if (contador < 4)
            {
                // Nos movemos a la siguiente posicion del minijuego
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesMinijuegoTapaCulata[contador], 1);
                Atornillar.singleton.ReiniciarValorSlider();
                posicionMinijuegoActual = posicionesMinijuegoTapaCulata[contador];
            }
            else
            {
                // Los que involucren tornillos
                if (asignarTornillos.Count > 0)
                {
                    asignarTornillos.RemoveAt(0);
                }

                // Reestablecemos valores minijuego
                btnAplicarTorque.onClick.RemoveListener(TorqueNissanTapaCulata);
                cantidadMinijuegosMotorNissan[6] = false;
                DesactivarMinijuego();
            }
        }
    }
}