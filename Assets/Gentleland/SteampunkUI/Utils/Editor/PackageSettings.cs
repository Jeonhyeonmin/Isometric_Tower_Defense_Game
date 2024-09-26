using UnityEngine;

// if you want to delete this file delete all Gentleland "Utils" folder 
// you can then delete GentlelandSettings folder too
namespace Gentleland.Utils.SteampunkUI
{
    public class PackageSettings : ScriptableObject
    {
        public const string PackageSettingsName= "GentlelandSettings_SteampunkUI";
        public const string PackageSettingsPath = "Assets/GentlelandSettings/GentlelandSettings_SteampunkUI.asset";
        public const string PackageSettingsFolder = "GentlelandSettings";
        public const string PackageSettingsFolderPath = "Assets/GentlelandSettings";
        public const string PackageDocumentationPath = "/Gentleland/SteampunkUI/SteampunkUI Documentation.pdf";
        public const string PackageDocumentationName = "SteampunkUI Documentation";
        public const string imagePath = "Assets/Gentleland/SteampunkUI/Overviews/SteampunkUICard.png";
        public const string imageName = "SteampunkUICard";
        public const string packageName = "SteampunkUI";
        public bool isFirstTimeUsingTheAsset = true;
    }
}
