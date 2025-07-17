using UnityEngine;
using System.Collections;
using DG.Tweening;



public class ReadyButtonDestroy : MonoBehaviour
{
    public float movePoint;
    private bool isDestroyed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movePoint = 3000;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDestroyed && ReadyManager.instance.readyManagerState == ReadyManagerState.Start)
        {
            isDestroyed = true;
            StartCoroutine(ButtonDestroy());
        }
    }

    public IEnumerator ButtonDestroy()
    {
        transform.DOMove(new Vector3(movePoint, transform.position.y, transform.position.z), 2f);

        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
    
}
