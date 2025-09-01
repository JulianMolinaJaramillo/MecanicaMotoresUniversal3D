using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class BotonControlador : MonoBehaviour
{
    [Header("Botones que quiero afectar")]
    public Button[] botonesAfectados;

    private float[] alfasOriginales;                // Para guardar los alfas originales
    private bool[] interactuablesOriginales;        // Para guardar el estado interactuable original

    public bool pasarPagina;
    public bool puedoValidar;
    private Coroutine coroutine;
    void Start()
    {
        // Inicializamos arrays para guardar valores originales
        alfasOriginales = new float[botonesAfectados.Length];
        interactuablesOriginales = new bool[botonesAfectados.Length];

        for (int i = 0; i < botonesAfectados.Length; i++)
        {
            if (botonesAfectados[i] == null) continue;

            // Guardar alfa
            Image img = botonesAfectados[i].GetComponent<Image>();
            if (img != null)
                alfasOriginales[i] = img.color.a;

            // Guardar interactuable
            interactuablesOriginales[i] = botonesAfectados[i].interactable;
        }

        if (pasarPagina)
        {
            // Suscribimos este botón a ejecutar la función cuando se clickea
            GetComponent<Button>().onClick.AddListener(RestaurarBotones);
        }
        else
        {
            // Suscribimos este botón a ejecutar la función cuando se clickea
            GetComponent<Button>().onClick.AddListener(DesactivarBotones);
        }
        
    }

    /// <summary>
    /// Método que recorre todos los botones y los desactiva, además baja el alfa a la mitad y desactiva BotonHide
    /// </summary>
    public void DesactivarBotones()
    {
        if (!puedoValidar)
        {
            puedoValidar = true;
            for (int i = 0; i < botonesAfectados.Length; i++)
            {
                if (botonesAfectados[i] == null) continue;

                // Desactivar botón
                botonesAfectados[i].interactable = false;

                // Bajar alfa
                Image img = botonesAfectados[i].GetComponent<Image>();
                if (img != null)
                {
                    Color c = img.color;
                    c.a = alfasOriginales[i] * 0.5f; // la mitad del valor original
                    img.color = c;
                }

                if (coroutine != null) StopCoroutine(coroutine);
                coroutine = StartCoroutine(Restaurar());
            }
        }
    }

    private IEnumerator Restaurar()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < botonesAfectados.Length; i++)
        {
            if (botonesAfectados[i] == null) continue;

            // Restaurar interactuable
            botonesAfectados[i].interactable = interactuablesOriginales[i];

            BotonControlador botonControlador = botonesAfectados[i].GetComponent<BotonControlador>();
            botonControlador.puedoValidar = false;

            // Restaurar alfa
            Image img = botonesAfectados[i].GetComponent<Image>();
            if (img != null)
            {
                Color c = img.color;
                c.a = alfasOriginales[i];
                img.color = c;
            }
        }
    }

    public void RestaurarBotones()
    {
        for (int i = 0; i < botonesAfectados.Length; i++)
        {
            if (botonesAfectados[i] == null) continue;

            // Restaurar interactuable
            botonesAfectados[i].interactable = interactuablesOriginales[i];

            BotonControlador botonControlador = botonesAfectados[i].GetComponent<BotonControlador>();
            botonControlador.puedoValidar = false;

            // Restaurar alfa
            Image img = botonesAfectados[i].GetComponent<Image>();
            if (img != null)
            {
                Color c = img.color;
                c.a = alfasOriginales[i];
                img.color = c;
            }
        }
    }
}
