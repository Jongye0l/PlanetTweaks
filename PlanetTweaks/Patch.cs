using System;
using System.Reflection;
using ADOFAI.ModdingConvenience;
using DG.Tweening;
using JALib.Core;
using JALib.Core.Patch;
using JALib.Tools;
using PlanetTweaks.Components;
using PlanetTweaks.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PlanetTweaks;

public static class Patch {
    public static scrFloor leftMovingFloor;
    public static scrFloor rightMovingFloor;

    [JAPatch(typeof(scrController), nameof(Update), PatchType.Postfix, false)]
    public static void Update() {
        if(!scnLevelSelect.instance || !scrCamera.instance || !leftMovingFloor || !rightMovingFloor) return;
        float x = (float) Math.Round(scrController.instance.planetarySystem.chosenPlanet.transform.position.x);
        float y = (float) Math.Round(scrController.instance.planetarySystem.chosenPlanet.transform.position.y);
        if(x is not (3 or -3) || y is < -18 or > -7) return;
        switch(y) {
            case <= -8 when KeyCode.UpArrow.WentDown() || Input.mouseScrollDelta.y > 0.4f:
                scrController.instance.planetarySystem.chosenPlanet.transform.DOComplete();
                leftMovingFloor.transform.DOComplete();
                rightMovingFloor.transform.DOComplete();
                scrController.instance.planetarySystem.chosenPlanet.transform.DOMoveY(y + 1, 0.2f);
                leftMovingFloor.transform.DOMoveY(y + 1, 0.2f);
                rightMovingFloor.transform.DOMoveY(y + 1, 0.2f);
                scrCamera.instance.frompos = scrCamera.instance.pos;
                scrCamera.instance.topos = new Vector3(x, y + 1, -10);
                scrCamera.instance.timer = 0;
                break;
            case >= -17 when KeyCode.DownArrow.WentDown() || Input.mouseScrollDelta.y < -0.4f:
                scrController.instance.planetarySystem.chosenPlanet.transform.DOComplete();
                leftMovingFloor.transform.DOComplete();
                rightMovingFloor.transform.DOComplete();
                scrController.instance.planetarySystem.chosenPlanet.transform.DOMoveY(y - 1f, 0.2f);
                leftMovingFloor.transform.DOMoveY(y - 1, 0.2f);
                rightMovingFloor.transform.DOMoveY(y - 1, 0.2f);
                scrCamera.instance.frompos = scrCamera.instance.pos;
                scrCamera.instance.topos = new Vector3(x, y - 1, -10);
                scrCamera.instance.timer = 0;
                break;
        }
    }

