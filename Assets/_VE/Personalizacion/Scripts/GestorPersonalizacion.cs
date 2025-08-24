using System.Collections;
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
    [Header("Referencias Principales")]
    public UISeleccionPersonalizacion seleccionPersonalizacion;
    public Botonera botonera;
    [Header("Catálogo de combinaciones")]
    [Tooltip("Arrastra aquí TODOS los ConfigPersonaje que crees (uno por combinación).")]
    public List<ConfigPersonaje> configuraciones = new List<ConfigPersonaje>();

    [Header("Instanciación")]
    [Tooltip("Dónde se instanciará el modelo. Si se deja vacío, usará el transform de este GameObject.")]
    public Transform puntoInstancia;

    [Header("Valores por defecto")]
    [Tooltip("Si el usuario elige morfología o atuendo primero, se usará esta raza por defecto.")]
    public Raza razaPorDefecto = Raza.Humano;
    [Tooltip("Estado inicial al arrancar (opcional).")]
    public Morfologia morfologiaInicial = Morfologia.Normal;
    public Atuendo atuendoInicial = Atuendo.SinAtuendo;
    public Sexo sexoPorDefecto = Sexo.Hombre;
    public bool instanciarAlIniciar = true;

    [Header("% Porcentajes de Selección")]
    public List<OpcionRaza> razasConProbabilidad;
    public List<OpcionMorfologia> morfologiasConProbabilidad;
    public List<OpcionAtuendo> atuendosConProbabilidad;
    public List<OpcionSexo> sexosConProbabilidad;

    [Header("Efecto de Disolver")]
    public float tiempoDisolver = 1.5f;  // tiempo de la interpolación
    private Coroutine coroutine;         // para manejar corrutinas

    [Header("Efecto de Disolver")]
    public ParticleSystem particulaSuelo;

    // --- Estado actual ---
    [SerializeField]
    public Raza RazaActual { get; private set; }
    [SerializeField]
    public Morfologia MorfologiaActual { get; private set; }
    public Atuendo AtuendoActual { get; private set; }
    public Sexo SexoActual { get; private set; }

    // Instancia viva del personaje
    [SerializeField]
    private string claveActiva = ""; // <- guarda la combinación activa
    [HideInInspector]
    private GameObject personajeActual;  
    [HideInInspector]
    public Texture texturaActualPersonaje;
    [HideInInspector]
    public Texture texturaOriginal; // cada prefab nuevo guarda aquí su textura base

    private Color ultimoColorSeleccionado = Color.white; // almacenamos el último color base

    // Mapa para acceso O(1) por clave (raza_morfologia_atuendo)
    private Dictionary<string, GameObject> mapaPrefabs;
    // Diccionario para almacenar combinaciones guardadas en runtime
    private Dictionary<string, (Raza raza, Morfologia morfo, Atuendo atuendo, Sexo sexo)> combinacionesGuardadas = new Dictionary<string, (Raza, Morfologia, Atuendo, Sexo)>();

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

    private T SeleccionarConPorcentaje<T>(List<(T item, float peso)> opciones)
    {
        float total = opciones.Sum(o => o.peso);
        if (total <= 0) return default;

        float random = Random.Range(0, total);
        float acumulado = 0;

        foreach (var op in opciones)
        {
            acumulado += op.peso;
            if (random <= acumulado)
                return op.item;
        }
        return opciones.Last().item;
    }

    public void GenerarAleatorio()
    {
        // Creamos un set para recordar combinaciones ya probadas en esta ejecución
        HashSet<string> intentadas = new HashSet<string>();

        // Bucle de reintentos
        for (int i = 0; i < 300; i++) // seguridad: máximo 100 intentos
        {
            Raza razaSel = SeleccionarConPorcentaje(razasConProbabilidad.Select(r => (r.raza, r.porcentaje)).ToList());
            Morfologia morfoSel = SeleccionarConPorcentaje(morfologiasConProbabilidad.Select(m => (m.morfologia, m.porcentaje)).ToList());
            Atuendo atuendoSel = SeleccionarConPorcentaje(atuendosConProbabilidad.Select(a => (a.atuendo, a.porcentaje)).ToList());
            Sexo sexoSel = SeleccionarConPorcentaje(sexosConProbabilidad.Select(s => (s.sexo, s.porcentaje)).ToList());

            string clave = Clave(razaSel, morfoSel, atuendoSel, sexoSel);

            if (intentadas.Contains(clave))
                continue; // ya probamos esta combinación en esta ejecución

            intentadas.Add(clave);
            Debug.Log($"Intento numero: {i} Se probó con la combinacion: {clave}");

            if (mapaPrefabs.ContainsKey(clave))
            {
                // Verificar que no sea la misma combinación activa
                if (clave == claveActiva)
                {
                    Debug.Log($"La combinación aleatoria {clave} coincide con la activa, probando otra...");
                    continue; // seguir buscando otra distinta
                }

                // Encontramos combinación válida y distinta
                RazaActual = razaSel;
                MorfologiaActual = morfoSel;
                AtuendoActual = atuendoSel;
                SexoActual = sexoSel;

                seleccionPersonalizacion.ReiniciarTexto();
                ActualizarPersonaje();
                Debug.Log($"Generado aleatoriamente: {clave}");
                return;
            }
        }

        // Si llegamos aquí, no se encontró nada válido
        Debug.LogError("No se pudo generar ninguna combinación válida tras múltiples intentos.");
    }

    /// <summary>
    /// Busca la combinación exacta y actualiza la instancia.
    /// </summary>
    public void ActualizarPersonaje()
    {
        string key = Clave(RazaActual, MorfologiaActual, AtuendoActual, SexoActual);

        // Evitar reinstanciar si ya está la misma configuración activa
        if (claveActiva == key && personajeActual != null)
        {
            Debug.Log($"Ya está activa la combinación {key}, no se reinstancia.");
            return;
        }

        if (!mapaPrefabs.TryGetValue(key, out GameObject prefab))
        {
            Debug.Log($"No hay prefab para {key}. ¿Falta crear/registrar la ConfigPersonaje?");
            return;
        }

        // Destruir anterior
        if (personajeActual != null) Destroy(personajeActual);

        // Instanciar nuevo
        personajeActual = Instantiate(prefab, puntoInstancia);
        claveActiva = key; // <- guardar la nueva clave activa
        botonera.RecibirClaveActual(claveActiva);
        

        // Asegura posición/orientación limpias (opcional)
        personajeActual.transform.localPosition = Vector3.zero;
        personajeActual.transform.localRotation = Quaternion.identity;
        personajeActual.transform.localScale = Vector3.one;

        Renderer rend = personajeActual.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            // Intentamos primero obtener desde _BaseMap (Shader Graph / Lit URP)
            if (rend.material.HasProperty("_BaseMap"))
            {
                texturaOriginal = rend.material.GetTexture("_BaseMap");
            }
            else
            {
                // Fallback: usamos mainTexture (por compatibilidad con otros shaders)
                texturaOriginal = rend.material.mainTexture;
            }

            // Si ya había una textura seleccionada por el jugador, se reaplica
            if (texturaActualPersonaje != null)
            {
                foreach (var mat in rend.materials)
                {
                    if (mat.HasProperty("_BaseMap"))
                    {
                        mat.SetTexture("_BaseMap", texturaActualPersonaje);
                    }
                }
            }
        }

        if (!particulaSuelo.isPlaying)
        {
            
        }
        particulaSuelo.Play();
        // Reaplicar el último color elegido al nuevo personaje
        if (ultimoColorSeleccionado != Color.clear)
        {
            CambiarColorBase(ultimoColorSeleccionado);
        }

        // Llamar a la corrutina de disolver para este nuevo personaje
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(AplicarDisolver(personajeActual));
    }

    /// <summary>
    /// Guarda la combinación actual (raza, morfo, atuendo, sexo) asociada a un nombre.
    /// Además lo persiste en PlayerPrefs.
    /// </summary>
    public void GuardarCombinacion(string nombre)
    {
        if (string.IsNullOrEmpty(nombre))
        {
            Debug.Log("El nombre para guardar combinación está vacío.");
            return;
        }

        // Guardar en diccionario runtime
        var datos = (RazaActual, MorfologiaActual, AtuendoActual, SexoActual);
        if (combinacionesGuardadas.ContainsKey(nombre))
            combinacionesGuardadas[nombre] = datos;
        else
            combinacionesGuardadas.Add(nombre, datos);

        // Persistir con PlayerPrefs (convertimos a string)
        string dataStr = $"{(int)RazaActual}|{(int)MorfologiaActual}|{(int)AtuendoActual}|{(int)SexoActual}";
        PlayerPrefs.SetString($"Combinacion_{nombre}", dataStr);
        PlayerPrefs.Save();

        seleccionPersonalizacion.ActualizarMensaje($"Combinación guardada con nombre '{nombre}'");
        Debug.Log($"Combinación guardada con nombre '{nombre}': {dataStr}");
    }

    /// <summary>
    /// Carga una combinación guardada por nombre.
    /// Primero busca en PlayerPrefs, si no la encuentra, revisa el diccionario.
    /// </summary>
    public void CargarCombinacion(string nombre)
    {
        if (string.IsNullOrEmpty(nombre))
        {
            Debug.Log("El nombre para cargar combinación está vacío.");
            return;
        }

        // Buscar en PlayerPrefs
        if (PlayerPrefs.HasKey($"Combinacion_{nombre}"))
        {
            string dataStr = PlayerPrefs.GetString($"Combinacion_{nombre}");
            var partes = dataStr.Split('|');
            if (partes.Length == 4)
            {
                RazaActual = (Raza)int.Parse(partes[0]);
                MorfologiaActual = (Morfologia)int.Parse(partes[1]);
                AtuendoActual = (Atuendo)int.Parse(partes[2]);
                SexoActual = (Sexo)int.Parse(partes[3]);

                ActualizarPersonaje();
                seleccionPersonalizacion.ActualizarMensaje($"Combinación de '{nombre}' cargada con exito ");
                Debug.Log($" Combinación '{nombre}' cargada desde PlayerPrefs: {dataStr}");
                return;
            }
        }

        // Si no está en PlayerPrefs, revisar el diccionario runtime
        if (combinacionesGuardadas.TryGetValue(nombre, out var datos))
        {
            RazaActual = datos.raza;
            MorfologiaActual = datos.morfo;
            AtuendoActual = datos.atuendo;
            SexoActual = datos.sexo;

            ActualizarPersonaje();
            Debug.Log($" Combinación '{nombre}' cargada desde diccionario runtime.");
        }
        else
        {
            Debug.Log($" No existe ninguna combinación guardada con el nombre '{nombre}'.");
        }
    }

    private IEnumerator AplicarDisolver(GameObject personaje)
    {
        Renderer[] renderers = personaje.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) yield break;

        // Creamos materiales instanciados para no modificar materiales globales
        List<Material> materialesInstanciados = new List<Material>();
        foreach (var rend in renderers)
        {
            Material[] nuevos = new Material[rend.materials.Length];
            for (int i = 0; i < rend.materials.Length; i++)
            {
                nuevos[i] = new Material(rend.materials[i]); // copia del material
                nuevos[i].SetFloat("_Frecuencia", -1f); // inicio en -1
                materialesInstanciados.Add(nuevos[i]);
            }
            rend.materials = nuevos;
        }

        // Interpolamos de -1 a 1
        float tiempo = 0f;
        while (tiempo < tiempoDisolver)
        {
            float t = tiempo / tiempoDisolver;
            float valor = Mathf.Lerp(1f, -1f, t);

            foreach (var mat in materialesInstanciados)
            {
                mat.SetFloat("_Frecuencia", valor);
            }

            tiempo += Time.deltaTime;
            yield return null;
        }

        // Aseguramos valor final
        foreach (var mat in materialesInstanciados)
        {
            mat.SetFloat("_Frecuencia", -1.2f);
        }

        coroutine = null;
    }
    public void CambiarColorBase(Color nuevoColor)
    {
        if (personajeActual == null) return;

        ultimoColorSeleccionado = nuevoColor; // Guardamos el último color elegido

        Renderer rend = personajeActual.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            foreach (var mat in rend.materials)
            {
                if (mat.HasProperty("_BaseColor")) // <- usa el Reference exacto de tu Shader Graph
                {
                    mat.SetColor("_BaseColor", nuevoColor);
                }
            }
        }
    }

    public void CambiarTextura(Texture nuevaTextura)
    {
        if (personajeActual == null) return;

        Renderer rend = personajeActual.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            foreach (var mat in rend.materials)
            {
                if (mat.HasProperty("_BaseMap"))
                {
                    mat.SetTexture("_BaseMap", nuevaTextura);
                }
            }
        }

        // Guardar esta como la textura actual
        texturaActualPersonaje = nuevaTextura;
    }

    public void ReiniciarTextura()
    {
        if (personajeActual == null) return;

        Renderer rend = personajeActual.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            foreach (var mat in rend.materials)
            {
                if (mat.HasProperty("_BaseMap"))
                {
                    // Restaurar la textura original del personaje actual
                    mat.SetTexture("_BaseMap", texturaOriginal);
                }
            }
        }

        // Actualizar el estado de textura actual (para que sepa que ahora usa la original)
        texturaActualPersonaje = texturaOriginal;
    }
}