using System.Collections;
using UnityEngine;

public class VibracionCamara : MonoBehaviour
{
    [Header("Referencias")]
    public Transform camara;
    private Transform objetivoMovimiento;

    [Header("Vibración")]
    public float intensidadVibracion = 0.01f;
    public float duracionVibracion = 5f;

    private Vector3 offsetVibracion = Vector3.zero;
    private Coroutine movimientoActual;

    private bool activo = false;

    public static VibracionCamara singleton;

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

    void LateUpdate()
    {
        if (!activo || camara == null)
            return;

        if (duracionVibracion > 0)
        {
            offsetVibracion = Random.insideUnitSphere * intensidadVibracion;
            duracionVibracion -= Time.deltaTime;
        }
        else
        {
            offsetVibracion = Vector3.zero;
            // Si no hay vibración y no hay movimiento activo, desactiva el LateUpdate
            if (movimientoActual == null)
                activo = false;
        }

        camara.localPosition += offsetVibracion;
    }

    public void IniciarVibracion(float duracion, float intensidad)
    {
        duracionVibracion = duracion;
        intensidadVibracion = intensidad;
        activo = true;
    }

    public void MoverCamaraConVibracion(Transform objetivo, float duracion, float intensidad)
    {
        if (movimientoActual != null)
            StopCoroutine(movimientoActual);

        objetivoMovimiento = objetivo;
        movimientoActual = StartCoroutine(MoverSuavemente(objetivoMovimiento.position, objetivoMovimiento.rotation, duracion, intensidad));
        activo = true;
    }

    private IEnumerator MoverSuavemente(Vector3 destinoPos, Quaternion destinoRot, float duracion, float intensidad)
    {
        if (camara == null)
            yield break;

        Vector3 inicioPos = camara.position;
        Quaternion inicioRot = camara.rotation;

        float tiempo = 0f;
        duracionVibracion = duracion;
        intensidadVibracion = intensidad;

        while (tiempo < duracion)
        {
            float t = tiempo / duracion;
            camara.position = Vector3.Lerp(inicioPos, destinoPos, t);
            camara.rotation = Quaternion.Lerp(inicioRot, destinoRot, t);

            tiempo += Time.deltaTime;
            yield return null;
        }

        camara.position = destinoPos;
        camara.rotation = destinoRot;

        movimientoActual = null;

        // Si ya terminó la vibración también, desactiva el LateUpdate
        if (duracionVibracion <= 0)
            activo = false;
    }
}
