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
            Debug.Log($"<b><color=#82fffe>[BackendManager] :</b></color><color=white> 게임 서버 초기화에 성공했습니다. \n {bro}</color>");
        }
        else
        {
            Debug.LogError($"<b><color=#ff2011>[BackendManager] :</b></color><color=white> 게임 서버 초기화에 실패했습니다. \n {bro}</color>");
        }

        GameSetup();
    }

    private void GameSetup()
    {
        if (playButton_Group == null || signButton_Group == null)
        {
            Debug.LogError("<b><color=#ff2011>[BackendManager] :</b></color><color=white> playButton_Group 또는 SignButton_Group이 null입니다.</color>");
            return;
        }

        playButton_Group.SetActive(false);
        signButton_Group.SetActive(true);

        Debug.Log("<b><color=#82fffe>[BackendManager] :</b></color><color=white> 게임 설정이 완료되었습니다. \n playButton_Group과 signButton_Group이 활성화되었습니다.</color>");
    }
}
