using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExchangeManager : SingletonManager<ExchangeManager>
{
	[SerializeField] private float minVariationTime;
	[SerializeField] private float maxVariationTime;

	public int maxTrendCount = 10;

	public float measurementTime;

	[SerializeField] private float exchangeRate = 1.999f;
	[SerializeField] private float feeRate = 0.199f;

	public float coinExchangeRate = 0.9f;
	public float originCoinExchangeRate = 0.9f;
	public float crystalExchangeRate = 0.5f;
	public float originCrystalExchangeRate = 0.5f;
	public float brickExchangeRate = 1.3f;
	public float originBrickExchangeRate = 1.3f;

	public float coinFeeRate = 0.5f;
	public float crystalFeeRate = 0.5f;
	public float brickFeeRate = 0.5f;

	public List<List<float>> trendsCurrency = new List<List<float>>();
	public List<float> picoCoinTrends = new List<float>();
	public List<float> glitterCrystalTrends = new List<float>();
	public List<float> brickCoinTrends = new List<float>();

	private void Start()
	{
		StopCoroutine(ExchangeRateFluctuation());
		StartCoroutine(ExchangeRateFluctuation());

		StopCoroutine(FeeRateFluctuation());
		StartCoroutine(FeeRateFluctuation());

		InvokeRepeating("PriceTrendRecord", 0, measurementTime);

		trendsCurrency.Add(picoCoinTrends);
		trendsCurrency.Add(glitterCrystalTrends);
		trendsCurrency.Add(brickCoinTrends);
	}

	private IEnumerator ExchangeRateFluctuation()
	{
		while (true)
		{
			float _tempRandomTimer = Random.Range(minVariationTime, maxVariationTime);
			yield return new WaitForSeconds(_tempRandomTimer);  // �־��� ���� �ð� ���� ����Ѵ�.

			#region ȭ�� ��ȯ ����

			coinExchangeRate = Random.Range(coinExchangeRate - exchangeRate, coinExchangeRate + exchangeRate);
			coinExchangeRate = Mathf.Clamp(coinExchangeRate, 0.001f, float.MaxValue);

			crystalExchangeRate = Random.Range(crystalExchangeRate - exchangeRate, crystalExchangeRate + exchangeRate);
			crystalExchangeRate = Mathf.Clamp(crystalExchangeRate, 0.001f, float.MaxValue);

			brickExchangeRate = Random.Range(brickExchangeRate - exchangeRate, brickExchangeRate + exchangeRate);
			brickExchangeRate = Mathf.Clamp(brickExchangeRate, 0.001f, float.MaxValue);

			#endregion
		}

	}   // �ǽð����� ȭ����� ��ȯ ������ ��ȭ��Ű�� �ڷ�ƾ�̴�.

	private IEnumerator FeeRateFluctuation()
	{
		while (true)
		{
			float _tempRandomTimer = Random.Range(minVariationTime, maxVariationTime);
			yield return new WaitForSeconds(_tempRandomTimer);  // �־��� ���� �ð� ���� ����Ѵ�.

			#region ȭ�� ������

			coinFeeRate = Random.Range(coinFeeRate - feeRate, coinFeeRate + feeRate * 2);
			coinFeeRate = Mathf.Clamp(coinFeeRate, 0, float.MaxValue);

			crystalFeeRate = Random.Range(crystalFeeRate - feeRate, crystalFeeRate + feeRate * 2);
			crystalFeeRate = Mathf.Clamp(crystalFeeRate, 0, float.MaxValue);

			brickFeeRate = Random.Range(brickFeeRate - feeRate, brickFeeRate + feeRate * 2);
			brickFeeRate = Mathf.Clamp(brickFeeRate, 0, float.MaxValue);

			#endregion
		}
	}   // �ǽð����� ȭ����� �����Ḧ ��ȭ��Ű�� �ڷ�ƾ�̴�.

	private void PriceTrendRecord()
	{
		int _tempInt = 0;

		foreach (List<float> trendCurrency in trendsCurrency)
		{
			switch (_tempInt)
			{
				case 0:
					AddToTrendList(trendCurrency, coinExchangeRate);
					break;
				case 1:
					AddToTrendList(trendCurrency, crystalExchangeRate);
					break;
				case 2:
					AddToTrendList(trendCurrency, brickExchangeRate);
					break;
			}

			_tempInt++;
		}
	}	// ���� ȭ����� ��ȯ ������ ����Ʈ�� ����ϱ� ���� ����ϴ� �ż���

	private void AddToTrendList(List<float> trendList, float newExchangeRate)
	{
		if (trendList.Count >= maxTrendCount)
		{
			trendList.RemoveAt(0);
		}

		trendList.Add(newExchangeRate);
	}	// ���������� ��ȯ ������ �ִ� ���� ������ ���ϰ�, �߰��ϴ� ���� �ż����̴�.
}
