using System.Threading.Tasks;
using UnityEngine;

namespace PlanetTweaks.Components;

public class RendererController : MonoBehaviour {
    public static RendererController redController;
    public static RendererController blueController;
    public static RendererController thirdController;
    public scrPlanet planet;
    public PlanetRenderer planetRenderer;
    public SpriteRenderer renderer;

    private void Awake() {
        planet = GetComponentInParent<scrPlanet>();
        SpriteRenderer renderer = this.renderer = gameObject.AddComponent<SpriteRenderer>();
        PlanetRenderer planetRenderer = this.planetRenderer = planet.planetRenderer;
        renderer.sortingOrder = planetRenderer.GetComponent<SpriteRenderer>().sortingOrder + 1;
        renderer.sortingLayerID = planetRenderer.faceDetails.sortingLayerID;
        renderer.sortingLayerName = planetRenderer.faceDetails.sortingLayerName;
        SetupStatic();
    }

    private void Update() {
        if(!planet || !renderer) return;
        if(planet.dummyPlanets) {
            Destroy(gameObject);
            return;
        }
        if(renderer.enabled != planetRenderer.sprite.visible) renderer.enabled = planetRenderer.sprite.visible;
    }

    private void SetupStatic() {
        PlanetarySystem planetarySystem = scrController.instance?.planetarySystem;
        scrPlanet planet = this.planet;
        if(!planetarySystem || !planet) {
            Task.Yield().GetAwaiter().OnCompleted(SetupStatic);
            return;
        }
        if(planet == planetarySystem.planetRed) redController = this;
        else if(planet == planetarySystem.planetBlue) blueController = this;
        else if(planet == planetarySystem.planetGreen) thirdController = this;
    }
}