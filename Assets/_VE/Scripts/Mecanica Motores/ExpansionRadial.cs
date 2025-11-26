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
    public bool piezasInternas;

    [HideInInspector]
    public bool noInteractuar;
    [HideInInspector]
    public bool expandir;
    private bool contraer;
    private List<Transform> hijos = new List<Transform>();
    private Dictionary<Transform, Vector3> posicionesOriginales = new Dictionary<Transform, Vector3>();
    private Coroutine expandirCoroutine;
    private Coroutine contraerCoroutine;

    private class SPInfo
    {
        public Transform spTransform;
        public Transform parentOriginal;
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;
    }

    private List<SPInfo> spInfos = new List<SPInfo>();

    [ContextMenu("Asignar")]
    public void AsignarHijos()
    {
        LimpiarHijos();
        gestorPiezas.TransferirPiezasColocadas();
        spInfos.Clear();

        foreach (Transform child in transform)
        {
            hijos.Add(child);
            posicionesOriginales[child] = child.localPosition;

            List<Transform> nietosSP = new List<Transform>(); // Este se reinicia por cada hijo

            foreach (Transform nieto in child)
            {
                if (nieto.name.StartsWith("SP"))
                {
                    spInfos.Add(new SPInfo
                    {
                        spTransform = nieto,
                        parentOriginal = child,
                        localPosition = nieto.localPosition,
                        localRotation = nieto.localRotation,
                        localScale = nieto.localScale
                    });

                    nietosSP.Add(nieto);
                }
            }

            // Después de recorrer TODOS los nietos de ese hijo, los desemparentamos
            foreach (Transform sp in nietosSP)
            {
                sp.SetParent(null, true);
                sp.hasChanged = false;
            }
        }
    }
    [ContextMenu("limpiar")]
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
                if (piezasInternas)// Solo si son las piezas internas
                {
                    MesaMotor.singleton.motorExpandido = true;
                    if (ManagerMinijuego.singleton != null && ManagerMinijuego.singleton.motorAnimadoActivo != null) ManagerMinijuego.singleton.motorAnimadoActivo.SetActive(false); // Desactivamos motor animado antes de expandir
                    if (ExplosionObjetosHijos.singleton != null) ExplosionObjetosHijos.singleton.ActivarHijos(ExplosionObjetosHijos.singleton.objetosPadres[1]); // Activamos los hijos antes de expandir
                }
                
                ControlCamaraMotor.singleton.ReestablecerPosicionCamara(); // Reiniciamos el indice para que la posicion de la camara sea correcta

                ManagerMinijuego.singleton.DeshabilitarBtnEnceder();
                expandir = true;
                AsignarHijos();
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(ControlCamaraMotor.singleton.posicionExpansion, 1);

                if (contraerCoroutine != null) StopCoroutine(contraerCoroutine);
                expandirCoroutine = StartCoroutine(ExpandirCoroutine());
            }
        }
    }

    [ContextMenu("Contraer")]
    public void Contraer()
    {
        if (!noInteractuar)
        {
            if (contraer)
            {
                ControlCamaraMotor.singleton.ReestablecerPosicionCamara(); // Reiniciamos el indice para que la posicion de la camara sea correcta

                ManagerMinijuego.singleton.DeshabilitarBtnEnceder();
                contraer = false;
                ControlCamaraMotor.singleton.IniciarMovimientoCamara(ControlCamaraMotor.singleton.posicionFrontal, 1);

                if (expandirCoroutine != null) StopCoroutine(expandirCoroutine);
                contraerCoroutine = StartCoroutine(ContraerCoroutine());

            }
        }
    }

    private IEnumerator ExpandirCoroutine()
    {
        yield return null; // Esperar 1 frame para asegurarnos de que todo fue desemparentado
        float elapsed = 0f;
        Vector3[] targetPositions = new Vector3[hijos.Count];

        for (int i = 0; i < hijos.Count; i++)
        {
            Vector3 dir;
            if (randomDirection)
            {
                dir = Random.onUnitSphere;
                Vector3 target = posicionesOriginales[hijos[i]] + dir.normalized * expansionRadius;
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

        ManagerMinijuego.singleton.HabilitarBtnEnceder();
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

        foreach (var info in spInfos)
        {
            info.spTransform.SetParent(info.parentOriginal, false);
            info.spTransform.localPosition = info.localPosition;
            info.spTransform.localRotation = info.localRotation;
            info.spTransform.localScale = info.localScale;
        }
        spInfos.Clear();

        if (ManagerMinijuego.singleton != null)
        {
            ManagerMinijuego.singleton.HabilitarBtnEnceder();

            if (piezasInternas) // Solo si son las piezas internas
            {
                MesaMotor.singleton.motorExpandido = false;

                if (!MesaMotor.singleton.motorRotando)
                {
                    if (ManagerMinijuego.singleton.minijuegoValidadoCorrectamente)
                    {
                        ManagerMinijuego.singleton.motorAnimadoActivo.SetActive(true); // Activamos motor animado luego de expandir   
                    }

                    if (ManagerMinijuego.singleton.minijuegoTerminado)
                    {
                        if (ExplosionObjetosHijos.singleton != null) ExplosionObjetosHijos.singleton.DesactivarHijos(ExplosionObjetosHijos.singleton.objetosPadres[1]); // Desactivamos los hijos antes de expandir 
                    }
                }
                
            }     
        }
   
        txtBoton.text = "Expandir";
        expandir = false;
        contraerCoroutine = null;
    }
}
