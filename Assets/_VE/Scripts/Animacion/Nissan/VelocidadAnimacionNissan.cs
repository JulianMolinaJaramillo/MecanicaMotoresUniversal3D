using UnityEngine.UI;
using UnityEngine;

public class VelocidadAnimacionNissan : MonoBehaviour
{
    [Header("Parámetros de velocidad")]
    public Slider sliderVelocidad;      // Control deslizante para ajustar la velocidad
    public float velocidadBase = 2f;    // Factor multiplicador

    [Header("Referencias")]
    public Animator animator;           // Referencia al Animator del modelo
    public MoverPiezaNissan[] retenedores;
    public MoverPiezaNissan[] resortes;
    public MoverPiezaNissan[] guias;
    public MoverPiezaNissan[] valvulas;
    public RotacionObjeto[] objetosRotatorios;
    public ParticleSystem[] explosionPiston;

    private void Start()
    {
        // Asignamos el evento del slider
        if (sliderVelocidad != null)
        {
            sliderVelocidad.onValueChanged.AddListener(OnCambiarVelocidad);
            OnCambiarVelocidad(sliderVelocidad.value); // Aplicar valor inicial
        }
    }

    private void Update()
    {
        if (sliderVelocidad.value >= 0.4f)
        {
            for (int i = 0; i < explosionPiston.Length; i++)
            {
                if (!explosionPiston[i].isPlaying)
                {
                    explosionPiston[i].Play();
                }
            }        
        }
    }

    private void OnCambiarVelocidad(float valor)
    {
        if (animator != null)
        {
            // Ajustar la velocidad del Animator
            animator.speed = valor * velocidadBase;

            for (int i = 0; i < objetosRotatorios.Length; i++)
            {
                objetosRotatorios[i].velocidadRotacion = sliderVelocidad.value * 1000;
            }

            if (valor >= 0.4f)
            {
                for (int i = 0; i < retenedores.Length; i++)
                {
                    //Retenedores
                    retenedores[i].velocidadMovimiento = 0.09f;
                    retenedores[i].tiempoFrecuencia = 0.1f;
                    //Resortes
                    resortes[i].velocidadMovimiento = 0.1f;
                    resortes[i].tiempoFrecuencia = 0.01f;
                    //Guias
                    guias[i].velocidadMovimiento = 0.2f;
                    guias[i].tiempoFrecuencia = 0.01f;
                    //Valvulas
                    valvulas[i].velocidadMovimiento = 0.1f;
                    valvulas[i].tiempoFrecuencia = 0.01f;
                }
            }
            else
            {
                for (int i = 0; i < retenedores.Length; i++)
                {
                    //Retenedores
                    retenedores[i].velocidadMovimiento = 0.6f;
                    retenedores[i].tiempoFrecuencia = 0.1f;
                    //Resortes
                    resortes[i].velocidadMovimiento = 0.5f;
                    resortes[i].tiempoFrecuencia = 0.1f;
                    //Guias
                    guias[i].velocidadMovimiento = 0.7f;
                    guias[i].tiempoFrecuencia = 0.1f;
                    //Valvulas
                    valvulas[i].velocidadMovimiento = 0.5f;
                    valvulas[i].tiempoFrecuencia = 0.1f;
                }
            }
        }
    }
}
