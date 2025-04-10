using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using BackEnd;
using BackEnd.BackndLitJson;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static BackEnd.Quobject.SocketIoClientDotNet.Parser.Parser.Encoder;

public class UIManager : MonoBehaviour
{
	#region Music 
	[Header("Music Variables")]
	[SerializeField] private Button musicButton;
	[SerializeField] private AudioSource music_AudioSource;
	[SerializeField] private Sprite onMusicSprite;
	[SerializeField] private Sprite offMusicSprite;
	[SerializeField] private Sprite pushOnMusicSprite;
	[SerializeField] private Sprite pushOffMusicSprite;
	public bool isPlayMusic;
	#endregion

	[Space(30)]

	#region Settings
	[Header("Setting Variables")]
	[SerializeField] private GameObject menuSettingGroup;
	#endregion

	#region GameReady
	[Header("GameReady Variables")]
	public GameObject gameReadyGroup;
	#endregion

	#region GameSystem
	[Header("Game System")]
	private Coroutine towerCoroutine;
	public GameObject towerNotifications;
	#endregion

	#region TowerInfo
	[Header("TowerInfo")]
	public GameObject towerInfo;
	#endregion

	#region Profile
	[Header("ProfileInfo")]
	public GameObject profileSettings;

	[SerializeField] private TMP_Text nicknameText;
	[SerializeField] private TMP_Text nicknameInputField_Hint;
	[SerializeField] private TMP_InputField nicknameInputField;

	[SerializeField] private TMP_Text currentLevel;
	[SerializeField] private TMP_Text currentExp;
	[SerializeField] private Image expBar;

    private Coroutine feedbackCoroutine;
	[SerializeField] private GameObject feddbackPanel;
	[SerializeField] private TMP_Text feedbackText;
	private string nickname;

	[SerializeField] private string phoneNumber = "01068024032";

	[SerializeField] private PhotoEditor photoEditor;

	private Coroutine galleryManagementCoroutine;
	[SerializeField] private GameObject galleryManagementPanel;
	[SerializeField] private TMP_Text galleryManagementText;

	[SerializeField] private TMP_InputField urlProfileInputField;
	[SerializeField] private TMP_Text urlProfileInputField_Hint;
	[SerializeField] private TMP_Text urlProfileText;

    [SerializeField] private Image profileImage;
	[SerializeField] private Image profileEditor_Image;

    #endregion

    #region QuickMenu

    private Coroutine stageErrorCoroutine;
	[SerializeField] private Button startStageButton;

	[SerializeField] private GameObject stageErrorPanel;
	[SerializeField] private TMP_Text stageErrorText;

	[SerializeField] private TMP_Text degreeOfProgressTitleText;
	[SerializeField] private TMP_Text degreeOfProgressSubText;

	[SerializeField] private Image degreeOfProgressTopBar;

	[SerializeField] private TMP_Text cristalMultipleText;
	[SerializeField] private TMP_Text coinMultipleText;

	[SerializeField] private TMP_ColorGradient oneTimesGradient;
	[SerializeField] private TMP_ColorGradient onePointFiveTimesGradient;
	[SerializeField] private TMP_ColorGradient twoTimesGradient;

	[SerializeField] private TMP_Text degreeOfProgressSliderText;

	[SerializeField] private TMP_FontAsset fontAsset;

	#endregion

	#region QuickMenuInGame

	[SerializeField] private GameObject quickMenuInGameObject;

	[SerializeField] private Button questButton;
	[SerializeField] private Button shopButton;

	#endregion

	#region Enemy

	[SerializeField] private EnemySpawner enemySpawner;
	[SerializeField] private TMP_Text remainEnemyCount;

    #endregion

    #region Stage

    [Header("Stage Finish")]
	[SerializeField] private GameObject victoryOrDefeatGameObject;
	[SerializeField] private TMP_Text victoryOrDefeatTitle_Text;
	[SerializeField] private List<Image> starImageList;
	[SerializeField] private Sprite normalStar;
	[SerializeField] private Sprite superStar;

	[SerializeField] private GameObject stageClearObject;
	[SerializeField] private GameObject stageFail;

	[SerializeField] private TMP_Text scoreText;

	[SerializeField] private GameObject victoryButtonGroup;
	[SerializeField] private GameObject DefeatButtonGroup;

	[SerializeField] private GameObject coinRewardObject;
	[SerializeField] private TMP_Text coinRewardTitle_Text;
	[SerializeField] private GameObject cristalRewardObject;
	[SerializeField] private TMP_Text cristalRewardTitle_Text;

