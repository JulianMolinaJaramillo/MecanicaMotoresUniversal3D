using System.Collections;
using UnityEngine;

public class ControlCamaraMotor : MonoBehaviour
{
    public Transform camara; // Camara objetivo
    public Transform posicionDown; // Posicion por defecto de la vista del motor
    public Transform[] posicionesCamara; // Lista de posiciones de cámara para ejercer una rotacion
    public Transform[] posicionesCamaraUp; // Lista de posiciones de cámara para ejercer una rotacion pero la vista desde arriba
    public Transform posicionExpansion;
    public float velocidadPos = 1; // Velocidad de desplazamiento

    private int indiceActual = 0;  // Índice de la posición actual
    private Coroutine miCoroutine;
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
        if (!ManagerMinijuego.singleton.minijuegoActivo)
        {  
            // Validamos si presionamos las flechas de direccion del tecla o las teclas ASDW
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                IniciarMovimientoCamara(posicionesCamaraUp[indiceActual], velocidadPos);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                IniciarMovimientoCamara(posicionesCamara[indiceActual], velocidadPos);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                indiceActual = (indiceActual + 1) % posicionesCamara.Length;
                IniciarMovimientoCamara(posicionesCamara[indiceActual], velocidadPos);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                indiceActual = (indiceActual - 1 + posicionesCamara.Length) % posicionesCamara.Length;
                IniciarMovimientoCamara(posicionesCamara[indiceActual], velocidadPos);      
            }
        }
    }


    public void IniciarMovimientoCamara(Transform posicionDeseada, float duracion)
    {
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
