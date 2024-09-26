using UnityEditor;
using UnityEngine;

// if you want to delete this file delete all Gentleland "Utils" folder 
// you can then delete GentlelandSettings folder too
namespace Gentleland.Utils.SteampunkUI
{
    public class WelcomeWindow : EditorWindow
    {
        const string WindowTitle = "Gentleland : " + PackageSettings.packageName;
        GUIStyle textStyle;
        GUIStyle linkStyle;
        Texture image;
        bool initialized = false;

        public static void OpenWindow()
        {
            PackageSettings settings = AssetDatabase.LoadAssetAtPath<PackageSettings>(PackageSettings.PackageSettingsPath);
            
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<PackageSettings>();
                AssetDatabase.CreateAsset(settings, PackageSettings.PackageSettingsPath);
            }
            if (!settings.isFirstTimeUsingTheAsset)
            {
                return;
            }
            WelcomeWindow wnd = GetWindow<WelcomeWindow>(true);
            wnd.titleContent = new GUIContent(WindowTitle);
            wnd.minSize = new Vector2(650, 650);
            wnd.maxSize = wnd.minSize;
            settings.isFirstTimeUsingTheAsset = false;
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }

        public void OnGUI()
        {
            if (!initialized)
            {
                string[] s = AssetDatabase.FindAssets(PackageSettings.imageName + " t:Texture"); 
                if (s.Length > 0)
                {
                    image = AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(s[0]));
                }
                textStyle = new GUIStyle(EditorStyles.label);
                textStyle.wordWrap = true;
                textStyle.margin = new RectOffset(20, 20, 20, 20);
                textStyle.alignment = TextAnchor.UpperLeft;
                linkStyle = new GUIStyle(textStyle);
                linkStyle.hover.textColor = linkStyle.normal.textColor * 0.5f;
                initialized = true;
            }
            float imageHeight = 280;
            float imageWidth = 420;
            float margin = 20;
            if (image != null)
            {
                GUI.DrawTexture(new Rect(position.width / 2 - imageWidth/2, margin, imageWidth, imageHeight), image, ScaleMode.ScaleToFit);
            }
            GUILayout.BeginArea(new Rect(20, imageHeight + 2 * margin, position.width - margin * 2, position.height - imageHeight + 2 * margin));
            GUILayout.Label(
@"Hello dear developer! 

Thank you for acquiring this asset pack from the Unity Asset Store!

We are a growing art outsourcing agency and are very thankful for your support!
If you like this asset pack, please consider leaving a review on the Unity Asset Store or recommend us to your friends! This would help us greatly!


If you encounter problems of any kind, checkout the documentation and feel free to reach out!
We will help you further.

Jacky Martin
CEO - Gentleland"
            , textStyle);
            if (GUILayout.Button("jacky@gentleland.net", linkStyle))
            {
                Application.OpenURL("mailto:jacky@gentleland.net");
            }
            if (GUILayout.Button("Documentation.pdf"))
            {
                string[]s = AssetDatabase.FindAssets(PackageSettings.PackageDocumentationName);
                if (s.Length > 0)
                {
                    Application.OpenURL(Application.dataPath +"/../"+ AssetDatabase.GUIDToAssetPath(s[0]));
                }
                else
                {
                    Debug.Log("Gentleland - " + PackageSettings.packageName + " : Couldn't Find Documentation file it is either deleted or renamed");
                }
            }
            GUILayout.EndArea();
        }

    }
}
