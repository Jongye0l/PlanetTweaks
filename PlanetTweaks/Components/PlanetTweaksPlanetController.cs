using System.Collections.Generic;
using UnityEngine;

namespace PlanetTweaks.Components;

public class PlanetTweaksPlanetController : MonoBehaviour {
    public static PlanetTweaksPlanetController instance;
    public Dictionary<scrPlanet, SpriteRenderer> spriteDictionary = new();

    public static PlanetTweaksPlanetController GetInstance() {
        if(!instance) instance = new GameObject("PlanetTweaksPlanetController").AddComponent<PlanetTweaksPlanetController>();
        return instance;
    }

    public SpriteRenderer GetPlanetRenderer(scrPlanet planet) {
        return !planet ? null : spriteDictionary.GetValueOrDefault(planet);
    }

    private void OnDestroy() {
        spriteDictionary.Clear();
        spriteDictionary = null;
        instance = null;
    }
}