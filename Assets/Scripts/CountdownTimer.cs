using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountdownTimer : SingletonManager<CountdownTimer>
{
    public List<TMP_Text> timerTextList;
	public TimeSpan remainingTime;
	public DateTime endTime;

	private void OnEnable()
	{
		string endTimeString = PlayerPrefs.GetString("ShopEndTime", null);

		if (!string.IsNullOrEmpty(endTimeString))
		{
			endTime = DateTime.Parse(endTimeString);
		}
		else
		{
			endTime = DateTime.Now.AddHours(24);
			PlayerPrefs.SetString("ShopEndTime", endTime.ToString());
		}
	}

	private void Update()
	{
		remainingTime = endTime - DateTime.Now;

		if (timerTextList.Count <= 0)
		{
			return;
		}

		if (remainingTime.TotalSeconds > 0)
		{
			foreach (TMP_Text timerText in timerTextList)
			{
				timerText.text = string.Format("새로고침까지 {0:00}:{1:00}:{2:00}",
					remainingTime.Hours,
					remainingTime.Minutes,
					remainingTime.Seconds);
			}
		}
		else
		{
			foreach (TMP_Text timerText in timerTextList)
			{
				timerText.text = "00:00:00";
			}
		}
	}
}
