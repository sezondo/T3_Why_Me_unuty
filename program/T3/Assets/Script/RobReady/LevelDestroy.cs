using UnityEngine;

public class LevelDestroy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ReadyManager.instance.readyManagerState == ReadyManagerState.Start)
        {
            Destroy(gameObject);
        }
    }
}
