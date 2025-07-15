using UnityEngine;

public class RobReadyEnable : MonoBehaviour
{
    private RobBaseReady RobBaseReady;
    private Renderer renderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RobBaseReady = GetComponentInParent<RobBaseReady>();
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        ColorChange();
    }

    void ColorChange()
    {
        if (RobBaseReady.readyState == ReadyState.Readying)
        {
            renderer.enabled = false;
        }
        else if(RobBaseReady.readyState == ReadyState.Readyed)
        {
            renderer.enabled = true;
        }
    }
}
