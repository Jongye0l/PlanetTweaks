using System;
using System.Collections.Generic;
using System.Reflection;
using ByteSheep.Events;
using DG.Tweening;
using JALib.Core;
using JALib.Tools;
using PlanetTweaks.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityModManagerNet;

namespace PlanetTweaks.Components;

public class ImageChangePage : MonoBehaviour {
    public static ImageChangePage instance;
    private List<Tween> activeTweens = [];

    public static void WipeToMove() => scrUIController.instance.WipeToBlack(WipeDirection.StartsFromRight, OnMove);

    private static void OnMove() {
        scrController controller = scrController.instance;
        PlanetarySystem planetarySystem = controller.planetarySystem;
        planetarySystem.chosenPlanet = planetarySystem.planetRed;
        controller.camy.zoomSize = 0.5f;
        controller.camy.isPulsingOnHit = false;
        new GameObject().AddComponent<ImageChangePage>();
        scrFloor floor = PlanetTweaksFloorController.instance.planetFloor;
        if(Main.settings.thirdPlanet) {
            planetarySystem.SetNumPlanets(3);
            floor.numPlanets = 3;
            planetarySystem.planetRed.currfloor = floor;
            planetarySystem.planetBlue.currfloor = floor;
            planetarySystem.planetGreen.currfloor = floor;
            FieldInfo field = typeof(scrPlanet).Field("endingTween");
            field.SetValue(planetarySystem.planetRed, 1);
            field.SetValue(planetarySystem.planetBlue, 1);
            field.SetValue(planetarySystem.planetGreen, 1);
        }
        planetarySystem.chosenPlanet.transform.LocalMoveXY(-15, -3);
        planetarySystem.chosenPlanet.transform.position = new Vector3(-15, -3);
        controller.camy.ViewObjectInstant(planetarySystem.chosenPlanet.transform);
        controller.camy.ViewVectorInstant(new Vector2(-18, -3.5f));
        controller.camy.isMoveTweening = false;
        scrUIController.instance.WipeFromBlack();
        planetarySystem.planetList.ForEach(p => p.currfloor = floor);
        PlanetTweaksFloorController.instance.showing = true;
    }

    public static void Exit() {
        scrController.instance.SetValue("exitingToMainMenu", true);
        if(instance) Destroy(instance);
        instance = null;
        GCS.sceneToLoad = GCNS.sceneLevelSelect;
        scrUIController.instance.WipeToBlack(WipeDirection.StartsFromRight, ExitAfterWipe);
    }

    private static void ExitAfterWipe() {
        DOTween.KillAll();
        SceneManager.LoadScene("scnLoading");
    }

    public static Color floorColor = new(0.78f, 0.78f, 0.886f);

    public void UpdateFloorIcons() {
        PlanetSettingFloor[] settingFloors = PlanetTweaksFloorController.instance.floors;
        for(int i = 0; i < settingFloors.Length - 1; i++) {
            PlanetSettingFloor cur = PlanetTweaksFloorController.instance.floors[i];
            Sprites.Apply(cur.icon, null);
            cur.nameText.text = null;
            cur.floor.SetTileColor((scrController.instance.planetarySystem.chosenPlanet.isRed    ? Sprites.RedSelected :
                                    !scrController.instance.planetarySystem.chosenPlanet.isExtra ? Sprites.BlueSelected : Sprites.ThirdSelected) == i + page * 23
                                       ? Color.yellow
                                       : floorColor);
        }
        for(int i = 0; i < settingFloors.Length - 1; i++) {
            if(i + page * 23 >= Sprites.sprites.Count) break;
            KeyValuePair<string, object> pair = Sprites.sprites.ElementAt(i + page * 23);
            PlanetSettingFloor cur = PlanetTweaksFloorController.instance.floors[i];
            Sprites.Apply(cur.icon, pair.Value);
            cur.nameText.text = pair.Key;
        }
    }

    private void UpdatePageText() => pageText.text = string.Format(Main.instance.Localization["ChangePage.Page2"], page + 1, Math.Max(Mathf.CeilToInt(Sprites.sprites.Count / 23f), 1));

