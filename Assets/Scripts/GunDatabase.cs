using UnityEngine;

[CreateAssetMenu(fileName = "GunDatabase", menuName = "Weapon/GunDatabase")]
public class GunDatabase : ScriptableObject
{
    public string databaseName = string.Empty;
    public string gunTypeName = string.Empty;
    public Gun[] guns;
}
