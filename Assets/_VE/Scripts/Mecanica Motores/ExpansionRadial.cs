using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExpansionRadial : MonoBehaviour
{
    public float expansionRadius = 2f;
    public float expansionDuration = 1f;
    public TextMeshProUGUI txtBoton;
    public GestorPiezas gestorPiezas;
    public bool randomDirection = true;
    public float alturaMinimaY = 0.5f;
    public float alturaMaximaY = 2.5f;
    [HideInInspector]
    public bool noInteractuar;

    private bool expandir;
    private bool contraer;
    private List<Transform> hijos = new List<Transform>();
    private Dictionary<Transform, Vector3> posicionesOriginales = new Dictionary<Transform, Vector3>();
    private Coroutine expandirCoroutine;
    private Coroutine contraerCoroutine;


    [ContextMenu ("Asignar")]
    public void AsignarHijos()
    {
        LimpiarHijos();
        gestorPiezas.TransferirPiezasColocadas();
        // Guardamos posiciones originales
        foreach (Transform child in transform)
        {     
            hijos.Add(child);
            posicionesOriginales[child] = child.localPosition;        
        }
    }

    public void LimpiarHijos()
    {
        hijos.Clear();
        posicionesOriginales.Clear();
    }
    [ContextMenu("Expandir")]
    public void Expandir()
    {
        if (!noInteractuar)
        {
            if (!expandir)
            {
                expandir = true;
                AsignarHijos();
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(ControlCamaraMotor.singleton.posicionExpansion, 1);

                if (contraerCoroutine != null)
                {
                    StopCoroutine(contraerCoroutine);
                }
                expandirCoroutine = StartCoroutine(ExpandirCoroutine());
            }
        }         
    }

    [ContextMenu("Contraer")]
    public void Contraer()
    {
        if (contraer)
        {
            contraer = false;
            ControlCamaraMotor.singleton.IniciarMovimientoCamara(ControlCamaraMotor.singleton.posicionDown, 1);

            if (expandirCoroutine != null)
            {
                StopCoroutine(expandirCoroutine);
            }
            contraerCoroutine = StartCoroutine(ContraerCoroutine());
        }
        if (!noInteractuar)
        {
            
        }        
    }

    private IEnumerator ExpandirCoroutine()
    {
        float elapsed = 0f;
        Vector3[] targetPositions = new Vector3[hijos.Count];

        for (int i = 0; i < hijos.Count; i++)
        {
            Vector3 dir;
            if (randomDirection)
            {
                dir = Random.onUnitSphere;
                Vector3 target = posicionesOriginales[hijos[i]] + dir.normalized * expansionRadius;

                // Clampear Y entre mínimo y máximo
                target.y = Mathf.Clamp(target.y, alturaMinimaY, alturaMaximaY);

                targetPositions[i] = target;
            }
            else
            {
                float angle = (360f / hijos.Count) * i;
                dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
                Vector3 target = posicionesOriginales[hijos[i]] + dir.normalized * expansionRadius;
                targetPositions[i] = target;
            }
        }

        while (elapsed < expansionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / expansionDuration;

            for (int i = 0; i < hijos.Count; i++)
            {
                hijos[i].localPosition = Vector3.Lerp(posicionesOriginales[hijos[i]], targetPositions[i], t);
            }

            yield return null;
        }
        txtBoton.text = "Contraer";
        contraer = true;
        expandirCoroutine = null;
    }

    private IEnumerator ContraerCoroutine()
    {
        float elapsed = 0f;

        Vector3[] startPositions = new Vector3[hijos.Count];
        for (int i = 0; i < hijos.Count; i++)
        {
            startPositions[i] = hijos[i].localPosition;
        }

        while (elapsed < expansionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / expansionDuration;

            for (int i = 0; i < hijos.Count; i++)
            {
                hijos[i].localPosition = Vector3.Lerp(startPositions[i], posicionesOriginales[hijos[i]], t);
            }

            yield return null;
        }
        txtBoton.text = "Expandir";    
        expandir = false;
        contraerCoroutine = null;
    }
}
