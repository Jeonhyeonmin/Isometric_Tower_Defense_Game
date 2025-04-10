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
        // Texture2D���� Sprite�� �����մϴ�.
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
		Debug.Log("������ ����â�� ���Ƚ��ϴ�.");

		if (profileEditor_Image.gameObject.activeSelf)
		{
			Debug.Log("������ �̹����� Ȱ��ȭ �Ǿ����ϴ�.");
            string base64 = PlayerWalletManager.Instance.profilebase64;
            byte[] imageBytes = Convert.FromBase64String(base64);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);
			Debug.Log(base64);

            profileEditor_Image.sprite = SpriteFromTexture2D(texture);
        }
		else
		{
            Debug.Log("������ �̹����� ��Ȱ��ȭ �Ǿ����ϴ�.");
        }
	}

	public void ValidateAndSetNickname(string name)
	{
		if (name.Length > 8)
		{
			feedbackText.text = "�г����� �ִ� 8���ڱ����� �����մϴ�.";
			onTriggerNicknameErrorNotify();
			return;
		}

		if (!Regex.IsMatch(name, @"^[��-�Ra-zA-Z0-9_]+$"))
		{
			feedbackText.text = "�г����� �ѱ�, ���ĺ�, ����, Ư������ '_'�� \n ����� �� �ֽ��ϴ�.";
			Debug.Log("�ƾ�");
			onTriggerNicknameErrorNotify();
			return;
		}

        Backend.BMember.UpdateNickname(nicknameInputField.text, callback =>
        {
            if (callback.IsSuccess())
			{
				Debug.Log("�г����� ���������� ����Ǿ����ϴ�.");

                nickname = name;
                nicknameText.text = nickname;
                nicknameInputField_Hint.text = nickname;
                nicknameInputField.text = string.Empty;

                PlayerWalletManager.Instance.nickname = nickname;
                Debug.Log($"�г����� ���� �� : {nickname}");
            }
			else
			{
                Debug.LogError($"�г��� ���� ����: {callback.GetCode()}");

				switch (int.Parse(callback.GetErrorCode()))
				{
					case 400:   // �̹� �ߺ��� �г����� �ִ� ���
						feedbackText.text = "�̹� ������� �г����Դϴ�.\n�ٸ� �г����� ������ּ���.";
						break;
					default:
						feedbackText.text = "�� �� ���� ������ �߻��߽��ϴ�.";
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
		Debug.Log("OpenURL : ����� ����");
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
			galleryManagementText.text = "URL ������ �����߽��ϴ�.";
			onTriggerGalleryManagementNotify();

			photoEditor.OpenEditor(url);
		}
		else
		{
			galleryManagementText.text = "�̹��� URL�� 'https://'��\n�����ؾ߸� �ν��� �� �ֽ��ϴ�.";
			onTriggerGalleryManagementNotify();
		}

		urlProfileText.text = string.Empty;
		urlProfileInputField.text = string.Empty;
	}

	public void OpenGallery()
	{
		NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
		{
			// ����ڰ� �̹����� �����ϰ� ���������� ��ȯ�� �̹��� ��ΰ� null�� �ƴ� ���
			if (path != null)
			{
				FileInfo seleted = new FileInfo(path);

				if (seleted.Length > 5000000)
				{
					galleryManagementText.text = "5MB�� �ʰ��ϴ� �̹�����\n����� �Ұ��� �մϴ�.";
					onTriggerGalleryManagementNotify();
					return;
				}

				string extension = Path.GetExtension(path).ToLower();

				if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
				{
					Texture2D selectedTexture = NativeGallery.LoadImageAtPath(path);

					if (selectedTexture == null)
					{
						Debug.LogError("�̹����� �ҷ����� ���߽��ϴ�.");
						galleryManagementText.text = "����ڰ� ������ �̹�����\n�� �� ���� ������ �ֽ��ϴ�.";
						onTriggerGalleryManagementNotify();
						return;
					}

					if (selectedTexture.width != selectedTexture.height)
					{
						Debug.LogError("�̹����� 1:1 ������ �ƴմϴ�.");
						galleryManagementText.text = "������ �̹����� 1:1 �����̾�� �մϴ�.\n�̹��� ���� ������ �����\n1:1 ������ ������ �Ͻñ� �ٶ��ϴ�.";
						onTriggerGalleryManagementNotify();
						return;
					}

					// ���õ� �̹��� ��θ� OpenEditor �޼��忡 �����Ͽ� �����⸦ ���ϴ�.
					photoEditor.OpenEditor(path);
				}
				else
				{
					Debug.LogError("�����ϴ� �ʴ� �̹��� ���� �Դϴ�.\nPNG, JPG, JPEG ����");
					galleryManagementText.text = "�����ϴ� �ʴ� �̹��� ���� �Դϴ�.\nPNG, JPG, JPEG�� ���� �����մϴ�.";
					onTriggerGalleryManagementNotify();
				}

			}
		},

		"�����ʷ� ����ϰ� ���� �̹����� �����ϼ���", "image/*");

		// ���� ���¿� ���� �߰� ó���� �� �� �ִ�.
		if (permission == NativeGallery.Permission.Denied) // ����ڰ� ��������� �ź��� ���,
		{
			Debug.Log("����ڰ� ������ ������ �ź��߽��ϴ�.");
			galleryManagementText.text = "���� ������ Ȯ�� ������ ���� �ۿ���\n������� �����ؾ� �մϴ�.";
			onTriggerGalleryManagementNotify();
		}
		else if (permission == NativeGallery.Permission.ShouldAsk)  // ����ڰ� ������ �ο��� ���� ����, ���� ��û�� �ʿ��� ���
		{
			Debug.Log("����ڿ��� ������ ���� ����� ��û�߽��ϴ�.");
			galleryManagementText.text = "���� ������ Ȯ�� ������ ���ٸ�\n�������� �����Ͻ� �� �����ϴ�.";
			onTriggerGalleryManagementNotify();
		}
		//else if (permission == NativeGallery.Permission.Granted)  // ������ �̹� �ο��� ���
		//{
		//    Debug.Log("������ �������� �����߽��ϴ�.");
		//    galleryManagementText.text = "���� ������ ������ ���� Ȯ�� �˴ϴ�.";
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
			stageErrorText.text = "�������� ����� �����ϰ�\n���� ���� ��ư�� �����ּ���.";
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
			stageErrorText.text = "������ ���������� ���� ��ġ ��������\n������ �ʽ��ϴ�.";
			onTriggerStageErrorNotify();
		}
	}

	private bool IsSceneInBuild(string sceneName)
	{
		// 1. ���� ���� ������ ���Ե� ������ ������ ������
		for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
		{
			// 2. �� ���� ��θ� ������ (��: "Assets/Scenes/MyScene.unity")
			string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
			// 3. �� ��ο��� �� �̸��� ���� (Ȯ���� ".unity"�� ������ �̸�)
			string sceneNameInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);
			// 4. �־��� �� �̸��� ���� ������ �ִ� �� �̸��� �� (��ҹ��� ���� ����)
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
				degreeOfProgressTitleText.text = "���� ���";
				degreeOfProgressSubText.text = "���� �� ���� ���� ���̵��̸�, �ʽ��ڿ��� ��õ�ϴ� ����̴�.\n���� �� �⺻ ����ǰ ����� ����Ǿ� Ư���� ������ ����.";
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
				degreeOfProgressTitleText.text = "���� ���";
				degreeOfProgressSubText.text = "���� �� �߰� ���̵��̸�, �Ϲ����� �÷��̾�� ������ ����̴�.\n����ǰ ����� �⺻���� ���� ����, ���� �� ���� ������ ���� �� �ִ�.";
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
				degreeOfProgressTitleText.text = "����� ���";
				degreeOfProgressSubText.text = "���� �� ���� ����� ���̵��̸�, ���� ������ ���� �÷��̾�� ��õ�ϴ� ����̴�.\n����ǰ ����� ������, ���� ���̵��� ���� ���� ������ ��ƴ�.";
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
			remainEnemyCount.text = $"���̺꿡 ���� ��: {GameManager.Instance.currentWaveLivingEnemy} / {enemySpawner.currentWaveEnemyCount} ����";
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

		victoryOrDefeatTitle_Text.text = "���� Ŭ����";
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
			Debug.Log("ũ����Ż ����");
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

		victoryOrDefeatTitle_Text.text = "�ƽ��� �й�";
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

		remainEnemyCount.text = "�� �Զ� ��";

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