    public void ChangePage(int page) {
        if(page == this.page) return;
        List<Tween> tweens = activeTweens;
        this.page = page;
        UpdatePageText();
        if(changing) {
            foreach(Tween activeTween in tweens) activeTween.Kill();
            tweens.Clear();
        }
        changing = true;
        Tween fade = null;
        for(int i = 0; i < 23; i++) {
            PlanetSettingFloor settingFloor = PlanetTweaksFloorController.instance.floors[i];
            ColorGetSetter colorGetter = new(settingFloor.floor.floorRenderer);
            tweens.AddRange([
                fade = DOTween.To(colorGetter.GetColor, colorGetter.SetColor, colorGetter.GetColor().WithAlpha(0), 0.3f),
                settingFloor.nameRenderer.material.DOFade(0, 0.3f),
                settingFloor.icon.material.DOFade(0, 0.3f)
            ]);
        }
        fade.OnComplete(ChangePageAfterHide);
    }

    private static void ChangePageAfterHide() {
        List<Tween> tweens = instance.activeTweens;
        tweens.Clear();
        instance.UpdateFloorIcons();
        Tween fade = null;
        for(int i = 0; i < 23; i++) {
            PlanetSettingFloor settingFloor = PlanetTweaksFloorController.instance.floors[i];
            ColorGetSetter colorGetter = new(settingFloor.floor.floorRenderer);
            tweens.AddRange([
                fade = DOTween.To(colorGetter.GetColor, colorGetter.SetColor, colorGetter.GetColor().WithAlpha(1), 0.3f),
                settingFloor.nameRenderer.material.DOFade(1, 0.3f),
                settingFloor.icon.material.DOFade(1, 0.3f)
            ]);
        }
        fade.OnComplete(ChangePageAfterShow);
    }

    private static void ChangePageAfterShow() {
        instance.activeTweens.Clear();
        instance.changing = false;
    }

    //private bool propertyPage = false;
    public bool changing;

    public int page;

    public TextMesh planetText;
    public TextMesh pageText;

    public void Awake() {
        if(instance) Destroy(instance.gameObject);
        instance = this;
        JALocalization localization = Main.instance.Localization;
        planetText = new GameObject().AddComponent<TextMesh>();
        planetText.richText = true;
        planetText.text = $"<color={ColorUtils.GetRealColor(true).Hex()}>{localization["Setting.RedColor"]}</color> {localization["ChangePage.Selected"]}";
        planetText.SetLocalizedFont();
        planetText.fontSize = 100;
        planetText.anchor = TextAnchor.UpperCenter;
        planetText.transform.position = new Vector3(-15.05f, -4.25f);
        planetText.transform.ScaleXY(0.045f, 0.045f);
        TextMesh exit = new GameObject().AddComponent<TextMesh>();
        exit.text = localization["ChangePage.Exit"];
        exit.SetLocalizedFont();
        exit.fontSize = 100;
        exit.transform.position = new Vector3(-15.2f, -5.29f);
        exit.transform.ScaleXY(0.05f, 0.05f);

        pageText = new GameObject().AddComponent<TextMesh>();
        pageText.text = string.Format(localization["ChangePage.Page"], 1);
        pageText.SetLocalizedFont();
        pageText.fontSize = 100;
        pageText.transform.position = new Vector3(-22, -1.23f);
        pageText.transform.ScaleXY(0.02f, 0.02f);
        UpdateFloorIcons();
        UpdatePageText();

        PlanetarySystem planetarySystem = scrController.instance.planetarySystem;
        SetupPlanet(planetarySystem.planetRed);
        SetupPlanet(planetarySystem.planetBlue);
        SetupPlanet(planetarySystem.planetGreen);
    }

    private static void SetupPlanet(scrPlanet planet) {
        planet.gameObject.AddComponent<PlanetTileMouseDetector>();
        Transform transform = planet.transform;
        int count = transform.childCount;
        for(int i = 0; i < count; i++) transform.GetChild(i).gameObject.AddComponent<PlanetTileMouseDetector>();
    }

    public void Update() {
        if(instance.page != 0 && Input.GetKeyDown(KeyCode.LeftArrow)) ChangePage(instance.page - 1);
        else if(Sprites.sprites.Count > (instance.page + 1) * 23 && Input.GetKeyDown(KeyCode.RightArrow)) instance.ChangePage(instance.page + 1);
    }

    private class ColorGetSetter(FloorRenderer floorRenderer) {
        public Color GetColor() => floorRenderer.color;
        public void SetColor(Color color) => floorRenderer.color = color;
    }
}