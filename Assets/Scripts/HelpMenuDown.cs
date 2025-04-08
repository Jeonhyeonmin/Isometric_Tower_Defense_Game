using UnityEngine;

public class HelpMenuDown : MonoBehaviour
{
    [SerializeField] GameObject obj = null;
    UIManager manager;

    private void Awake()
    {
        manager = obj.GetComponent<UIManager>();
    }

    private void Update()
    {
        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            manager.CurrentHelpPageNum++;
            

            if (manager.CurrentHelpPageNum >= 4)
            {
                manager.OnClickHelpMenuButton();
                manager.CurrentHelpPageNum = 0;
            }
            else
            {
                manager.OnClickHelpMenuButton();
            }
        }
    }
}
