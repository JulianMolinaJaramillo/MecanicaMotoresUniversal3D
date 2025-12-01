using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ValoresDinamometro : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Slider sliderControlador;
    public Transform aguja;      // La aguja del velocímetro
    public Transform aguja2;      // La aguja del velocímetro

    public RotacionObjeto[] objetosRotatoriosPositivos;
    public RotacionObjeto[] objetosRotatoriosNegativos;

    public float anguloMin = -130f;
    public float anguloMax = 130f;

    public float anguloMin2 = -130f;
    public float anguloMax2 = 130f;

    public TextMeshProUGUI txtCO;
    public TextMeshProUGUI txtCO2;
    public TextMeshProUGUI txtO2;
    public TextMeshProUGUI txtHC;
    public TextMeshProUGUI txtHP;
    public TextMeshProUGUI txtTorqueNm;

    public float suavizado = 10f;       // Suavidad del movimiento
    public float amplitudOscilacion = 2f;  // Qué tanto se mueve la oscilación
    public float velocidadOscilacion = 4f; // Velocidad de la oscilación

    private float valorAnterior;
    private Quaternion rotacionOriginalAguja1;
    private Quaternion rotacionOriginalAguja2;

    private float valorCO;
    private float valorCO2;
    private float valorO2;
    private float valorHC;
    private float valorHP;
    private float valorTorqueNm;
    [HideInInspector]
    public bool puedoActualizar;

    public static ValoresDinamometro singleton;

    private void Awake()
    {
        // Configurar Singleton
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(this);
        }
    }
    private void Start()
    {
        rotacionOriginalAguja1 = aguja.localRotation;
        rotacionOriginalAguja2 = aguja2.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (ManagerDesplazamientoMotor.singleton.desplazamientoFinalizado)
        {
            if (puedoActualizar)
            {
                if (sliderControlador.value < 0.99f)
                {
                    if (ManagerMinijuego.singleton.motorActivo == "Diesel")
                    {
                        valorCO = sliderControlador.value * 0.1f;
                        valorCO2 = sliderControlador.value * 12f;
                        valorO2 = sliderControlador.value * 10f;
                        valorHC = sliderControlador.value * 80f;
                        valorHP = sliderControlador.value * 220f;
                        valorTorqueNm = sliderControlador.value * 500f;

                        txtCO.text = valorCO.ToString("F3");
                        txtCO2.text = valorCO2.ToString("F2");
                        txtO2.text = valorO2.ToString("F2");
                        txtHC.text = valorHC.ToString("F0");
                        txtHP.text = valorHP.ToString("F0");
                        txtTorqueNm.text = valorTorqueNm.ToString("F0");

                        anguloMax = 63;
                        anguloMax2 = 110;
                    }
                    else if (ManagerMinijuego.singleton.motorActivo == "Nissan")
                    {
                        valorCO = sliderControlador.value * 0.8f;
                        valorCO2 = sliderControlador.value * 15f;
                        valorO2 = sliderControlador.value * 2f;
                        valorHC = sliderControlador.value * 150f;
                        valorHP = sliderControlador.value * 115f;
                        valorTorqueNm = sliderControlador.value * 160f;

                        txtCO.text = valorCO.ToString("F2");
                        txtCO2.text = valorCO2.ToString("F2");
                        txtO2.text = valorO2.ToString("F2");
                        txtHC.text = valorHC.ToString("F0");
                        txtHP.text = valorHP.ToString("F0");
                        txtTorqueNm.text = valorTorqueNm.ToString("F0");

                        anguloMax = 43;
                        anguloMax2 = 31;
                    }
                }

                if (sliderControlador.value == 0)
                {
                    ValoresMotorApagado();
                    // Volver a la rotación original de la aguja
                    aguja.localRotation = Quaternion.Lerp(aguja.localRotation, rotacionOriginalAguja1, Time.deltaTime * suavizado);
                    aguja2.localRotation = Quaternion.Lerp(aguja2.localRotation, rotacionOriginalAguja2, Time.deltaTime * suavizado);
                }
            }

            if (sliderControlador.value > 0)
            {
                float valor = sliderControlador.value;

                bool sliderSeMueve = Mathf.Abs(valor - valorAnterior) > 0.001f;
                valorAnterior = valor;

                // Normalizar slider (0.1 → 1) a (0 → 1)
                float t = Mathf.InverseLerp(0.1f, 1f, valor);

                // Interpolar entre ángulos según el slider
                float rotZ = Mathf.Lerp(anguloMin, anguloMax, t);

                // Interpolar entre ángulos según el slider
                float rotZ2 = Mathf.Lerp(anguloMin2, anguloMax2, t);

                // Si el slider NO se mueve, añadimos oscilación
                if (!sliderSeMueve)
                {
                    float oscilacion = Mathf.Sin(Time.time * velocidadOscilacion) * amplitudOscilacion;
                    rotZ += oscilacion;
                    rotZ2 += oscilacion;
                }

                // Rotación suave en el eje Z local
                Quaternion targetRot = Quaternion.Euler(0f, 0f, rotZ);
                Quaternion targetRot2 = Quaternion.Euler(0f, 0f, rotZ2);
                aguja.localRotation = Quaternion.Lerp(aguja.localRotation, targetRot, Time.deltaTime * suavizado);
                aguja2.localRotation = Quaternion.Lerp(aguja2.localRotation, targetRot2, Time.deltaTime * suavizado);

                for (int i = 0; i < objetosRotatoriosPositivos.Length; i++)
                {
                    objetosRotatoriosPositivos[i].velocidadRotacion = sliderControlador.value * 1000;
                }

                for (int i = 0; i < objetosRotatoriosNegativos.Length; i++)
                {
                    objetosRotatoriosNegativos[i].velocidadRotacion = sliderControlador.value * -1000;
                }
            }
        }    
    }

    public void ValoresMotorApagado()
    {
        txtCO.text = "0.01";
        txtCO2.text = "0.04";
        txtO2.text = "20.5";
        txtHC.text = "0";
        txtHP.text = "0";
        txtTorqueNm.text = "0";
    }

    /// <summary>
    /// Metodo invocado al momento de manipular el slider
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (ManagerDesplazamientoMotor.singleton.desplazamientoFinalizado) puedoActualizar = true;
    }

    /// <summary>
    /// Metodo invocado al momento de dejar de manipular el slider
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (ManagerDesplazamientoMotor.singleton.desplazamientoFinalizado) puedoActualizar = false;
    }
}
