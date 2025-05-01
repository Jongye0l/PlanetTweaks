using DG.Tweening;
using JALib.Core;
using PlanetTweaks.Utils;
using UnityEngine;

namespace PlanetTweaks.Components;

public class PlanetTileMouseDetector : MonoBehaviour {
    private void OnMouseEnter() {
        PlanetTweaksFloorController.instance.planetFloor.transform.DOScale(new Vector3(1, 1), 0.5f);
    }
    
    private void OnMouseExit() {
        PlanetTweaksFloorController.instance.planetFloor.transform.DOScale(new Vector3(0.8f, 0.8f), 0.5f);
    }
    
    private void OnMouseDown() {
        ImageChangePage.instance.changing = true;
        scrUIController.instance.WipeToBlack(WipeDirection.StartsFromRight, AfterWipe);
    }

    private static void AfterWipe() {
        ImageChangePage imageChange = ImageChangePage.instance;
        imageChange.changing = false;
        PlanetarySystem planetarySystem = scrController.instance.planetarySystem;
        planetarySystem.chosenPlanet = scrController.instance.planetarySystem.chosenPlanet.next;
        planetarySystem.chosenPlanet.transform.LocalMoveXY(-15, -3);
        planetarySystem.chosenPlanet.transform.position = new Vector3(-15, -3);
        JALocalization localization = Main.instance.Localization;
        imageChange.planetText.text = (planetarySystem.chosenPlanet.isRed    ? $"<color={ColorUtils.GetRealColor(true).Hex()}>{localization["Setting.RedColor"]}</color>" :
                                       !planetarySystem.chosenPlanet.isExtra ? $"<color={ColorUtils.GetRealColor(false).Hex()}>{localization["Setting.BlueColor"]}</color>" :
                                                                               $"<color={ColorUtils.GetRealThirdColor().Hex()}>{localization["Setting.ThirdColor"]}</color>")
                                      + ' ' + localization["ChangePage.Selected"];
        imageChange.UpdateFloorIcons();
        scrUIController.instance.WipeFromBlack();
    }
}