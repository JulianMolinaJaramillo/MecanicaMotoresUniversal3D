using UnityEngine;
using UnityEngine.UI;

public class ApretarTornillos : MonoBehaviour
{
    public float moveDistance = 0.05f; // Distancia que recorre el tornillo (subir/bajar)
    public float rotations = 5f;       // Número de vueltas completas que da en todo el slider

    private Vector3 startPos;

    [ContextMenu("activar")]
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
        // Movimiento lineal (subir/bajar)
        float yOffset = Mathf.Lerp(0f, -moveDistance, value);
        transform.localPosition = startPos + new Vector3(0, 0, yOffset);

        // Rotación (simula rosca)
        float angle = value * 360f * rotations;
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