	public bool isOnResultWindow;

    #endregion

    #region PlayerWallet

    [SerializeField] private TMP_Text coinText;
	[SerializeField] private TMP_Text crystalText;

	#endregion

	#region SkillUpgrade

	[SerializeField] private TMP_Text skill_TotalCoinText;
    [SerializeField] private TMP_Text skill_TotalCrystal;

	#endregion

	#region helpMenu

	[SerializeField] Image[] helpPanel_Group;
	private int currentHelpPageNum;
	public int CurrentHelpPageNum
	{
		get => currentHelpPageNum;
		set => currentHelpPageNum = value;
	}

	#endregion helpMenu

    private void Awake()
	{
		PlayerWalletManager.Instance.VirtualAwake();

        #region Music 

        if (music_AudioSource == null)
		{
			isPlayMusic = false;
		}
		else
		{
			isPlayMusic = true;
		}

		#endregion

		#region GameReadyPanel

		if (gameReadyGroup != null)
		{
			gameReadyGroup.SetActive(true);
		}

		#endregion

		#region GameSystem

		if (towerNotifications != null)
		{
			towerNotifications.SetActive(false);
		}

		#endregion

		#region Profile

		if (nicknameText != null)
		{
			if (nicknameInputField != null)
			{
                nicknameInputField.onEndEdit.AddListener(ValidateAndSetNickname);
                urlProfileInputField.onEndEdit.AddListener(ValidationandURLPassing);
            }

			Backend.BMember.GetUserInfo((callback =>
			{
				string nickname = callback.GetReturnValuetoJSON()["row"]["nickname"].ToString();
				nicknameText.text = nickname;

                if (nicknameInputField != null)
                    nicknameInputField_Hint.text = nickname;
            }));

            string base64 = PlayerWalletManager.Instance.profilebase64;
            byte[] imageBytes = Convert.FromBase64String(base64);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);

            profileImage.sprite = SpriteFromTexture2D(texture);

			currentLevel.text = "Lv. " + PlayerWalletManager.Instance.PlayerLevel.ToString();
            currentExp.text = PlayerWalletManager.Instance.PlayerExp.ToString() + " / " + PlayerWalletManager.Instance.levelChart[PlayerWalletManager.Instance.PlayerLevel-1].maxExperience;
        }

		#endregion

		#region StageFinsih

		#endregion

		#region helpPanel

		currentHelpPageNum = 0;

