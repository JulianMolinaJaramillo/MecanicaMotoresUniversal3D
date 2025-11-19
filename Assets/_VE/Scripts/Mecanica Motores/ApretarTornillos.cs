using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ApretarTornillos : MonoBehaviour
{
    public float moveDistance = 0.05f;
    public float rotations = 5f;
    public bool rotarZ;
    public bool rotarX;
    public bool prensaValvula;

    private Vector3 startPos;

    private MeshRenderer meshRendererObjetivo;  // Ahora es el mesh target
    private Material[] materialesOriginales;

    public Slider sliderPrueba;

    private void Awake()
    {
        // Buscar si tiene hijos con MeshRenderer
        MeshRenderer[] hijos = GetComponentsInChildren<MeshRenderer>();

        if (hijos.Length > 1)
        {
            // El primer elemento hijos[0] suele ser el padre, hijos[1] sería el primer hijo REAL
            meshRendererObjetivo = hijos[1];
        }
        else
        {
            // No tiene hijos → usar el MeshRenderer propio
            meshRendererObjetivo = GetComponent<MeshRenderer>();
        }
    }

    private void Start()
    {
        if (prensaValvula) HabilitarSliderPrueba();

        StartCoroutine(AsignarMaterial());
    }

    private IEnumerator AsignarMaterial()
    {
        yield return new WaitForSeconds(2f);

        materialesOriginales = meshRendererObjetivo.materials;
    }

    public void QuitarMaterial()
    {
        meshRendererObjetivo.materials = new Material[] { materialesOriginales[0] };
    }

    [ContextMenu("activar")]
    public void HabilitarSliderPrueba()
    {
        startPos = transform.localPosition;
        sliderPrueba.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public void HabilitarSlider(Slider slider)
    {
        startPos = transform.localPosition;
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public void DeshabilitarSlider(Slider slider)
    {
        slider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    void OnSliderValueChanged(float value)
    {
        if (rotarZ)
        {
            float yOffset = Mathf.Lerp(0f, -moveDistance, value);
            transform.localPosition = startPos + new Vector3(0, 0, yOffset);

            float angle = value * 360f * rotations;
            transform.localRotation = Quaternion.Euler(0, 0, angle);
        }
        else if (rotarX)
        {
            float yOffset = Mathf.Lerp(0f, -moveDistance, value);
            transform.localPosition = startPos + new Vector3(yOffset, 0, 0);

            float angle = value * 360f * rotations;
            transform.localRotation = Quaternion.Euler(angle, 0, 90f);
        }
    }
}
