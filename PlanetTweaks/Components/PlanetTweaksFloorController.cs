using UnityEngine;

namespace PlanetTweaks.Components;

public class PlanetTweaksFloorController : MonoBehaviour {
    public static PlanetTweaksFloorController instance;
    public scrFloor planetFloor;
    public scrFloor exitFloor;
    public PlanetSettingFloor[] floors = new PlanetSettingFloor[24];

    private void Awake() {
        instance = this;
    }
}