using UnityEngine;

public enum TowerType
{
	Archer,
	Wizard,
	Barrack,
}

public abstract class Tower : MonoBehaviour
{
	public TowerType towerType;

	public TowerTemplate towerTemplate;

	private int currentLevel = 0;
	private int maxLevel;

	[SerializeField] private float towerResponseTime;

	[SerializeField] private PlaceTile placeTile;

	public PlaceTile GetPlaceTile
	{
		set => placeTile = value;
		get => placeTile;
	}

	public int CurrentLevel
	{
		set => currentLevel = value;
		get => currentLevel < maxLevel ? currentLevel : maxLevel;
	}

	public int MaxLevel => maxLevel;

	// 원본 속성을 저장
	[HideInInspector] public float baseDamage;
	[HideInInspector] public float baseRange;
	[HideInInspector] public float baseRate;
	[HideInInspector] public float baseSupport;
	[HideInInspector] public int baseaVailableSoldiers;

	// 런타임 속성을 저장
	public float runtimeDamage;
	public float runtimeRange;
	public float runtimeRate;
	public float runtimeSupport;
	public int runtimeVailableSoldiers;

	private void Start()
	{
		SetupBaseStats();
		InitializeRuntimeStats();
	}

	public void SetupBaseStats()
	{
		baseDamage = towerTemplate.WeaponPropertie[CurrentLevel].damage;
		baseRange = towerTemplate.WeaponPropertie[CurrentLevel].range;
		baseRate = towerTemplate.WeaponPropertie[CurrentLevel].rate;
		baseSupport = towerTemplate.WeaponPropertie[CurrentLevel].support;
		baseaVailableSoldiers = towerTemplate.WeaponPropertie[CurrentLevel].availableSoldiers;
	}

	public void InitializeRuntimeStats()
	{
		runtimeDamage = baseDamage;
		runtimeRange = baseRange;
		runtimeRate = baseRate;
		runtimeSupport = baseSupport;
		runtimeVailableSoldiers = baseaVailableSoldiers;
	}

	public float Damage
	{
		get => runtimeDamage;
		set => runtimeDamage = value;
	}

	public float Range
	{
		get => runtimeRange;
		set => runtimeRange = value;
	}

	public float Rate
	{
		get => runtimeRate;
		set => runtimeRate = Mathf.Clamp(value, 0.15f, float.MaxValue);
	}

	public float Support
	{
		get => runtimeSupport;
		set => runtimeSupport = value;
	}

	public int AvailableSoldiers
	{
		get => runtimeVailableSoldiers;
		set => runtimeVailableSoldiers = value;
	}

	public TowerTemplate TowerTemplate
	{
		set => towerTemplate = value;
		get => towerTemplate;
	}

	public float TowerResponseTime
	{
		set => towerResponseTime = value;
		get => towerResponseTime;
	}

	public virtual void Setup()
	{
		maxLevel = towerTemplate.WeaponPropertie.Length;
	}

	public abstract void SearchTarget();

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, Range);
	}

	public void UpgradeTower()
	{
		// SupportBuffTower의 인스턴스를 찾아서 버프를 재적용합니다.
		SupportBuffTower supportBuffTower = FindAnyObjectByType<SupportBuffTower>();

		if (supportBuffTower != null)
		{
			// 현재 적용된 버프 제거
			supportBuffTower.RemoveSupportBuff(new SupportBuff { tower = this });
			// 새로운 기본 스텟에 버프 재적용
			supportBuffTower.ReapplyAllBuffs();
		}
	}
}
