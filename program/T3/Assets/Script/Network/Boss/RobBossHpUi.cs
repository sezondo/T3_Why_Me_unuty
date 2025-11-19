using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class RobBossHpUi : NetworkBehaviour
{
    private RobBase robBase;
    private RobHp robHp;
    [HideInInspector] public float currentHpUi;
    public RawImage imgBar;
    [Networked]
    private float currentHp_UI{get; set;}
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        

        if (robBase == null)
        {
            robBase = GetComponent<RobBase>();
            return;
        }
        if (robHp == null)
        {
            robHp = GetComponent<RobHp>();
            return;
        }

        if (robBase == null || robHp == null || imgBar == null)
            return;

        if (Runner.IsServer){
            currentHp_UI = robHp.currentHp;
        }

        imgBar.transform.localScale = new Vector3(currentHp_UI/robBase.data.maxHp,1,1);
    }
}

