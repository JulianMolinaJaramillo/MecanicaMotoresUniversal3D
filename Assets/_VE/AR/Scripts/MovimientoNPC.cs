using System.Collections;
using UnityEngine;

public class MovimientoNPC : MonoBehaviour
{
    [Header("Configuración")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Transform[] puntos;           // Puntos de movimiento (en local)
    public float velocidad = 0.1f;         // Velocidad de movimiento
    public float tiempoEspera = 2f;      // Tiempo de espera en cada punto
    public bool npcFinal;
    
    //Configuraciones Privadas
    private Transform destinoActual;
    private bool esperando = false;
    private bool corriendo = false;
    private Coroutine coroutine;
    private Vector3 posicionAnterior;

    void Start()
    {
        
        posicionAnterior = transform.localPosition; // Guardamos la posición inicial
        ElegirNuevoDestino();
    }

    void Update()
    {
        if (destinoActual == null || esperando) return;

        // --- Movimiento en espacio local ---
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, destinoActual.localPosition, velocidad * Time.deltaTime);
        Vector3 posicionActual = transform.localPosition;

        // Comparamos la posición en X con la anterior
        if (posicionActual.x > posicionAnterior.x)
        {
            // Va hacia la derecha
            spriteRenderer.flipX = false;
        }
        else if (posicionActual.x < posicionAnterior.x)
        {
            // Va hacia la izquierda
            spriteRenderer.flipX = true;
        }

        // Actualizamos la posición anterior
        posicionAnterior = posicionActual;

        if (!corriendo)
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", true);
            animator.SetBool("Run", false);
        }

        if (npcFinal)
        {
            // Revisar si ya llegó (exacto)
            if (transform.localPosition == destinoActual.localPosition)
            {
                destinoActual = null;
                animator.SetBool("Idle", true);
                animator.SetBool("Walk", false);
                animator.SetBool("Run", false);     
            }
        }
        else
        {
            // Revisar si ya llegó (exacto)
            if (transform.localPosition == destinoActual.localPosition)
            {
                if (coroutine != null) StopCoroutine(coroutine);
                coroutine = StartCoroutine(EsperarYContinuar());
            }
        }      
    }

    [ContextMenu("correr")]
    public void CorrerPorSuVida()
    {
        corriendo = true;
        animator.SetBool("Run", true);
        animator.SetBool("Walk", false);
        animator.SetBool("Idle", false);
        tiempoEspera = 0.01f;
        velocidad = 0.2f;

        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(EsperarYContinuar());    
    }

    IEnumerator EsperarYContinuar()
    {
        esperando = true;

        if (!corriendo)
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Idle", true);
            animator.SetBool("Run", false);
        }
        
        yield return new WaitForSeconds(tiempoEspera);
        ElegirNuevoDestino();
        esperando = false;
    }

    void ElegirNuevoDestino()
    {
        if (puntos.Length == 0) return;
        destinoActual = puntos[Random.Range(0, puntos.Length)];
    }

    private void OnEnable()
    {
        if (corriendo)
        {
            CorrerPorSuVida();
        }
        else
        {
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(EsperarYContinuar());
        }       
    }
}
