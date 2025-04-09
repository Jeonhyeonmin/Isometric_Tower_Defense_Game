using System;
using UnityEngine;
using BackEnd;

// SendQueue를 손쉽게 사용하기 위한 SendQueueMgr 클래스 입니다.
// 해당 스크립트를 유니티 객체에 스크롤 해서 적용해주세요.
// SendQueue에 대한 자세한 설명은 아래 개발자 문서를 참고해주세요.
// https://developer.thebackend.io/unity3d/guide/Async/SendQueue/

public class SendQueueMgr : MonoBehaviour
{
    void Awake()
    {
        // 씬이 바뀌어도 계속 SendQueue를 사용하기 위해서
        // 아래 DontDestroyOnLoad 함수를 통해 SendQueue 객체를 파괴되지 않는 오브젝트로 만듭니다.
        // 자세한 설명은 아래 유니티 개발자문서를 참고해주세요.
        // https://docs.unity3d.com/kr/current/ScriptReference/Object.DontDestroyOnLoad.html
        DontDestroyOnLoad(this);

        // 만약 SendQueue가 초기화 되지 않았다면 초기화 수행
        if (SendQueue.IsInitialize == false)
        {
            // SendQueue는 시작과 동시에 초기화가 수행됩니다.
            // 디버그 로그 활성화, 예외 이벤트 핸들러 등록
            SendQueue.StartSendQueue(true, ExceptionEvent);
        }
    }

    void Update()
    {
        // SendQueue가 초기화 되었을 때만 Poll 함수를 호출
        if (SendQueue.IsInitialize)
        {
            // SendQueue를 정상적으로 사용하기 위해서는 
            // 아래 Poll 함수가 반드시 정기적으로 호출되어야 합니다.
            BackEnd.SendQueue.Poll();
        }
    }

    // SendQueue 내부에서 예외가 발생했을 경우  
    // 아래 이벤트 핸들러를 통해 예외 이벤트가 전달됩니다.
    void ExceptionEvent(Exception e)
    {
        Debug.Log(e.ToString());
    }


    void OnApplicationPause(bool isPause)
    {
        if (isPause == false)
        {
            // 어플리케이션이 재실행 되었을 때 SendQueue를 재실행 시킴
            SendQueue.ResumeSendQueue();
        }
        else
        {
            // 어플리케이션이 정지되었을 때 SendQueue를 일시 정지 시킴 
            SendQueue.PauseSendQueue();
        }
    }

    void OnApplicationQuit()
    {
        // 어플리케이션이 종료되었을 때 SendQueue를 정지 시킴
        BackEnd.SendQueue.StopSendQueue();
    }
}
