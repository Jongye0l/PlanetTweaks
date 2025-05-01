using UnityEngine;

namespace PlanetTweaks.Components;

public class RendererController : MonoBehaviour {
    public scrPlanet planet;
    public PlanetRenderer planetRenderer;
    public SpriteRenderer renderer;

    private void Awake() {
        planet = GetComponentInParent<scrPlanet>();
        this.planetRenderer = planet.planetRenderer;
        if(!(this.renderer = gameObject.GetComponent<SpriteRenderer>())) {
            SpriteRenderer renderer = this.renderer = gameObject.AddComponent<SpriteRenderer>();
            PlanetRenderer planetRenderer = this.planetRenderer;
            renderer.sortingOrder = planetRenderer.GetComponent<SpriteRenderer>().sortingOrder + 1;
            renderer.sortingLayerID = planetRenderer.faceDetails.sortingLayerID;
            renderer.sortingLayerName = planetRenderer.faceDetails.sortingLayerName;
        }
        PlanetTweaksPlanetController.GetInstance().spriteDictionary[planet] = renderer;
    }

    private void Update() {
        if(!planet || !renderer) return;
        if(planet.dummyPlanets) {
            Destroy(gameObject);
            return;
        }
        if(renderer.enabled != planetRenderer.sprite.visible) renderer.enabled = planetRenderer.sprite.visible;
    }
}