using UnityEngine;
using UnityEngine.EventSystems;

public class ControlSliderMinijuego : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public RotacionAngularObjeto[] rotacionAngularObjeto;
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
                for (int i = 0; i < rotacionAngularObjeto.Length; i++)
                {
                    rotacionAngularObjeto[i].estaManipulando = true;
                }
            }
            else
            {
                if (ManagerCanvas.singleton != null)
                {
                    if (AudioManager.singleton != null) AudioManager.singleton.PlayEfectString("Error"); // Ejecutamos el efecto nombrado

                    if (ManagerMinijuego.singleton.sizeHerramienta == 1)
                    {
                        string texto = "Estas utilizando la herramienta incorrecta, necesitas prensa de valvulas para generar presión ";
                        ManagerCanvas.singleton.AlertarMensaje(texto);
                    }
                    else
                    {
                        string texto = "Estas utilizando el tamaño de herramienta incorrecto, necesitas la llave o copa de " + ManagerMinijuego.singleton.sizeHerramienta + " mm, vuelve a intentarlo";
                        ManagerCanvas.singleton.AlertarMensaje(texto);
                    }               
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
            for (int i = 0; i < rotacionAngularObjeto.Length; i++)
            {
                rotacionAngularObjeto[i].estaManipulando = false;
            }
        }        
    }
}
