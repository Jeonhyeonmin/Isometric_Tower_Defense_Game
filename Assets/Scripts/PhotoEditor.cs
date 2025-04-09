using System;
using System.Collections;
using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PhotoEditor : MonoBehaviour
{
	[SerializeField] private GameObject EditorTool;      // 사진 편집기 도구의 UI 객체를 참조합니다.
	[SerializeField] private RawImage photoDisplay;       // 선택된 사진을 표시할 UI RawImage를 참조합니다.
	[SerializeField] private RectTransform cropArea;      // 자르기 영역을 정의하는 UI RectTransform을 참조합니다.
	[SerializeField] private Button confirmButton;        // 확인 버튼을 참조합니다.
	[SerializeField] private Button cancelButton;

	private Texture2D selectedPhoto;                      // 선택된 사진을 저장할 Texture2D 객체입니다.

	[SerializeField] private Image previewProfileImage;   // 프로필 사진의 미리보기를 표시할 Image를 참조합니다.
	[SerializeField] private Image profileImage;          // 최종 프로필 사진을 설정할 Image를 참조합니다.

	private Vector2 lastMousePosition;                    // 마지막 마우스 위치를 저장하는 변수입니다.
	[SerializeField] private float minSize = 100f;        // 자르기 영역의 최소 크기입니다.
	[SerializeField] private float maxSize = 500f;        // 자르기 영역의 최대 크기입니다.

	[SerializeField] private TMP_Text unityWebRequestText;
	[SerializeField] private Sprite networkError;
	[SerializeField] private Sprite protocol404Error;
	[SerializeField] private Sprite protocol500Error;
	[SerializeField] private Sprite protocol403Error;
	[SerializeField] private Sprite protocolDefaultError;

	private void Start()
	{
		// 확인 버튼에 CropPhoto 메서드를 리스너로 등록합니다.
		confirmButton.onClick.AddListener(CropPhoto);
		cancelButton.onClick.AddListener(CancelCropPhoto);
	}

	public void OpenEditor(string imagePath)
	{
		// 편집 도구 UI를 활성화합니다.
		EditorTool.SetActive(true);

		// 이미지 경로를 사용하여 이미지를 비동기적으로 로드합니다.
		StartCoroutine(LoadImage(imagePath));
	}

	private IEnumerator LoadImage(string imagePath)
	{
		// 플랫폼에 따라 이미지 경로를 설정합니다.
		string filePath = "";


		if (imagePath.StartsWith("https://", System.StringComparison.OrdinalIgnoreCase))
		{
			filePath = imagePath;
		}
		else
		{
#if UNITY_ANDROID

			// 안드로이드에서는 'file://' 프로토콜을 사용합니다.
			filePath = "file://" + imagePath;
#else

                        // 다른 플랫폼에서는 'file:///' 프로토콜을 사용합니다.
                        filePath = "file:///" + imagePath;
#endif
		}

		// 이미지 로드를 위한 UnityWebRequest를 생성합니다.
		UnityWebRequest www = UnityWebRequestTexture.GetTexture(filePath);
		// 웹 요청을 비동기적으로 수행합니다.
		yield return www.SendWebRequest();

		if (www.result == UnityWebRequest.Result.Success)
		{
			// 요청이 성공하면, 응답에서 Texture2D 객체를 가져옵니다.
			selectedPhoto = DownloadHandlerTexture.GetContent(www);
			// 로드된 사진을 UI RawImage에 표시합니다.
			photoDisplay.texture = selectedPhoto;
			unityWebRequestText.text = "The image was successfully loaded";
		}
		else if (www.result == UnityWebRequest.Result.InProgress)
		{
			unityWebRequestText.text = "Trying to load an image";
		}
		else if (www.result == UnityWebRequest.Result.ConnectionError)
		{
			unityWebRequestText.text = "Network connection error";
			photoDisplay.texture = textureFromSprite(networkError);
		}
		else if (www.result == UnityWebRequest.Result.ProtocolError)
		{
			long responseCode = www.responseCode;

			switch (responseCode)
			{
				case 404:
					unityWebRequestText.text = "오류 404\n요청한 리소스를 찾을 수 없습니다.";
					photoDisplay.texture = textureFromSprite(protocol404Error);
					break;
				case 500:
					unityWebRequestText.text = "오류 500\n서버 내부 오류.";
					photoDisplay.texture = textureFromSprite(protocol500Error);
					break;
				case 403:
					unityWebRequestText.text = "오류 403\n접근이 금지되었습니다.";
					photoDisplay.texture = textureFromSprite(protocol403Error);
					break;
				default:
					unityWebRequestText.text = $"프로토콜 오류\nHTTP {responseCode}";
					photoDisplay.texture = textureFromSprite(protocolDefaultError);
					break;
			}
		}
		else
		{
			unityWebRequestText.text = $"Unknown Error\n" + www.error;
			photoDisplay.texture = textureFromSprite(protocolDefaultError);
		}
	}

	private Texture2D textureFromSprite(Sprite sprite)
	{
		if (sprite == null)
		{
			Debug.LogError("오류에 맞는 스프라이트 이미지가 없습니다.");
			EditorTool.SetActive(false);
			selectedPhoto = null;
			return null;
		}

		return sprite.texture;
	}

	private void Update()
	{
		// 자르기 영역의 스케일링을 처리합니다.
		HandleCropAreaScaling();
	}

	private void HandleCropAreaScaling()
	{
		// 두 손가락으로 터치할 때 핀치 줌을 처리합니다.
		if (Input.touchCount == 2)
		{
			Touch touchZero = Input.GetTouch(0);   // 첫 번째 터치
			Touch touchOne = Input.GetTouch(1);    // 두 번째 터치

			// 이전 프레임의 터치 위치를 계산합니다.
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			//Debug.Log("touchZeroPrevPos : " + touchZeroPrevPos);
			//Debug.Log("touchZeroDelta : " + touchZero.deltaPosition);
			//Debug.Log("TouchZeroPosition : " + touchZero.position);
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
			//Debug.Log("touchOnePrevPos : " + touchOnePrevPos);
			//Debug.Log("touchOneDelta : " + touchOne.deltaPosition);
			//Debug.Log("TouchOnePosition : " + touchOne.position);

			// 터치 간의 이전 및 현재 거리 차이를 계산합니다.
			float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			//Debug.Log("prevMagnitude : " + prevMagnitude);
			float currentMagnitude = (touchZero.position - touchOne.position).magnitude;
			//Debug.Log("Magnitude : " + currentMagnitude);

			// 거리 차이를 사용하여 자르기 영역의 크기를 조정합니다.
			float difference = currentMagnitude - prevMagnitude;
			ResizeCropArea(difference * 0.1f);  // 거리 차이의 10%를 크기 변경으로 적용합니다.
		}

		// 마우스 휠 스크롤을 사용하여 자르기 영역의 크기를 조정합니다.
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		//Debug.Log(scroll);

		if (scroll != 0)
		{
			ResizeCropArea(scroll * 100f);  // 스크롤 값에 100을 곱하여 크기 변경으로 적용합니다.
		}
	}

	private void ResizeCropArea(float increment)
	{
		// 자르기 영역의 새로운 크기를 계산합니다.
		Vector2 newSize = cropArea.sizeDelta + new Vector2(increment, increment);

		// 자르기 영역의 크기를 최소 및 최대 크기로 제한합니다.
		newSize.x = Mathf.Clamp(newSize.x, minSize, maxSize);
		newSize.y = Mathf.Clamp(newSize.y, minSize, maxSize);

		// 새로운 크기를 RectTransform에 적용합니다.
		cropArea.sizeDelta = newSize;
	}

	private void CropPhoto()
	{
		if (selectedPhoto == null)
		{
			// 선택된 사진이 없으면 경고 메시지를 출력합니다.
			Debug.LogWarning("No photo selected!");
			EditorTool.SetActive(false);
			selectedPhoto = null;
			return;
		}

		// photoDisplay의 크기를 가져옵니다.
		RectTransform photoDisplayRectTransform = photoDisplay.GetComponent<RectTransform>();
		Vector2 photoDisplaySize = photoDisplayRectTransform.sizeDelta;

		// 자르기 영역의 크기와 위치를 계산합니다.
		Rect cropRect = GetCropRect();

		// 자르기 영역을 원본 이미지 크기에 맞게 조정합니다.
		float x = cropRect.x / photoDisplaySize.x * selectedPhoto.width;
		// -60 / 780 * 1255 = -96.54
		float y = cropRect.y / photoDisplaySize.y * selectedPhoto.height;
		// -60 / 780 * 1255 = -96.54
		float width = cropRect.width / photoDisplaySize.x * selectedPhoto.width;
		// 900 / 780 * 1255 = 1,448.08
		float height = cropRect.height / photoDisplaySize.y * selectedPhoto.height;
		// 900 / 780 * 1255 = 1,448.08

		// 조정된 크기와 위치로 새로운 Rect를 생성합니다.
		cropRect = new Rect(x, y, width, height);
		// -96.54, -96.54, 1,448.08, 1,448.08

		// 자르기 작업을 수행합니다.
		Texture2D croppedPhoto = CropToCircle(selectedPhoto, cropRect);

		// 최종 프로필 사진을 설정합니다.
		SetProfilePicture(croppedPhoto);
	}

	private Rect GetCropRect()
	{
		// cropArea의 크기와 위치를 가져옵니다.
		Vector2 size = cropArea.rect.size;
		Vector2 position = cropArea.anchoredPosition;

		// photoDisplay의 크기를 가져옵니다.
		RectTransform photoDisplayRectTransform = photoDisplay.GetComponent<RectTransform>();
		Vector2 photoDisplaySize = photoDisplayRectTransform.sizeDelta;

		// cropArea의 위치를 photoDisplay의 좌표계에 맞추어 변환합니다.
		float x = position.x + photoDisplaySize.x / 2;
		float y = position.y + photoDisplaySize.y / 2;
		return new Rect(x - size.x / 2, y - size.y / 2, size.x, size.y);
		// -60, - 60, 900, 900
	}

	private Texture2D CropToCircle(Texture2D originalTexture, Rect cropRect)
	{
		// cropRect를 원본 이미지의 경계 내로 조정합니다.
		cropRect.x = Mathf.Clamp(cropRect.x, 0, originalTexture.width);
		cropRect.y = Mathf.Clamp(cropRect.y, 0, originalTexture.height);
		cropRect.width = Mathf.Clamp(cropRect.width, 0, originalTexture.width - cropRect.x);
		cropRect.height = Mathf.Clamp(cropRect.height, 0, originalTexture.height - cropRect.y);

		// 자를 영역의 크기로 새로운 Texture2D를 생성합니다.
		int width = Mathf.RoundToInt(cropRect.width);
		int height = Mathf.RoundToInt(cropRect.height);
		Texture2D croppedTexture = new Texture2D(width, height);

		// 자를 영역의 픽셀 데이터를 가져옵니다.
		Color[] pixels = originalTexture.GetPixels(Mathf.RoundToInt(cropRect.x), Mathf.RoundToInt(cropRect.y), width, height);

		// 원형 영역의 반지름과 중심점을 계산합니다.
		float radius = width / 2f;
		Vector2 center = new Vector2(width / 2f, height / 2f);

		// 자르기 영역의 각 픽셀에 대해 루프를 수행합니다.
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				// 현재 픽셀의 위치를 계산합니다.
				Vector2 pixelPos = new Vector2(x, y);

				// 현재 픽셀이 원형 영역 안에 있는지 확인합니다.
				if (Vector2.Distance(pixelPos, center) <= radius)
				{
					// 원형 영역 안에 있는 경우 해당 픽셀의 색상을 설정합니다.
					croppedTexture.SetPixel(x, y, pixels[y * width + x]);
				}
				else
				{
					// 원형 영역 밖에 있는 경우 해당 픽셀을 투명하게 설정합니다.
					croppedTexture.SetPixel(x, y, Color.clear);
				}
			}
		}

		// 변경된 텍스처 데이터를 적용합니다.
		croppedTexture.Apply();

		// 자른 텍스처를 반환합니다.
		return croppedTexture;
	}

	public void SetProfilePicture(Texture2D profilePhoto)
	{
		// 미리보기 이미지와 프로필 이미지를 설정합니다.
		previewProfileImage.sprite = SpriteFromTexture2D(profilePhoto);
		profileImage.sprite = previewProfileImage.sprite;
		PlayerWalletManager.Instance.profile = profileImage.sprite; 
        SaveTextureToPNG(profilePhoto, "profile.png");
        // 편집 도구 UI를 비활성화합니다.
        EditorTool.SetActive(false);
	}
	
	private void SaveTextureToPNG(Texture2D texture, string fileName)
	{
		byte[] pngData = texture.EncodeToPNG();
		string base64Image = Convert.ToBase64String(pngData);

		Param param = new Param();
		param.Add("profile", base64Image);
        PlayerWalletManager.Instance.profilebase64 = base64Image;
        Backend.CloudSave.Upload("User_Profile", param, callback =>
		{
			if (callback.IsSuccess())
			{

				Debug.Log("Profile picture uploaded successfully.");
                

            }
			else
			{
				Debug.LogError("Failed to upload profile picture: " + callback.GetErrorCode());
			}
        });
    }

	private Sprite SpriteFromTexture2D(Texture2D texture)
	{
		// Texture2D에서 Sprite를 생성합니다.
		Rect rect = new Rect(0, 0, texture.width, texture.height);
		return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
	}

	private void CancelCropPhoto()
	{
		StopAllCoroutines();
		selectedPhoto = null;
		EditorTool.SetActive(false);
	}

	private void OnDisable()
	{
		confirmButton.onClick.RemoveListener(CropPhoto);
		cancelButton.onClick.RemoveListener(CancelCropPhoto);
	}
}
