using DG.Tweening;
using UnityEngine;

namespace PlanetTweaks.Components;

public class ExitTileMouseDetector : MonoBehaviour {
    private void OnMouseEnter() {
        scrFloor floor = PlanetTweaksFloorController.instance.exitFloor;
        floor.DOKill();
        floor.transform.DOScale(new Vector3(0.55f, 0.55f), 0.5f);
    }
    
    private void OnMouseExit() {
        scrFloor floor = PlanetTweaksFloorController.instance.exitFloor;
        floor.DOKill();
        floor.transform.DOScale(new Vector3(0.5f, 0.5f), 0.5f);
    }

    private void OnMouseDown() => ImageChangePage.Exit();
}