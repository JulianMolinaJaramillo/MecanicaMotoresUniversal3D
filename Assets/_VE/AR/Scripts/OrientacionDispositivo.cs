using UnityEngine;
using UnityEngine.UI;

public class OrientacionDispositivo : MonoBehaviour
{
    public CanvasScaler scaler;

    void Update()
    {
        switch (Screen.orientation)
        {
            case ScreenOrientation.Portrait:
                scaler.referenceResolution = new Vector2(1080, 1920);
                break;

            case ScreenOrientation.PortraitUpsideDown:
                scaler.referenceResolution = new Vector2(1080, 1920);
                break;

            case ScreenOrientation.LandscapeLeft:
                scaler.referenceResolution = new Vector2(1920, 1080);
                break;

            case ScreenOrientation.LandscapeRight:
                scaler.referenceResolution = new Vector2(1920, 1080);
                break;

            default:
                
                break;
        }
    }
}