    [JAPatch(typeof(scnLevelSelect), nameof(Start), PatchType.Postfix, false)]
    public static void Start() {
        scrFloor eventFloor;
        if(!(eventFloor = FloorUtils.AddEventFloor(-2, -3, delegate {
               scrUIController.instance.WipeToBlack(WipeDirection.StartsFromRight, delegate {
                   scrController controller = scrController.instance;
                   PlanetarySystem planetarySystem = controller.planetarySystem;
                   planetarySystem.chosenPlanet = planetarySystem.planetRed;
                   controller.camy.zoomSize = 0.5f;
                   controller.camy.isPulsingOnHit = false;
                   new GameObject().AddComponent<ImageChangePage>();
                   if(!Main.settings.thirdPlanet) return;
                   planetarySystem.SetNumPlanets(3);
                   scrFloor floor = PlanetTweaksFloorController.instance.planetFloor;
                   floor.numPlanets = 3;
                   planetarySystem.planetRed.currfloor = floor;
                   planetarySystem.planetBlue.currfloor = floor;
                   planetarySystem.planetGreen.currfloor = floor;
                   FieldInfo field = typeof(scrPlanet).Field("endingTween");
                   field.SetValue(planetarySystem.planetRed, 1);
                   field.SetValue(planetarySystem.planetBlue, 1);
                   field.SetValue(planetarySystem.planetGreen, 1);
                   planetarySystem.chosenPlanet.transform.LocalMoveXY(-15, -3);
                   planetarySystem.chosenPlanet.transform.position = new Vector3(-15, -3);
                   controller.camy.ViewObjectInstant(planetarySystem.chosenPlanet.transform);
                   controller.camy.ViewVectorInstant(new Vector2(-18, -3.5f));
                   controller.camy.isMoveTweening = false;
                   scrUIController.instance.WipeFromBlack();
                   planetarySystem.planetList.ForEach(p => p.currfloor = floor);
               });
           }, GameObject.Find("outer ring").transform))) return;
        PlanetTweaksFloorController controller = new GameObject("PlanetTweaksFloorController").AddComponent<PlanetTweaksFloorController>();
        controller.eventFloor = eventFloor;
        if(!(controller.planetFloor = FloorUtils.AddEventFloor(-15, -3, null))) controller.planetFloor = FloorUtils.AddFloor(-15, -3);
        scrFloor exitFloor = controller.exitFloor = FloorUtils.AddFloor(-13.9f, -5.65f);
        exitFloor.transform.ScaleXY(0.5f, 0.5f);
        exitFloor.isportal = true;
        exitFloor.floorRenderer.sortingOrder = 0;
        {
            scrPortalParticles particle = Object.Instantiate(PrefabLibrary.instance.lastTilePortalPrefab, exitFloor.transform);
            particle.Invoke("Start");
            Object.Destroy(particle);
        }
        GameObject images = new() {
            name = "PlanetTweaks_Images"
        };
        JALocalization localization = Main.instance.Localization;
        for(int i = 0; i < 4; i++)
            for(int j = 0; j < 6; j++) {
                GameObject obj = new() {
                    transform = {
                        parent = images.transform
                    }
                };
                PlanetSettingFloor settingFloor = controller.floors[i * 6 + j] = obj.AddComponent<PlanetSettingFloor>();
                scrFloor floor = settingFloor.floor = FloorUtils.AddFloor(-21.7f + j * 0.9f, -1.9f - i * 1.1f, obj.transform);
                floor.transform.ScaleXY(0.8f, 0.8f);
                floor.dontChangeMySprite = true;
                if(j == 5 && i == 3) {
                    floor.isportal = true;
                    floor.floorRenderer.sortingOrder = 0;
                    scrPortalParticles particle = Object.Instantiate(PrefabLibrary.instance.lastTilePortalPrefab, floor.transform);
                    particle.Invoke("Start");
                    Object.Destroy(particle);
                } else {
                    floor.floorRenderer.renderer.sortingOrder = 1;
                    floor.floorRenderer.renderer.sortingLayerID = 0;
                    floor.floorRenderer.renderer.sortingLayerName = "Default";
                }

                TextMesh textMesh = settingFloor.nameText = new GameObject().AddComponent<TextMesh>();
                textMesh.transform.parent = obj.transform;
                (settingFloor.nameRenderer = textMesh.gameObject.GetOrAddComponent<MeshRenderer>()).sortingOrder = 0;
                textMesh.SetLocalizedFont();
                textMesh.fontSize = 100;
                if(j == 5 && i == 3) {
                    textMesh.text = localization["ChangePage.Import"];
                    textMesh.anchor = TextAnchor.MiddleCenter;
                    textMesh.transform.position = new Vector3(floor.x, floor.y - 0.5f);
                    textMesh.transform.ScaleXY(0.02f, 0.02f);
                    continue;
                }
                textMesh.transform.position = new Vector3(floor.x - 0.35f, floor.y - 0.38f);
                textMesh.transform.ScaleXY(0.015f, 0.015f);

                textMesh = settingFloor.preview = new GameObject().AddComponent<TextMesh>();
                textMesh.transform.parent = obj.transform;
                textMesh.gameObject.GetOrAddComponent<MeshRenderer>().sortingOrder = 3;
                textMesh.SetLocalizedFont();
                textMesh.fontSize = 100;
                textMesh.text = localization["ChangePage.Preview"];
                textMesh.anchor = TextAnchor.MiddleRight;
                textMesh.transform.position = new Vector3(-21.7f + j * 0.9f + 0.46f, -1.9f - i * 1.1f - 0.36f);
                textMesh.transform.ScaleXY(0.018f, 0.018f);
                textMesh.gameObject.SetActive(false);

                SpriteRenderer icon = settingFloor.icon = new GameObject().AddComponent<SpriteRenderer>();
                icon.transform.parent = obj.transform;
                icon.sortingOrder = 2;
                icon.transform.position = floor.transform.position;
                icon.transform.ScaleXY(0.7f, 0.7f);
            }
        foreach(PlanetSettingFloor floor in PlanetTweaksFloorController.instance.floors) {
            floor.floor.isLandable = false;
            floor.floor.gameObject.SetActive(false);
        }
        leftMovingFloor = FloorUtils.AddEventFloor(-3, -7, null);
        rightMovingFloor = FloorUtils.AddEventFloor(3, -7, null);
        GameObject inputField = Object.Instantiate(Main.bundle.LoadAsset<GameObject>("InputField"));
        inputField.AddComponent<RenameInputField>();
    }

