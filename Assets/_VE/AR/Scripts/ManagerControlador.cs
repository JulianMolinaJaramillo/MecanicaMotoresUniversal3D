using System.Collections;
using UnityEngine;

public class ManagerControlador : MonoBehaviour
{
    [Header("Configuraciónes para Zappar Visible e invisible")]
    public GameObject padre;
    public GameObject iguana;
    [Header("Configuraciónes evento lluvia")]
    public NivelAgua rio; // arrastras el objeto con el material al inspector
    
    public CrecimientoNubes crecimientoNubes;
    public ParticleSystem particulas;
    public ParticleSystem particulasNubes;
    public ParticleSystem particulasPrueba;

    [Header("Configuraciónes Cuenca")]
    public GameObject cuenca;
    public GameObject canvasCuenca;
    public PulsoEscala pulsoEscalaCuenca;
    public GameObject panelInformativoCuenca;
    public TextoEscalonado txtPanelCuenca;
    public Temblor temblorTerrenoCuenca;
    public MovimientoNPC[] npcsCuenca;
    public MovimientoNPC[] npcsCuencaFinales;
    public Rigidbody[] npcAhogados;
    public DerrumbeCasas[] temblorCasas;
    public NivelAgua rioCuenca; // arrastras el objeto con el material al inspector
    public Roca[] rocas;
    public GameObject panelPuntoReunion;
    public PulsoEscala botonPuntoReunion;
    public GameObject terrenoCuenca;
    public GameObject particulasDerrumbe;
    public ControlDeslizamiento terrenoModificado;

    [Header("Configuraciónes Casas")]
    public GameObject casas;
    public GameObject canvasCasas;
    public PulsoEscala pulsoEscalaCasas;
    public GameObject panelInformativoCasas;
    public TextoEscalonado txtPanelCasas;
    public Temblor[] temblorTerrenoCasas;
    public MovimientoNPC[] npcsCasas;
    public CamaraAlerta camaraAlerta;
    public NivelAgua rioCasas; // arrastras el objeto con el material al inspector
    public MovimientoSuavizado movimientoSuavizadoCasas;

    [Header("Configuraciónes adicionales")]
    public GameObject imgLLuvia;
    public GameObject botonLluviaCreciente;
    public GameObject botonReunion;
    public ManagerVehiculos[] vehiculos;
    public GameObject panelDespedida;
    public PulsoEscala[] pulsosInternos;
    public TextoEscalonado canvasInformativo;
    public TextoEscalonado canvasInformativoCuencaInterno;
    public TextoEscalonado canvasInformativoCasasInterno;
    public GameObject imagenIguana;
    public GameObject imagenCuenca;
    public GameObject imagenCasas;
    public GameObject imagenes;

    private bool lluviaActiva;
    private Coroutine coroutine;
    private Coroutine coroutine2;
    private Coroutine coroutine3;
    private Coroutine coroutine4;
    private Coroutine coroutine5;
    //
    private bool iniciarPDF;
    private bool desastreCuencaActivo;
    private bool desastreCasasActivo;
    [HideInInspector]
    public bool desastreInicialActivo;
    [HideInInspector]
    public bool desastreSecundarioActivo;
    [HideInInspector]
    public bool puntoEncuentroActivo;

    public static ManagerControlador singleton;
    private bool imagenIguanaActiva;
    private bool imagenCuencaActiva;
    private bool imagenCasasActiva;

    private bool momentoUnoCuencaActivo;
    private bool momentoUnoCasasActivo;

    private bool momentoUnoCuencaTerminado;
    private bool momentoUnoCasasTerminado;

    [HideInInspector]
    public bool momentoDosCuencaTerminado;
    [HideInInspector]
    public bool momentoDosCasasTerminado;
    [HideInInspector]
    public bool momentoReunionTerminado;
    [HideInInspector]
    public bool momentoReunionTerminadoCuenca;
    [HideInInspector]
    public bool tutorialTerminado;

    private bool activacion;

