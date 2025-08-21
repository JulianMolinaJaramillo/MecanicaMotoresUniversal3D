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
    //[HideInInspector]
    public Color colorActualPersonaje;
    //[HideInInspector]
    public Texture texturaActualPersonaje;
    //[HideInInspector]
    public float intensidadFija = 5f; // siempre usará esta intensidad

    private Color ultimoColorSeleccionado = Color.white; // almacenamos el último color base

    // Mapa para acceso O(1) por clave (raza_morfologia_atuendo)
    private Dictionary<string, GameObject> mapaPrefabs;

    private string Clave(Raza r, Morfologia m, Atuendo a, Sexo s) => $"{r}_{m}_{a}_{s}";

    private void Awake()
    {
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
    /// Seleccionar Raza. Si la combinación exacta no existe,
    /// busca la primera combinación válida disponible de esa raza.
    /// </summary>
    public void SeleccionarRaza(Raza nueva)
    {
        RazaActual = nueva;

        // ¿Existe la combinación exacta con lo que ya estaba elegido?
        string keyExacta = Clave(RazaActual, MorfologiaActual, AtuendoActual, SexoActual);
        if (mapaPrefabs.ContainsKey(keyExacta))
        {
            ActualizarPersonaje();
            return;
        }

        // No existe: buscamos la "primera" de esa raza, priorizando lo actual
        var alternativa = BuscarPrimeraDeRaza(RazaActual, MorfologiaActual, AtuendoActual, SexoActual);

        if (alternativa != null)
        {
            // Ajustamos el estado a lo encontrado
            RazaActual = alternativa.raza;
            MorfologiaActual = alternativa.morfologia;
            AtuendoActual = alternativa.atuendo;
            SexoActual = alternativa.sexo;

            ActualizarPersonaje();
            return;
        }

        // Si la raza elegida no tiene ninguna combinación, caemos a la raza por defecto
        Debug.LogWarning($"No se encontró ninguna alternativa para Raza={RazaActual}. Probando raza por defecto.");
        var fallback = BuscarPrimeraDeRaza(razaPorDefecto, morfologiaInicial, atuendoInicial, sexoPorDefecto);
        if (fallback != null)
        {
            RazaActual = fallback.raza;
            MorfologiaActual = fallback.morfologia;
            AtuendoActual = fallback.atuendo;
            SexoActual = fallback.sexo;

            ActualizarPersonaje();
        }
        else
        {
            Debug.LogError("No hay combinaciones ni siquiera para la raza por defecto. Revisa tus ConfigPersonaje.");
        }
    }

    /// <summary>
    /// Devuelve la primera combinación disponible para una raza dada,
    /// intentando respetar primero (morfo, atuendo, sexo) actuales
    /// y relajando condiciones si no existen.
    /// </summary>
    private ConfigPersonaje BuscarPrimeraDeRaza(Raza razaObjetivo, Morfologia morfoPref, Atuendo atuendoPref, Sexo sexoPref)
    {
        // 1) Misma raza + morfo + atuendo + sexo (preferencia exacta)
        var match = configuraciones.FirstOrDefault(c =>
            c.raza == razaObjetivo && c.morfologia == morfoPref && c.atuendo == atuendoPref && c.sexo == sexoPref);
        if (match != null) return match;

        // 2) Misma raza + morfo + sexo (cualquier atuendo)
        match = configuraciones.FirstOrDefault(c =>
            c.raza == razaObjetivo && c.morfologia == morfoPref && c.sexo == sexoPref);
        if (match != null) return match;

        // 3) Misma raza + morfo (cualquier atuendo/sexo)
        match = configuraciones.FirstOrDefault(c =>
            c.raza == razaObjetivo && c.morfologia == morfoPref);
        if (match != null) return match;

        // 4) Misma raza + atuendo + sexo (cualquier morfo)
        match = configuraciones.FirstOrDefault(c =>
            c.raza == razaObjetivo && c.atuendo == atuendoPref && c.sexo == sexoPref);
        if (match != null) return match;

        // 5) Misma raza + atuendo (cualquier morfo/sexo)
        match = configuraciones.FirstOrDefault(c =>
            c.raza == razaObjetivo && c.atuendo == atuendoPref);
        if (match != null) return match;

        // 6) Misma raza + sexo (cualquier morfo/atuendo)
        match = configuraciones.FirstOrDefault(c =>
            c.raza == razaObjetivo && c.sexo == sexoPref);
        if (match != null) return match;

        // 7) Cualquier combinación de esa raza (primera que exista)
        match = configuraciones.FirstOrDefault(c => c.raza == razaObjetivo);
        if (match != null) return match;

        // Nada para esa raza
        return null;
    }

    /// <summary>
    /// Seleccionar Morfología. Si la combinación exacta no existe,
    /// busca una combinación disponible lo más cercana posible.
    /// </summary>
    public void SeleccionarMorfologia(Morfologia nueva)
    {
        if (!System.Enum.IsDefined(typeof(Raza), RazaActual))
            RazaActual = razaPorDefecto;

        // Guardamos el intento original
        Morfologia morfoAnt = MorfologiaActual;
        MorfologiaActual = nueva;

        // Verificamos si existe la combinación exacta
        string keyExacta = Clave(RazaActual, MorfologiaActual, AtuendoActual, SexoActual);
        if (mapaPrefabs.ContainsKey(keyExacta))
        {
            ActualizarPersonaje();
            return;
        }

        // Buscar alternativa más cercana
        ConfigPersonaje alternativa = BuscarCombinacionDisponible(RazaActual, MorfologiaActual, AtuendoActual, SexoActual);

        if (alternativa != null)
        {
            Debug.Log($"No existe {keyExacta}, usando alternativa {alternativa.raza}_{alternativa.morfologia}_{alternativa.atuendo}_{alternativa.sexo}");
            // Actualizamos estado con lo que encontramos
            RazaActual = alternativa.raza;
            MorfologiaActual = alternativa.morfologia;
            AtuendoActual = alternativa.atuendo;
            SexoActual = alternativa.sexo;

            ActualizarPersonaje();
        }
        else
        {
            Debug.LogWarning($"No se encontró ninguna alternativa para Raza={RazaActual}, Morfo={MorfologiaActual}. Restaurando morfo anterior.");
            MorfologiaActual = morfoAnt; // restaurar si no hay nada
        }
    }

    /// <summary>
    /// Busca la combinación más cercana disponible según prioridad.
    /// </summary>
    private ConfigPersonaje BuscarCombinacionDisponible(Raza raza, Morfologia morfo, Atuendo atuendo, Sexo sexo)
    {
        // 1. Misma raza + morfo + cualquier atuendo + mismo sexo
        var match = configuraciones.FirstOrDefault(c =>
            c.raza == raza && c.morfologia == morfo && c.sexo == sexo);
        if (match != null) return match;

        // 2. Misma raza + morfo + mismo atuendo + cualquier sexo
        match = configuraciones.FirstOrDefault(c =>
            c.raza == raza && c.morfologia == morfo && c.atuendo == atuendo);
        if (match != null) return match;

        // 3. Misma raza + morfo + cualquier atuendo + cualquier sexo
        match = configuraciones.FirstOrDefault(c =>
            c.raza == raza && c.morfologia == morfo);
        if (match != null) return match;

        // 4. Fallback a raza por defecto + morfo
        match = configuraciones.FirstOrDefault(c =>
            c.raza == razaPorDefecto && c.morfologia == morfo);
        if (match != null) return match;

        // Nada encontrado
        return null;
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
            Debug.Log($"No hay prefab para {key}. ¿Falta crear/registrar la ConfigPersonaje?");
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

        Renderer rend = personajeActual.GetComponentInChildren<Renderer>();
        texturaActualPersonaje = rend.material.mainTexture;
    }

    public void CambiarColorConIntensidad(Color nuevoColor)
    {
        if (personajeActual == null) return;

        Renderer rend = personajeActual.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            rend.material.EnableKeyword("_EMISSION");

            // Guardar el último color elegido (tono puro)
            ultimoColorSeleccionado = nuevoColor;

            // Aplicar el color con la intensidad fija
            rend.material.SetColor("_EmissionColor", nuevoColor * intensidadFija);
        }
    }

    public void ResetearEmission()
    {
        if (personajeActual == null) return;

        Renderer rend = personajeActual.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            // Apagar emisión, pero sin perder el último color
            rend.material.SetColor("_EmissionColor", ultimoColorSeleccionado * 0f);
        }
    }

    public void RestaurarEmission()
    {
        if (personajeActual == null) return;

        Renderer rend = personajeActual.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            // Volver a encender con intensidad fija
            rend.material.SetColor("_EmissionColor", ultimoColorSeleccionado * intensidadFija);
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
        ResetearEmission();

        Renderer rend = personajeActual.GetComponentInChildren<Renderer>();

        if (rend != null)
        {
            rend.material.mainTexture = nuevaTextura;
        }
    }

    public void ReiniciarTextura()
    {
        if (personajeActual == null) return;
        RestaurarEmission();

        Renderer rend = personajeActual.GetComponentInChildren<Renderer>();

        if (rend != null)
        {
            rend.material.mainTexture = texturaActualPersonaje;
        }
    }
}