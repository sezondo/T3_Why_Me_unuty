using UnityEngine;
using System.Collections;


public class BattleManager : MonoBehaviour
{
    [SerializeField] private LayerMask allyLayerMask;
    [SerializeField] private LayerMask enemyLayerMask;
    public BattleState battleState;
    public static BattleManager instance;
    private bool isCoroutine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // 싱글톤 변수 instance가 비어있는가?
        if (instance == null)
        {
            // instance가 비어있다면(null) 그곳에 자기 자신을 할당
            instance = this;
        }
        else
        {
            // instance에 이미 다른 GameManager 오브젝트가 할당되어 있는 경우

            // 씬에 두개 이상의 GameManager 오브젝트가 존재한다는 의미.
            // 싱글톤 오브젝트는 하나만 존재해야 하므로 자신의 게임 오브젝트를 파괴
            Debug.LogWarning("씬에 두개 이상의 게임 매니저가 존재합니다!");
            Destroy(gameObject);
        }

        battleState = BattleState.inJudgment;
    }


    // Update is called once per frame
    void Update()
    {
        if (!isCoroutine && ReadyManager.instance.readyManagerState == ReadyManagerState.Start)
        {
            isCoroutine = true;
            StartCoroutine(EndGameCheck());
        }
    }

    public IEnumerator EndGameCheck()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            Collider[] collidersAlly = Physics.OverlapSphere(Vector3.zero, 1000f, allyLayerMask);
            Collider[] collidersEnemy = Physics.OverlapSphere(Vector3.zero, 1000f, enemyLayerMask);

            if (collidersEnemy.Length == 0)
            {
                battleState = BattleState.win;
                break;
            }
            else if (collidersAlly.Length == 0)
            {
                battleState = BattleState.loss;
                break;
            }

            yield return new WaitForSeconds(2f);
        }
    }
}
