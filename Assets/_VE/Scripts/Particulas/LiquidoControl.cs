using UnityEngine;

public class LiquidoControl : MonoBehaviour
{
    public Transform liquidoTransform;
    public float minFillY = 0.1f;
    public float maxFillY = 1.0f;
    public float currentFill = 1.0f; // De 0 a 1

    private void Start()
    {
        SetFill(1f);
    }
    void Update()
    {
        // Controla la altura del líquido
        float yScale = Mathf.Lerp(minFillY, maxFillY, currentFill);
        Vector3 scale = liquidoTransform.localScale;
        scale.y = yScale;
        liquidoTransform.localScale = scale;
    }

    public void SetFill(float value)
    {
        currentFill = Mathf.Clamp01(value);
    }

    
}
