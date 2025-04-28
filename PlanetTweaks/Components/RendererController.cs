using UnityEngine;

namespace PlanetTweaks.Components;

public class RendererController : MonoBehaviour {
    public static RendererController redController;
    public static RendererController blueController;
    public static RendererController thirdController;
    public scrPlanet planet;
    public SpriteRenderer renderer;

    private void Awake() {
        planet = GetComponentInParent<scrPlanet>();
        renderer = gameObject.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = planet.planetRenderer.GetComponent<SpriteRenderer>().sortingOrder + 1;
        renderer.sortingLayerID = planet.planetRenderer.faceDetails.sortingLayerID;
        renderer.sortingLayerName = planet.planetRenderer.faceDetails.sortingLayerName;
        scrController controller = scrController.instance;
        if(planet == controller.planetRed) redController = this;
        else if(planet == controller.planetBlue) blueController = this;
        else if(planet == controller.planetGreen) thirdController = this;
    }

    private void Update() {
        if(!planet || !renderer) return;
        if(planet.dummyPlanets) {
            Destroy(gameObject);
            return;
        }
        if(renderer.enabled != planet.planetRenderer.sprite.visible) renderer.enabled = planet.planetRenderer.sprite.visible;
    }
}