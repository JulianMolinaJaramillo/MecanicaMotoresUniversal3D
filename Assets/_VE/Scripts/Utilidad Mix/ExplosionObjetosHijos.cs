using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionObjetosHijos : MonoBehaviour
{
    [Header("Objetos Padres que contienen hijos con Rigidbody")]
    public List<GameObject> objetosPadres = new List<GameObject>();

    [Header("Configuración de la explosión")]
    public float fuerza = 300f;
    public float upwardModifier = 0.5f;
    public float radioExplosion = 5f;

    [Header("Configuración de vibración previa a la explosión")]
    public float duracionVibracion = 1.5f;
    public float intensidadVibracion = 0.05f; // qué tanto se mueven las piezas

    [ContextMenu("explot")]
    public void ExplotarTodo()
    {
        foreach (GameObject padre in objetosPadres)
        {
            StartCoroutine(VibrarYExplotar(padre));
        }
    }

    // Recolecta todos los descendientes (nietos en adelante) sin incluir al hijo directo
    private void RecolectarDescendientes(Transform nodo, List<Transform> lista)
    {
        foreach (Transform subHijo in nodo)
        {
            lista.Add(subHijo);
            RecolectarDescendientes(subHijo, lista); // Repetir recursivamente
        }
    }

    // Activa colliders y aplica la explosión
    private void ActivarExplosiónRecursiva(Transform objeto, Vector3 origenExplosion)
    {
        Collider[] colliders = objeto.GetComponents<Collider>();
        foreach (var col in colliders)
        {
            col.enabled = true;
        }

        Rigidbody rb = objeto.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;

            rb.AddExplosionForce(fuerza, origenExplosion, radioExplosion, upwardModifier, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// Currutnia encargada de hacer vibrar las piezas antes de la explosion
    /// </summary>
    /// <param name="padre"> Objeto padre </param>
    /// <returns></returns>
    private IEnumerator VibrarYExplotar(GameObject padre)
    {
        if (padre == null) yield break;

        // 1. Recolectar y reparentar los descendientes al padre
        List<Transform> descendientes = new List<Transform>();
        foreach (Transform child in padre.transform)
        {
            RecolectarDescendientes(child, descendientes);
        }

        foreach (Transform desc in descendientes)
        {
            desc.SetParent(padre.transform, true);
        }

        // 2. Vibrar cada hijo del padre
        List<Vector3> posicionesOriginales = new List<Vector3>();
        List<Transform> piezas = new List<Transform>();

        foreach (Transform pieza in padre.transform)
        {
            posicionesOriginales.Add(pieza.localPosition);
            piezas.Add(pieza);
        }

        float timer = 0f;
        while (timer < duracionVibracion)
        {
            for (int i = 0; i < piezas.Count; i++)
            {
                if (piezas[i] != null)
                {
                    Vector3 offset = Random.insideUnitSphere * intensidadVibracion;
                    piezas[i].localPosition = posicionesOriginales[i] + offset;
                }
            }

            timer += Time.deltaTime;
            yield return null; // Esperar un frame (fluido y constante)
        }

        // Restaurar posición original antes de la explosión (opcional)
        for (int i = 0; i < piezas.Count; i++)
        {
            if (piezas[i] != null)
            {
                piezas[i].localPosition = posicionesOriginales[i];
            }
        }

        // 3. Aplicar explosión
        foreach (Transform pieza in padre.transform)
        {
            ActivarExplosiónRecursiva(pieza, padre.transform.position);
        }
    }


    // Destruye todos los hijos de un objeto padre específico
    public void DestruirHijos(GameObject padre)
    {
        if (padre == null) return;

        List<Transform> hijos = new List<Transform>();
        foreach (Transform hijo in padre.transform)
        {
            hijos.Add(hijo);
        }

        foreach (Transform hijo in hijos)
        {
            Destroy(hijo.gameObject);
        }
    }
    [ContextMenu("destruir")]
    // Destruye los hijos de todos los objetos padres del listado
    public void DestruirTodosLosHijos()
    {
        foreach (GameObject padre in objetosPadres)
        {
            DestruirHijos(padre);
        }
    }
}
