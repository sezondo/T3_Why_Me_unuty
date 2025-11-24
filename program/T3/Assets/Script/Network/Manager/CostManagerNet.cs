using UnityEngine;
using Fusion;
using System;

public class CostManagerNet : MonoBehaviour
{
    public static CostManagerNet instance;
    public float currentCost{ get; private set; }
    public float max { get; private set; } = 10f;
    public float regenPerSec;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log("[CostManagerNet] Awake: instance assigned.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentCost = 3f;
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    void Update()
    {

        currentCost = Mathf.Min(max, currentCost + regenPerSec * Time.deltaTime);

     //   Debug.Log(currentCost);
    }

    public void TakeCost(float minusCost)
    {
        currentCost -= minusCost;
    }

}
