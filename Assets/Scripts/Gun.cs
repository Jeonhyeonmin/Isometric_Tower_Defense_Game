using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum GunType
{
    Pistol,
    Rifle,
    SniperRifle,
}

[Serializable]
public class GunClass
{
    public float damage;
    public float fireRate;
    public int magazineSize;
    public float aimAccuracy;
    public int effectiveRange;
    public float moveSpeed;
}

[CreateAssetMenu(fileName = "NewGun", menuName = "Weapon/Gun")]
public class Gun : ScriptableObject
{
    [Header("�ѱ�")]
    [Space(10)]
    public string gunName;
    public string gunDescriptionTitle;
    public string gunDescriptionSubTitle;
	public string simpleDescription;
	public int price;
    public GunType gunType;
    public Sprite gunIcon;
    public bool isUnlocking;
    public int gunCurrentLevel;

    [Header("���� ����")]
    [Space(10)]
    public bool haveScope;
    public bool haveBarrel;
    public bool haveMagazine;
    public bool haveGrip;
    public bool haveStock;

    [Header("�������� ���� ���� ����")]
    [Space(10)]

    public bool isUnlockScope;
    public bool isUnlockBarrel;
    public bool isUnlockMagazine;
    public bool isUnlockGrip;
    public bool isUnlockStock;

    [Space(30)]

    [Header("�ѱ� �Ӽ�")]
    [Space(10)]
    public List<GunClass> gunList = new List<GunClass>();
}
