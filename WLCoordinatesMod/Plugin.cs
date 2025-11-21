using System;
using System.IO;
using System.Reflection;
using BepInEx;
using UnityEngine;

namespace WobblyLifeCoordinateMod
{
    [BepInPlugin("cae.coords.mod", "Coordinate Display", "1.0.0")]
    public class CoordinateDisplay : BaseUnityPlugin
    {
        private GUIStyle style;
        private bool isToggled;

        private AssetBundle fontBundle;
        private Font customFont;

        void Start()
        {
            LoadFontBundle();

            style = new GUIStyle
            {
                fontSize = 22,
                font = customFont,
                normal = { textColor = Color.white }
            };
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F6))
                isToggled = !isToggled;
        }

        void OnGUI()
        {
            if (!isToggled) return;
            if (!GameInstance.InstanceExists ||
                !GameInstance.Instance.GetFirstLocalPlayerController() || 
                !GameInstance.Instance.GetFirstLocalPlayerController().GetPlayerCharacter() ||
                !GameInstance.Instance.GetFirstLocalPlayerController().GetPlayerCharacter().GetPlayerBody()) return;

            var player = GameInstance.Instance.GetFirstLocalPlayerController()
                .GetPlayerCharacter()
                .GetPlayerBody();

            if (player)
            {
                var pos = player.transform.position;
                GUI.Label(new Rect(20, 20, 400, 40),
                    $"X:{pos.x:F2}  Y:{pos.y:F2}  Z:{pos.z:F2}",
                    style);
            }
            else
            {
                GUI.Label(new Rect(20, 20, 400, 40),
                    "Player not found", style);
            }
        }

        private void LoadFontBundle()
        {
            const string resourceName = "WLCoordinatesMod.Resources.wlcoordinatemod.resources";

            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

            if (stream == null)
            {
                Logger.LogError("ERROR: Embedded AssetBundle not found: " + resourceName);
                return;
            }

            var data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);

            fontBundle = AssetBundle.LoadFromMemory(data);

            if (fontBundle == null)
            {
                Logger.LogError("ERROR: Failed to load AssetBundle from memory!");
                return;
            }

            customFont = fontBundle.LoadAsset<Font>("minecraft_font");

            if (customFont == null)
            {
                Logger.LogError("ERROR: Font could not be loaded from AssetBundle!");
            }
            else
            {
                Logger.LogInfo("Custom font loaded successfully: " + customFont.name);
            }
        }
    }
}