    private void Awake()
    {
        // Si ya existe una instancia y no es esta → destruir el duplicado
        if (singleton != null && singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        // Asignamos la instancia
        singleton = this;

        //// Accedemos al material del objeto
        //Material mat = rio.material;

        //mat.SetFloat("_DisplaceAmp", 0.2f);

        canvasInformativo.textoAlmacenado = "1. Cuenca en estado normal.";
        canvasInformativo.MostrarTexto("1. Cuenca en estado normal.");
    }

    private void Start()
    {
        casas.SetActive(true);
        cuenca.SetActive(true);
        SimpleAudioManager.singleton.RestaurarAudioFondo();
    }

    private void Update()
    {
        if (!activacion)
        {
            casas.SetActive(false);
            cuenca.SetActive(false);
            activacion = true;
        }

        if (momentoUnoCasasTerminado && momentoUnoCuencaTerminado)
        {
            botonLluviaCreciente.SetActive(true);
            momentoUnoCasasTerminado = false;
            momentoUnoCuencaTerminado = false;
        }

        if (momentoDosCasasTerminado && momentoDosCuencaTerminado)
        {
            if (iguana.activeInHierarchy)
            {
                botonReunion.SetActive(true);
                pulsoEscalaCasas.gameObject.SetActive(false);
                pulsoEscalaCuenca.gameObject.SetActive(false);
            }
            else
            {
                momentoReunionTerminado = true;
            }
        }
    }

    public void TerminarTutorial()
    {
        tutorialTerminado = true;
    }

    public void SalirAplicacion()
    {
        panelDespedida.SetActive(true);
        Application.Quit();
    }

    public void ActivarImagenIguana()
    {
        imagenIguanaActiva = true;
        imagenCuencaActiva = false;
        imagenCasasActiva = false;

        imagenIguana.SetActive(true);
        imagenCasas.SetActive(false);
        imagenCuenca.SetActive(false);
    }

    public void ActivarImagenCuenca()
    {
        imagenIguanaActiva = false;
        imagenCuencaActiva = true;
        imagenCasasActiva = false;

        imagenIguana.SetActive(false);
        imagenCasas.SetActive(false);
        imagenCuenca.SetActive(true);
    }

    public void ActivarImagenCasas()
    {
        imagenIguanaActiva = false;
        imagenCuencaActiva = false;
        imagenCasasActiva = true;

        imagenIguana.SetActive(false);
        imagenCasas.SetActive(true);
        imagenCuenca.SetActive(false);
    }

    public void ActivarPanelCasas()
    {
        panelInformativoCasas.SetActive(true);
    }


    public void ActivarPanelCuenca()
    {
        panelInformativoCuenca.SetActive(true);
    }

    public void DesactivarPanelCasas()
    {
        panelInformativoCasas.SetActive(false);
    }


    public void DesactivarPanelCuenca()
    {
        panelInformativoCuenca.SetActive(false);
    }

    [ContextMenu("Iniciar")]
    public void EmpezarEventoLluviaCreciente()
    {
        // Empezamos el evento
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(LluviaCreciente());
    }

    private IEnumerator LluviaCreciente()
    {
        for (int i = 0; i < pulsosInternos.Length; i++)
        {
            pulsosInternos[i].gameObject.SetActive(false);
        }

        canvasInformativo.textoAlmacenado = "2. Precipitación.";
        canvasInformativo.MostrarTexto("2. Precipitación.");

        desastreInicialActivo = true;
        // Generamos las nubes
        crecimientoNubes.ActivarCrecimiento();
        IntensidadLuz.singleton.AumentarIntensidad();

        lluviaActiva = true;
        if (particulasNubes != null) particulasNubes.Play();

        yield return new WaitForSeconds(1f);

        // Activamos sonido lluvia
        if (SimpleAudioManager.singleton != null)
        {
            SimpleAudioManager.singleton.DetenerAudioFondo();
            SimpleAudioManager.singleton.audioSourceFondo.clip = SimpleAudioManager.singleton.clips[2];
            SimpleAudioManager.singleton.RestaurarAudioFondo();
        }

        
        // Activamos la lluvia
        if (particulas != null) particulas.Play();

        if (rio.gameObject.activeInHierarchy)
        {
            rio.SubirDisplace();
        }
        else
        {
            rio.rioActivo = true;
        }
        
        yield return new WaitForSeconds(1f);
        
        DesactivarPanelCasas();
        DesactivarPanelCuenca();
        iniciarPDF = true;

        pulsoEscalaCuenca.IniciarAlerta();
        ActivarPanelCuenca();
        txtPanelCuenca.textoAlmacenado = "El cielo de la cuenca comienza a cubrirse de nubes y se inicia una llovizna; el nivel de la quebrada empieza a crecer.";
        txtPanelCuenca.MostrarTexto("El cielo de la cuenca comienza a cubrirse de nubes y se inicia una llovizna; el nivel de la quebrada empieza a crecer.");

        canvasInformativoCuencaInterno.textoAlmacenado = "El agua golpea las laderas con pendientes pronunciadas, lo que acelera los procesos de escorrentía superficial.";
        canvasInformativoCuencaInterno.MostrarTexto("El agua golpea las laderas con pendientes pronunciadas, lo que acelera los procesos de escorrentía superficial.");

        yield return new WaitForSeconds(0.5f);

        pulsoEscalaCasas.IniciarAlerta();
        ActivarPanelCasas();  
        txtPanelCasas.textoAlmacenado = "A la zona residencial empiezan a llegar las nubes cargadas de lluvia; crece peligrosamente el nivel del agua.";
        txtPanelCasas.MostrarTexto("A la zona residencial empiezan a llegar las nubes cargadas de lluvia; crece peligrosamente el nivel del agua.");

        canvasInformativoCasasInterno.textoAlmacenado = "los habitantes se percatan del incremento en el caudal, mientras el sensor de nivel registra un aumento significativo en sus datos.";
        canvasInformativoCasasInterno.MostrarTexto("los habitantes se percatan del incremento en el caudal, mientras el sensor de nivel registra un aumento significativo en sus datos.");


        if (cuenca.activeInHierarchy)
        {
            momentoUnoCuencaActivo = true;
            DesastreCuenca();
        }
        else
        {
            momentoUnoCuencaActivo = true;
        }

        if (casas.activeInHierarchy)
        {
            momentoUnoCasasActivo = true;
            DesastreCasas();
        }
        else
        {
            momentoUnoCasasActivo = true;
        }
    }

    [ContextMenu("Iniciar 2")]
    public void AumentarEventoLluviaCreciente()
    {
        // Empezamos el evento
        if (coroutine2 != null) StopCoroutine(coroutine2);
        coroutine2 = StartCoroutine(ContinuaLluviaCreciente());
    }

    private IEnumerator ContinuaLluviaCreciente()
    {
        for (int i = 0; i < pulsosInternos.Length; i++)
        {
            pulsosInternos[i].gameObject.SetActive(false);
        }

        canvasInformativo.textoAlmacenado = "3. Concentración de caudales";
        canvasInformativo.MostrarTexto("3. Concentración de caudales");

        desastreSecundarioActivo = true;

        // Aumentar Particulas lluvia
        var main = particulas.main;  // Módulo Main
        main.maxParticles = 1000;

        var emission = particulas.emission;  // Módulo Emission
        emission.rateOverTime = 1000;


        yield return new WaitForSeconds(1f);

        DesactivarPanelCasas();
        DesactivarPanelCuenca();

        ActivarPanelCuenca();
        txtPanelCuenca.textoAlmacenado = "Cunde el caos entre las personas, se presentan desprendimientos de tierra, las viviendas colapsan y la quebrada se desborda.";
        txtPanelCuenca.MostrarTexto("Cunde el caos entre las personas, se presentan desprendimientos de tierra, las viviendas colapsan y la quebrada se desborda.");

        canvasInformativoCuencaInterno.textoAlmacenado = "El suelo se desprende progresivamente, arrastrado por la corriente, lo que genera erosión y socavamiento en las laderas.";
        canvasInformativoCuencaInterno.MostrarTexto("El suelo se desprende progresivamente, arrastrado por la corriente, lo que genera erosión y socavamiento en las laderas.");
        pulsoEscalaCuenca.IniciarAlerta();

        if (rio.gameObject.activeInHierarchy)
        {
            rio.tope = 0.031f;
            rio.SubirDisplace();
        }
        else
        {
            rio.rioActivo = true;
        }
        
        if (cuenca.activeInHierarchy)
        {
            desastreCuencaActivo = true;
            SimpleAudioManager.singleton.Gritos();
            DesastreCuenca();
        }
        else
        {
            desastreCuencaActivo = true;
        }

        yield return new WaitForSeconds(0.5f);

        ActivarPanelCasas();
        txtPanelCasas.textoAlmacenado = "La capacidad de infiltración del suelo se ve superada; las zonas urbanas se inundan y suena la alarma para una pronta evacuación.";
        txtPanelCasas.MostrarTexto("La capacidad de infiltración del suelo se ve superada; las zonas urbanas se inundan y suena la alarma para una pronta evacuación.");

        canvasInformativoCasasInterno.textoAlmacenado = "El nivel del agua aumenta, se activa una alarma comunitaria de emergencia instalada como parte del sistema de monitoreo y alertas tempranas - SATC.";
        canvasInformativoCasasInterno.MostrarTexto("El nivel del agua aumenta, se activa una alarma comunitaria de emergencia instalada como parte del sistema de monitoreo y alertas tempranas - SATC.");
        pulsoEscalaCasas.IniciarAlerta();

        if (casas.activeInHierarchy)
        {
            desastreCasasActivo = true;
            SimpleAudioManager.singleton.Gritos();
            SimpleAudioManager.singleton.Alarma();
            DesastreCasas();
        }
        else
        {
            desastreCasasActivo = true;
        }

    }

    [ContextMenu("Iniciar 3")]
    public void EmpezarEventoPuntoEncuentro()
    {
        // Empezamos el evento
        if (coroutine5 != null) StopCoroutine(coroutine5);
        coroutine5 = StartCoroutine(PuntoEncuentro());
    }

    private IEnumerator PuntoEncuentro()
    {
        pulsoEscalaCasas.gameObject.SetActive(false);
        canvasInformativo.textoAlmacenado = "4. Reunión punto de encuentro.";
        canvasInformativo.MostrarTexto("4. Reunión punto de encuentro.");
        SimpleAudioManager.singleton.PlaySound(6);
        pulsoEscalaCuenca.gameObject.SetActive(true);
        pulsosInternos[0].gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < npcsCuenca.Length; i++)
        {
            npcsCuenca[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < npcsCuencaFinales.Length; i++)
        {
            npcsCuencaFinales[i].gameObject.SetActive(true);
        }

        DesactivarPanelCasas();
        DesactivarPanelCuenca();
        
        ActivarPanelCuenca();
        txtPanelCuenca.textoAlmacenado = "El flujo de personas converge hacia un punto de encuentro seguro en una zona alta, donde se organizan bajo la guía de líderes comunitarios.";
        txtPanelCuenca.MostrarTexto("El flujo de personas converge hacia un punto de encuentro seguro en una zona alta, donde se organizan bajo la guía de líderes comunitarios.");
        pulsoEscalaCuenca.IniciarAlerta();
        puntoEncuentroActivo = true;      
    }

    private IEnumerator IniciadorPuntoEncuentroEventoCuenca()
    {
        SimpleAudioManager.singleton.Hablando();
        panelPuntoReunion.SetActive(true);     
        pulsosInternos[0].gameObject.SetActive(false);
        SimpleAudioManager.singleton.PlaySound(6);
        canvasInformativoCuencaInterno.textoAlmacenado = "Gracias a los sistemas de alerta temprana integrados al SIRMED se logró una exitosa evacuación por las rutas integradas.";
        canvasInformativoCuencaInterno.MostrarTexto("Gracias a los sistemas de alerta temprana integrados al SIRMED se logró una exitosa evacuación por las rutas integradas.");
        IntensidadLuz.singleton.RestaurarIntensidad();

        yield return new WaitForSeconds(0.5f);
        botonPuntoReunion.IniciarAlerta();

        yield return new WaitForSeconds(7f);

        SimpleAudioManager.singleton.PlaySound(6);
        canvasInformativoCuencaInterno.textoAlmacenado = "Lentamente comienzan a disminuir las fuertes lluvias, lo que ayuda también a disminuir la presión de la corriente.";
        canvasInformativoCuencaInterno.MostrarTexto("Lentamente comienzan a disminuir las fuertes lluvias, lo que ayuda también a disminuir la presión de la corriente.");

        var main = particulas.main;  // Módulo Main
        main.maxParticles = 500;

        var emission = particulas.emission;  // Módulo Emission
        emission.rateOverTime = 100;

        crecimientoNubes.velocidad = 0.3f;
        crecimientoNubes.RestablecerCrecimiento();
        
        rioCuenca.ResetDisplace();
        yield return new WaitForSeconds(7f);

        canvasInformativoCuencaInterno.textoAlmacenado = "La lluvia se detiene por completo, sale el sol y el nivel de la corriente vuelve a la normalidad, la comunidad procede con la evaluación de daños.";
        canvasInformativoCuencaInterno.MostrarTexto("La lluvia se detiene por completo, sale el sol y el nivel de la corriente vuelve a la normalidad, la comunidad procede con la evaluación de daños.");

        // Desactivamos sonido lluvia y restauramos fondo
        if (SimpleAudioManager.singleton != null)
        {
            SimpleAudioManager.singleton.DetenerAudioFondo();
            SimpleAudioManager.singleton.audioSourceFondo.clip = SimpleAudioManager.singleton.clips[7];
            SimpleAudioManager.singleton.RestaurarAudioFondo();
        }

        particulas.Stop();
        particulasNubes.Stop();
        lluviaActiva = false;
        particulas.gameObject.SetActive(false);
        particulasNubes.gameObject.SetActive(false);

        pulsosInternos[0].gameObject.SetActive(true);
        SimpleAudioManager.singleton.PlaySound(6);

        yield return new WaitForSeconds(0.5f);

        pulsosInternos[0].RestablecerEscalaColor();
        momentoReunionTerminadoCuenca = true;
    }

    private IEnumerator IniciadorPuntoEncuentroEventoCasas()
    {
        rioCasas.ResetGain();

        for (int i = 0; i < npcsCasas.Length; i++)
        {
            npcsCasas[i].gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(1f);

        pulsosInternos[1].gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        pulsosInternos[1].RestablecerEscalaColor();
        
    }

    public void DesastreCuenca()
    {
        if (coroutine3 != null) StopCoroutine(coroutine3);
        coroutine3 = StartCoroutine(IniciadorCuenca());
    }

    private IEnumerator IniciadorCuenca()
    {
        yield return new WaitForSeconds(1f);

        if (momentoUnoCuencaActivo)
        {
            momentoUnoCuencaActivo = false;
            // Empezamos el evento
            if (coroutine3 != null) StopCoroutine(coroutine3);
            coroutine3 = StartCoroutine(MomentoUnoCuenca());
        }

        if (desastreCuencaActivo)
        {
            desastreCuencaActivo = false;
            // Empezamos el evento
            if (coroutine3 != null) StopCoroutine(coroutine3);
            coroutine3 = StartCoroutine(DesastreCuencaCorrutina());
        }

        if (puntoEncuentroActivo)
        {
            puntoEncuentroActivo = false;
            // Empezamos el evento
            if (coroutine3 != null) StopCoroutine(coroutine3);
            coroutine3 = StartCoroutine(IniciadorPuntoEncuentroEventoCuenca());
        }
    }

    private IEnumerator MomentoUnoCuenca()
    {
        rioCuenca.SubirDisplace();
        
        yield return new WaitForSeconds(5f);

        pulsosInternos[0].gameObject.SetActive(true);
        SimpleAudioManager.singleton.PlaySound(6);

        yield return new WaitForSeconds(0.5f);

        pulsosInternos[0].IniciarAlerta();
        momentoUnoCuencaTerminado = true;
    }

    private IEnumerator DesastreCuencaCorrutina()
    {
        SimpleAudioManager.singleton.PlaySound(3);
       
        particulasDerrumbe.SetActive(true);
        

        rioCuenca.tope = 0.077f;
        rioCuenca.SubirDisplace();
        
        yield return new WaitForSeconds(3f);

        for (int i = 0; i < rocas.Length; i++)
        {
            yield return new WaitForSeconds(0.05f);
            rocas[i].IniciarRecorrido();
        }

        SimpleAudioManager.singleton.PlaySound2(8);
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < temblorCasas.Length; i++)
        {
            temblorCasas[i].IniciarDerrumbe();
        }

        for (int i = 0; i < npcsCuenca.Length; i++)
        {
            npcsCuenca[i].CorrerPorSuVida();
        }

        for (int i = 0; i < vehiculos.Length; i++)
        {
            vehiculos[i].DetenerSpawn();
        }

        terrenoCuenca.SetActive(false);
        terrenoModificado.AnimarBlendShape();

        yield return new WaitForSeconds(7f);

        for (int i = 0; i < npcsCuenca.Length - 1; i++)
        {
            npcsCuenca[i].enabled = false;
        }

        for (int i = 0; i < npcAhogados.Length; i++)
        {
            npcAhogados[i].gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(1f);

        particulasDerrumbe.SetActive(false);
        pulsosInternos[0].gameObject.SetActive(true);
        SimpleAudioManager.singleton.PlaySound2(6);

        yield return new WaitForSeconds(0.5f);

        pulsosInternos[0].IniciarAlerta();
        momentoDosCuencaTerminado = true;
    }

    public void DesastreCasas()
    {
        if (momentoReunionTerminadoCuenca)
        {
            movimientoSuavizadoCasas.ReiniciarPosicion();
        }

        if (coroutine3 != null) StopCoroutine(coroutine3);
        coroutine3 = StartCoroutine(IniciadorCasas());
    }

    private IEnumerator IniciadorCasas()
    {
        yield return new WaitForSeconds(1f);

        if (momentoUnoCasasActivo)
        {
            momentoUnoCasasActivo = false;
            // Empezamos el evento
            if (coroutine3 != null) StopCoroutine(coroutine3);
            coroutine3 = StartCoroutine(MomentoUnoCasas());
        }

        if (desastreCasasActivo)
        {
            desastreCasasActivo = false;
            // Empezamos el evento
            if (coroutine4 != null) StopCoroutine(coroutine4);
            coroutine4 = StartCoroutine(DesastreCasasCorrutina());
        }
     
        if (momentoReunionTerminadoCuenca)
        {
            // Empezamos el evento
            if (coroutine3 != null) StopCoroutine(coroutine3);
            coroutine3 = StartCoroutine(IniciadorPuntoEncuentroEventoCasas());
        }
    }

    private IEnumerator MomentoUnoCasas()
    {
        movimientoSuavizadoCasas.IniciarDesplazamiento();
        camaraAlerta.IniciarAlerta();
        rioCasas.SubirGain();
        yield return new WaitForSeconds(5f);

        pulsosInternos[1].gameObject.SetActive(true);
        SimpleAudioManager.singleton.PlaySound(6);

        yield return new WaitForSeconds(0.5f);

        pulsosInternos[1].IniciarAlerta();
        momentoUnoCasasTerminado = true;
    }

    private IEnumerator DesastreCasasCorrutina()
    {
        movimientoSuavizadoCasas.velocidad = 0.8f; ;
        movimientoSuavizadoCasas.CambiarObjetivoSecundario();

        for (int i = 0; i < temblorTerrenoCasas.Length; i++)
        {
            temblorTerrenoCasas[i].Vibrar();
        }

        for (int i = 0; i < npcsCasas.Length; i++)
        {
            npcsCasas[i].CorrerPorSuVida();
        }

        yield return new WaitForSeconds(7f);

        pulsosInternos[1].gameObject.SetActive(true);
        SimpleAudioManager.singleton.PlaySound(6);

        yield return new WaitForSeconds(0.5f);

        pulsosInternos[1].IniciarAlerta();
        momentoDosCasasTerminado = true;
    }

    public void AntesDeNormalizarCuenca()
    {
        // Empezamos el evento
        if (coroutine3 != null) StopCoroutine(coroutine3);
        coroutine3 = StartCoroutine(AntesDeNormalizarCuencaCorrutina());
    }

    private IEnumerator AntesDeNormalizarCuencaCorrutina()
    {
        yield return new WaitForSeconds(1f);

        botonReunion.SetActive(true);
        pulsoEscalaCasas.gameObject.SetActive(false);
        pulsoEscalaCuenca.gameObject.SetActive(false);
        momentoReunionTerminado = true;

        momentoDosCasasTerminado = false;
        momentoDosCuencaTerminado = false;
    }

    public void NormalizarCuenca()
    {
        // Empezamos el evento
        if (coroutine3 != null) StopCoroutine(coroutine3);
        coroutine3 = StartCoroutine(NormalizarCuencaCorrutina());
    }

    private IEnumerator NormalizarCuencaCorrutina()
    {
        botonReunion.SetActive(false);
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < vehiculos.Length; i++)
        {
            vehiculos[i].ResstablecerSpawn();
        }

        rio.rioActivo = false;
        rio.ResetDisplace();

        canvasInformativo.textoAlmacenado = "5. Cuenca después del desastre.";
        canvasInformativo.MostrarTexto("5. Cuenca después del desastre.");

        ActivarPanelCuenca();
        txtPanelCuenca.textoAlmacenado = "Estado normal de la cuenca, luego del desastre natural y en proceso de evaluación de daños.";
        txtPanelCuenca.MostrarTexto("Estado normal de la cuenca, luego del desastre natural y en proceso de evaluación de daños.");

        canvasInformativoCuencaInterno.textoAlmacenado = "Estado normal de la cuenca, luego del desastre natural y en proceso de evaluación de daños.";
        canvasInformativoCuencaInterno.MostrarTexto("Estado normal de la cuenca, luego del desastre natural y en proceso de evaluación de daños.");


        ActivarPanelCasas();
        txtPanelCasas.textoAlmacenado = "Zona de viviendas en su estado normal, luego del desastre natural y en proceso de evaluación de daños.";
        txtPanelCasas.MostrarTexto("Zona de viviendas en su estado normal, luego del desastre natural y en proceso de evaluación de daños.");

        canvasInformativoCasasInterno.textoAlmacenado = "Zona de viviendas en su estado normal, luego del desastre natural y en proceso de evaluación de daños.";
        canvasInformativoCasasInterno.MostrarTexto("Zona de viviendas en su estado normal, luego del desastre natural y en proceso de evaluación de daños.");

        desastreSecundarioActivo = false;
        pulsoEscalaCuenca.gameObject.SetActive(true);
        pulsoEscalaCasas.gameObject.SetActive(true);
        pulsosInternos[0].gameObject.SetActive(true);
        pulsosInternos[1].gameObject.SetActive(true);

        pulsoEscalaCuenca.RestablecerEscalaColor();
        pulsoEscalaCasas.RestablecerEscalaColor();
        pulsosInternos[0].RestablecerEscalaColor();
        pulsosInternos[1].RestablecerEscalaColor();

        momentoDosCasasTerminado = false;
        momentoDosCuencaTerminado = false;
    }
}
