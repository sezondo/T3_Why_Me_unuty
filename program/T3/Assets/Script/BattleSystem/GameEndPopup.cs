using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


public class GameEndPopup : MonoBehaviour
{
    public Image winImage;
    public Image lossTmage;
    private bool isTween;

    // Start is called once before 
    // the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isTween = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTween && BattleManager.instance.battleState == BattleState.win)
        {
            isTween = true;
            winImage.transform.DOMove(new Vector3(winImage.transform.position.x, 1000, winImage.transform.position.z), 1f);
        }
        if (!isTween && BattleManager.instance.battleState == BattleState.loss)
        {
            isTween = true;
            lossTmage.transform.DOMove(new Vector3(lossTmage.transform.position.x, 1000, lossTmage.transform.position.z), 1f);
        }
    }
}
