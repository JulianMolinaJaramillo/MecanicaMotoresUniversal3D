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

    [HideInInspector] 
    public bool estaManipulando = false;

    private RectTransform rectTransform;
    private float anguloInicial;
    private float valorSliderActual;
    private float tiempoAnimacion = 0f;

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

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        anguloInicial = rectTransform.localEulerAngles.z;
    }

    void Update()
    {
        if (!estaManipulando) return;

        float velocidadActual = Mathf.Lerp(1f, 20f, sliderVelocidad.value);
        tiempoAnimacion += Time.deltaTime * velocidadActual;

        float rotacionZ = Mathf.Sin(tiempoAnimacion) * anguloMaximo;
        rectTransform.localRotation = Quaternion.Euler(0, 0, anguloInicial + rotacionZ);

        float valorConvertido = sliderVelocidad.value * 100f;
        torque.text = valorConvertido.ToString("F0");

        valorSliderActual = sliderVelocidad.value;
    }

    /// <summary>
    /// Metodo invocado desde btnCerrar en el mensaje de alerta del canvas
    /// </summary>
    public void ReestablecerValorSlider()
    {
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
