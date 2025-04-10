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

    bool isLoginMode = false; // �α��� ������� ����

    private bool OnValidate
    {
        get
        {
            string id = id_InputField.text.Trim();
            string pw = pw_InputField.text.Trim();

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pw))
            {
                messageText.text = "���̵�� ��й�ȣ ��� �Է����ּ���.";
                return false;
            }

            if (!Regex.IsMatch(id, @"^[a-zA-Z0-9]{6,}$"))
            {
                messageText.text = "���̵�� �����ڿ� ���ڷθ� �̷������ �ϸ�, �ּ� 6�� �̻��̾�� �մϴ�.";
                return false;
            }

            if (pw.Length < 8 || !Regex.IsMatch(pw, @"[!@#$%^&*(),.?:{ }|<>]"))
            {
                messageText.text = "��й�ȣ�� �ּ� 8�� �̻��̾�� �ϸ�, Ư�����ڸ� �����ؾ� �մϴ�.";
                return false;
            }

            Debug.Log("���̵�� ��й�ȣ ��ȿ�� �˻� ���");
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

            loginAndSign_Button.GetComponentInChildren<TextMeshProUGUI>().text = "ȸ������";
        }
        else
        {
            ok_Button.onClick.RemoveListener(OnClickCustomSignup);
            ok_Button.onClick.RemoveListener(OnClickCustomLogin);

            ok_Button.onClick.AddListener(OnClickCustomSignup);

            loginAndSign_Button.GetComponentInChildren<TextMeshProUGUI>().text = "�α���";
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
                messageText.text = "ȸ������ ����!";
                Debug.Log("ȸ������ ����");
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
                    case 401:   // ������Ʈ ���°� '����'�� ���
                        messageText.text = "401\n���� ���Դϴ�.\n��� �� �ٽ� �õ����ּ���.";
                        break;
                    case 403:   // ���ܴ��� ����̽��� ���
                        messageText.text = "403\n���ܵ� ����̽��Դϴ�.\n�����ڿ��� �������ּ���.";
                        break;
                    case 409:   // �̹� ���Ե� ���̵��� ���
                        messageText.text = "409\n�̹� ���Ե� ���̵��Դϴ�.\n�ٸ� ���̵� ������ּ���.";
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
            messageText.text = "���̵�� ��й�ȣ ��� �Է����ּ���.";
            return;
        }

        // Perform login logic here
        // For example, you can call a backend service to authenticate the user
        // If login is successful, you can show a success message
        Backend.BMember.CustomLogin(id_InputField.text, pw_InputField.text, callback =>
        {
            if (callback.IsSuccess())
            {
                messageText.text = "�α��� ����!";
                Debug.Log("�α��� ����");
                ok_Button.interactable = false;
                SceneManager.LoadScene("LevelSelect");
                PlayerWalletManager.Instance.LoadLevelChart();
            }
            else
            {
                StartCoroutine(StartCoroutineOK());
                switch (int.Parse(callback.GetStatusCode()))
                {
                    case 401:   // �������� �ʴ� ���̵��� ���
                        messageText.text = "401\n�������� �ʴ� ���̵��̰ų� ��й�ȣ�Դϴ�.\n�ٽ� �Է����ּ���.";
                        break;
                    case 403:   // ���ܴ��� ������ ���
                        messageText.text = "403\n���ܵ� �����Դϴ�.\n�����ڿ��� �������ּ���.";
                        break;
                    case 410:   // Ż�� �������� ���(WithdrawAccount �Լ� ȣ�� ����)
                        messageText.text = "410\nŻ�� �������Դϴ�.\n�����ڿ��� �������ּ���.";
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
