using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PulsoBoton : MonoBehaviour
{
    public float escalaExtra = 1.1f;
    public float tiempo = 0.2f;

    private Vector3 escalaOriginal;
    private Coroutine animacionActiva;

    void Awake()
    {
        escalaOriginal = transform.localScale;

        // Crear EventTrigger dinámicamente
        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();

        // Evento Enter
        var entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((data) => { AnimarEntrada(); });
        trigger.triggers.Add(entryEnter);

        // Evento Exit
        var entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        entryExit.callback.AddListener((data) => { AnimarSalida(); });
        trigger.triggers.Add(entryExit);
    }

    private void AnimarEntrada()
    {
        if (animacionActiva != null) StopCoroutine(animacionActiva);
        animacionActiva = StartCoroutine(AnimarEscala(escalaOriginal * escalaExtra));
    }

    private void AnimarSalida()
    {
        if (animacionActiva != null) StopCoroutine(animacionActiva);
        animacionActiva = StartCoroutine(AnimarEscala(escalaOriginal));
    }

    private IEnumerator AnimarEscala(Vector3 objetivo)
    {
        Vector3 inicio = transform.localScale;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / tiempo;
            transform.localScale = Vector3.Lerp(inicio, objetivo, t);
            yield return null;
        }

        transform.localScale = objetivo;
    }
}
