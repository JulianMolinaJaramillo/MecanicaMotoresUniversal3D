using System.Collections;
using UnityEngine;

public class ControlCamaraMotor : MonoBehaviour
{
    public Camera camara; // Camara objetivo
    public Transform posicionFrontal; // Posicion por defecto de la vista del motor    
    public Transform[] posicionesCamara; // Lista de posiciones de cámara para ejercer una rotacion
    public Transform[] posicionesCamaraArriba; // Lista de posiciones de cámara para ejercer una rotacion pero la vista desde arriba
    public Transform[] posicionesCamaraAbajo; // Lista de posiciones de cámara para ejercer una rotacion pero la vista desde abajo
    public Transform posicionExpansion;
    public float velocidadPos = 1; // Velocidad de desplazamiento
    [HideInInspector]
    public bool noMover; // Para validar si puedo o no mover la camara

    [HideInInspector]
    public int indiceActual = 0;  // Índice de la posición actual
    //[HideInInspector]
    public bool posicionadoArriba; // Para confirmar si estoy en las camaras sobre el motor
    //[HideInInspector]
    public bool posicionadoEnMedio; // Para confirmar si estoy en las camaras alrededor del motor
    //[HideInInspector]
    public bool posicionadoAbajo; // Para confirmar si estoy en las camaras debajo del motor
    private Coroutine miCoroutine;
    private Transform posicionActual;
    public static ControlCamaraMotor singleton;

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
    private void Update()
    {
        if (!ManagerMinijuego.singleton.minijuegoActivo && !noMover)
        {
            // Modificamos el near
            if (posicionadoAbajo)
            {
                camara.nearClipPlane = 0.35f;
            }
            else
            {
                camara.nearClipPlane = 0.01f;
            }

            // Validamos si presionamos las flechas de direccion del tecla o las teclas ASDW
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                if (posicionadoAbajo)
                {
                    IniciarMovimientoCamara(posicionesCamara[indiceActual], velocidadPos);
                    posicionadoAbajo = false;
                    posicionadoEnMedio = true;
                }
                else
                {
                    IniciarMovimientoCamara(posicionesCamaraArriba[indiceActual], velocidadPos);
                    posicionadoEnMedio = false;
                    posicionadoArriba = true;
                }            
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                if (!posicionadoAbajo)
                {
                    if (posicionadoEnMedio)
                    {
                        IniciarMovimientoCamara(posicionesCamaraAbajo[indiceActual], velocidadPos);
                        posicionadoAbajo = true;
                        posicionadoEnMedio = false;
                    }
                    else
                    {
                        IniciarMovimientoCamara(posicionesCamara[indiceActual], velocidadPos);
                        posicionadoEnMedio = true;
                    }
                    posicionadoArriba = false;
                }          
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                indiceActual = (indiceActual + 1) % posicionesCamara.Length;

                if (posicionadoArriba)
                {
                    IniciarMovimientoCamara(posicionesCamaraArriba[indiceActual], velocidadPos);
                }
                else if (posicionadoAbajo)
                {
                    IniciarMovimientoCamara(posicionesCamaraAbajo[indiceActual], velocidadPos);
                }
                else
                {
                    IniciarMovimientoCamara(posicionesCamara[indiceActual], velocidadPos);
                }
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                indiceActual = (indiceActual - 1 + posicionesCamara.Length) % posicionesCamara.Length;

                if (posicionadoArriba)
                {
                    IniciarMovimientoCamara(posicionesCamaraArriba[indiceActual], velocidadPos);
                }
                else if (posicionadoAbajo)
                {
                    IniciarMovimientoCamara(posicionesCamaraAbajo[indiceActual], velocidadPos);
                }
                else
                {
                    IniciarMovimientoCamara(posicionesCamara[indiceActual], velocidadPos);
                }                
            }
        }
        if (ManagerMinijuego.singleton.minijuegoActivo)
        {
            camara.nearClipPlane = 0.01f;
        }
    }
    [ContextMenu("reiniar")]
    public void ReestablecerPosicionCamara()
    {
        indiceActual = 0;
        posicionadoArriba = false;
        posicionadoAbajo = false;
        posicionadoEnMedio = true;
    }


    public void IniciarMovimientoCamara(Transform posicionDeseada, float duracion)
    {
        posicionActual = posicionDeseada; // guardamos la posicion actual
        if (miCoroutine != null)
        {
            StopCoroutine(miCoroutine);
        }

        miCoroutine = StartCoroutine(MoverCamara(posicionDeseada, duracion));
    }

    /// <summary>
    /// Currutina encargada del movimiento de la pieza suavizado
    /// </summary>
    /// <param name="posicionDeseada"> La posicion a la cual queremos moder la camara </param>
    /// <param name="duracion"> Tiempo del movimiento de la pieza </param
    private IEnumerator MoverCamara(Transform posicionDeseada, float duracion)
    {
        Vector3 posicionInicio = camara.transform.position; //  Guardamos la posicion de inicio
        Quaternion rotacionInicio = camara.transform.rotation; //  Guardamos la rotacion de inicio

        float tiempo = 0f; // Damos un tiempo para la interpolacion

        while (tiempo < duracion)
        {
            // Asignamos la posicion y rotacion de la camara, con interpolacion lineal
            camara.transform.position = Vector3.Lerp(posicionInicio, posicionDeseada.position, tiempo / duracion);
            camara.transform.rotation = Quaternion.Lerp(rotacionInicio, posicionDeseada.rotation, tiempo / duracion);

            tiempo += Time.deltaTime;
            yield return null;
        }

        camara.transform.position = posicionDeseada.position; // Aseguramos la posición final
        camara.transform.rotation = posicionDeseada.rotation; // Aseguramos la rotacion final

        if (ManagerMinijuego.singleton != null)
        {
            ManagerMinijuego.singleton.aplicandoTorque = false;
        }
    }
}
