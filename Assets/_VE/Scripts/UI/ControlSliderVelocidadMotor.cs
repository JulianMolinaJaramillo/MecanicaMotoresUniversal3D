using UnityEngine;
using UnityEngine.EventSystems;

public class ControlSliderVelocidadMotor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public ControlVelocidadAnimacion controlVelocidadAnimacion;
    /// <summary>
    /// Metodo invocado al momento de manipular el slider
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        controlVelocidadAnimacion.puedoValidar = true;
    }

    /// <summary>
    /// Metodo invocado al momento de dejar de manipular el slider
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        controlVelocidadAnimacion.puedoValidar = false;
    }
}
