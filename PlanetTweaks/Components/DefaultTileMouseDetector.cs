using DG.Tweening;
using UnityEngine;

namespace PlanetTweaks.Components;

public class DefaultTileMouseDetector : MonoBehaviour {
    public int index;
    
    private void OnMouseEnter() {
        PlanetSettingFloor settingFloor = PlanetTweaksFloorController.instance.floors[index];
        scrFloor floor = settingFloor.floor;
        SpriteRenderer sprite = settingFloor.icon;
        if(!sprite.sprite) return;
        floor.transform.DOKill();
        floor.transform.DOScale(new Vector3(1, 1), 0.5f).OnComplete(delegate {
            int index = ImageChangePage.instance.page * 23 + this.index;
            if((scrController.instance.planetarySystem.chosenPlanet.isRed    ? Sprites.RedSelected :
                !scrController.instance.planetarySystem.chosenPlanet.isExtra ? Sprites.BlueSelected : Sprites.ThirdSelected) == index)
                return;
            settingFloor.preview.gameObject.SetActive(true);
            object value = Sprites.sprites.ElementAt(index).Value;
            if(scrController.instance.planetarySystem.chosenPlanet.isRed) Sprites.RedPreview = value;
            else if(!scrController.instance.planetarySystem.chosenPlanet.isExtra) Sprites.BluePreview = value;
            else Sprites.ThirdPreview = value;
        }).SetAutoKill(false);
        sprite.transform.DOKill();
        sprite.transform.DOScale(new Vector3(0.875f, 0.875f), 0.5f);
    }
    
    private void OnMouseExit() {
        PlanetSettingFloor settingFloor = PlanetTweaksFloorController.instance.floors[index];
        scrFloor floor = settingFloor.floor;
        floor.transform.DOKill();
        floor.transform.DOScale(new Vector3(0.8f, 0.8f), 0.5f);
        settingFloor.preview.gameObject.SetActive(false);
        SpriteRenderer sprite = settingFloor.icon;
        sprite.transform.DOKill();
        sprite.transform.DOScale(new Vector3(0.7f, 0.7f), 0.5f);
        if(scrController.instance.planetarySystem.chosenPlanet.isRed) Sprites.RedPreview = null;
        else if(!scrController.instance.planetarySystem.chosenPlanet.isExtra) Sprites.BluePreview = null;
        else Sprites.ThirdPreview = null;
    }
    
    private void OnMouseDown() {
        PlanetSettingFloor settingFloor = PlanetTweaksFloorController.instance.floors[this.index];
        scrFloor floor = settingFloor.floor;
        SpriteRenderer sprite = settingFloor.icon;
        if(!sprite.sprite) return;
        floor.transform.DOComplete();
        settingFloor.preview.gameObject.SetActive(false);
        sprite.transform.DOComplete();
        int index = ImageChangePage.instance.page * 23 + this.index;
        if(Input.GetMouseButtonDown(0))
            if((scrController.instance.planetarySystem.chosenPlanet.isRed ? Sprites.RedSelected : !scrController.instance.planetarySystem.chosenPlanet.isExtra ? Sprites.BlueSelected : Sprites.ThirdSelected) == index) {
                if(scrController.instance.planetarySystem.chosenPlanet.isRed) Sprites.RedSelected = -1;
                else if(!scrController.instance.planetarySystem.chosenPlanet.isExtra) Sprites.BlueSelected = -1;
                else Sprites.ThirdSelected = -1;
            } else {
                if(scrController.instance.planetarySystem.chosenPlanet.isRed) Sprites.RedSelected = index;
                else if(!scrController.instance.planetarySystem.chosenPlanet.isExtra) Sprites.BlueSelected = index;
                else Sprites.ThirdSelected = index;
            }
        else if(Input.GetMouseButtonDown(1)) {
            Sprites.Remove(index);
            if(scrController.instance.planetarySystem.chosenPlanet.isRed) Sprites.RedPreview = null;
            else if(!scrController.instance.planetarySystem.chosenPlanet.isExtra) Sprites.BluePreview = null;
            else Sprites.ThirdPreview = null;
        } else return;
        ImageChangePage.instance.UpdateFloorIcons();
    }
}