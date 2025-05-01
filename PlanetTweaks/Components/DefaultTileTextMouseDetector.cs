using System.IO;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace PlanetTweaks.Components;

public class DefaultTileTextMouseDetector : MonoBehaviour {
    public int index;
    
    private void OnMouseEnter() {
        TextMesh text = PlanetTweaksFloorController.instance.floors[index].nameText;
        text.DOKill();
        DOTween.To(() => text.color, c => text.color = c, Color.yellow, 0.5f).SetTarget(text);
    }
    
    private void OnMouseExit() {
        TextMesh text = PlanetTweaksFloorController.instance.floors[index].nameText;
        text.DOKill();
        DOTween.To(() => text.color, c => text.color = c, Color.white, 0.5f).SetTarget(text);
    }
    
    private void OnMouseDown() {
        TextMesh text = PlanetTweaksFloorController.instance.floors[this.index].nameText;
        int index = ImageChangePage.instance.page * 23 + this.index;
        if(index >= Sprites.sprites.Count) return;
        RenameInputField.instance.Show(text.text, s => {
            if(s.Trim().IsNullOrEmpty() || s == text.text) return;
            s = Path.GetInvalidPathChars().Aggregate(s, (cur, c) => cur.Replace(c.ToString(), "_"));
            string first = s;
            for(int k = 1; Sprites.sprites.ContainsKey(s); k++) s = first + k;
            if(index == Sprites.RedSelected) Main.settings.redSelected = s;
            if(index == Sprites.BlueSelected) Main.settings.blueSelected = s;
            text.text = s;
            Sprites.sprites.Replace(index, s, Sprites.sprites.ElementAt(index).Value);
        });
    }
}