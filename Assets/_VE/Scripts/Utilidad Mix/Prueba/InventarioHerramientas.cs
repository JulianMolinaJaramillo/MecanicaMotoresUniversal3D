using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestiona qué herramienta está actualmente en uso y cómo se acoplan las piezas.
/// No depende de la cámara, solo de un punto fijo de ensamblaje en la escena.
/// </summary>
public class InventarioHerramientas : MonoBehaviour
{
    [Header("Herramienta actualmente en uso (base del ensamblaje)")]
    public HerramientaArmable herramientaActiva; // Herramienta principal que el jugador está ensamblando
    public List<HerramientaArmable> herramientasTomadas; // Herramienta principal que el jugador está ensamblando
    public List<GameObject> herramientasIndividuales = new List<GameObject>();

    [Header("Punto fijo en el entorno donde se colocan las herramientas")]
    public Transform puntoArmado; // Objeto vacío en la escena que actúa como mesa de ensamblaje

    public static InventarioHerramientas singleton;
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

    /// <summary>
    /// Método llamado por ToolComponent cuando se hace clic sobre una herramienta.
    /// </summary>
    /// <param name="tool">Herramienta clickeada</param>
    public void ClickHerramienta(HerramientaArmable tool)
    {
        // Si no hay herramienta en uso, tomamos esta como base
        if (herramientaActiva == null && tool.piezaInicial)
        {
            herramientasTomadas.Add(tool);
            ColocarHerramienta(tool);
        }
        else
        {
            if (herramientaActiva != null)
            {
                // Si ya hay herramienta, intentamos acoplar la nueva
                if (herramientaActiva.puedoUnir(tool))
                {
                    herramientasTomadas.Add(tool);
                    tool.transform.SetParent(null); // Por si estaba en otro objeto
                    herramientaActiva.Unir(tool);
                    herramientaActiva = tool; // actualizamos la herramienta actualmente seleccionado

                    if (tool.piezaFinal)
                    {
                        InventarioUI.singleton.AgregarHerramientaInventario(tool.icono, tool.nombreHerramientaImagen, tool.sizeHerramienta);
                        ManagerCanvas.singleton.BorrarInformacionPieza(); // Retiramos la informacion de la pieza del canvas
                    }
                }
                else
                {
                    Debug.Log("No es posible acoplar esta herramienta.");
                }
            }
            else
            {
                Debug.Log("Necesitas primero la base de la herramienta");
            }
        }

        if (herramientasIndividuales.Count > 0)
        {
            herramientasIndividuales[0].SetActive(true); // Reactivamos la herramienta antes desactivada
            herramientasIndividuales.Clear();// limpiamos de herramientas
            InventarioUI.singleton.ReestablecerHerramientaInventario();
        }
        
    }

    /// <summary>
    /// Coloca una herramienta en el punto de ensamblaje como herramienta principal.
    /// </summary>
    /// <param name="tool">Herramienta a colocar</param>
    void ColocarHerramienta(HerramientaArmable tool)
    {
        herramientaActiva = tool;
        StartCoroutine(ColocarHerramientaSuavemente(tool));
    }

    /// <summary>
    /// Mueve la herramienta suavemente al punto de ensamblaje.
    /// </summary>
    /// <param name="tool">Herramienta que se moverá</param>
    IEnumerator ColocarHerramientaSuavemente(HerramientaArmable tool)
    {
        float duration = 0.5f; // Duración del movimiento en segundos
        float elapsed = 0f;

        // Guardamos posición y rotación inicial
        Vector3 startPos = tool.transform.position;
        Quaternion startRot = tool.transform.rotation;

        // Mientras no se alcance la duración total, interpolamos
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Interpolación suave
            tool.transform.position = Vector3.Lerp(startPos, puntoArmado.position, t);
            tool.transform.rotation = Quaternion.Lerp(startRot, puntoArmado.rotation, t);

            yield return null;
        }

        // Ajustamos jerarquía y valores finales
        tool.transform.SetParent(puntoArmado);
        tool.transform.localPosition = Vector3.zero;
        tool.transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// Restaura TODAS las herramientas de la escena a sus posiciones originales.
    /// </summary>
    [ContextMenu("restaurar")]
    public void ReactivarHerramientasTomadas()
    {
        for (int i = 0; i < herramientasTomadas.Count; i++)
        {
            herramientasTomadas[i].RestaurarPosicionOriginal();
        }
        herramientasTomadas.Clear();
        herramientaActiva = null;

        Debug.Log("Todas las herramientas han vuelto a su posición original.");
    }

    public void ReactivarHerramientasIndividuales()
    {
        if (AudioManager.singleton != null) AudioManager.singleton.PlayEfectString("Pop"); // Ejecutamos el efecto nombrado 

        // Reestablecemos las herramientas que hayamos tomado
        for (int i = 0; i < herramientasIndividuales.Count; i++)
        {
            herramientasIndividuales[i].SetActive(true);
        }
    }

    /// <summary>
    /// Metodo invocado desde btnSoltarHerramienta en el canvas
    /// </summary>
    public void ReactivarTodo()
    {
        ReactivarHerramientasTomadas();
        ReactivarHerramientasIndividuales();
    }

}

