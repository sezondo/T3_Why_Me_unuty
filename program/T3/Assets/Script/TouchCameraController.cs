using UnityEngine;

public class TouchCameraController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float zoomSpeed = 10f;
    public float minZoom = 5f;
    public float maxZoom = 20f;

    public float xLimit_left = 10;
    public float xLimit_right = 10;
    public float zLimit_left = 10;
    public float zLimit_right = 50;

    private Vector2 lastTouchPos;
    private bool isDragging = false;

    private Vector2 xLimit;
    private Vector2 zLimit;

    void Start()
    {   
        xLimit = new Vector2(transform.position.x - xLimit_left, transform.position.x + xLimit_right);
        zLimit = new Vector2(transform.position.z - zLimit_left, transform.position.z + zLimit_right);
    }


    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            // 드래그 시작
            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPos = touch.position;
                isDragging = true;
            }

            // 드래그 중
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector2 delta = touch.position - lastTouchPos;
                Vector3 move = new Vector3(-delta.x, 0, -delta.y) * moveSpeed * Time.deltaTime;

                // 이동 적용 전에 현재 위치 저장
                Vector3 newPosition = transform.position + move;

                // X/Z 위치를 Clamp해서 범위 제한
                newPosition.x = Mathf.Clamp(newPosition.x, xLimit.x, xLimit.y);
                newPosition.z = Mathf.Clamp(newPosition.z, zLimit.x, zLimit.y);


                transform.position = newPosition;

                lastTouchPos = touch.position;
            }

            // 드래그 종료
            else if (touch.phase == TouchPhase.Ended)
            {
                isDragging = false;
            }
        }

        // 핀치 줌 (두 손가락)
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 prevT0 = t0.position - t0.deltaPosition;
            Vector2 prevT1 = t1.position - t1.deltaPosition;

            float prevDist = Vector2.Distance(prevT0, prevT1);
            float currDist = Vector2.Distance(t0.position, t1.position);
            float delta = prevDist - currDist;

            Camera cam = Camera.main;
            if (cam != null)
            {
                float newY = cam.transform.position.y + delta * zoomSpeed * Time.deltaTime;
                newY = Mathf.Clamp(newY, minZoom, maxZoom);

                Vector3 camPos = cam.transform.position;
                camPos.y = newY;
                cam.transform.position = camPos;
            }
        }
    }

    
}
