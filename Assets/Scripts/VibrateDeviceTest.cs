using UnityEngine;

public class VibrateDeviceTest : MonoBehaviour
{
	public void OnClickVibrate()
	{
		if (SettingsMenuManager.isVibrate)
		{
			Handheld.Vibrate();
			Debug.Log("Vibrate");
		}
	}
}
