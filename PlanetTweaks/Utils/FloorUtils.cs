using ADOFAI.ModdingConvenience;
using ByteSheep.Events;
using DG.Tweening;
using JALib.Tools;
using UnityEngine;

namespace PlanetTweaks.Utils;

public static class FloorUtils {
    private static Transform gemTransform;

    public static Transform GetGemTransform() {
        if(!gemTransform) gemTransform = GameObject.Find("outer ring").transform.Find("ChangingRoomGem").Find("MovingGem");
        return gemTransform;
    }

    public static scrFloor AddFloor(float x, float y, Transform parent = null) {
        scrFloor obj = Object.Instantiate(PrefabLibrary.instance.scnLevelSelectFloorPrefab, parent);
        obj.transform.position = new Vector3(x, y);
        return obj;
    }

    public static scrFloor AddEventFloor(float x, float y, QuickAction action, Transform parent = null) {
        scrFloor obj = CreateGem(parent);
        obj.transform.position = new Vector3(x, y);
        ffxCallFunction func = obj.gameObject.AddComponent<ffxCallFunction>();
        func.ue = new QuickEvent {
            persistentCalls = new QuickPersistentCallGroup()
        };
        func.ue.AddListener(action);
        return obj;
    }

    public static scrFloor CreateGem(Transform parent = null) {
        scrFloor floor = Object.Instantiate(GetGemTransform(), parent).GetComponent<scrFloor>();
        Object.DestroyImmediate(floor.GetComponent<scrGem>());
        Object.DestroyImmediate(floor.GetComponent<scrDisableIfWorldNotComplete>());
        Object.DestroyImmediate(floor.GetComponent<ffxCallFunction>());
        floor.DOKill(true);
        floor.gameObject.SetActive(false);
        scrGem gem = floor.gameObject.AddComponent<scrGem>();
        gem.Invoke("LocalRotate");
        Object.DestroyImmediate(gem);
        floor.gameObject.SetActive(true);
        return floor;
    }
}