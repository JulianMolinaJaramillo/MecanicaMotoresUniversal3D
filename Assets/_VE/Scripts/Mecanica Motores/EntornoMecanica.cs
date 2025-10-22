
using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class EntornoMecanica : MonoBehaviour
{
    [Header("MANIPULACION DE LAS COMPUERTAS")]
    public SueloInteractivo sueloInteractivo;
    public Light luzPrincipal;
    public GameObject[] luces;
    public GameObject[] puntosIntanciasPiezas;
    public ParticleSystem[] sistemasParticulas;
    public ParticleSystem[] particulasHumo;
    public ControlarShape[] particulasCascadaShape;
    public ActivarMaterialesDisolverHijos[] puntosIntanciasPiezasMateriales;
    public MoverObjeto mesa;
    public RotacionObjeto rotacionObjeto;
    public MoverObjeto[] compuertas;
    public MoverObjeto[] brazoMecanico;
    public MoverObjeto[] brazoMecanicoDedos;
    public Transform[] posicionDeseada;
    public MeshRenderer[] meshBrazoMecanico;
    public ExpansionRadial expansionRadialPiezasInternas;

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
        if (AudioManager.singleton != null) AudioManager.singleton.PlayEfectString("Compuerta"); // Ejecutamos el efecto nombrado

        if (ManagerMinijuego.singleton.minijuegoTerminado && !MesaMotor.singleton.motorRotando && !expansionRadialPiezasInternas.expandir)
        {
            if (ExplosionObjetosHijos.singleton != null) ExplosionObjetosHijos.singleton.ActivarHijos(ExplosionObjetosHijos.singleton.objetosPadres[1]); // Activamos los hijos antes de expandir
        }

        for (int i = 0; i < compuertas.Length; i++)
        {
            compuertas[i].IniciarDesplazamientoObjeto();
        }

        ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionDeseada[0],1.5f);

        for (int i = 0; i < particulasHumo.Length; i++)
        {
            if (!particulasHumo[i].isPlaying)
            {
                particulasHumo[i].Play();
            }
        }

        yield return new WaitForSeconds(2f);

        luzPrincipal.enabled = true;
        SubirIntensidadLuzPrincipal();

        mesa.IniciarDesplazamientoObjeto();
        rotacionObjeto.enabled = true;

        if (VibracionCamara.singleton != null)
        {
            VibracionCamara.singleton.IniciarVibracion(1f, 0.007f);
        }

        yield return new WaitForSeconds(1f);

        if (AudioManager.singleton != null) AudioManager.singleton.PlayEfectString("Particulas"); // Ejecutamos el efecto nombrado
        if (VibracionCamara.singleton != null)
        {
            VibracionCamara.singleton.MoverCamaraConVibracion(posicionDeseada[1],5f,0.007f);
        }

        for (int i = 0; i < sistemasParticulas.Length; i++)
        {
            if (!sistemasParticulas[i].isPlaying)
            {
                sistemasParticulas[i].Play();
            }
        }

        for (int i = 0; i < particulasCascadaShape.Length; i++)
        {
            particulasCascadaShape[i].AumentarEscala();
        }

        for (int i = 0; i < puntosIntanciasPiezas.Length; i++)
        {
            puntosIntanciasPiezas[i].SetActive(true);
            puntosIntanciasPiezasMateriales[i].ActivarMaterialesDisolucion(6,1);
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

        BajarIntensidadLuzPrincipal();

        if (ManagerMinijuego.singleton.minijuegoActivo)
        {
            ManagerMinijuego.singleton.herramientasRotatorias.SetActive(true);

            if (InventarioUI.singleton.tamanoHerramienta == 1)
            {
                ManagerMinijuego.singleton.prensaValvulas.SetActive(true);
            }
        }

        for (int i = 0; i < meshBrazoMecanico.Length; i++)
        {
            meshBrazoMecanico[i].enabled = true;
        }

        if (ManagerMinijuego.singleton.minijuegoTerminado && !MesaMotor.singleton.motorRotando && !expansionRadialPiezasInternas.expandir)
        {
            if (ExplosionObjetosHijos.singleton != null) ExplosionObjetosHijos.singleton.DesactivarHijos(ExplosionObjetosHijos.singleton.objetosPadres[1]); // Desactivamos los hijos antes de expandir
            ManagerMinijuego.singleton.motorAnimadoActivo.SetActive(true); // Activamos motor animado luego de expandir   
        }

        ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionDeseada[2], 1);
        sueloInteractivo.IngresandoInteraccion();
        iniciarCompuertas = null;
    }

    public void CerrarCompuerta()
    {
        iniciarCompuertas = StartCoroutine(IniciarAnimacionCerrarCompuertas());
    }

    private IEnumerator IniciarAnimacionCerrarCompuertas()
    {
        if (AudioManager.singleton != null) AudioManager.singleton.PlayEfectString("Particulas"); // Ejecutamos el efecto nombrado

        if (ManagerMinijuego.singleton.minijuegoTerminado && !MesaMotor.singleton.motorRotando && !expansionRadialPiezasInternas.expandir)
        {
            if (ExplosionObjetosHijos.singleton != null) ExplosionObjetosHijos.singleton.ActivarHijos(ExplosionObjetosHijos.singleton.objetosPadres[1]); // Activamos los hijos antes de expandir
            if (ManagerMinijuego.singleton != null && ManagerMinijuego.singleton.motorAnimadoActivo != null) ManagerMinijuego.singleton.motorAnimadoActivo.SetActive(false); // Desactivamos motor animado antes de expandir
        }
           
        for (int i = 0; i < meshBrazoMecanico.Length; i++)
        {
            meshBrazoMecanico[i].enabled = false;
        }

        if (ManagerMinijuego.singleton.minijuegoActivo)
        {
            ManagerMinijuego.singleton.herramientasRotatorias.SetActive(false);

            if (InventarioUI.singleton.tamanoHerramienta == 1 || ManagerMinijuego.singleton.sizeHerramienta == 1)
            {
                ManagerMinijuego.singleton.prensaValvulas.SetActive(false);
            }
        }
        
        MesaMotor.singleton.mesaMotorActiva = false;
        SubirIntensidadLuzPrincipal();

        if (ControlCamaraMotor.singleton != null) // Si es diferente de null deshabilitamos el script
        {
            ControlCamaraMotor.singleton.enabled = false;

            // Si el miijuego esta activo lo desactivamos al momento de salir de la interaccion de la mesa de armado
            if (ManagerMinijuego.singleton.minijuegoActivo)
            {
                ManagerMinijuego.singleton.miniJuegoAtornillar.SetActive(false);
            }
        }

        if (VibracionCamara.singleton != null)
        {
            VibracionCamara.singleton.IniciarVibracion(3f, 0.004f);
        }

        for (int i = 0; i < sistemasParticulas.Length; i++)
        {
            if (!sistemasParticulas[i].isPlaying)
            {
                sistemasParticulas[i].Play();
            }
        }

        for (int i = 0; i < particulasCascadaShape.Length; i++)
        {
            particulasCascadaShape[i].RestaurarEscala();
        }

        if (!MesaMotor.singleton.interaccionEjecutada)
        {
            for (int i = 0; i < puntosIntanciasPiezasMateriales.Length; i++)
            {
                puntosIntanciasPiezasMateriales[i].ActivarMaterialesDisolucion(3, 0);
            }
        }
       
        yield return new WaitForSeconds(3f);

        if (AudioManager.singleton != null) AudioManager.singleton.PlayEfectString("Compuerta"); // Ejecutamos el efecto nombrado

        mesa.RetornarPosicionOriginal();
        ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionDeseada[0], 1.5f);

        if (!MesaMotor.singleton.interaccionEjecutada)
        {
            for (int i = 0; i < puntosIntanciasPiezas.Length; i++)
            {
                puntosIntanciasPiezas[i].SetActive(false);
            }
        }

        
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < particulasHumo.Length; i++)
        {
            if (!particulasHumo[i].isPlaying)
            {
                particulasHumo[i].Play();
            }
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < luces.Length; i++)
        {
            luces[i].SetActive(false);
        }

        for (int i = 0; i < compuertas.Length; i++)
        {
            compuertas[i].RetornarPosicionOriginal();
        }

        

        yield return new WaitForSeconds(2);

        rotacionObjeto.enabled = false;
        luzPrincipal.enabled = false;

        yield return new WaitForSeconds(1);

        sueloInteractivo.IngresandoInteraccion();
        sueloInteractivo.HabilitarInfoMesaArmado();
        
        iniciarCompuertas = null;
    }

    public void SubirIntensidadLuzPrincipal()
    {
        luzPrincipal.intensity = 20;
    }

    public void BajarIntensidadLuzPrincipal()
    {
        luzPrincipal.intensity = 1;
    }

    public void ActivarMeshBrazos()
    {
        for (int i = 0; i < meshBrazoMecanico.Length; i++)
        {
            meshBrazoMecanico[i].enabled = true;
        }
    }

    public void DesactivarMeshBrazos()
    {
        for (int i = 0; i < meshBrazoMecanico.Length; i++)
        {
            meshBrazoMecanico[i].enabled = false;
        }
    }
}
