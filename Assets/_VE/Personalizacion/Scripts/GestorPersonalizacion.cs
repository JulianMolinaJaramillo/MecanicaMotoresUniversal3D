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
    [Tooltip("Arrastrar aqui las configuraciones de los personajes disponibles.")]
    public List<ConfigPersonaje> configuraciones = new List<ConfigPersonaje>();

    [Header("Instanciación")]
    [Tooltip("Dónde se instanciará el modelo. Si se deja vacío, usará el transform de este objeto.")]
    public Transform puntoInstancia;

    [Header("Valores por defecto")]
    [Tooltip("Selecciona la raza que quieres tener por defecto.")]
    public Raza razaPorDefecto = Raza.Humano;
    public Morfologia morfologiaInicial = Morfologia.Normal;
    public Atuendo atuendoInicial = Atuendo.SinAtuendo;
    public Sexo sexoPorDefecto = Sexo.Hombre;
    public bool instanciarAlIniciar = true;

    
    [Header("% Porcentajes de Selección")]
    [Tooltip("Porcentaje de probabilidades para cada una de las razas.")]
    public List<OpcionRaza> razasConProbabilidad;
    public List<OpcionMorfologia> morfologiasConProbabilidad;
    public List<OpcionAtuendo> atuendosConProbabilidad;
    public List<OpcionSexo> sexosConProbabilidad;

    [Header("Efecto de Disolver.")]
    public float tiempoDisolver = 1.5f;  // tiempo de la interpolación
    public float rango;
    private Coroutine coroutine;
    private Coroutine coroutine2;

    [Header("Efectos y particulas.")]
    public ParticleSystem particulaSuelo;
    public ParticleSystem particulasHumo;

    // --- Estado actual ---
    [SerializeField]
    public Raza RazaActual { get; private set; }
    [SerializeField]
    public Morfologia MorfologiaActual { get; private set; }
    public Atuendo AtuendoActual { get; private set; }
    public Sexo SexoActual { get; private set; }

    // --- Instancia viva del personaje ---
    //[HideInInspector]
    public Texture texturaActualPersonaje; // Referencia a la textura actual del personaje
    //[HideInInspector]
    public Texture texturaOriginal; // Referencia a la textura original del personaje

    private bool puedoActualizar;
    private string claveActiva = ""; // Para guardar la combinación activa
    private GameObject personajeActual; // Referencia al personaje actual en scena
    private Color ultimoColorSeleccionado = Color.white; // Referencia al último color base 
    private Dictionary<string, GameObject> mapaPrefabs; // Mapa para acceso O(1) por clave (raza_morfologia_atuendo)

    // Diccionario para almacenar combinaciones guardadas en runtime
    private Dictionary<string, (Raza raza, Morfologia morfo, Atuendo atuendo, Sexo sexo)> combinacionesGuardadas = new Dictionary<string, (Raza, Morfologia, Atuendo, Sexo)>();
    private string Clave(Raza r, Morfologia m, Atuendo a, Sexo s) => $"{r}_{m}_{a}_{s}";


    private void Awake()
    {
        // Construimos el diccionario
        mapaPrefabs = new Dictionary<string, GameObject>();

        //Recorremos y llenamos nuestro diccionario, si encuentra configuraciones duplicadas, nos avisa
        foreach (var cfg in configuraciones.Where(c => c != null && c.prefab != null))
        {
            var key = Clave(cfg.raza, cfg.morfologia, cfg.atuendo, cfg.sexo);
            if (!mapaPrefabs.ContainsKey(key))
                mapaPrefabs.Add(key, cfg.prefab);
            else
                Debug.LogWarning($"Clave duplicada {key}. Revisa configuraciones repetidas.");
        }

        // Inicializamos el estado actual
        RazaActual = razaPorDefecto;
        MorfologiaActual = morfologiaInicial;
        AtuendoActual = atuendoInicial;
        SexoActual = sexoPorDefecto;

        if (instanciarAlIniciar)
        {
            ActualizarPersonaje(); // Mostrar primer modelo válido
        }
    }

    private void Start()
    {
        // Las partículas del humo
        particulasHumo.Play();
    }

    /// <summary>
    /// Metodo incovado desde el scrip UISeleccionPersonalizacion para cambiar de sexo al personaje
    /// </summary>
    /// <param name="nuevoSexo"> Indica el tipo de sexo, Masculino, Femenino, Otro </param>
    public void SeleccionarSexo(Sexo nuevoSexo)
    {
        if (!puedoActualizar)
        {
            SexoActual = nuevoSexo;
            ActualizarPersonaje();
        }
        
    }

    /// <summary>
    /// Metodo incovado para Seleccionar Raza. Si la combinación exacta no existe,
    /// busca la primera combinación válida disponible de esa raza.
    /// </summary>
    public void SeleccionarRaza(Raza nueva)
    {
        if (!puedoActualizar)
        {
            RazaActual = nueva;

            //  Validamos si existe la combinación exacta con lo que ya estaba elegido
            string keyExacta = Clave(RazaActual, MorfologiaActual, AtuendoActual, SexoActual);
            if (mapaPrefabs.ContainsKey(keyExacta))
            {
                ActualizarPersonaje();
                return;
            }

            // Si No existe: buscamos la "primera" de esa raza, priorizando lo actual
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
            Debug.Log($"No se encontró ninguna alternativa para Raza={RazaActual}. Probando raza por defecto.");

            //var fallback = BuscarPrimeraDeRaza(razaPorDefecto, morfologiaInicial, atuendoInicial, sexoPorDefecto);
            //if (fallback != null)
            //{
            //    RazaActual = fallback.raza;
            //    MorfologiaActual = fallback.morfologia;
            //    AtuendoActual = fallback.atuendo;
            //    SexoActual = fallback.sexo;

            //    ActualizarPersonaje();
            //}
            //else
            //{
            //    Debug.LogError("No hay combinaciones ni siquiera para la raza por defecto. Revisa tus ConfigPersonaje.");
            //}
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
        if (!puedoActualizar)
        {
            // Si no hay definida ninguna raza asignamos la por defecto
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
    /// Metodo invocado para seleccionar Atuendo. Si nunca se eligió raza, usa la raza por defecto.
    /// </summary>
    public void SeleccionarAtuendo(Atuendo nuevo)
    {
        if (!puedoActualizar)
        {
            if (!System.Enum.IsDefined(typeof(Raza), RazaActual))
                RazaActual = razaPorDefecto;

            AtuendoActual = nuevo;
            ActualizarPersonaje();
        }
        
    }

    /// <summary>
    /// Para ponderar los porcentajes y elegir un personaje alzar entre estos porcentajes
    /// </summary>
    /// <typeparam name="T"> Item, es decir, raza, morfologia, traje o sexo </typeparam>
    /// <param name="opciones"> Valor porcentual </param>
    /// <returns> Valor ponderado </returns>
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

    /// <summary>
    /// Metodo invocado desde el boton Generar Personaje Con Datos desde el canvas
    /// </summary>
    public void GenerarAleatorio()
    {
        if (!puedoActualizar)
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

                    ActualizarPersonaje();
                    Debug.Log($"Generado aleatoriamente: {clave}");
                    return;
                }
            }

            // Si llegamos aquí, no se encontró nada válido
            Debug.LogError("No se pudo generar ninguna combinación válida tras múltiples intentos.");
        }
            
    }

    /// <summary>
    /// Metodo principal invocado para realizar la actualizacion de cada personaje
    /// </summary>
    public void ActualizarPersonaje()
    {
        if (!puedoActualizar)
        {
            
            string key = Clave(RazaActual, MorfologiaActual, AtuendoActual, SexoActual);

            if (claveActiva == key && personajeActual != null)
                return;

            if (!mapaPrefabs.TryGetValue(key, out GameObject prefab))
            {
                Debug.Log($"No hay prefab para {key}.");
                return;
            }

            if (personajeActual != null)
            {
                Destroy(personajeActual, 1);                
                if (coroutine2 != null) StopCoroutine(coroutine2);
                coroutine2 = StartCoroutine(Solver(personajeActual));
                puedoActualizar = true;
            }

            personajeActual = Instantiate(prefab, puntoInstancia);
            claveActiva = key;
            botonera.RecibirClaveActual(claveActiva);

            personajeActual.transform.localPosition = Vector3.zero;
            personajeActual.transform.localRotation = Quaternion.identity;
            personajeActual.transform.localScale = Vector3.one;

            Renderer rend = personajeActual.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                // Guardar textura original
                if (rend.material.HasProperty("_BaseMap"))
                    texturaOriginal = rend.material.GetTexture("_BaseMap");
                else
                    texturaOriginal = rend.material.mainTexture;

                // Si ya teníamos textura guardada → aplicarla de una
                if (texturaActualPersonaje != null)
                {
                    foreach (var mat in rend.materials)
                        if (mat.HasProperty("_BaseMap"))
                            mat.SetTexture("_BaseMap", texturaActualPersonaje);
                }
            }

            // Si venimos de cargar, ya tenemos color/textura definidos:
            if (ultimoColorSeleccionado != Color.clear)
                CambiarColorBase(ultimoColorSeleccionado);

            // Las partículas del suelo
            particulaSuelo.Play();
            if (AudioManager.singleton != null) AudioManager.singleton.PlayEfectString("2"); // Ejecutamos el efecto nombrado

            // lanzar el disolver solo DESPUÉS de aplicar texturas/colores
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(AplicarDisolver(personajeActual));
        }      
    }

    /// <summary>
    /// Guarda la combinación actual (raza, morfo, atuendo, sexo) asociada a un nombre.
    /// Además lo persiste en PlayerPrefs.
    /// </summary>
    public void GuardarCombinacion(string nombre)
    {
        if (personajeActual != null)
        {
            if (string.IsNullOrEmpty(nombre))
            {
                seleccionPersonalizacion.ActualizarMensaje("El nombre para guardar combinación está vacío.");
                if (AudioManager.singleton != null) AudioManager.singleton.PlayEfectString("error"); // Ejecutamos el efecto nombrado
                Debug.Log("El nombre para guardar combinación está vacío.");
                return;
            }

            // Guardar datos básicos (raza, morfo, atuendo, sexo)
            var datos = (RazaActual, MorfologiaActual, AtuendoActual, SexoActual);
            if (combinacionesGuardadas.ContainsKey(nombre))
                combinacionesGuardadas[nombre] = datos;
            else
                combinacionesGuardadas.Add(nombre, datos);

            // Guardar en PlayerPrefs (convertimos todo a string)
            string dataStr = $"{(int)RazaActual}|{(int)MorfologiaActual}|{(int)AtuendoActual}|{(int)SexoActual}";

            // Guardar color actual en formato RGBA
            string colorStr = $"{ultimoColorSeleccionado.r}|{ultimoColorSeleccionado.g}|{ultimoColorSeleccionado.b}|{ultimoColorSeleccionado.a}";

            // Guardar textura (si existe) con el nombre de la textura
            string texturaStr = (texturaActualPersonaje != null) ? texturaActualPersonaje.name : "NULL";

            // Unimos todo
            string dataFinal = $"{dataStr}|{colorStr}|{texturaStr}";

            PlayerPrefs.SetString($"Combinacion_{nombre}", dataFinal);
            PlayerPrefs.Save();

            if (AudioManager.singleton != null) AudioManager.singleton.PlayEfectString("2"); // Ejecutamos el efecto nombrado
            seleccionPersonalizacion.ActualizarMensaje($"Combinación guardada con nombre '{nombre}'");
            Debug.Log($"Combinación guardada con nombre '{nombre}': {dataFinal}");
        }
        else
        {
            seleccionPersonalizacion.ActualizarMensaje("No hay personaje seleccionado");
            if (AudioManager.singleton != null) AudioManager.singleton.PlayEfectString("error"); // Ejecutamos el efecto nombrado
        }
    }

    /// <summary>
    /// Carga una combinación guardada por nombre.
    /// Primero busca en PlayerPrefs, si no la encuentra, revisa el diccionario.
    /// </summary>
    public void CargarCombinacion(string nombre)
    {
        if (string.IsNullOrEmpty(nombre))
        {
            seleccionPersonalizacion.ActualizarMensaje("El nombre para cargar combinación está vacío.");
            if (AudioManager.singleton != null) AudioManager.singleton.PlayEfectString("error"); // Ejecutamos el efecto nombrado
            Debug.Log("El nombre para cargar combinación está vacío.");
            return;
        }

        if (PlayerPrefs.HasKey($"Combinacion_{nombre}"))
        {
            string dataStr = PlayerPrefs.GetString($"Combinacion_{nombre}");
            var partes = dataStr.Split('|');

            // Esperamos 9 partes: 4 ints + 4 floats de color + 1 string de textura
            if (partes.Length >= 9)
            {
                RazaActual = (Raza)int.Parse(partes[0]);
                MorfologiaActual = (Morfologia)int.Parse(partes[1]);
                AtuendoActual = (Atuendo)int.Parse(partes[2]);
                SexoActual = (Sexo)int.Parse(partes[3]);

                // Color
                float r = float.Parse(partes[4]);
                float g = float.Parse(partes[5]);
                float b = float.Parse(partes[6]);
                float a = float.Parse(partes[7]);
                Color colorCargado = new Color(r, g, b, a);

                // Textura
                string nombreTextura = partes[8];
                Texture texturaCargada = null;
                if (nombreTextura != "NULL")
                {
                    // Buscar en texturas disponibles por nombre
                    texturaCargada = Resources.FindObjectsOfTypeAll<Texture>().FirstOrDefault(t => t.name == nombreTextura);
                }

                

                // Ahora sí: como ya guardamos valores, reaplicarlos:
                ultimoColorSeleccionado = colorCargado;
                CambiarColorBase(colorCargado);

                // Aplicar textura cargada si existe
                if (texturaCargada != null)
                    CambiarTextura(texturaCargada);

                // Actualizamos personaje
                ActualizarPersonaje();

                seleccionPersonalizacion.ActualizarMensaje($"Combinación de '{nombre}' cargada con éxito");
                Debug.Log($"Combinación '{nombre}' cargada con datos extra: Color={colorCargado}, Textura={nombreTextura}");
                return;
            }
        }

        Debug.Log($"No existe ninguna combinación guardada con el nombre '{nombre}'.");
    }

    /// <summary>
    /// Metodo utilizado para cambiar a cada color
    /// </summary>
    /// <param name="nuevoColor"> Color objetivo a cambiar </param>
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

    /// <summary>
    /// Metodo utilizado para cambiar a cada textura
    /// </summary>
    /// <param name="nuevoColor"> textura objetivo a cambiar </param>
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

    /// <summary>
    /// Meotodo invocado desde el boton para reiniciar la textura en el canvas
    /// </summary>
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

        // Dejamos la textuta actual en nulo
        texturaActualPersonaje = null;
    }


    /// <summary>
    /// Corrutina empleada para aplicar el efecto de disolver en el shader
    /// </summary>
    /// <param name="personaje"> Personaje instanciado </param>
    private IEnumerator AplicarDisolver(GameObject personaje)
    {
        Debug.Log("curr");
        Renderer[] renderers = personaje.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) yield break;

        // Creamos materiales instanciados para no modificar materiales globales
        List<Material> materialesInstanciados = new List<Material>();
        foreach (var rend in renderers)
        {
            Material[] nuevos = new Material[rend.materials.Length];
            for (int i = 0; i < rend.materials.Length; i++)
            {
                nuevos[i] = new Material(rend.materials[i]);
                Debug.Log(rend.materials[i].name);
                nuevos[i].SetFloat("_Frecuencia", 1f); // 👈 arranca invisible
                materialesInstanciados.Add(nuevos[i]);
            }
            rend.materials = nuevos;
        }

        // Interpolamos de 1 → -1
        
        float tiempo = 0f;
        while (tiempo < tiempoDisolver)
        {
            float t = tiempo / tiempoDisolver;
            float valor = Mathf.Lerp(rango, -rango, t);

            foreach (var mat in materialesInstanciados)
            {
                mat.SetFloat("_Frecuencia", valor);
            }

            tiempo += Time.deltaTime;
            yield return null;
        }
        puedoActualizar = false;
        coroutine = null;
         
    }
    private IEnumerator Solver(GameObject personaje)
    {
        Debug.Log("curr");
        Renderer[] renderers = personaje.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) yield break;

        // Creamos materiales instanciados para no modificar materiales globales
        List<Material> materialesInstanciados = new List<Material>();
        foreach (var rend in renderers)
        {
            Material[] nuevos = new Material[rend.materials.Length];
            for (int i = 0; i < rend.materials.Length; i++)
            {
                nuevos[i] = new Material(rend.materials[i]);
                Debug.Log(rend.materials[i].name);
                nuevos[i].SetFloat("_Frecuencia", 1f); // 👈 arranca invisible
                materialesInstanciados.Add(nuevos[i]);
            }
            rend.materials = nuevos;
        }

        // Interpolamos de 1 → -1
        float tiempo = 0f;
        while (tiempo < tiempoDisolver)
        {
            float t = tiempo / tiempoDisolver;
            float valor = Mathf.Lerp(-rango, rango, t);

            foreach (var mat in materialesInstanciados)
            {
                mat.SetFloat("_Frecuencia", valor);
            }

            tiempo += Time.deltaTime;
            yield return null;
        }

        // Aseguramos valor final visible
        foreach (var mat in materialesInstanciados)
        {
           // mat.SetFloat("_Frecuencia", -1.2f);
        }

        coroutine = null;
    }
}