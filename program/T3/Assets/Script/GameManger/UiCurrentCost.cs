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
        //Debug.Log(CostManagerNet.instance.currentCost);
        currentCostText.text = "Current Cost : " + ((int)CostManagerNet.instance.currentCost).ToString();
    }
}
