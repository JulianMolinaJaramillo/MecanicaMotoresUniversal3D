using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script adjunto a la mano que hace la animacion de atornillar
/// </summary>
public class Atornillar : MonoBehaviour
{
    public float anguloMaximo = 15f;
    public Slider sliderVelocidad;
    public TextMeshProUGUI torque;
    public RotacionAngularObjeto rotacionAngularObjeto;

    public float valorSliderActual;

    public static Atornillar singleton;

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

    void Update()
    {
        float valorConvertido = sliderVelocidad.value * 100f;
        torque.text = valorConvertido.ToString("F0");

        rotacionAngularObjeto.SetVelocidad(sliderVelocidad.value * 30);

        valorSliderActual = sliderVelocidad.value;
    }

    /// <summary>
    /// Metodo invocado desde btnCerrar en el mensaje de alerta del canvas
    /// </summary>
    public void ReestablecerValorSlider()
    {
        valorSliderActual = 0f;
        sliderVelocidad.value = valorSliderActual;
        float valorConvertido = sliderVelocidad.value * 100f;
        torque.text = valorConvertido.ToString("F0");    
    }

    public float AsignarValorTorque()
    {
        return valorSliderActual * 100;
    }

    public void ReiniciarValorSlider()
    {
        valorSliderActual = 0f;
        sliderVelocidad.value = 0f;
        torque.text = "0";
    }
}
