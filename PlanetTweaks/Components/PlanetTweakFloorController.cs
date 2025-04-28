using UnityEngine;

namespace PlanetTweaks.Components;

public class PlanetTweakFloorController : MonoBehaviour {
    public static PlanetTweakFloorController instance;
    public scrFloor planetFloor;
    public scrFloor exitFloor;
    public scrFloor[] floors = new scrFloor[24];

    private void Awake() {
        instance = this;
    }
}