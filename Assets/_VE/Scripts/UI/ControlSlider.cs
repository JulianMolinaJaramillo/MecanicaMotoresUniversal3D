using UnityEngine;
using UnityEngine.EventSystems;

public class ControlSlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Atornillar manoAnimada;  // arrastra aquí la imagen con el script de la mano

    /// <summary>
    /// Metodo invocado al momento de manipular el slider
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (manoAnimada != null)
            manoAnimada.estaManipulando = true;
    }

    /// <summary>
    /// Metodo invocado al momento de dejar de manipular el slider
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (manoAnimada != null)
            manoAnimada.estaManipulando = false;
    }
}
