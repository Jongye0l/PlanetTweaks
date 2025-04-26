using HarmonyLib;
using PlanetTweaks.Utils;
using UnityEngine;

namespace PlanetTweaks.Patch;

[HarmonyPatch(typeof(scrPlanet), "Awake")]
public static class PlanetAwakePatch {
    public static void Postfix(scrPlanet __instance) {
        if(__instance.dummyPlanets || __instance.planetRenderer.objectDecoration)
            return;
        if(__instance.transform.Find("PlanetTweaksRenderer"))
            return;
        SpriteRenderer renderer = Sprites.GetOrAddRenderer(__instance);
        if(__instance.isRed) {
            renderer.transform.localScale = new Vector3(Main.Settings.redSize, Main.Settings.redSize);
            if(Main.Settings.redColor)
                renderer.color = ColorUtils.GetRealColor(true);
        } else if(!__instance.isExtra) {
            renderer.transform.localScale = new Vector3(Main.Settings.blueSize, Main.Settings.blueSize);
            if(Main.Settings.blueColor)
                renderer.color = ColorUtils.GetRealColor(false);
        }
    }
}

[HarmonyPatch(typeof(PlanetRenderer), "LoadPlanetColor")]
public static class LoadPlanetColorPatch {
    public static void Postfix(PlanetRenderer __instance) {
        scrPlanet planet = __instance.GetComponent<scrPlanet>();
        if(planet.dummyPlanets || __instance.objectDecoration)
            return;
        if(!__instance.sprite.visible) return;
        if(planet.isRed) Sprites.RedSelected = Sprites.RedSelected;
        else if(!planet.isExtra) Sprites.BlueSelected = Sprites.BlueSelected;
        else {
            Sprites.ThirdSelected = Sprites.ThirdSelected;
            ColorUtils.SetThirdColor();
        }
    }
}

[HarmonyPatch(typeof(scrController), "Awake")]
[HarmonyPatch(typeof(PlanetarySystem), "ColorPlanets")]
[HarmonyPatch(typeof(PlanetarySystem), "SetNumPlanets")]
public static class ThirdPlanetPatch {
    public static void Postfix() {
        Sprites.ThirdSelected = Sprites.ThirdSelected;
        ColorUtils.SetThirdColor();
        SpriteRenderer renderer = PlanetUtils.GetThirdPlanet().GetOrAddRenderer();
        renderer.transform.localScale = new Vector3(Main.Settings.thirdSize, Main.Settings.thirdSize);
        renderer.color = Main.Settings.thirdColor ? ColorUtils.GetThirdColor() : Color.white;
    }
}