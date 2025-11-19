using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GameEndPopupNet : MonoBehaviour
{
    [SerializeField] private Image winImage;
    [SerializeField] private Image lossImage;
    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip lossClip;
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
        if (!isTween && BattleManagerNet.instance.decision == BattleState.win)
        {
            SoundManager.instance.PlaySFXUI(winClip);
            isTween = true;
            winImage.transform.DOMove(new Vector3(winImage.transform.position.x, 1000, winImage.transform.position.z), 1f);


            PlayerManager.instance.UpdateClearNumberIfHigherNet(ReadyManager.instance.levelData.Level);
            
        }
        if (!isTween && BattleManagerNet.instance.decision == BattleState.loss)
        {
            SoundManager.instance.PlaySFXUI(lossClip);
            isTween = true;
            lossImage.transform.DOMove(new Vector3(lossImage.transform.position.x, 1000, lossImage.transform.position.z), 1f);
        }
    }
}
