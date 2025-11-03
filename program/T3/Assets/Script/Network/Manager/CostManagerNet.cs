using UnityEngine;
using Fusion;
using System;

public class CostManagerNet : MonoBehaviour
{
    public static CostManagerNet instance;

    [Header("Cost")]
    public float currentCost { get; private set; }
    public float max { get; private set; } = 10f;
    public float regenPreSec { get; private set; }


    [Header("Network Tick Timer")]
    [SerializeField] private float tickSeconds = 0.2f;
    private TickTimer _regenTimer;


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
        if (Matchmaker.Runner == null)
        {
            return;
        }

        if (currentCost >= max)
        {
            currentCost = max;
        }

        if (currentCost < max && regenPreSec > 0f)
        {
            if (regenPreSec > 0.1)
            {
                currentCost += regenPreSec;
                regenPreSec = 0f;
            }
        }

        if (!_regenTimer.IsRunning)
        {
            _regenTimer = TickTimer.CreateFromSeconds(Matchmaker.Runner, Math.Max(0.01f, tickSeconds));
        }

        if (_regenTimer.Expired(Matchmaker.Runner))
        {
            regenPreSec += tickSeconds;
        }
    }

}
