using System.Collections.Generic;
using UnityEngine;

public class SupportBuffTower : MonoBehaviour
{
	public List<SupportBuff> supportBuffs = new List<SupportBuff>();
	[SerializeField] private float buffCheckInterval = 1.0f; // 버프 체크 간격
	[SerializeField] private float buffLifeTime; // 버프 유지 시간

	private void Start()
	{
		InvokeRepeating("CheckSupportBuffs", 0f, buffCheckInterval);
	}

	private void CheckSupportBuffs()
	{
		float currentTime = Time.time;
		for (int i = supportBuffs.Count - 1; i >= 0; i--)
		{
			SupportBuff _Buff = supportBuffs[i];
			if (currentTime - _Buff.lastAppliedTime > buffLifeTime)
			{
				Debug.Log($"현재 시간 {currentTime} / 마지막 접촉 {_Buff.lastAppliedTime} : {currentTime - _Buff.lastAppliedTime > buffLifeTime}");
				RemoveSupportBuff(_Buff);
			}
		}
	}

	public void RemoveSupportBuff(SupportBuff _Buff)
	{
		Debug.Log("버프 제거 중...");

		_Buff.tower.Damage -= _Buff.tower.baseDamage * _Buff.buffAmout;
		_Buff.tower.Range -= _Buff.tower.baseRange * _Buff.buffAmout;
		_Buff.tower.Rate += _Buff.tower.baseRate * _Buff.buffAmout; // 공격 속도는 증가가 아니라 감소였으므로 빼줍니다.

		supportBuffs.Remove(_Buff);
	}

	public void AddTower(Tower _Tower, float _BuffAmout)
	{
		foreach (SupportBuff _Buff in supportBuffs)
		{
			if (_Buff.tower == _Tower)
			{
				_Buff.buffAmout = _BuffAmout;
				_Buff.lastAppliedTime = Time.time;
				return;
			}
		}

		SupportBuff newTower = new SupportBuff
		{
			tower = _Tower,
			buffAmout = _BuffAmout,
			lastAppliedTime = Time.time
		};

		supportBuffs.Add(newTower);
		ApplyBuff(newTower);
	}

	public void ReapplyAllBuffs()
	{
		foreach (SupportBuff _Buff in supportBuffs)
		{
			ApplyBuff(_Buff);
		}
	}

	private void ApplyBuff(SupportBuff _Buff)
	{
		_Buff.tower.Damage += _Buff.tower.baseDamage * _Buff.buffAmout;
		_Buff.tower.Range += _Buff.tower.baseRange * _Buff.buffAmout;
		_Buff.tower.Rate -= _Buff.tower.baseRate * _Buff.buffAmout; // 공격 속도는 증가가 아니라 감소였으므로 빼줍니다.
	}
}

[System.Serializable]
public class SupportBuff
{
	public Tower tower;
	public float buffAmout;
	public float lastAppliedTime;
}
