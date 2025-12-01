using System.Collections;
using UnityEngine;

public class ManagerDesplazamientoMotor : MonoBehaviour
{
    public BrazoMecanicoMejorado brazoMecanicoMejorado;
    public MoverObjeto movimientoBrazo;
    public MoverObjeto capo;
    public ControlVelocidadAnimacion[] controlVelocidadAnimacions;

    public GameObject motorNissanInterno;
    public GameObject motorNissanExterno;
    public GameObject motorDieselInterno;
    public GameObject motorDieselExterno;
    public GameObject motoresEstaticos;

    public GameObject motorAnimadoNissanInterno;
    public GameObject motorAnimadoDieselInterno;
    public GameObject motorAnimadoDieselDinamometro;

    public Transform[] posicionesIniciales;
    public Transform[] posicionesDinamometro;
    public Transform posicionesInicialMotorEstatico;

    public bool desplazamientoEjecutado;
    public bool desplazamientoFinalizado;

    private bool desplazamientoIniciado;
    private Coroutine coroutine;

    public static ManagerDesplazamientoMotor singleton;

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

    public void IniciarDesplazamiento()
    {
        if (!desplazamientoIniciado)
        {
            desplazamientoIniciado = true;
            desplazamientoEjecutado = true;

            if (coroutine != null) StopCoroutine(coroutine);

            coroutine = StartCoroutine(DesplazamientoIniciado());
        }  
    }

    IEnumerator DesplazamientoIniciado()
    {
        MesaMotor.singleton.ValidarExpansionRotacion();
        ManagerMinijuego.singleton.controlVelocidadMotor.SetActive(false);
        MesaMotor.singleton.sliderVelocidadMotor.value = 0f;
        ValoresDinamometro.singleton.sliderControlador.value = 0f;

        yield return new WaitForSeconds(2f);

        brazoMecanicoMejorado.IniciarPickUp();

        yield return new WaitForSeconds(3.5f);

        movimientoBrazo.IniciarDesplazamientoObjeto();

        yield return new WaitForSeconds(10f);

        capo.IniciarDesplazamientoObjeto();

        yield return new WaitForSeconds(2f);

        brazoMecanicoMejorado.ColocarObjetoEn();

        yield return new WaitForSeconds(3.5f);

        movimientoBrazo.RetornarPosicionOriginal();

        yield return new WaitForSeconds(12f);

        movimientoBrazo.RegresarPosicionOriginal();
        brazoMecanicoMejorado.IniciarIdle();

        desplazamientoIniciado = false;
    }


    public void DesactivacionInicialMotores()
    {
        if (ManagerMinijuego.singleton.motorActivo == "Diesel")
        {
            motorDieselInterno.SetActive(true);
            motorDieselExterno.SetActive(true);
            motorAnimadoDieselInterno.gameObject.SetActive(false);
        }
        else if (ManagerMinijuego.singleton.motorActivo == "Nissan")
        {
            motorNissanInterno.SetActive(true);
            motorNissanExterno.SetActive(true);

            motorAnimadoNissanInterno.transform.position = posicionesDinamometro[1].position;
            motorAnimadoNissanInterno.transform.rotation = posicionesDinamometro[1].rotation;

            motorAnimadoNissanInterno.gameObject.SetActive(false);
        }

        for (int i = 0; i < EntornoMecanica.singleton.puntosIntanciasPiezas.Length; i++)
        {
            EntornoMecanica.singleton.puntosIntanciasPiezas[i].SetActive(false);
        }
    }

    public void ActivacionPosteriorMotores()
    {
        if (ManagerMinijuego.singleton.motorActivo == "Diesel")
        {
            motorDieselInterno.SetActive(false);
            motorAnimadoDieselDinamometro.gameObject.SetActive(true);
        }
        else if (ManagerMinijuego.singleton.motorActivo == "Nissan")
        {
            motorNissanInterno.SetActive(false);
            motorAnimadoNissanInterno.gameObject.SetActive(true);
        }

        ValoresDinamometro.singleton.sliderControlador.interactable = true;
        desplazamientoFinalizado = true;
    }

    public void ReinicioMotores()
    {

        for (int i = 0; i < controlVelocidadAnimacions.Length; i++)
        {
            controlVelocidadAnimacions[i].puedoValidar = true;
        }

        movimientoBrazo.RegresarPosicionOriginal();
        capo.RetornarPosicionOriginal();
        ValoresDinamometro.singleton.puedoActualizar = true;
        ValoresDinamometro.singleton.sliderControlador.value = 0f;
        MesaMotor.singleton.sliderVelocidadMotor.value = 0f;
        MesaMotor.singleton.ReestablecerRotacionExpasion();
        ValoresDinamometro.singleton.sliderControlador.interactable = false;
        ValoresDinamometro.singleton.ValoresMotorApagado();

        desplazamientoEjecutado = false;
        desplazamientoFinalizado = false;
        
        motoresEstaticos.transform.position = posicionesInicialMotorEstatico.position;
        motoresEstaticos.transform.rotation = posicionesInicialMotorEstatico.rotation;

        motorDieselInterno.SetActive(false);
        motorDieselExterno.SetActive(false);

        motorNissanInterno.SetActive(false);
        motorNissanExterno.SetActive(false);

        motorAnimadoNissanInterno.transform.position = posicionesIniciales[1].position;
        motorAnimadoNissanInterno.transform.rotation = posicionesIniciales[1].rotation;

        motorAnimadoDieselDinamometro.gameObject.SetActive(false);
        motorAnimadoNissanInterno.gameObject.SetActive(false);

        for (int i = 0; i < EntornoMecanica.singleton.puntosIntanciasPiezas.Length; i++)
        {
            EntornoMecanica.singleton.puntosIntanciasPiezas[i].SetActive(true);
        }


    }
}
