
using System.Collections;
using UnityEngine;

public class EntornoMecanica : MonoBehaviour
{
    [Header("MANIPULACION DE LAS COMPUERTAS")]
    public SueloInteractivo sueloInteractivo;
    public Light luzPrincipal;
    public GameObject[] luces;
    public GameObject[] puntosIntanciasPiezas;
    public ParticleSystem particulasCascada;
    public ActivarMaterialesDisolverHijos[] puntosIntanciasPiezasMateriales;
    public MoverObjeto mesa;
    public RotacionObjeto rotacionObjeto;
    public MoverObjeto[] compuertas;
    public MoverObjeto[] brazoMecanico;
    public MoverObjeto[] brazoMecanicoDedos;
    public Transform[] posicionDeseada;

    public static EntornoMecanica singleton;
    private Coroutine iniciarCompuertas;

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

    public void AbrirCompuerta(Transform posicionObjetivo)
    {
        posicionDeseada[2] = posicionObjetivo;
        iniciarCompuertas = StartCoroutine(IniciarAnimacionAbrirCompuertas());
    }

    private IEnumerator IniciarAnimacionAbrirCompuertas()
    {
        for (int i = 0; i < compuertas.Length; i++)
        {
            compuertas[i].IniciarDesplazamientoObjeto();
        }
        ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionDeseada[0],1.5f);

        yield return new WaitForSeconds(2f);

        luzPrincipal.enabled = true;
        luzPrincipal.intensity = 20;

        mesa.IniciarDesplazamientoObjeto();
        rotacionObjeto.enabled = true;

        yield return new WaitForSeconds(1f);

        ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionDeseada[1], 2f);

        if (!particulasCascada.isPlaying)
        {
            particulasCascada.Play();
        }

        for (int i = 0; i < puntosIntanciasPiezas.Length; i++)
        {
            puntosIntanciasPiezas[i].SetActive(true);
            puntosIntanciasPiezasMateriales[i].ActivarMaterialesDisolucion();
        }

        for (int i = 0; i < brazoMecanico.Length; i++)
        {
            brazoMecanico[i].IniciarDesplazamientoObjeto();
        }

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < brazoMecanicoDedos.Length; i++)
        {
            brazoMecanicoDedos[i].IniciarDesplazamientoObjeto();
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < brazoMecanicoDedos.Length; i++)
        {
            brazoMecanicoDedos[i].RetornarPosicionOriginal();
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < brazoMecanico.Length; i++)
        {
            brazoMecanico[i].RetornarPosicionOriginal();
        }

        yield return new WaitForSeconds(2f);

        if (ControlCamaraMotor.singleton != null) // Si es diferente de null habilitamos el script del movimiento de camaras
        {
            ControlCamaraMotor.singleton.enabled = true;

            // Si el miijuego esta activo lo activamos al momento de entrar en la interaccion de la mesa de armado
            if (ManagerMinijuego.singleton.minijuegoActivo)
            {
                ManagerMinijuego.singleton.miniJuegoAtornillar.SetActive(true);
            }
        }
        MesaMotor.singleton.mesaMotorActiva = true;

        for (int i = 0; i < luces.Length; i++)
        {
            luces[i].SetActive(true);
        }

        luzPrincipal.intensity = 1;
        rotacionObjeto.enabled = false;
        ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionDeseada[2], 1);
        sueloInteractivo.SaliendoInteraccion();
        iniciarCompuertas = null;
    }

    public void CerrarCompuerta()
    {
        iniciarCompuertas = StartCoroutine(IniciarAnimacionCerrarCompuertas());
    }

    private IEnumerator IniciarAnimacionCerrarCompuertas()
    {
        luzPrincipal.intensity = 20;
        if (ControlCamaraMotor.singleton != null) // Si es diferente de null deshabilitamos el script
        {
            ControlCamaraMotor.singleton.enabled = false;

            // Si el miijuego esta activo lo desactivamos al momento de salir de la interaccion de la mesa de armado
            if (ManagerMinijuego.singleton.minijuegoActivo)
            {
                ManagerMinijuego.singleton.miniJuegoAtornillar.SetActive(false);
            }
        }

        mesa.RetornarPosicionOriginal();
        ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionDeseada[0], 1.5f);

        if (!MesaMotor.singleton.interaccionEjecutada)
        {
            for (int i = 0; i < puntosIntanciasPiezas.Length; i++)
            {
                puntosIntanciasPiezas[i].SetActive(false);
            }
        }
        
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < luces.Length; i++)
        {
            luces[i].SetActive(false);
        }

        for (int i = 0; i < compuertas.Length; i++)
        {
            compuertas[i].RetornarPosicionOriginal();
        }
        yield return new WaitForSeconds(2);

        luzPrincipal.enabled = false;

        yield return new WaitForSeconds(1);

        sueloInteractivo.SaliendoInteraccion();
        sueloInteractivo.HabilitarInfoMesaArmado();
        MesaMotor.singleton.mesaMotorActiva = false;
        iniciarCompuertas = null;
    }
}
