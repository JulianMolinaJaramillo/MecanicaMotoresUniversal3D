using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControlSliderVelocidadMotor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public ControlVelocidadAnimacion controlVelocidadAnimacion;
    public TextMeshProUGUI txtbotonEncender;
    public float duracionApagado;

    private Slider sliderVelocidad;
    private bool motorEncendido;
    private bool apagandoMotor;
    private Coroutine coroutine;

    private void Awake()
    {
        sliderVelocidad = GetComponent<Slider>();
    }

    private void Update()
    {
        if (motorEncendido && sliderVelocidad.value == 0f)
        {
            sliderVelocidad.interactable = false;
            sliderVelocidad.value = 0f; // aseguramos el valor final
            txtbotonEncender.text = "Encender";
            motorEncendido = false;
        }
    }

    /// <summary>
    /// Metodo invocado al momento de manipular el slider
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!apagandoMotor)
        {
            controlVelocidadAnimacion.puedoValidar = true;
        }
        
    }

    /// <summary>
    /// Metodo invocado al momento de dejar de manipular el slider
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!apagandoMotor)
        {
            controlVelocidadAnimacion.puedoValidar = false;
        }    
    }

    /// <summary>
    /// Metodo invocado desde btnEncenderMotor en el canvas principal
    /// </summary>
    public void EncenderMotor()
    {
        if (ManagerMinijuego.singleton.minijuegoValidadoCorrectamente)
        {
            if (!motorEncendido)
            {
                sliderVelocidad.interactable = true;
                sliderVelocidad.value = 0.1f;
                txtbotonEncender.text = "Apagar";
                controlVelocidadAnimacion.puedoValidar = true;
            }
        }
        else
        {
            sliderVelocidad.value = 0.1f;
        }
    }

    /// <summary>
    /// Metodo invocado desde btnEncenderMotor en el canvas principal
    /// </summary>
    public void ApagarMotor()
    {
        if (ManagerMinijuego.singleton.minijuegoValidadoCorrectamente)
        {
            if (motorEncendido)
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
                apagandoMotor = true;
                coroutine = StartCoroutine(BajarValorSlider());
                return;
            }
            motorEncendido = true;
        }     
    }

     private IEnumerator BajarValorSlider()
    {
        controlVelocidadAnimacion.puedoValidar = true;
        sliderVelocidad.interactable = false;
        txtbotonEncender.text = "Apagando";

        float inicial = sliderVelocidad.value;
        float tiempo = 0f;

        while (tiempo < duracionApagado)
        {
            sliderVelocidad.value = Mathf.Lerp(inicial, 0f, tiempo / duracionApagado);
            tiempo += Time.deltaTime;
            yield return null;
        }

        
        controlVelocidadAnimacion.puedoValidar = false;
        sliderVelocidad.value = 0f; // aseguramos el valor final
        txtbotonEncender.text = "Encender";
        motorEncendido = false;
        apagandoMotor = false;
    }
}
