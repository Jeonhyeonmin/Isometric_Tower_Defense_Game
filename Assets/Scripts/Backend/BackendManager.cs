using BackEnd;
using UnityEngine;

public class BackendManager : MonoBehaviour
{
    [SerializeField] private GameObject playButton_Group;
    [SerializeField] private GameObject signButton_Group;

    private void Awake()
    {
        BackendSetup();
    }

    private void BackendSetup()
    {
        var bro = Backend.Initialize();

        if (bro.IsSuccess())
        {
            Debug.Log($"<b><color=#82fffe>[BackendManager] :</b></color><color=white> ���� ���� �ʱ�ȭ�� �����߽��ϴ�. \n {bro}</color>");
        }
        else
        {
            Debug.LogError($"<b><color=#ff2011>[BackendManager] :</b></color><color=white> ���� ���� �ʱ�ȭ�� �����߽��ϴ�. \n {bro}</color>");
        }

        GameSetup();
    }

    private void GameSetup()
    {
        if (playButton_Group == null || signButton_Group == null)
        {
            Debug.LogError("<b><color=#ff2011>[BackendManager] :</b></color><color=white> playButton_Group �Ǵ� SignButton_Group�� null�Դϴ�.</color>");
            return;
        }

        playButton_Group.SetActive(false);
        signButton_Group.SetActive(true);

        Debug.Log("<b><color=#82fffe>[BackendManager] :</b></color><color=white> ���� ������ �Ϸ�Ǿ����ϴ�. \n playButton_Group�� signButton_Group�� Ȱ��ȭ�Ǿ����ϴ�.</color>");
    }
}
