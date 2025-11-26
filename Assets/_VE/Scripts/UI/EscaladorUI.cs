using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class EscaladorUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configuración de Escala")]
    [Tooltip("Escala objetivo al aumentar el tamaño")]
    public Vector3 escalaObjetivo = new Vector3(1.2f, 1.2f, 1f);

    [Tooltip("Duración de la interpolación de escala")]
    public float duracion = 0.3f;

    [Header("Configuración de Funcionalidad")]
    public bool esBoton;
    public bool esMensaje;
    public bool esPop;
    public bool desactivarAlRestaurar;
    public GameObject brillo;

    private bool cerrando;
    private RectTransform rectTransform;
    private Vector3 escalaInicial;
    private Coroutine escalaCoroutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        escalaInicial = rectTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (esBoton)
        {
            // Escala hacia el objetivo cuando el cursor entra
            Escalar();
        }      
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (esBoton)
        {
            // Vuelve a la escala inicial cuando el cursor sale
            Restaurar();
        }   
    }
    /// <summary>
    /// Escala el elemento UI hacia la escala definida en el inspector.
    /// </summary>
    [ContextMenu ("Escalar")]
    public void Escalar()
    {
        // Si ya hay una corrutina activa, la detenemos

        if (brillo != null) brillo.SetActive(true);

        if (escalaCoroutine != null) StopCoroutine(escalaCoroutine);

        escalaCoroutine = StartCoroutine(InterpolarEscala(escalaObjetivo, duracion));
    }

    /// <summary>
    /// Restaura la escala inicial del elemento UI.
    /// </summary>
    [ContextMenu("Restaurar")]
    public void Restaurar()
    {
        if (esMensaje || esPop) cerrando = true; // Solo para los tipo mensaje

        if (brillo != null) brillo.SetActive(false);

        if (escalaCoroutine != null) StopCoroutine(escalaCoroutine);

        escalaCoroutine = StartCoroutine(InterpolarEscala(escalaInicial, duracion));
    }

    /// <summary>
    /// Corrutina que interpola suavemente entre dos escalas.
    /// </summary>
    private IEnumerator InterpolarEscala(Vector3 objetivo, float duracion)
    {
        Vector3 escalaInicio = rectTransform.localScale;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float t = Mathf.Clamp01(tiempo / duracion);
            rectTransform.localScale = Vector3.Lerp(escalaInicio, objetivo, t);
            yield return null;
        }

        rectTransform.localScale = objetivo;
        escalaCoroutine = null;

        if (cerrando && esMensaje) this.gameObject.SetActive(false);

        if (desactivarAlRestaurar && cerrando) this.gameObject.SetActive(false);

        cerrando = false;
    }

    private void OnEnable()
    {
        if (esMensaje)
        {
            // Escalar al activar el objeto
            Escalar();
        }

        if (esPop)
        {
            // Escalar al activar el objeto
            Escalar();
        }

        if (esBoton)
        {
            // Escalar al activar el objeto
            Restaurar();
        }
    }

    private void OnDisable()
    {
        if (esPop)
        {
            rectTransform.localScale = escalaInicial;
        }

        //if (esMensaje)
        //{
        //    Restaurar();
        //}
    }
}
