using System;
using UnityEngine;

namespace PlanetTweaks.Components;

public class PlanetTweaksFloorController : MonoBehaviour {
    public static PlanetTweaksFloorController instance;
    public scrFloor eventFloor;
    public scrFloor planetFloor;
    public scrFloor exitFloor;
    public PlanetSettingFloor[] floors = new PlanetSettingFloor[24];

    public static void Dispose() {
        if(!instance) return;
        Destroy(instance.eventFloor.gameObject);
        Destroy(instance.planetFloor.gameObject);
        Destroy(instance.exitFloor.gameObject);
        Destroy(instance.floors[0].transform.parent.gameObject);
        Destroy(instance.gameObject);
        instance = null;
    }

    private void Awake() {
        instance = this;
    }

    private void OnDestroy() {
        floors = null;
    }
}