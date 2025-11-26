using System.Collections;
using UnityEngine;

public class MesaMotor : MonoBehaviour
{
    [Header("ESTA ES UNA CLASE SINGLETON")]
    public bool mesaMotorActiva;
    public bool interaccionEjecutada; // Para confirmar si se ejecuto la interaccion del encendido del motor
    public bool motorRotando; // Para validar cuando el motor este rotando y no permitir que se coloquen piezas
    public bool motorExpandido; // Para validar cuando el motor expandido
    public RotadorPiezas[] rotadorPiezas;
    public ExpansionRadial[] expansionRadials;
    public ParticleSystem[] partciulasHumoMotor;

    [HideInInspector]
    public bool estoyEnMesa;
    public static MesaMotor singleton;
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

    public void DetenerInteraccionesMotor()
    {
        StartCoroutine(DetenerMotor()); 
    }

    private IEnumerator DetenerMotor()
    {
        interaccionEjecutada = true;
        if (!expansionRadials[1].expandir)
        {
            if (ManagerMinijuego.singleton != null) ManagerMinijuego.singleton.motorAnimadoActivo.SetActive(true);
        }

        if (ManagerCanvas.singleton != null) 
        {
            ManagerCanvas.singleton.DeshabilitarBtnBajarPlataforma();
            ManagerCanvas.singleton.DeshabilitarBtnExpandir();
            ManagerCanvas.singleton.DeshabilitarBtnRotar();
        }

        if (ManagerMinijuego.singleton != null) ManagerMinijuego.singleton.btnEncenderMotor.gameObject.SetActive(false);

        for (int i = 0; i < rotadorPiezas.Length; i++)
        {
            rotadorPiezas[i].velocidadRetorno = 10;
            rotadorPiezas[i].RegresarARotacionOriginal();
            rotadorPiezas[i].dejarDeRotar = true;
        }

        for (int i = 0; i < expansionRadials.Length; i++)
        {
            expansionRadials[i].Contraer();
            expansionRadials[i].noInteractuar = true;
        }

        yield return new WaitForSeconds(1);

        if (ManagerMinijuego.singleton != null) ManagerMinijuego.singleton.motorAnimadoActivo.SetActive(true);

        yield return new WaitForSeconds(1);

        ExplosionObjetosHijos.singleton.ExplotarTodo();

        yield return new WaitForSeconds(ExplosionObjetosHijos.singleton.duracionVibracion);

        yield return new WaitForSeconds(2f);

        DesactivarParticulasHumo();

        if (ManagerCanvas.singleton != null) 
        {
            ManagerCanvas.singleton.HabilitarBtnBajarPlataforma();

            string texto = "Al parecer los torques y el armado en general del motor no fué el correcto, vuelve a probar.";
            ManagerCanvas.singleton.AlertarMensaje(texto);
        }

        for (int i = 0; i < rotadorPiezas.Length; i++)
        {
            rotadorPiezas[i].velocidadRetorno = 2;
            rotadorPiezas[i].dejarDeRotar = false;
        }

        for (int i = 0; i < expansionRadials.Length; i++)
        {
            expansionRadials[i].noInteractuar = false;
        }
    }

    public void ActivarParticulasHumo()
    {
        StartCoroutine(ParticulasHumoMotor());
        
    }
    private IEnumerator ParticulasHumoMotor()
    {
        if (!interaccionEjecutada)
        {
            if (ManagerMinijuego.singleton != null) ManagerMinijuego.singleton.btnEncenderMotor.gameObject.SetActive(false);
            ManagerCanvas.singleton.DeshabilitarBtnExpandir();
            ManagerCanvas.singleton.DeshabilitarBtnRotar();
        }

        for (int i = 0; i < partciulasHumoMotor.Length; i++)
        {
            yield return new WaitForSeconds(0.3f);
            partciulasHumoMotor[i].Play();
        }

        if (!interaccionEjecutada)
        {
            string texto = "Al parecer una o algunas partes te quedaron sin lubricar correctamente, vuelve a probar.";
            ManagerCanvas.singleton.AlertarMensaje(texto);
        }    
    }

    public void DesactivarParticulasHumo()
    {
        for (int i = 0; i < partciulasHumoMotor.Length; i++)
        {
            partciulasHumoMotor[i].Stop();
        }
    }
}
