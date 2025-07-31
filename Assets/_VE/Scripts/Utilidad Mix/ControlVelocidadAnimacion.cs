using UnityEngine;

public class ControlVelocidadAnimacion : MonoBehaviour
{
    [Header("Parámetros de velocidad")]
    [Range(0f, 1f)]
    public float velocidadSlider = 1f; // Control desde el Inspector

    public float velocidadBase = 2f;   // Factor multiplicador

    [Header("Referencias")]
    public Animator animator;

    private void Update()
    {
        if (animator != null)
        {
            animator.speed = velocidadSlider * velocidadBase;
        }
    }
}
