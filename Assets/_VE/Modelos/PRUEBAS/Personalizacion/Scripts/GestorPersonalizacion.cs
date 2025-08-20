using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Mantiene el estado actual de selección y muestra el prefab correcto
/// según (Raza, Morfologia, Atuendo). Permite elegir en cualquier orden.
/// Si se elige algo antes de la raza, usa una raza por defecto.
/// </summary>
public class GestorPersonalizacion : MonoBehaviour
{
    [Header("Catálogo de combinaciones")]
    [Tooltip("Arrastra aquí TODOS los ConfigPersonaje que crees (uno por combinación).")]
    public List<ConfigPersonaje> configuraciones = new List<ConfigPersonaje>();

    public Texture[] texturasDisponibles;
    private int indiceTextura = 0;

    [Header("Instanciación")]
    [Tooltip("Dónde se instanciará el modelo. Si se deja vacío, usará el transform de este GameObject.")]
    public Transform spawnRoot;

    [Header("Valores por defecto")]
    [Tooltip("Si el usuario elige morfología o atuendo primero, se usará esta raza por defecto.")]
    public Raza razaPorDefecto = Raza.Humano;
    [Tooltip("Estado inicial al arrancar (opcional).")]
    public Morfologia morfologiaInicial = Morfologia.Normal;
    public Atuendo atuendoInicial = Atuendo.SinAtuendo;
    public Sexo sexoPorDefecto = Sexo.Hombre;
    public bool instanciarAlIniciar = true;

    // --- Estado actual ---
    public Raza RazaActual { get; private set; }
    public Morfologia MorfologiaActual { get; private set; }
    public Atuendo AtuendoActual { get; private set; }

    public Sexo SexoActual { get; private set; }

    // Instancia viva del personaje
    private GameObject personajeActual;

    // Mapa para acceso O(1) por clave (raza_morfologia_atuendo)
    private Dictionary<string, GameObject> mapaPrefabs;

    private string Clave(Raza r, Morfologia m, Atuendo a, Sexo s) => $"{r}_{m}_{a}_{s}";

    private void Awake()
    {
        if (spawnRoot == null) spawnRoot = this.transform;

        // Construir diccionario
        mapaPrefabs = new Dictionary<string, GameObject>();
        foreach (var cfg in configuraciones.Where(c => c != null && c.prefab != null))
        {
            var key = Clave(cfg.raza, cfg.morfologia, cfg.atuendo, cfg.sexo);
            if (!mapaPrefabs.ContainsKey(key))
                mapaPrefabs.Add(key, cfg.prefab);
            else
                Debug.LogWarning($"Clave duplicada {key}. Revisa configuraciones repetidas.");
        }

        // Estado inicial
        RazaActual = razaPorDefecto;
        MorfologiaActual = morfologiaInicial;
        AtuendoActual = atuendoInicial;
        SexoActual = sexoPorDefecto;

        if (instanciarAlIniciar)
            ActualizarPersonaje(); // Mostrar primer modelo válido
    }

    public void SeleccionarSexo(Sexo nuevoSexo)
    {
        SexoActual = nuevoSexo;
        ActualizarPersonaje();
    }

    /// <summary>
    /// Seleccionar Raza. Actualiza el modelo manteniendo morfología y atuendo actuales.
    /// </summary>
    public void SeleccionarRaza(Raza nueva)
    {
        RazaActual = nueva;
        ActualizarPersonaje();
    }

    /// <summary>
    /// Seleccionar Morfología. Si nunca se eligió raza, usa la raza por defecto.
    /// </summary>
    public void SeleccionarMorfologia(Morfologia nueva)
    {
        // Si por cualquier motivo RazaActual no se ha establecido, garantiza el default
        // (en la práctica ya está inicializada en Awake, pero esto lo hace a prueba de todo)
        if (!System.Enum.IsDefined(typeof(Raza), RazaActual))
            RazaActual = razaPorDefecto;

        MorfologiaActual = nueva;
        ActualizarPersonaje();
    }

    /// <summary>
    /// Seleccionar Atuendo. Si nunca se eligió raza, usa la raza por defecto.
    /// </summary>
    public void SeleccionarAtuendo(Atuendo nuevo)
    {
        if (!System.Enum.IsDefined(typeof(Raza), RazaActual))
            RazaActual = razaPorDefecto;

        AtuendoActual = nuevo;
        ActualizarPersonaje();
    }

    /// <summary>
    /// Busca la combinación exacta y actualiza la instancia.
    /// </summary>
    public void ActualizarPersonaje()
    {
        string key = Clave(RazaActual, MorfologiaActual, AtuendoActual, SexoActual);

        if (!mapaPrefabs.TryGetValue(key, out GameObject prefab))
        {
            Debug.LogWarning($"No hay prefab para {key}. ¿Falta crear/registrar la ConfigPersonaje?");
            return;
        }

        // Destruir anterior
        if (personajeActual != null)
            Destroy(personajeActual);

        // Instanciar nuevo
        personajeActual = Instantiate(prefab, spawnRoot);

        // Asegura posición/orientación limpias (opcional)
        personajeActual.transform.localPosition = Vector3.zero;
        personajeActual.transform.localRotation = Quaternion.identity;
        personajeActual.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Útil si agregas más Configs por código o en runtime: vuelve a construir el diccionario.
    /// </summary>
    //public void ReconstruirMapa()
    //{
    //    mapaPrefabs.Clear();
    //    foreach (var cfg in configuraciones.Where(c => c != null && c.prefab != null))
    //    {
    //        var key = Clave(cfg.raza, cfg.morfologia, cfg.atuendo);
    //        if (!mapaPrefabs.ContainsKey(key))
    //            mapaPrefabs.Add(key, cfg.prefab);
    //    }
    //    ActualizarPersonaje();
    //}

    public void CambiarColor(Color nuevoColor)
    {
        if (personajeActual == null) return;

        Renderer rend = personajeActual.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            // Asegura que la emisión esté activa
            rend.material.EnableKeyword("_EMISSION");
            rend.material.SetColor("_EmissionColor", nuevoColor);
        }
    }

    public void BTN_SiguienteTextura()
    {
        if (texturasDisponibles.Length == 0 || personajeActual == null) return;

        indiceTextura = (indiceTextura + 1) % texturasDisponibles.Length;
        CambiarTextura(texturasDisponibles[indiceTextura]);
    }

    public void CambiarTextura(Texture nuevaTextura)
    {
        if (personajeActual == null) return;

        Renderer rend = personajeActual.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            rend.material.mainTexture = nuevaTextura;
        }
    }    
}