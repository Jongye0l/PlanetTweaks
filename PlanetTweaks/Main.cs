using System;
using System.IO;
using System.Xml.Linq;
using PlanetTweaks.Utils;
using JALib.Core;
using JALib.Tools;
using PlanetTweaks.Components;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PlanetTweaks;

public class Main : JAMod {
    public static Main instance;
    public static Settings settings;
    public static AssetBundle bundle;
    private static SettingGUI settingGUI;
    private static GUIStyle labelStyle;
    private static GUIStyle labelStyle2;
    private static string[] _cacheSettingStrings;
    private static int cachedVertices;
    private static Texture2D cachedTexture;

    public Main() : base(typeof(Settings)) {
        _cacheSettingStrings = new string[7];
        cachedVertices = -1;
    }

    protected override void OnSetup() {
        settings = (Settings) Setting;
        settingGUI = new SettingGUI(this);
        MainThread.Run(this, ImageChangePage.Init);
        Patcher.AddPatch(typeof(Patch));
        SettingMigration();
    }

    protected override void OnEnable() {
        bundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(Path, "planettweaks"));
        Sprites.Load();
        Sprites.Init();
        if(!scrController.instance) return;
        foreach(scrPlanet planet in scrController.instance.planetarySystem.allPlanets) planet.GetOrAddRenderer();
        if(scnLevelSelect.instance && !PlanetTweaksFloorController.instance) Patch.Start();
    }

    protected override void OnDisable() {
        PlanetTweaksPlanetController controller = PlanetTweaksPlanetController.instance;
        if(controller) {
            foreach(SpriteRenderer renderer in controller.spriteDictionary.Values) Object.Destroy(renderer.gameObject);
            Object.Destroy(controller.gameObject);
        }
        PlanetTweaksFloorController.Dispose();
        Sprites.Dispose();
        if(bundle) {
            bundle.Unload(true);
            bundle = null;
        }
    }

    protected override void OnGUI() {
        labelStyle ??= new GUIStyle(GUI.skin.label);
        labelStyle2 ??= new GUIStyle(GUI.skin.label) {
            fontStyle = FontStyle.Bold
        };
        JALocalization localization = Localization;
        Settings setting = settings;
        SettingGUI settingGUI = Main.settingGUI;
        PlanetarySystem planetarySystem = scrController.instance.planetarySystem;
        settingGUI.AddSettingSliderFloat(ref setting.redSize, 1, ref _cacheSettingStrings[0], localization["Setting.RedPlanetSize"], 0, 2, () => {
            scrController.instance.planetarySystem.planetRed.GetOrAddRenderer().transform.localScale = new Vector2(settings.redSize, settings.redSize);
        });
        settingGUI.AddSettingSliderFloat(ref setting.blueSize, 1, ref _cacheSettingStrings[1], localization["Setting.BluePlanetSize"], 0, 2, () => {
            scrController.instance.planetarySystem.planetBlue.GetOrAddRenderer().transform.localScale = new Vector2(settings.blueSize, settings.blueSize);
        });
        if(GUILayout.Toggle(false, $"<color={ColorUtils.GetRealColor(true).Hex()}>{localization["Setting.RedColor"]}</color> " + localization["Setting.ColorBehind"] +
                                   $"  <color=grey>{(setting.redColor ? "O" : "Ⅹ")}</color>", labelStyle)) {
            setting.redColor = !setting.redColor;
            planetarySystem.planetRed.GetOrAddRenderer().color = setting.redColor ? ColorUtils.GetRealColor(true) : Color.white;
            SaveSetting();
        }
        if(GUILayout.Toggle(false, $"<color={ColorUtils.GetRealColor(false).Hex()}>{localization["Setting.BlueColor"]}</color> " + localization["Setting.ColorBehind"] +
                                   $"  <color=grey>{(setting.blueColor ? "O" : "Ⅹ")}</color>", labelStyle)) {
            setting.blueColor = !setting.blueColor;
            planetarySystem.planetBlue.GetOrAddRenderer().color = setting.blueColor ? ColorUtils.GetRealColor(false) : Color.white;
            SaveSetting();
        }
        if(NeoCosmosManager.instance.installed && NeoCosmosManager.instance.own) {
            GUILayout.Space(60);
            GUILayout.Label($"<size=20>{localization["Setting.DLC"]}</size>", labelStyle2);
            GUILayout.Space(10);
            if(GUILayout.Toggle(false, localization["Setting.ThirdPlanet"] + " <color=grey>" + (setting.thirdPlanet ? "O" : "Ⅹ") + "</color>", labelStyle)) {
                setting.thirdPlanet = !setting.thirdPlanet;
                SaveSetting();
            }
            settingGUI.AddSettingSliderFloat(ref setting.thirdSize, 1, ref _cacheSettingStrings[2], localization["Setting.ThirdPlanetSize"], 0, 2, () => {
                scrController.instance.planetarySystem.planetGreen.GetOrAddRenderer().transform.localScale = new Vector2(settings.thirdSize, settings.thirdSize);
            });
            if(GUILayout.Toggle(false, $"<color={ColorUtils.GetRealColor(false).Hex()}>{localization["Setting.ThirdColor"]}</color> " + localization["Setting.ColorBehind"] +
                                       $"  <color=grey>{(setting.thirdColor ? "O" : "Ⅹ")}</color>", labelStyle)) {
                setting.thirdColor = !setting.thirdColor;
                planetarySystem.planetGreen.GetOrAddRenderer().color = setting.blueColor ? ColorUtils.GetRealThirdColor() : Color.white;
                SaveSetting();
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label($"<color={ColorUtils.GetRealThirdColor().Hex()}>{localization["Setting.ThirdColor2"]}</color>");
            GUILayout.Space(10);
            int newColorType = GUILayout.Toolbar(setting.thirdColorType, [
                $"<color={ColorUtils.GetRealDefaultThirdColor().Hex()}>{localization["Setting.ThirdColorType1"]}</color>",
                $"<color={ColorUtils.GetRealColor(true).Hex()}>{localization["Setting.ThirdColorType2"]}</color>",
                $"<color={ColorUtils.GetRealColor(false).Hex()}>{localization["Setting.ThirdColorType3"]}</color>",
                $"<color={setting.thirdColorCustom.Hex()}>{localization["Setting.ThirdColorType4"]}</color>"]);
            if(newColorType != setting.thirdColorType) {
                setting.thirdColorType = newColorType;
                ColorUtils.SetThirdColor();
                SaveSetting();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Label(localization["Setting.CustomColor"]);
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            settingGUI.AddSettingSliderFloat(ref setting.thirdColorCustom.r, 0.3f, ref _cacheSettingStrings[3], "R", 0, 1, ColorUtils.SetThirdColor);
            settingGUI.AddSettingSliderFloat(ref setting.thirdColorCustom.g, 0.3f, ref _cacheSettingStrings[4], "G", 0, 1, ColorUtils.SetThirdColor);
            settingGUI.AddSettingSliderFloat(ref setting.thirdColorCustom.b, 0.3f, ref _cacheSettingStrings[5], "B", 0, 1, ColorUtils.SetThirdColor);
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        GUILayout.Space(60);
        GUILayout.Label($"<size=20>{localization["Setting.Extra"]}</size>", labelStyle2);
        GUILayout.Space(10);
        settingGUI.AddSettingToggle(ref settings.shapedRotation, localization["Setting.ShapedRotation"]);
        if(!settings.shapedRotation) return;
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginVertical();
        settingGUI.AddSettingInt(ref setting.shapedAngle, 4, ref _cacheSettingStrings[6], localization["Setting.ShapedAngle"], 2);
        if(cachedVertices != settings.shapedAngle || !cachedTexture) {
            cachedTexture = new Texture2D(100, 100);
            Vector2Int middle = new(50, 50);
            for(int i = 0; i < settings.shapedAngle; i++) {
                float angle = Mathf.PI * 2 / settings.shapedAngle * i;
                Vector2Int point = middle + new Vector2Int((int) (Mathf.Sin(angle) * 30), (int) (Mathf.Cos(angle) * 30));
                for(int j = -10; j <= 10; j++)
                    for(int k = -10; k <= 10; k++)
                        if(j * j + k * k <= 10 * 10)
                            cachedTexture.SetPixel(point.x + j, point.y + k, Color.black);
            }
            cachedTexture.Apply();
            cachedVertices = settings.shapedAngle;
        }
        GUILayout.Label(cachedTexture);
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    private static void SettingMigration() {
        string path = System.IO.Path.Combine(instance.Path, "Settings.xml");
        if(!File.Exists(path)) return;
        try {
            XDocument xDoc = XDocument.Load(path);
            Settings settings = Main.settings;
            XElement root = xDoc.Root;
            XElement sub = root.Element("redSize");
            if(sub != null) settings.redSize = float.Parse(sub.Value);
            sub = root.Element("blueSize");
            if(sub != null) settings.blueSize = float.Parse(sub.Value);
            sub = root.Element("redColor");
            if(sub != null) settings.redColor = bool.Parse(sub.Value);
            sub = root.Element("blueColor");
            if(sub != null) settings.blueColor = bool.Parse(sub.Value);
            sub = root.Element("shapedRotation");
            if(sub != null) settings.shapedRotation = bool.Parse(sub.Value);
            sub = root.Element("shapedAngle");
            if(sub != null) settings.shapedAngle = int.Parse(sub.Value);
            sub = root.Element("thirdSize");
            if(sub != null) settings.thirdSize = float.Parse(sub.Value);
            sub = root.Element("thirdColor");
            if(sub != null) settings.thirdColor = bool.Parse(sub.Value);
            sub = root.Element("thirdPlanet");
            if(sub != null) settings.thirdPlanet = bool.Parse(sub.Value);
            sub = root.Element("thirdColorType");
            if(sub != null) settings.thirdColorType = int.Parse(sub.Value);
            sub = root.Element("thirdColorRed");
            if(sub != null) settings.thirdColorCustom.r = float.Parse(sub.Value) / 255;
            sub = root.Element("thirdColorGreen");
            if(sub != null) settings.thirdColorCustom.g = float.Parse(sub.Value) / 255;
            sub = root.Element("thirdColorBlue");
            if(sub != null) settings.thirdColorCustom.b = float.Parse(sub.Value) / 255;
            instance.SaveSetting();
            sub = root.Element("spriteDirectory");
            try {
                File.Delete(path);
            } catch (Exception) {
                // ignored
            }
            if(sub == null || sub.Value == Sprites.GetPath()) return;
            foreach(string file in Directory.GetFiles(sub.Value))
                try {
                    File.Copy(file, System.IO.Path.Combine(Sprites.GetPath(), System.IO.Path.GetFileName(file)), true);
                } catch (Exception) {
                    // ignored
                }
        } catch (Exception e) {
            instance.LogReportException("Error while migration settings", e);
        }
    }
}