        #endregion helpPanel
    }

    private Sprite SpriteFromTexture2D(Texture2D texture)
    {
        // Texture2D¿¡¼­ Sprite¸¦ »ý¼ºÇÕ´Ï´Ù.
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
    }

    private void Update()
	{
		ChangeMusicSprite();
		AutomaticGameModeChange();

		#region Enemy

		RemainEnemy();

		#endregion

		RefreshMenuWallet();
		RefreshSkillUpgrade();
        if (expBar != null)
        {
            float currentExp = PlayerWalletManager.Instance.PlayerExp;
            float maxExp = PlayerWalletManager.Instance.levelChart[PlayerWalletManager.Instance.PlayerLevel - 1].maxExperience;
            expBar.fillAmount = currentExp / maxExp;

            currentLevel.text = "Lv. " + PlayerWalletManager.Instance.PlayerLevel.ToString();
            this.currentExp.text = PlayerWalletManager.Instance.PlayerExp.ToString() + " / " + PlayerWalletManager.Instance.levelChart[PlayerWalletManager.Instance.PlayerLevel - 1].maxExperience;
        }
    }

    private void ChangeMusicSprite()
	{
		if (musicButton == null)
			return;

		var spriteState = musicButton.spriteState;

		if (isPlayMusic)
		{
			spriteState.pressedSprite = pushOnMusicSprite;

			musicButton.GetComponent<Image>().sprite = onMusicSprite;
		}
		else
		{
			spriteState.pressedSprite = pushOffMusicSprite;

			musicButton.GetComponent<Image>().sprite = offMusicSprite;
		}

		musicButton.spriteState = spriteState;
	}

	public void ToggleMusic()
	{
		if (music_AudioSource == null)
			return;

		isPlayMusic = !isPlayMusic;

		if (isPlayMusic) { music_AudioSource.Play(); }
		else { music_AudioSource.Pause(); }
	}

	public void OpenOptions()
	{
		menuSettingGroup.SetActive(true);
	}

	public void onClickQuit()
	{
		SceneManager.LoadScene("LevelSelect");
	}

	public void onTriggerTowerErrorNotify()
	{
		if (towerCoroutine == null)
		{
			towerCoroutine = StartCoroutine(TowerNotify());
		}
	}

	IEnumerator TowerNotify()
	{
		towerNotifications.SetActive(true);
		towerNotifications.GetComponent<Animator>().SetTrigger("isError");

		Animator animator = towerNotifications.GetComponent<Animator>();
		while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
		{
			yield return null;
		}

		towerNotifications.SetActive(false);

		towerCoroutine = null;
	}

	public void ProfileSettings()
	{
		profileSettings.SetActive(true);
		Debug.Log("ÇÁ·ÎÇÊ ¼¼ÆÃÃ¢ÀÌ ¿­·È½À´Ï´Ù.");

		if (profileEditor_Image.gameObject.activeSelf)
		{
			Debug.Log("ÇÁ·ÎÇÊ ÀÌ¹ÌÁö°¡ È°¼ºÈ­ µÇ¾ú½À´Ï´Ù.");
            string base64 = PlayerWalletManager.Instance.profilebase64;
            byte[] imageBytes = Convert.FromBase64String(base64);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);
			Debug.Log(base64);

            profileEditor_Image.sprite = SpriteFromTexture2D(texture);
        }
		else
		{
            Debug.Log("ÇÁ·ÎÇÊ ÀÌ¹ÌÁö°¡ ºñÈ°¼ºÈ­ µÇ¾ú½À´Ï´Ù.");
        }
	}

	public void ValidateAndSetNickname(string name)
	{
		if (name.Length > 8)
		{
			feedbackText.text = "´Ð³×ÀÓÀº ÃÖ´ë 8±ÛÀÚ±îÁö¸¸ °¡´ÉÇÕ´Ï´Ù.";
			onTriggerNicknameErrorNotify();
			return;
		}

		if (!Regex.IsMatch(name, @"^[°¡-ÆRa-zA-Z0-9_]+$"))
		{
			feedbackText.text = "´Ð³×ÀÓÀº ÇÑ±Û, ¾ËÆÄºª, ¼ýÀÚ, Æ¯¼ö¹®ÀÚ '_'¸¸ \n »ç¿ëÇÒ ¼ö ÀÖ½À´Ï´Ù.";
			Debug.Log("¾Æ¾Æ");
			onTriggerNicknameErrorNotify();
			return;
		}

        Backend.BMember.UpdateNickname(nicknameInputField.text, callback =>
        {
            if (callback.IsSuccess())
			{
				Debug.Log("´Ð³×ÀÓÀÌ ¼º°øÀûÀ¸·Î º¯°æµÇ¾ú½À´Ï´Ù.");

                nickname = name;
                nicknameText.text = nickname;
                nicknameInputField_Hint.text = nickname;
                nicknameInputField.text = string.Empty;

                PlayerWalletManager.Instance.nickname = nickname;
                Debug.Log($"´Ð³×ÀÓÀÌ ¼³Á¤ µÊ : {nickname}");
            }
			else
			{
                Debug.LogError($"´Ð³×ÀÓ º¯°æ ½ÇÆÐ: {callback.GetCode()}");

				switch (int.Parse(callback.GetErrorCode()))
				{
					case 400:   // ÀÌ¹Ì Áßº¹µÈ ´Ð³×ÀÓÀÌ ÀÖ´Â °æ¿ì
						feedbackText.text = "ÀÌ¹Ì »ç¿ëÁßÀÎ ´Ð³×ÀÓÀÔ´Ï´Ù.\n´Ù¸¥ ´Ð³×ÀÓÀ» »ç¿ëÇØÁÖ¼¼¿ä.";
						break;
					default:
						feedbackText.text = "¾Ë ¼ö ¾ø´Â ¿À·ù°¡ ¹ß»ýÇß½À´Ï´Ù.";
						break;
				}

				onTriggerNicknameErrorNotify();
            }
        });
	}

	public void onTriggerNicknameErrorNotify()
	{
		if (towerCoroutine == null)
		{
			feedbackCoroutine = StartCoroutine(NicknameNotify());
		}
	}

	IEnumerator NicknameNotify()
	{
		feddbackPanel.SetActive(true);
		feddbackPanel.GetComponent<Animator>().SetTrigger("isError");

		Animator animator = feddbackPanel.GetComponent<Animator>();
		while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
		{
			yield return null;
		}

		feddbackPanel.SetActive(false);

		nicknameInputField.text = string.Empty;
		feedbackCoroutine = null;
	}

	public void CreditList()
	{
		Application.OpenURL("https://github.com/Jeonhyeonmin");
		Debug.Log("OpenURL : ±êÇãºê ¿­¸²");
	}

	public void ContactCustomerService()
	{
		string smsLink = $"sms:{phoneNumber}";

		Application.OpenURL(smsLink);
	}

	public void ValidationandURLPassing(string url)
	{
		if (url.StartsWith("https://", System.StringComparison.OrdinalIgnoreCase))
		{
			galleryManagementText.text = "URL °ËÁõ¿¡ ¼º°øÇß½À´Ï´Ù.";
			onTriggerGalleryManagementNotify();

			photoEditor.OpenEditor(url);
		}
		else
		{
			galleryManagementText.text = "ÀÌ¹ÌÁö URLÀº 'https://'·Î\n½ÃÀÛÇØ¾ß¸¸ ÀÎ½ÄÇÒ ¼ö ÀÖ½À´Ï´Ù.";
			onTriggerGalleryManagementNotify();
		}

		urlProfileText.text = string.Empty;
		urlProfileInputField.text = string.Empty;
	}

	public void OpenGallery()
	{
		NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
		{
			// »ç¿ëÀÚ°¡ ÀÌ¹ÌÁö¸¦ ¼±ÅÃÇÏ°í °¶·¯¸®¿¡¼­ ¹ÝÈ¯µÈ ÀÌ¹ÌÁö °æ·Î°¡ nullÀÌ ¾Æ´Ñ °æ¿ì
			if (path != null)
			{
				FileInfo seleted = new FileInfo(path);

				if (seleted.Length > 5000000)
				{
					galleryManagementText.text = "5MB¸¦ ÃÊ°úÇÏ´Â ÀÌ¹ÌÁö´Â\n»ç¿ëÀÌ ºÒ°¡´É ÇÕ´Ï´Ù.";
					onTriggerGalleryManagementNotify();
					return;
				}

				string extension = Path.GetExtension(path).ToLower();

				if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
				{
					Texture2D selectedTexture = NativeGallery.LoadImageAtPath(path);

					if (selectedTexture == null)
					{
						Debug.LogError("ÀÌ¹ÌÁö¸¦ ºÒ·¯¿ÀÁö ¸øÇß½À´Ï´Ù.");
						galleryManagementText.text = "»ç¿ëÀÚ°¡ ¼±ÅÃÇÑ ÀÌ¹ÌÁö¿¡\n¾Ë ¼ö ¾ø´Â ¹®Á¦°¡ ÀÖ½À´Ï´Ù.";
						onTriggerGalleryManagementNotify();
						return;
					}

					if (selectedTexture.width != selectedTexture.height)
					{
						Debug.LogError("ÀÌ¹ÌÁö°¡ 1:1 ºñÀ²ÀÌ ¾Æ´Õ´Ï´Ù.");
						galleryManagementText.text = "¼±ÅÃÇÑ ÀÌ¹ÌÁö´Â 1:1 ºñÀ²ÀÌ¾î¾ß ÇÕ´Ï´Ù.\nÀÌ¹ÌÁö ÆíÁý µµ±¸¸¦ »ç¿ëÇØ\n1:1 ºñÀ²·Î ÀçÁ¤ÀÇ ÇÏ½Ã±æ ¹Ù¶ø´Ï´Ù.";
						onTriggerGalleryManagementNotify();
						return;
					}

					// ¼±ÅÃµÈ ÀÌ¹ÌÁö °æ·Î¸¦ OpenEditor ¸Þ¼­µå¿¡ Àü´ÞÇÏ¿© ÆíÁý±â¸¦ ¿±´Ï´Ù.
					photoEditor.OpenEditor(path);
				}
				else
				{
					Debug.LogError("Áö¿øÇÏ´Â ¾Ê´Â ÀÌ¹ÌÁö Çü½Ä ÀÔ´Ï´Ù.\nPNG, JPG, JPEG Áö¿ø");
					galleryManagementText.text = "Áö¿øÇÏ´Â ¾Ê´Â ÀÌ¹ÌÁö Çü½Ä ÀÔ´Ï´Ù.\nPNG, JPG, JPEG¸¸ ¼±ÅÃ °¡´ÉÇÕ´Ï´Ù.";
					onTriggerGalleryManagementNotify();
				}

			}
		},

		"ÇÁ·ÎÇÊ·Î »ç¿ëÇÏ°í ½ÍÀº ÀÌ¹ÌÁö¸¦ ¼±ÅÃÇÏ¼¼¿ä", "image/*");

		// ±ÇÇÑ »óÅÂ¿¡ µû¸¥ Ãß°¡ Ã³¸®¸¦ ÇÒ ¼ö ÀÖ´Ù.
		if (permission == NativeGallery.Permission.Denied) // »ç¿ëÀÚ°¡ ¸í½ÃÀûÀ¸·Î °ÅºÎÇÑ °æ¿ì,
		{
			Debug.Log("»ç¿ëÀÚ°¡ °¶·¯¸® ±ÇÇÑÀ» °ÅºÎÇß½À´Ï´Ù.");
			galleryManagementText.text = "¾ÛÀÇ °¶·¯¸® È®ÀÎ ±ÇÇÑÀ» ¼³Á¤ ¾Û¿¡¼­\nÇã¿ëÀ¸·Î º¯°æÇØ¾ß ÇÕ´Ï´Ù.";
			onTriggerGalleryManagementNotify();
		}
		else if (permission == NativeGallery.Permission.ShouldAsk)  // »ç¿ëÀÚ°¡ ±ÇÇÑÀ» ºÎ¿©ÇÑ ÀûÀÌ ¾ø°í, ±ÇÇÑ ¿äÃ»ÀÌ ÇÊ¿äÇÑ °æ¿ì
		{
			Debug.Log("»ç¿ëÀÚ¿¡°Ô °¶·¯¸® ±ÇÇÑ Çã¿ëÀ» ¿äÃ»Çß½À´Ï´Ù.");
			galleryManagementText.text = "¾ÛÀÇ °¶·¯¸® È®ÀÎ ±ÇÇÑÀÌ ¾ø´Ù¸é\nÇÁ·ÎÇÊÀ» º¯°æÇÏ½Ç ¼ö ¾ø½À´Ï´Ù.";
			onTriggerGalleryManagementNotify();
		}
		//else if (permission == NativeGallery.Permission.Granted)  // ±ÇÇÑÀÌ ÀÌ¹Ì ºÎ¿©µÈ °æ¿ì
		//{
		//    Debug.Log("°¶·¯¸® ¿¢¼¼½º¿¡ ¼º°øÇß½À´Ï´Ù.");
		//    galleryManagementText.text = "¾ÛÀÇ °¶·¯¸® ±ÇÇÑÀÌ Á¤»ó È®ÀÎ µË´Ï´Ù.";
		//    onTriggerGalleryManagementNotify();
		//}
	}

	public void onTriggerGalleryManagementNotify()
	{
		if (galleryManagementCoroutine == null)
		{
			galleryManagementCoroutine = StartCoroutine(GalleryManagementNotify());
		}
	}

	IEnumerator GalleryManagementNotify()
	{
		galleryManagementPanel.SetActive(true);
		galleryManagementPanel.GetComponent<Animator>().SetTrigger("isError");

		Animator animator = galleryManagementPanel.GetComponent<Animator>();
		while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
		{
			yield return null;
		}

		galleryManagementPanel.SetActive(false);

		galleryManagementText.text = string.Empty;
		galleryManagementCoroutine = null;
	}

	private void OnDestroy()
	{
		if (nicknameInputField != null)
		{
			nicknameInputField.onEndEdit.RemoveListener(ValidateAndSetNickname);
			urlProfileInputField.onEndEdit.RemoveListener(ValidationandURLPassing);
		}
	}

	public void StartStage()
	{
		if (startStageButton.GetComponent<Animator>().GetBool("Seleted") == false)
		{
			stageErrorText.text = "½ºÅ×ÀÌÁö ±ê¹ßÀ» ¼±ÅÃÇÏ°í\n°ÔÀÓ ½ÃÀÛ ¹öÆ°À» ´­·¯ÁÖ¼¼¿ä.";
			onTriggerStageErrorNotify();
			return;
		}

		string sceneName = StageSelectorRay.Instance.seletedStageName;

		if (IsSceneInBuild(sceneName))
		{
			SceneManager.LoadScene(sceneName);
		}
		else
		{
			stageErrorText.text = "¼±ÅÃÇÑ ½ºÅ×ÀÌÁö´Â ÇöÀç ÆÐÄ¡ ¹öÀü¿¡¼­\nÇã¿ëµÇÁö ¾Ê½À´Ï´Ù.";
			onTriggerStageErrorNotify();
		}
	}

	private bool IsSceneInBuild(string sceneName)
	{
		// 1. ÇöÀç ºôµå ¼³Á¤¿¡ Æ÷ÇÔµÈ ¾ÀµéÀÇ °³¼ö¸¦ °¡Á®¿È
		for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
		{
			// 2. °¢ ¾ÀÀÇ °æ·Î¸¦ °¡Á®¿È (¿¹: "Assets/Scenes/MyScene.unity")
			string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
			// 3. ¾À °æ·Î¿¡¼­ ¾À ÀÌ¸§¸¸ ÃßÃâ (È®ÀåÀÚ ".unity"¸¦ Á¦¿ÜÇÑ ÀÌ¸§)
			string sceneNameInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);
			// 4. ÁÖ¾îÁø ¾À ÀÌ¸§°ú ºôµå ¼³Á¤¿¡ ÀÖ´Â ¾À ÀÌ¸§À» ºñ±³ (´ë¼Ò¹®ÀÚ ±¸ºÐ ¾øÀÌ)
			if (sceneNameInBuild.Equals(sceneName, System.StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}

		return false;
	}

	public void onTriggerStageErrorNotify()
	{
		if (stageErrorCoroutine == null)
		{
			stageErrorCoroutine = StartCoroutine(SceneErrorNotify());
		}
	}

	IEnumerator SceneErrorNotify()
	{
		stageErrorPanel.SetActive(true);
		stageErrorPanel.GetComponent<Animator>().SetTrigger("isError");

		Animator animator = stageErrorPanel.GetComponent<Animator>();
		while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
		{
			yield return null;
		}

		stageErrorPanel.SetActive(false);

		stageErrorText.text = string.Empty;
		stageErrorCoroutine = null;
	}

	private void AutomaticGameModeChange()
	{
		if (coinMultipleText == null || cristalMultipleText == null)
			return;

		switch (GameManager.Instance.modeType)
		{
			case GameManager.ModeType.isEasyMode:
				degreeOfProgressTitleText.text = "½¬¿ò ¸ðµå";
				degreeOfProgressSubText.text = "°ÔÀÓ ³» °¡Àå ½¬¿î ³­ÀÌµµÀÌ¸ç, ÃÊ½ÉÀÚ¿¡°Ô ÃßÃµÇÏ´Â ¸ðµåÀÌ´Ù.\n°ÔÀÓ ³» ±âº» Àü¸®Ç° ¹è¼ö°¡ Àû¿ëµÇ¾î Æ¯º°ÇÑ ÀÌÁ¡Àº ¾ø´Ù.";
				coinMultipleText.text = "X 1.0";
				cristalMultipleText.text = "X 1.0";
				coinMultipleText.colorGradientPreset = oneTimesGradient;
				coinMultipleText.color = UnityEngine.Color.green;
				cristalMultipleText.colorGradientPreset = oneTimesGradient;
				cristalMultipleText.color = UnityEngine.Color.green;

				Material textMaterialEasy = coinMultipleText.fontMaterial;
				textMaterialEasy.SetColor("_UnderlayColor", UnityEngine.Color.green);
				break;
			case GameManager.ModeType.isNormalMode:
				degreeOfProgressTitleText.text = "º¸Åë ¸ðµå";
				degreeOfProgressSubText.text = "°ÔÀÓ ³» Áß°£ ³­ÀÌµµÀÌ¸ç, ÀÏ¹ÝÀûÀÎ ÇÃ·¹ÀÌ¾î¿¡°Ô ÀûÇÕÇÑ ¸ðµåÀÌ´Ù.\nÀü¸®Ç° ¹è¼ö°¡ ±âº»º¸´Ù Á¶±Ý ³ô¾Æ, Á¶±Ý ´õ ¸¹Àº º¸»óÀ» ¾òÀ» ¼ö ÀÖ´Ù.";
				coinMultipleText.text = "X 1.5";
				cristalMultipleText.text = "X 1.5";
				coinMultipleText.colorGradientPreset = onePointFiveTimesGradient;
				coinMultipleText.color = UnityEngine.Color.yellow;
				cristalMultipleText.colorGradientPreset = onePointFiveTimesGradient;
				cristalMultipleText.color = UnityEngine.Color.yellow;
				Material textMaterialNormal = coinMultipleText.fontMaterial;
				textMaterialNormal.SetColor("_UnderlayColor", UnityEngine.Color.yellow);
				break;
			case GameManager.ModeType.isHardMode:
				degreeOfProgressTitleText.text = "¾î·Á¿ò ¸ðµå";
				degreeOfProgressSubText.text = "°ÔÀÓ ³» °¡Àå ¾î·Á¿î ³­ÀÌµµÀÌ¸ç, µµÀü Á¤½ÅÀÌ °­ÇÑ ÇÃ·¹ÀÌ¾î¿¡°Ô ÃßÃµÇÏ´Â ¸ðµåÀÌ´Ù.\nÀü¸®Ç° ¹è¼ö°¡ ³ôÁö¸¸, ³ôÀº ³­ÀÌµµ·Î ÀÎÇØ °ÔÀÓ ÁøÇàÀÌ ¾î·Æ´Ù.";
				coinMultipleText.text = "X 2.0";
				cristalMultipleText.text = "X 2.0";
				coinMultipleText.colorGradientPreset = twoTimesGradient;
				coinMultipleText.color = UnityEngine.Color.red;
				cristalMultipleText.colorGradientPreset = twoTimesGradient;
				cristalMultipleText.color = UnityEngine.Color.red;
				Material textMaterialHard = coinMultipleText.fontMaterial;
				textMaterialHard.SetColor("_UnderlayColor", UnityEngine.Color.red);
				break;
		}

		degreeOfProgressSliderText.text = $"0 / {GameManager.Instance.StageFlagCount}";
	}

	public void ChangeGameMode(string _Direction)
	{
		int currentModeIndex = (int)GameManager.Instance.modeType;

		switch (_Direction)
		{
			case "Left":
				currentModeIndex--;

				if (currentModeIndex < 0)
				{
					currentModeIndex = System.Enum.GetValues(typeof(GameManager.ModeType)).Length - 1;
				}
				break;
			case "Right":
				currentModeIndex++;

				if (currentModeIndex >= System.Enum.GetValues(typeof(GameManager.ModeType)).Length)
				{
					currentModeIndex = 0;
				}
				break;
		}

		GameManager.Instance.modeType = (GameManager.ModeType)currentModeIndex;
	}

	public void OnClickInGameMenuButton()
	{
		quickMenuInGameObject.GetComponent<Animator>().SetBool("isNormal", !quickMenuInGameObject.GetComponent<Animator>().GetBool("isNormal"));
	}

	public void OnClickHelpMenuButton()
	{
		switch (currentHelpPageNum)
		{
			case 0:
				for (int i = 0; i < helpPanel_Group.Length; i++)
				{
					if (i == 0)
					{
						helpPanel_Group[i].gameObject.SetActive(true);
					}
					else
					{
                        helpPanel_Group[i].gameObject.SetActive(false);
                    }
				}
				break;
			case 1:
                for (int i = 0; i < helpPanel_Group.Length; i++)
                {
                    if (i == 1)
                    {
                        helpPanel_Group[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        helpPanel_Group[i].gameObject.SetActive(false);
                    }
                }
                break;
			case 2:
                for (int i = 0; i < helpPanel_Group.Length; i++)
                {
                    if (i == 2)
                    {
                        helpPanel_Group[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        helpPanel_Group[i].gameObject.SetActive(false);
                    }
                }
                break;
			case 3:
                for (int i = 0; i < helpPanel_Group.Length; i++)
                {
                    if (i == 3)
                    {
                        helpPanel_Group[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        helpPanel_Group[i].gameObject.SetActive(false);
                    }
                }
                break;
			default:
                for (int i = 0; i < helpPanel_Group.Length; i++)
                {
                    helpPanel_Group[i].gameObject.SetActive(false);
                }
                break;
		}
	}

	private void RemainEnemy()
	{
		if (enemySpawner == null)
			return;

		if (enemySpawner.maximumWave != 0)
		{
			remainEnemyCount.text = $"¿þÀÌºê¿¡ ³²Àº Àû: {GameManager.Instance.currentWaveLivingEnemy} / {enemySpawner.currentWaveEnemyCount} ¸¶¸®";
		}
	}

	public void OnDefeat()
	{
		foreach (Transform child in GameManager.Instance.poolingManager.transform)
		{
			Destroy(child.gameObject);
		}

		if (!isOnResultWindow)
		{
			Debug.Log(99);
			DefeatGame();
			isOnResultWindow = true;
		}
	}

	public void OnVictory()
	{
		if (!isOnResultWindow)
		{
			VictoryGame();
		}
	}

	private void DefeatGame()
	{
		StartCoroutine(StartDefeatResultWindow());
	}

	private void VictoryGame()
	{
		StartCoroutine(StartVictoryResultWindow());
	}

	private IEnumerator StartVictoryResultWindow()
	{
		victoryOrDefeatGameObject.SetActive(true);

		victoryOrDefeatTitle_Text.text = "·¹º§ Å¬¸®¾î";
		stageClearObject.SetActive(true);
		stageFail.gameObject.SetActive(false);

		switch (GameEvaluationManager.Instance.StarEvaluationCheck())
		{
			case 1:
				starImageList[0].sprite = normalStar;
				break;
			case 2:
				starImageList[0].sprite = normalStar;
				starImageList[1].sprite = normalStar;
				break;
			case 3:
				starImageList[0].sprite = normalStar;
				starImageList[1].sprite = superStar;
				starImageList[2].sprite = normalStar;
				break;
		}

		scoreText.text = GameManager.Instance.currentScore.ToString("F0");

		SaveScoreManager.Instance.SetStars(SceneManager.GetActiveScene().name, GameEvaluationManager.Instance.StarEvaluationCheck());
		SaveScoreManager.Instance.SetScore(SceneManager.GetActiveScene().name, GameManager.Instance.currentScore);

		if (PlayerStatus.Instance.IsGetCoin)
		{
			coinRewardObject.SetActive(true);
			coinRewardTitle_Text.text = (PlayerStatus.Instance.Coin / 4).ToString("F0");
			PlayerWalletManager.Instance.TotalCoin += PlayerStatus.Instance.Coin / 4;
		}

		if (PlayerStatus.Instance.IsGetCrystal)
		{
			Debug.Log("Å©¸®½ºÅ» ¾òÀ½");
			cristalRewardObject.SetActive(true);
			cristalRewardTitle_Text.text = PlayerStatus.Instance.Crystal.ToString("F0");
			PlayerWalletManager.Instance.TotalCrystal += PlayerStatus.Instance.Crystal;
		}

		victoryButtonGroup.gameObject.SetActive(true);
		DefeatButtonGroup.gameObject.SetActive(false);
		menuSettingGroup.gameObject.SetActive(false);
		towerInfo.gameObject.SetActive(false);

		yield return null;
	}

	private IEnumerator StartDefeatResultWindow()
	{
		victoryOrDefeatGameObject.SetActive(true);

		victoryOrDefeatTitle_Text.text = "¾Æ½±°Ô ÆÐ¹è";
		stageClearObject.SetActive(false);
		stageFail.gameObject.SetActive(true);
		victoryButtonGroup.gameObject.SetActive(false);
		DefeatButtonGroup.gameObject.SetActive(true);

		scoreText.text = GameManager.Instance.currentScore.ToString("F0");

		coinRewardObject.SetActive(false);
		coinRewardTitle_Text.text = "0";

		cristalRewardObject.SetActive(false);
		cristalRewardTitle_Text.text = "0";

		menuSettingGroup.gameObject.SetActive(false);
		towerInfo.gameObject.SetActive(false);

		remainEnemyCount.text = "¼º ÇÔ¶ô µÊ";

		yield return null;
	}

	public void onPlay()
	{
		SceneManager.LoadScene("LevelSelect");
	}

	public void OnClickOK()
	{
		SceneManager.LoadScene("LevelSelect");
	}

	public void OnClickRestart()
	{
		string currentSceneName = SceneManager.GetActiveScene().name;
		SceneManager.LoadScene(currentSceneName);
	}

	private void RefreshMenuWallet()
	{
		if (coinText == null || crystalText == null)
			return;

		coinText.text = PlayerWalletManager.Instance.GetTotalCoin().ToString("#,##0");
		crystalText.text = PlayerWalletManager.Instance.GetTotalCrystal().ToString("#,##0");
	}

	private void RefreshSkillUpgrade()
	{
		if (skill_TotalCoinText == null || skill_TotalCrystal == null)
			return;

		skill_TotalCoinText.text = PlayerWalletManager.Instance.GetTotalCoin().ToString("#,##0");
		skill_TotalCrystal.text = PlayerWalletManager.Instance.GetTotalCrystal().ToString("#,##0");
    }


    private void OnDisable()
	{
		isOnResultWindow = false;
	}
}
