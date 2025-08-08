using System.Collections;
using UnityEngine;

public class MesaMotor : MonoBehaviour
{
    [Header("ESTA ES UNA CLASE SINGLETON")]
    public bool mesaMotorActiva;
    public bool interaccionEjecutada; // Para confirmar si se ejecuto la interaccion del encendido del motor
    public bool motorRotando; // Para validar cuando el motor este rotando y no permitir que se coloquen piezas
    public RotadorPiezas[] rotadorPiezas;
    public ExpansionRadial[] expansionRadials;

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
        if (ManagerCanvas.singleton != null) ManagerCanvas.singleton.DeshabilitarBtnBajarPlataforma();

        for (int i = 0; i < rotadorPiezas.Length; i++)
        {
            rotadorPiezas[i].RegresarARotacionOriginal();
            rotadorPiezas[i].dejarDeRotar = true;
        }

        for (int i = 0; i < expansionRadials.Length; i++)
        {
            expansionRadials[i].Contraer();
            expansionRadials[i].noInteractuar = true;
        }

        yield return new WaitForSeconds(2);

        ExplosionObjetosHijos.singleton.ExplotarTodo();

        yield return new WaitForSeconds(ExplosionObjetosHijos.singleton.duracionVibracion);
        if (ManagerCanvas.singleton != null) 
        {
            ManagerCanvas.singleton.btnReutilizableHabilitado = true;
            ManagerCanvas.singleton.HabilitarBtnBajarPlataforma();
        } 
    }

    /// <summary>
    /// Metodo invocado desde btnReutilizarMotor en el canvas principal
    /// </summary>
    public void RehabilitarInteraccionesMotor()
    {
        interaccionEjecutada = false;
        if (ManagerCanvas.singleton != null) ManagerCanvas.singleton.btnReutilizableHabilitado = false;

        for (int i = 0; i < rotadorPiezas.Length; i++)
        {
            rotadorPiezas[i].dejarDeRotar = false;
        }

        for (int i = 0; i < expansionRadials.Length; i++)
        {
            expansionRadials[i].noInteractuar = false;
        }
    }
}
