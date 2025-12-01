using UnityEngine;

public class RotadorPiezas : MonoBehaviour
{
    // Velocidad de rotación en grados por segundo
    public float velocidadRotacion = 50f;
    public bool rotarEnZ, rotarEnY, rotarEnX;
    public float velocidadRetorno = 2f; // Velocidad del retorno suave
    public GameObject btnRotar, btnNoRotar;
    public GestorPiezas gestorPiezas;
    [HideInInspector]
    public bool dejarDeRotar;

    private Quaternion rotacionInicial;
    [HideInInspector]
    public bool regresandoARotacionOriginal = false;

    void Start()
    {
        // Guardar la rotación inicial
        rotacionInicial = transform.rotation;
    }

    void Update()
    {
        if (regresandoARotacionOriginal)
        {
            // Interpolación suave hacia la rotación original
            transform.rotation = Quaternion.Lerp(transform.rotation, rotacionInicial, Time.deltaTime * velocidadRetorno);

            // Opcional: detener cuando esté lo suficientemente cerca
            if (Quaternion.Angle(transform.rotation, rotacionInicial) < 0.1f)
            {
                transform.rotation = rotacionInicial;
                regresandoARotacionOriginal = false;
                btnRotar.SetActive(true);
                btnNoRotar.SetActive(false);

                if (MesaMotor.singleton != null)
                {
                    MesaMotor.singleton.motorRotando = false;

                    if (!MesaMotor.singleton.motorExpandido)
                    {
                        if (ManagerMinijuego.singleton.minijuegoValidadoCorrectamente)
                        {
                            if (ExplosionObjetosHijos.singleton != null) ExplosionObjetosHijos.singleton.DesactivarHijos(ExplosionObjetosHijos.singleton.objetosPadres[1]); // Activamos los hijos antes de expandir
                            ManagerMinijuego.singleton.motorAnimadoActivo.SetActive(true); // Activamos motor animado luego de expandir  
                        }                     
                    }              
                }
            }

            return;
        }

        if (!dejarDeRotar)
        {
            if (rotarEnZ)
            {
                // Rotar el objeto que tenga el script alrededor del eje Z
                transform.Rotate(0, 0, velocidadRotacion * Time.deltaTime);
            }
            else if (rotarEnY)
            {
                // Rotar el objeto que tenga el script alrededor del eje Y
                transform.Rotate(0, velocidadRotacion * Time.deltaTime, 0);
            }
            else if (rotarEnX)
            {
                // Rotar el objeto que tenga el script alrededor del eje X
                transform.Rotate(velocidadRotacion * Time.deltaTime, 0, 0);
            }
        }    
    }

    /// <summary>
    /// Metodo para retornar a la rotacion original
    /// </summary>
    public void RegresarARotacionOriginal()
    {
        rotarEnY = false;
        rotarEnX = false;
        rotarEnZ = false;
        regresandoARotacionOriginal = true;
    }

    public void RotarEnX()
    {
        IndicarRotacion();
        gestorPiezas.TransferirPiezasColocadas();
        rotarEnY = false;
        rotarEnZ = false;
        rotarEnX = true;
    }

    public void RotarEnY()
    {
        IndicarRotacion();
        gestorPiezas.TransferirPiezasColocadas();

        if (ManagerMinijuego.singleton != null && ManagerMinijuego.singleton.motorAnimadoActivo != null) ManagerMinijuego.singleton.motorAnimadoActivo.SetActive(false); // Desactivamos motor animado antes de expandir
        if (ExplosionObjetosHijos.singleton != null) ExplosionObjetosHijos.singleton.ActivarHijos(ExplosionObjetosHijos.singleton.objetosPadres[1]); // Activamos los hijos antes de expandir

        rotarEnX = false;
        rotarEnZ = false;
        rotarEnY = true;
    }

    public void RotarEnZ()
    {
        IndicarRotacion();
        gestorPiezas.TransferirPiezasColocadas();
        rotarEnY = false;
        rotarEnX = false;
        rotarEnZ = true;
    }

    public void RotarEnTodosLosEjes()
    {
        IndicarRotacion();
        gestorPiezas.TransferirPiezasColocadas();
        rotarEnY = true;
        rotarEnX = true;
        rotarEnZ = true;
    }

    public void IndicarRotacion()
    {
        if (MesaMotor.singleton != null)
        {
            MesaMotor.singleton.motorRotando = true;
        }
    }
}
