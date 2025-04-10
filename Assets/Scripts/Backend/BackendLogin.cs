using System.Collections;
using System.Text.RegularExpressions;
using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// BackendLogin is a singleton class that manages the login process for the game.
/// </summary>
public class BackendLogin : MonoBehaviour
{
    [SerializeField] private TMP_InputField id_InputField;
    [SerializeField] private TMP_InputField pw_InputField;
    [SerializeField] private Button loginAndSign_Button;
    [SerializeField] private Button ok_Button;

    [SerializeField] private TMP_Text messageText;
    [SerializeField] private UIManager uIManager;

    bool isLoginMode = false; // 로그인 모드인지 여부

    private bool OnValidate
    {
        get
        {
            string id = id_InputField.text.Trim();
            string pw = pw_InputField.text.Trim();

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pw))
            {
                messageText.text = "아이디와 비밀번호 모두 입력해주세요.";
                return false;
            }

            if (!Regex.IsMatch(id, @"^[a-zA-Z0-9]{6,}$"))
            {
                messageText.text = "아이디는 영문자와 숫자로만 이루어져야 하며, 최소 6자 이상이어야 합니다.";
                return false;
            }

            if (pw.Length < 8 || !Regex.IsMatch(pw, @"[!@#$%^&*(),.?:{ }|<>]"))
            {
                messageText.text = "비밀번호는 최소 8자 이상이어야 하며, 특수문자를 포함해야 합니다.";
                return false;
            }

            Debug.Log("아이디와 비밀번호 유효성 검사 통과");
            return true;
        }
    }

    public void OnClickButtonChangeMode()
    {
        isLoginMode = !isLoginMode;

        if (isLoginMode)
        {
            ok_Button.onClick.RemoveListener(OnClickCustomSignup);
            ok_Button.onClick.RemoveListener(OnClickCustomLogin);

            ok_Button.onClick.AddListener(OnClickCustomLogin);

            loginAndSign_Button.GetComponentInChildren<TextMeshProUGUI>().text = "회원가입";
        }
        else
        {
            ok_Button.onClick.RemoveListener(OnClickCustomSignup);
            ok_Button.onClick.RemoveListener(OnClickCustomLogin);

            ok_Button.onClick.AddListener(OnClickCustomSignup);

            loginAndSign_Button.GetComponentInChildren<TextMeshProUGUI>().text = "로그인";
        }
    }

    private void Awake()
    {
        ok_Button.interactable = true;
        OnClickButtonChangeMode();
    }

    /// <summary>
    /// CustomSignup is a method that handles the signup process for the game.
    /// </summary>
    private void OnClickCustomSignup()
    {
        if (!OnValidate)
        {
            return;
        }

        // Perform signup logic here
        // For example, you can call a backend service to create a new user account
        // If signup is successful, you can show a success message
        Backend.BMember.CustomSignUp(id_InputField.text, pw_InputField.text, callback =>
        {
            if (callback.IsSuccess())
            {
                messageText.text = "회원가입 성공!";
                Debug.Log("회원가입 성공");
                ok_Button.interactable = false;
                UpdateNickname(id_InputField.text);
                SceneManager.LoadScene("LevelSelect");
                PlayerWalletManager.Instance.LoadLevelChart();
            }
            else
            {
                StartCoroutine(StartCoroutineOK());

                switch (int.Parse(callback.GetStatusCode()))
                {
                    case 401:   // 프로젝트 상태가 '점검'일 경우
                        messageText.text = "401\n점검 중입니다.\n잠시 후 다시 시도해주세요.";
                        break;
                    case 403:   // 차단당한 디바이스일 경우
                        messageText.text = "403\n차단된 디바이스입니다.\n관리자에게 문의해주세요.";
                        break;
                    case 409:   // 이미 가입된 아이디일 경우
                        messageText.text = "409\n이미 가입된 아이디입니다.\n다른 아이디를 사용해주세요.";
                        break;
                }
            }
        });

    }

    /// <summary>
    /// CustomLogin is a method that handles the login process for the game.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pw"></param>
    private void OnClickCustomLogin()
    {
        if (string.IsNullOrEmpty(id_InputField.text) || string.IsNullOrEmpty(pw_InputField.text))
        {
            messageText.text = "아이디와 비밀번호 모두 입력해주세요.";
            return;
        }

        // Perform login logic here
        // For example, you can call a backend service to authenticate the user
        // If login is successful, you can show a success message
        Backend.BMember.CustomLogin(id_InputField.text, pw_InputField.text, callback =>
        {
            if (callback.IsSuccess())
            {
                messageText.text = "로그인 성공!";
                Debug.Log("로그인 성공");
                ok_Button.interactable = false;
                SceneManager.LoadScene("LevelSelect");
                PlayerWalletManager.Instance.LoadLevelChart();
            }
            else
            {
                StartCoroutine(StartCoroutineOK());
                switch (int.Parse(callback.GetStatusCode()))
                {
                    case 401:   // 존재하지 않는 아이디의 경우
                        messageText.text = "401\n존재하지 않는 아이디이거나 비밀번호입니다.\n다시 입력해주세요.";
                        break;
                    case 403:   // 차단당한 유저인 경우
                        messageText.text = "403\n차단된 유저입니다.\n관리자에게 문의해주세요.";
                        break;
                    case 410:   // 탈퇴가 진행중일 경우(WithdrawAccount 함수 호출 이후)
                        messageText.text = "410\n탈퇴가 진행중입니다.\n관리자에게 문의해주세요.";
                        break;
                }
            }
        });
    }

    /// <summary>
    /// UpdateNickname is a method that updates the user's nickname in the backend.
    /// </summary>
    /// <param name="nickname"></param>
    public void UpdateNickname(string nickname)
    {
        Backend.BMember.UpdateNickname(nickname);
    }

    IEnumerator StartCoroutineOK()
    {
        ok_Button.interactable = false;
        yield return new WaitForSeconds(1f);
        ok_Button.interactable = true;
    }
}