    [JAPatch(typeof(scrPlanet), nameof(Awake), PatchType.Postfix, false)]
    public static void Awake(scrPlanet __instance) {
        if(__instance.dummyPlanets || __instance.planetRenderer.objectDecoration || __instance.transform.Find("PlanetTweaksRenderer")) return;
        SpriteRenderer renderer = __instance.GetOrAddRenderer();
        if(__instance.isRed) {
            renderer.transform.localScale = new Vector3(Main.settings.redSize, Main.settings.redSize);
            if(Main.settings.redColor) renderer.color = ColorUtils.GetRealColor(true);
        } else if(!__instance.isExtra) {
            renderer.transform.localScale = new Vector3(Main.settings.blueSize, Main.settings.blueSize);
            if(Main.settings.blueColor) renderer.color = ColorUtils.GetRealColor(false);
        }
    }

    [JAPatch(typeof(PlanetRenderer), nameof(LoadPlanetColor), PatchType.Postfix, false)]
    public static void LoadPlanetColor(PlanetRenderer __instance) {
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

    [JAPatch(typeof(scrController), nameof(Awake), PatchType.Postfix, false)]
    [JAPatch(typeof(PlanetarySystem), nameof(ColorPlanets), PatchType.Postfix, false)]
    [JAPatch(typeof(PlanetarySystem), "SetNumPlanets", PatchType.Postfix, false)]
    public static void ColorPlanets() {
        Sprites.ThirdSelected = Sprites.ThirdSelected;
        ColorUtils.SetThirdColor();
        SpriteRenderer renderer = scrController.instance.planetarySystem.planetGreen.GetOrAddRenderer();
        renderer.transform.localScale = new Vector3(Main.settings.thirdSize, Main.settings.thirdSize);
        renderer.color = Main.settings.thirdColor ? ColorUtils.GetThirdColor() : Color.white;
    }

    [JAPatch(typeof(Persistence), nameof(SetPlayerColor), PatchType.Postfix, false)]
    public static void SetPlayerColor(bool red) {
        try {
            if(red && Main.settings.redColor)
                scrController.instance.planetarySystem.planetRed.GetOrAddRenderer().color = ColorUtils.GetRealColor(true);
            else if(!red && Main.settings.blueColor)
                scrController.instance.planetarySystem.planetBlue.GetOrAddRenderer().color = ColorUtils.GetRealColor(false);
        } catch (Exception) {
            // ignored
        }
    }

    [JAPatch(typeof(scrPlanet), nameof(Update_RefreshAngles), PatchType.Postfix, false)]
    public static void Update_RefreshAngles(scrPlanet __instance, Vector3 ___tempTransPos, scrPlanet ___movingToNext) {
        if(!Main.settings.shapedRotation || !__instance.isChosen) return;
        float angle = 360f / Main.settings.shapedAngle * Mathf.Deg2Rad;
        int planets = __instance.currfloor?.numPlanets ?? 3;
        if(planets > 2) return;
        Vector3 substract = (___movingToNext.transform.position - ___tempTransPos) / __instance.cosmeticRadius;
        float realAngle = (Mathf.Asin(substract.x) + Mathf.PI * 2) % (Mathf.PI * 2);
        if(substract.y < 0) realAngle = (-realAngle + Mathf.PI * 3) % (Mathf.PI * 2);
        float shaped = (int) (realAngle / angle) * angle;
        ___movingToNext.transform.position = new Vector3(___tempTransPos.x + Mathf.Sin(shaped) * __instance.cosmeticRadius, ___tempTransPos.y + Mathf.Cos(shaped) * __instance.cosmeticRadius, ___tempTransPos.z);
    }

    [JAPatch(typeof(scrPlanet), nameof(SwitchChosen), PatchType.Prefix, false)]
    public static bool SwitchChosen(scrPlanet __instance, ref scrPlanet __result) {
        if(!scnLevelSelect.instance) return true;
        float x = (float) Math.Round(__instance.transform.position.x);
        float y = (float) Math.Round(__instance.transform.position.y);
        if(x is not (3 or -3) || y is < -18 or > -7 || !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow)) return true;
        if(x == 3) {
            __instance.currfloor = leftMovingFloor;
            __instance.other.currfloor = leftMovingFloor;
        } else {
            __instance.currfloor = rightMovingFloor;
            __instance.other.currfloor = rightMovingFloor;
        }
        __result = __instance;
        return false;
    }

    [JAPatch(typeof(scrPlanet), nameof(SwitchChosen), PatchType.Postfix, false)]
    public static void SwitchChosenPostfix(scrPlanet __instance, ref scrPlanet __result) {
        if(!scnLevelSelect.instance || __instance == __result) return;
        float x = scrController.instance.planetarySystem.chosenPlanet.other.transform.position.x;
        float y = scrController.instance.planetarySystem.chosenPlanet.other.transform.position.y;
        if(x is <= -6 or >= 6 || y is < -18 or > -6) return;
        leftMovingFloor.transform.DOMoveY(y >= -7 ? -7 : y, 0.5f);
        rightMovingFloor.transform.DOMoveY(y >= -7 ? -7 : y, 0.5f);
    }
}