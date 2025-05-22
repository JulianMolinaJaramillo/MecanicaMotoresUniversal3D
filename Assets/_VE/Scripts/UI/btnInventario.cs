
using UnityEngine;
using UnityEngine.EventSystems;

public class btnInventario : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject prebafInstancia; //Prefab a instanciar
    public Transform posicionInstancia; // Punto en el que vamos a realizar la instancia
    public string nombre,descripcion;
    /// <summary>
    /// Metodo utilizado para instanciar las piezas que vamos a armar en el punto deseado
    /// </summary>
    public void InstanciarPiezaMotor()
    {
        if (MesaMotor.singleton.mesaMotorActiva)
        {
            //Instantiate(prebafInstancia, posicionInstancia.position, prebafInstancia.transform.rotation); // Instanciar sin ser hijos de ningun objeto
            Instantiate(prebafInstancia, posicionInstancia); // Instanciar dentro de un objeto como hijos
            Destroy(this.gameObject);
        }       
    }

    /// <summary>
    /// Se llama automáticamente cuando el mouse entra en el botón
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        InformacionUI.singleton.ActualizarInformacionPieza(nombre, descripcion); // Actualizamos la informacion de la pieza en el canvas
    }

    /// <summary>
    /// Se llama automáticamente cuando el mouse sale del botón
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        InformacionUI.singleton.BorrarInformacionPieza(); // Retiramos la informacion de la pieza del canvas
    }

}
