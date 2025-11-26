using UnityEngine;
using UnityEngine.UI;

public class LineaFrecuencia : MonoBehaviour
{
    public Slider sliderVelocidad;      // Control deslizante para ajustar la velocidad
    //public Slider sliderVelocidadSecundario;      // Control deslizante para ajustar la velocidad
    public int totalPuntos = 50;        // Número de puntos en la línea
    public float velocidad = 0.05f;   // Velocidad de actualización
    public float amplitud = 1f;        // Escala vertical general
    public float ruido = 0.1f;    // Pequeña variación aleatoria
    public float pico = 2f;    // Cuánto se eleva el pico
    public float picoIntervalo = 2f;    // Cada cuánto ocurre un pico (en segundos)
    public float velocidadMovimiento = 10f; // Velocidad con la que la línea se desplaza a la izquierda
    public bool multiplicarRuidoX2;

    private LineRenderer line;
    private Vector3[] puntos;
    private float tiempo;
    private float acumuladorDesplazamiento;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = totalPuntos;
        puntos = new Vector3[totalPuntos];

        // Inicializamos la línea plana
        for (int i = 0; i < totalPuntos; i++)
            puntos[i] = new Vector3(i * 0.1f, 0, 0);

        line.SetPositions(puntos);

        sliderVelocidad.onValueChanged.AddListener(ActualizarValores);
        //sliderVelocidadSecundario.onValueChanged.AddListener(ActualizarValores);
    }

    void Update()
    {
        tiempo += Time.deltaTime;
        acumuladorDesplazamiento += Time.deltaTime * velocidadMovimiento;

        // Cuando acumulamos suficiente desplazamiento, movemos un punto a la izquierda
        if (acumuladorDesplazamiento >= 0.1f) // cada "salto" horizontal equivale a 0.1 en X
        {
            acumuladorDesplazamiento = 0f;

            // Desplazar todos los puntos hacia la izquierda
            for (int i = 0; i < totalPuntos - 1; i++)
                puntos[i].y = puntos[i + 1].y;

            // Generar nuevo valor (puede venir de tu float)
            float newY = Random.Range(-ruido, ruido);

            // Cada cierto tiempo genera un "pico"
            if (tiempo >= picoIntervalo)
            {
                newY = pico;
                tiempo = 0;
            }

            // Insertar nuevo punto al final
            puntos[totalPuntos - 1].y = Mathf.Lerp(puntos[totalPuntos - 2].y, newY, 0.5f);
        }

        // Actualizar las posiciones horizontales
        for (int i = 0; i < totalPuntos; i++)
            puntos[i].x = i * 0.1f;

        line.SetPositions(puntos);
    }

    public void ActualizarValores(float valor)
    {
        if (ManagerMinijuego.singleton.minijuegoValidadoCorrectamente)
        {
            if (multiplicarRuidoX2)
            {
                ruido = valor * 2;
                pico = valor;
            }
            else
            {
                ruido = valor;
                pico = valor;
            }
        }          
    }
}
