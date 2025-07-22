using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


public class GameEndPopup : MonoBehaviour
{
    public Image winImage;
    public Image lossImage;
    private bool isTween;
    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip lossClip;



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
            SoundManager.instance.PlaySFXUI(winClip);
            isTween = true;
            winImage.transform.DOMove(new Vector3(winImage.transform.position.x, 1000, winImage.transform.position.z), 1f);
        }
        if (!isTween && BattleManager.instance.battleState == BattleState.loss)
        {
            SoundManager.instance.PlaySFXUI(lossClip);
            isTween = true;
            lossImage.transform.DOMove(new Vector3(lossImage.transform.position.x, 1000, lossImage.transform.position.z), 1f);
        }
    }
}
