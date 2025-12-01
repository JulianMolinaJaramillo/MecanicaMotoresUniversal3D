using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class btnInventario : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image imagenIcono;
    public GameObject prebafInstancia; //Prefab a instanciar
    public Transform posicionInstancia; // Punto en el que vamos a realizar la instancia
    public string nombre,descripcion;
    /// <summary>
    /// Metodo utilizado para instanciar las piezas que vamos a armar en el punto deseado
    /// </summary>
    public void InstanciarPiezaMotor()
    {
        if (MesaMotor.singleton.mesaMotorActiva) // Solo instanciamos si estamos en la mesa de armado
        {
            if (AudioManager.singleton != null) AudioManager.singleton.PlayEfectString("AparecerPieza"); // Ejecutamos el efecto nombrado

            // Generar variación aleatoria en cada eje
            float offsetX;
            if (Random.value < 0.5f)
            {
                offsetX = Random.Range(-0.5f, -0.2f); // Lado izquierdo
            }
            else
            {
                offsetX = Random.Range(0.2f, 0.5f);   // Lado derecho
            }
            float offsetY = Random.Range(-0.4f, 0.1f);
            float offsetZ = Random.Range(-0.1f, 0.2f);

            // Crear una nueva posición basada en los cambios de los ejes
            Vector3 offsetLocal = new Vector3(offsetX, offsetY, offsetZ);

            // Instanciar como hijo de la posición deseada (ya con la rotación del prefab)
            GameObject nuevaPieza = Instantiate(prebafInstancia, posicionInstancia);

            // Seteamos la posicion inicil en 0
            nuevaPieza.transform.localPosition = Vector3.zero;

            // Aplicar desplazamiento en espacio local respecto al padre
            nuevaPieza.transform.localPosition += offsetLocal;

            InventarioUI.singleton.contadorInstancias -= 1; // Liberamos espacio en el inventario
            Destroy(this.gameObject); // Destruimos el boton
        }
        else
        {
            string texto = "Debes ir primero a la mesa de armado para colocar la pieza";
            ManagerCanvas.singleton.AlertarMensaje(texto);
        }
    }

    /// <summary>
    /// Metodo utilizado para actualizar la informacion de la pieza, Se llama automáticamente cuando el mouse entra en el botón
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        ManagerCanvas.singleton.ActualizarInformacionPieza(nombre, descripcion); // Actualizamos la informacion de la pieza en el canvas
    }

    /// <summary>
    /// Metodo utilizado para borrar la informacion de la pieza, Se llama automáticamente cuando el mouse sale del botón
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        ManagerCanvas.singleton.BorrarInformacionPieza(); // Retiramos la informacion de la pieza del canvas
    }

    /// <summary>
    /// Metodo utilizado para borrar la informacion de la pieza, Se llama automáticamente cuando se hace click
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        ManagerCanvas.singleton.BorrarInformacionPieza(); // Retiramos la informacion de la pieza del canvas
    }

}
