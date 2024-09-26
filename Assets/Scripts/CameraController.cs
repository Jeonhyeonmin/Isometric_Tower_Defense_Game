using UnityEngine;

public class CameraController : MonoBehaviour
{
	[Header("카메라 이동 설정")]
	[SerializeField] private float panSpeed = 0.1f;

	[Header("줌 속도")]
	[SerializeField] private float zoomspeed = 10f;

	[Header("줌 최소, 최대값")]
	[SerializeField] private float minZoomValue = 2.5f;
	[SerializeField] private float maxZoomValue = 5.5f;

	[Header("카메라 경계")]
	[SerializeField] private Vector2 minCameraPosition;
	[SerializeField] private Vector2 maxCameraPosition;

	[Header("부드러운 카메라 이동")]
	private Vector3 velocity;
	[SerializeField] private float smoothTime = 0.2f;
	private Vector3 targetPosition; // 목표 위치

	private Camera cam;
	private Vector3 lastTouchPosition;
	private Vector3 lastMousePosition;
	private bool isPanning;

	private void Awake()
	{
		cam = Camera.main;
		targetPosition = cam.transform.position;
	}

	private void Update()
	{
#if UNITY_EDITOR_WIN || UNITY_STANDALONE

		HandleCameraMovement_PC();
		HandleCameraZoom_PC();
#endif

#if UNITY_ANDROID && !UNITY_EDITOR

        HandleCameraMovement();
        HandleCameraZoom();

#endif
	}

	/// <summary>
	/// WINDOW 운영체제 유니티 에디터에서만 실행이 가능한 코드이다.
	/// </summary>

#if UNITY_EDITOR_WIN || UNITY_STANDALONE

	private void HandleCameraMovement_PC()
	{
		if (Input.GetMouseButtonDown(0))  // 마우스 버튼을 처음 눌렀을 때
		{
			lastMousePosition = Input.mousePosition;
			isPanning = true;
		}
		else if (Input.GetMouseButton(0) && isPanning)  // 마우스 버튼을 누르고 있을 때
		{
			Vector3 delta = Input.mousePosition - lastMousePosition;
			Vector3 move = delta * panSpeed * Time.deltaTime;
			targetPosition = cam.transform.position - move; // targetPosition 업데이트

			// 경계 조건 검사
			targetPosition.x = Mathf.Clamp(targetPosition.x, minCameraPosition.x, maxCameraPosition.x);
			targetPosition.y = Mathf.Clamp(targetPosition.y, minCameraPosition.y, maxCameraPosition.y);

			lastMousePosition = Input.mousePosition;
		}
		else if (Input.GetMouseButtonUp(0))  // 마우스 버튼을 놓았을 때
		{
			isPanning = false;
		}

		// SmoothDamp을 사용해 부드러운 이동 및 감속 처리
		cam.transform.position = Vector3.SmoothDamp(cam.transform.position, targetPosition, ref velocity, smoothTime);

		// 패닝이 끝난 후 velocity를 서서히 0으로 줄이기 위해 작은 값으로 줄임
		if (!isPanning && velocity.magnitude > 0.01f) // 미세한 움직임 방지
		{
			velocity = Vector3.Lerp(velocity, Vector3.zero, smoothTime);
		}
	}

	private void HandleCameraZoom_PC()
	{
		float scroll = Input.GetAxis("Mouse ScrollWheel");

		if (scroll != 0)
		{
			float newZoom = cam.orthographicSize - scroll * zoomspeed * Time.deltaTime;
			cam.orthographicSize = Mathf.Clamp(newZoom, minZoomValue, maxZoomValue);
		}
	}

#endif


	/// <summary>
	/// 안드로이드에서만 실행 가능한 코드
	/// 유니티 에디터에서 실행하기 위해서는 "!UNITY_EDITOR"를 제거해야 한다.
	/// </summary>

#if UNITY_ANDROID && !UNITY_EDITOR
    private void HandleCameraMovement()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position;
                isPanning = true;
            }
            else if (touch.phase == TouchPhase.Moved && isPanning)
            {
                Vector3 delta = new Vector3(touch.position.x, touch.position.y, 0) - new Vector3(lastTouchPosition.x, lastTouchPosition.y, 0);
                Vector3 move = delta * panSpeed * Time.deltaTime;
                targetPosition = cam.transform.position - move; // targetPosition 업데이트

                // 경계 조건 검사
                targetPosition.x = Mathf.Clamp(targetPosition.x, minCameraPosition.x, maxCameraPosition.x);
                targetPosition.y = Mathf.Clamp(targetPosition.y, minCameraPosition.y, maxCameraPosition.y);

                lastTouchPosition = touch.position; // 업데이트된 위치 저장
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isPanning = false;
            }

            // 부드러운 이동 및 감속 처리
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, targetPosition, ref velocity, smoothTime);

            if (!isPanning && velocity.magnitude > 0.01f)
            {
                velocity = Vector3.Lerp(velocity, Vector3.zero, smoothTime);
            }
        }
    }

    private void HandleCameraZoom()
    {
        if(Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
            Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

            float prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;
            float touchDeltaMag = (touch1.position - touch2.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + deltaMagnitudeDiff * zoomspeed * Time.deltaTime, minZoomValue, maxZoomValue);
        }
    }

#endif
}