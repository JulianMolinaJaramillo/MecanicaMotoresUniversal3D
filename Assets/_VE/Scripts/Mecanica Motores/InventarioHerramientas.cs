using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    public void ClickHerramienta(HerramientaArmable tool, Collider collider)
    {
        // Si es otra pieza inicial y ya tenia una anterior, reposiciono antes
        if (herramientaActiva != null && tool.piezaInicial && tool.tipoHerramienta == ToolType.Rachet)
        {
            ReactivarHerramientasTomadas();
        }

        // Si no hay herramienta en uso, tomamos esta como base
        if (herramientaActiva == null && tool.piezaInicial)
        {
            herramientasTomadas.Add(tool);
            ColocarHerramienta(tool);
            collider.enabled = false;
        }
        else
        {
            if (herramientaActiva != null)
            {
                // Si llega una copa, cuando ya tengo otra seleccionada
                if (tool.tipoHerramienta == herramientasTomadas[herramientasTomadas.Count - 1].tipoHerramienta)
                {
                    RestaurarCopas();
                }

                if (herramientasTomadas.Count > 1 && herramientasTomadas[1] != null)
                {
                    // Si llega una cabeza, cuando ya tengo otra seleccionada
                    if (tool.tipoHerramienta == herramientasTomadas[1].tipoHerramienta && tool.tipoHerramienta == ToolType.Head)
                    {
                        RestaurarCabezas();
                    }
                }
                

                // Si ya hay herramienta, intentamos acoplar la nueva
                if (herramientaActiva.puedoUnir(tool))
                {
                    collider.enabled = false;
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
                    string texto = "No es posible acoplar esta herramienta.";
                    ManagerCanvas.singleton.AlertarMensaje(texto);
                }
            }
            else
            {
                string texto = "Necesitas primero la base de la herramienta.";
                ManagerCanvas.singleton.AlertarMensaje(texto);
            }
        }

        //Para activar nuevamente las herramientas que son tomadas una sola vez
        if (herramientasIndividuales.Count > 0 && herramientaActiva != null)
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
            herramientasTomadas[i].RestaurarPosicionActual();
            herramientasTomadas[i].collider.enabled = true;
        }
        herramientasTomadas.Clear();
        herramientaActiva = null;
    }

    /// <summary>
    /// Metodo invocado si se intentan personalizar las copas, teniendo las mismas seleccionadas
    /// </summary>
    public void RestaurarCopas()
    {
        herramientasTomadas[herramientasTomadas.Count - 1].RestaurarPosicionActual();
        herramientasTomadas[herramientasTomadas.Count - 1].collider.enabled = true;
        herramientasTomadas.RemoveAt(herramientasTomadas.Count - 1);

        //Solo asignmos la herramienta activa, si ya hemos tomado herramientas antes
        if (herramientasTomadas.Count > 0)
        {
            herramientaActiva = herramientasTomadas[herramientasTomadas.Count - 1];
        }     
    }

    /// <summary>
    /// Metodo invocado si se intentan personalizar las cabezas, teniendo las mismas seleccionadas
    /// </summary>
    public void RestaurarCabezas()
    {
        for (int i = 1; i < herramientasTomadas.Count; i++)
        {
            herramientasTomadas[i].RestaurarPosicionActual();     
            herramientasTomadas[i].collider.enabled = true;
        }

        // Removemos de atras hacia adelante
        for (int i = herramientasTomadas.Count - 1; i >= 1; i--)
        {
            herramientasTomadas.RemoveAt(i);
        }

        herramientaActiva = herramientasTomadas[0];
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

