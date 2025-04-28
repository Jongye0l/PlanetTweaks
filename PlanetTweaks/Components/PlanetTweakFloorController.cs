using UnityEngine;

namespace PlanetTweaks.Components;

public class PlanetTweakFloorController : MonoBehaviour {
    public static PlanetTweakFloorController instance;
    public scrFloor planetFloor;
    public scrFloor exitFloor;
    public PlanetSettingFloor[] floors = new PlanetSettingFloor[24];

    private void Awake() {
        instance = this;
    }
}