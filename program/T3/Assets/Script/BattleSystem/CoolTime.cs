using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class CoolTime : MonoBehaviour
{
    private Slider healthSlider;
    private RobReadyNet robReadyNet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        robReadyNet = GetComponentInParent<RobReadyNet>();
        healthSlider = GetComponent<Slider>();
        healthSlider.maxValue = robReadyNet.robNetworkUnitData.spongeWaitingTime;
        healthSlider.value = 0;
    }

    // Update is called once per frame
    void Update()
    {

        if (!robReadyNet.IsSpawned)
        {
            return;
        }
        if (robReadyNet.Tick.IsRunning)
        {
            healthSlider.value = healthSlider.maxValue - (robReadyNet.Tick.RemainingTime(Matchmaker.Runner) ?? 0f);
        }
        
    }
}
