using System.IO;
using System.Xml.Serialization;
using JALib.Core;
using JALib.Core.Setting;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityModManagerNet;

namespace PlanetTweaks;

public class Settings(JAMod mod, JObject jsonObject = null) : JASetting(mod, jsonObject) {
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
    public Color thirdColorCustom = new(0.3f, 0.7f, 0);
}