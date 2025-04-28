using JALib.Tools;
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
        scrPlanet planet = this.planet = GetComponentInParent<scrPlanet>();
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
        scrController controller = scrController.instance;
        if(!controller) {
            MainThread.Run(Main.instance, SetupStatic);
            return;
        }
        if(planet == controller.planetRed) redController = this;
        else if(planet == controller.planetBlue) blueController = this;
        else if(planet == controller.planetGreen) thirdController = this;
    }
}