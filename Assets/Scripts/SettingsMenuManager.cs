using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenuManager : MonoBehaviour
{
	public static bool isVibrate;

	public TMP_Dropdown graphicsDropdown;
	public Slider masterVol, musicVol, sfxVol;
	public AudioMixer mainAudioMixer;
	public Toggle vibrateToggle;

	private void Start()
	{
		Setup();

	}

	private void Setup()
	{
		isVibrate = true;

		ChangeMasterVolume();
		ChangeMusicVolume();
		ChangeSfxVolume();
		SetQualityBaseOnDevice();
	}

	public void ChangeMasterVolume()
	{
		mainAudioMixer.SetFloat("MasterVol", masterVol.value);
	}

	public void ChangeMusicVolume()
	{
		mainAudioMixer.SetFloat("MusicVol", musicVol.value);
	}

	public void ChangeSfxVolume()
	{
		mainAudioMixer.SetFloat("SfxVol", sfxVol.value);
	}

	public void ChangeVibrate()
	{
		isVibrate = vibrateToggle.isOn;
	}

	private void SetQualityBaseOnDevice()
	{
		int qualityIndex = DetermineQualityLevel();

		if (graphicsDropdown.options.Count > qualityIndex)
		{
			graphicsDropdown.value = qualityIndex;
			ChangeGraphicsQuality();
		}
	}

	private int DetermineQualityLevel()
	{
		if (SystemInfo.systemMemorySize < 2000)
		{
			return 0;
		}
		else if (SystemInfo.systemMemorySize < 4000)
		{
			return 1;
		}
		else if (SystemInfo.systemMemorySize < 6000)
		{
			return 2;
		}
		else
		{
			return 3;
		}
	}

	public void ChangeGraphicsQuality()
	{
		QualitySettings.SetQualityLevel(graphicsDropdown.value);
	}

	public void AcceptChangesOptions()
	{
		this.gameObject.SetActive(false);
	}
}
