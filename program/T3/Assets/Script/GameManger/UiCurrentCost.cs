using UnityEngine;
using UnityEngine.UI;

public class UiCurrentCost : MonoBehaviour
{
    [SerializeField] private Text currentCostText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentCostText == null)
        {
            Debug.LogWarning("UiCurrentCost: currentCostText not assigned");
            return;
        }
        currentCostText.text = $"Current Cost : {(int)CostManagerNet.instance.currentCost}";
    
    }
}
