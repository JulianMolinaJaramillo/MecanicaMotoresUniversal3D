using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Atornillar : MonoBehaviour
{
    public float anguloMaximo = 15f;
    public Slider sliderVelocidad;
    public TextMeshProUGUI torque;

    [HideInInspector] 
    public bool estaManipulando = false;

    private RectTransform rectTransform;
    private float anguloInicial;
    private float tiempoAnimacion = 0f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        anguloInicial = rectTransform.localEulerAngles.z;
    }

    void Update()
    {
        if (!estaManipulando) return;

        float velocidadActual = Mathf.Lerp(1f, 20f, sliderVelocidad.value);
        tiempoAnimacion += Time.deltaTime * velocidadActual;

        float rotacionZ = Mathf.Sin(tiempoAnimacion) * anguloMaximo;
        rectTransform.localRotation = Quaternion.Euler(0, 0, anguloInicial + rotacionZ);

        float valorConvertido = sliderVelocidad.value * 100f;
        torque.text = valorConvertido.ToString("F0");
    }
}
