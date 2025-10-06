
using UnityEngine;
using UnityEngine.VFX;

public class MirarCamara : MonoBehaviour
{
    [Header("Configuración de sensibilidad")]
    public float sensibilidadX = 2f;  // Velocidad de rotación horizontal
    public float sensibilidadY = 2f;  // Velocidad de rotación vertical

    [Header("Límites de rotación")]
    public float limiteX = 80f; // Límite vertical (mirar arriba/abajo)
    public float limiteY = 80f; // Límite horizontal (mirar izquierda/derecha)

    private float rotacionX = 0f;
    private float rotacionY = 0f;
    public bool detener;

    public static MirarCamara singleton;
    private void Awake()
    {
        // Implementación Singleton
        if (singleton != null && singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        singleton = this;

        // Si quieres que persista entre escenas:
        // DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        // Opcional: bloquear cursor en el centro
        //Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    void Update()
    {
        if (!detener)
        {
            // Obtener movimiento del mouse
            float mouseX = Input.GetAxis("Mouse X") * sensibilidadX;
            float mouseY = Input.GetAxis("Mouse Y") * sensibilidadY;

            // Acumular rotaciones
            rotacionY += mouseX; // Horizontal
            rotacionX -= mouseY; // Vertical (resta porque mouse arriba = mirar arriba)

            // Limitar los ángulos
            rotacionX = Mathf.Clamp(rotacionX, -limiteX, limiteX);
            rotacionY = Mathf.Clamp(rotacionY, -limiteY, limiteY);

            // Aplicar rotación
            transform.localRotation = Quaternion.Euler(rotacionX, rotacionY, 0f);
        }       
    }

    public void DetenerRotacion()
    {
        detener = true;
    }

    public void ReanudarRotacion()
    {
        detener = false;
    }
}
