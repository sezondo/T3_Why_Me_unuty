using UnityEngine;

public class Billboard : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate() // 업데이트로 하니 반응이 느려서 레이트로 함
    {
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }
}
