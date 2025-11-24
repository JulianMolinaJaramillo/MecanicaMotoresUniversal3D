using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class CompartirValoresSlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Slider sliderCopiar;  // Referencia al slider con el que quiero compartir valores
    private Slider sliderPropio;
    private bool puedoActualiza;

    private void Awake()
    {
        sliderPropio = GetComponent<Slider>(); // Guardamos referencia al slider del script
    }

    // Update is called once per frame
    void Update()
    {
        if (puedoActualiza)
        {
            sliderCopiar.value = sliderPropio.value;
        }       
    }

    /// <summary>
    /// Metodo invocado al momento de manipular el slider
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        puedoActualiza = true;
    }

    /// <summary>
    /// Metodo invocado al momento de dejar de manipular el slider
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        puedoActualiza = false;
    }
}
