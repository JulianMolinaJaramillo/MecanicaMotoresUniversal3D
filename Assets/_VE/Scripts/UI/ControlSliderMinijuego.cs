using UnityEngine;
using UnityEngine.EventSystems;

public class ControlSliderMinijuego : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    /// <summary>
    /// Metodo invocado al momento de manipular el slider
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (Atornillar.singleton != null)
        {
            if (InventarioUI.singleton.tamanoHerramienta == ManagerMinijuego.singleton.sizeHerramienta)
            {
                Atornillar.singleton.estaManipulando = true;
            }
            else
            {
                if (ManagerCanvas.singleton != null)
                {
                    if (AudioManager.singleton != null) AudioManager.singleton.PlayEfectString("Error"); // Ejecutamos el efecto nombrado
                    string texto = "Estas utilizando el tamaño de llave incorrecta, necesitas la llave de     "+ ManagerMinijuego.singleton.sizeHerramienta+ " mm, vuelve a intentarlo";
                    ManagerCanvas.singleton.AlertarMensaje(texto);
                }         
            }
        }          
    }

    /// <summary>
    /// Metodo invocado al momento de dejar de manipular el slider
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (Atornillar.singleton != null)
        {
            Atornillar.singleton.estaManipulando = false;
        }        
    }
}
