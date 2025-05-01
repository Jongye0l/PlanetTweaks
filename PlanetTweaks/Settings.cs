using JALib.Core;
using JALib.Core.Setting;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace PlanetTweaks;

public class Settings(JAMod mod, JObject jsonObject = null) : JASetting(mod, jsonObject) {
    public static readonly Color DefaultThirdColor = new(0.2980392156862745f, 0.6980392156862745f, 0);
    public string redSelected = null;
    public string blueSelected = null;
    public float redSize = 1;
    public float blueSize = 1;
    public bool redColor = false;
    public bool blueColor = false;
    public bool shapedRotation = false;
    public int shapedAngle = 4;
    public string thirdSelected = null;
    public float thirdSize = 1;
    public bool thirdColor = false;
    public bool thirdPlanet = false;
    public int thirdColorType;
    public Color thirdColorCustom = DefaultThirdColor;
}