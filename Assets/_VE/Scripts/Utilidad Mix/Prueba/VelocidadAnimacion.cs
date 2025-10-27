using UnityEngine.UI;
using UnityEngine;

public class VelocidadAnimacion : MonoBehaviour
{
    [Header("Parámetros de velocidad")]
    public Slider sliderVelocidad;      // Control deslizante para ajustar la velocidad
    public float velocidadBase = 2f;    // Factor multiplicador

    [Header("Referencias")]
    public Animator animator;           // Referencia al Animator del modelo

    private void Start()
    {
        // Asignamos el evento del slider
        if (sliderVelocidad != null)
        {
            sliderVelocidad.onValueChanged.AddListener(OnCambiarVelocidad);
            OnCambiarVelocidad(sliderVelocidad.value); // Aplicar valor inicial
        }
    }

    private void OnCambiarVelocidad(float valor)
    {
        if (animator != null)
        {
            // Ajustar la velocidad del Animator
            animator.speed = valor * velocidadBase;
        }
    }
}
