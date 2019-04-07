using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    public int avgFrameRate;
    public Text display_Text;
    public float refreshRate;

    float nextRefresh = 0f;

    public void Update()
    {
        if (nextRefresh < Time.time)
        {
            avgFrameRate = (int)(1.0f / Time.deltaTime);
            display_Text.text = avgFrameRate.ToString() + " FPS";
            nextRefresh = Time.time + refreshRate;
        }
    }
}