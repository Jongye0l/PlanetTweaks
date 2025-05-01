using System;
using DG.Tweening;
using UnityEngine;

namespace PlanetTweaks.Components;

public class ImportTileMouseDetector : MonoBehaviour {

    private void OnMouseEnter() {
        scrFloor floor = PlanetTweaksFloorController.instance.floors[23].floor;
        floor.transform.DOKill();
        floor.transform.DOScale(new Vector3(0.9f, 0.9f), 0.5f);
    }

    private void OnMouseExit() {
        scrFloor floor = PlanetTweaksFloorController.instance.floors[23].floor;
        floor.transform.DOKill();
        floor.transform.DOScale(new Vector3(0.8f, 0.8f), 0.5f);
    }

    private void OnMouseDown() {
        string file = Sprites.ShowOpenFileDialog();
        try {
            Sprites.Add(file);
            ImageChangePage.instance.UpdateFloorIcons();
        } catch (Exception e) {
            Main.instance.Warning("wrong file '" + file + "'!");
            Main.instance.Warning(e.StackTrace);
        }
    }
}