using PlanetTweaks.Utils;
using UnityEngine;

namespace PlanetTweaks.Components;

public class GifRenderer : MonoBehaviour {
    public GifImage Image;
    public SpriteRenderer renderer;

    private float timePassed;
    private float offset;

    public void SetData(GifImage image, SpriteRenderer renderer) {
        Image = image;
        this.renderer = renderer;
        timePassed = 0;
        offset = 0;
    }

    public void Update() {
        if(Image == null || !renderer || !renderer.enabled) return;
        long elapsed = (long) ((timePassed += Time.unscaledDeltaTime) * 1000 + offset);
        if(elapsed >= Image.Length) {
            timePassed = 0;
            elapsed -= Image.Length;
            offset = elapsed;
        }
    }

    public void LateUpdate() {
        if(Image == null || !renderer || !renderer.enabled) return;
        renderer.sprite = Image.GetFrameAt((long) (timePassed * 1000 + offset));
    }
}