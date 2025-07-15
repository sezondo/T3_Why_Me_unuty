using UnityEngine;
using DG.Tweening;

public class ButtonMove : MonoBehaviour
{
    private bool isMove;
    private Vector3 currtTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isMove = false;
        currtTransform = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!ReadyManager.instance.useButton && isMove)
        {
            isMove = false;
            StopMove();
        }

    }

    public void StratMove() // 터치에서 조작
    {
        transform.DOMove(new Vector3(transform.position.x, 200, transform.position.z), 1f);
        isMove = true;
    }
    public void StopMove()
    {
        transform.DOMove(new Vector3(transform.position.x, currtTransform.y, transform.position.z), 1f);
    }
}
