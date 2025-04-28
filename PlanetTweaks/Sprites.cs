using Ookii.Dialogs;
using PlanetTweaks.Components;
using PlanetTweaks.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;

namespace PlanetTweaks;

public static class Sprites {
    public static IndexedDictionary<string, object> sprites = new();
    private static VistaOpenFileDialog fileDialog;
    private static VistaFolderBrowserDialog dirDialog;

    public static string GetPath() => Path.Combine(Main.instance.Path, "sprites");

    public static SpriteRenderer GetOrAddRenderer(this scrPlanet planet) {
        if(!planet) return null;
        SpriteRenderer renderer = planet.transform.Find("PlanetTweaksRenderer")?.GetComponent<SpriteRenderer>();
        if(!renderer) {
            GameObject obj = new("PlanetTweaksRenderer");
            obj.AddComponent<RendererController>();
            renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = planet.planetRenderer.GetComponent<SpriteRenderer>().sortingOrder + 1;
            renderer.sortingLayerID = planet.planetRenderer.faceDetails.sortingLayerID;
            renderer.sortingLayerName = planet.planetRenderer.faceDetails.sortingLayerName;
            renderer.transform.SetParent(planet.transform);
            renderer.transform.position = planet.transform.position;
            Apply(renderer, planet.isRed ? RedSprite : (!planet.isExtra ? BlueSprite : ThirdSprite));
        }
        return renderer;
    }

    public static int Size => 100;
    public static float FSize => Size;

    private static object redPreview;

    public static object RedPreview {
        get => redPreview;

        set {
            redPreview = value;
            scrPlanet planet = scrController.instance?.planetarySystem?.planetRed;
            if(!planet) return;
            Apply(planet, value ?? RedSprite);
        }
    }

    private static object bluePreview;

    public static object BluePreview {
        get => bluePreview;

        set {
            bluePreview = value;
            scrPlanet planet = scrController.instance?.planetarySystem?.planetBlue;
            if(!planet) return;
            Apply(planet, value ?? BlueSprite);
        }
    }

    private static object thirdPreview;

    public static object ThirdPreview {
        get { return thirdPreview; }

        set {
            thirdPreview = value;
            scrPlanet planet = scrController.instance?.planetarySystem?.planetGreen;
            if(!planet) return;
            Apply(planet, value ?? ThirdSprite);
        }
    }

    public static object RedSprite { get; private set; }
    public static object BlueSprite { get; private set; }
    public static object ThirdSprite { get; private set; }

    public static int RedSelected {
        get {
            if(Main.settings.redSelected == null)
                return -1;
            if(sprites.ContainsKey(Main.settings.redSelected))
                return sprites.Keys.ToList().IndexOf(Main.settings.redSelected);
            Main.settings.redSelected = null;
            return -1;
        }

        set {
            var planet = scrController.instance?.planetarySystem?.planetRed;

            if(value < 0) {
                Main.settings.redSelected = null;
                RedSprite = null;
                Apply(planet, null);
                return;
            }
            if(value >= sprites.Count) return;
            KeyValuePair<string, object> pair = sprites.ElementAt(value);
            Main.settings.redSelected = pair.Key;
            RedSprite = pair.Value;

            if(!planet) return;
            Apply(planet, RedSprite);
        }
    }

    public static int BlueSelected {
        get {
            if(Main.settings.blueSelected == null)
                return -1;
            if(sprites.ContainsKey(Main.settings.blueSelected))
                return sprites.Keys.ToList().IndexOf(Main.settings.blueSelected);
            Main.settings.blueSelected = null;
            return -1;
        }

        set {
            scrPlanet planet = scrController.instance?.planetarySystem?.planetBlue;
            if(value < 0) {
                Main.settings.blueSelected = null;
                BlueSprite = null;
                Apply(planet, null);
                return;
            }
            if(value >= sprites.Count) return;
            KeyValuePair<string, object> pair = sprites.ElementAt(value);
            Main.settings.blueSelected = pair.Key;
            BlueSprite = pair.Value;

            if(!planet) return;
            Apply(planet, BlueSprite);
        }
    }

    public static int ThirdSelected {
        get {
            if(Main.settings.thirdSelected == null)
                return -1;
            if(sprites.ContainsKey(Main.settings.thirdSelected))
                return sprites.Keys.ToList().IndexOf(Main.settings.thirdSelected);
            Main.settings.thirdSelected = null;
            return -1;
        }

        set {
            scrPlanet planet = scrController.instance?.planetarySystem?.planetGreen;
            if(value < 0) {
                Main.settings.thirdSelected = null;
                ThirdSprite = null;
                Apply(planet, null);
                return;
            }
            if(value >= sprites.Count) return;
            KeyValuePair<string, object> pair = sprites.ElementAt(value);
            Main.settings.thirdSelected = pair.Key;
            ThirdSprite = pair.Value;

            if(!planet) return;
            Apply(planet, ThirdSprite);
        }
    }

    public static void Apply(scrPlanet planet, object v) {
        SpriteRenderer renderer = planet.GetOrAddRenderer();
        renderer.transform.position = planet.transform.position;
        Apply(renderer, v);
    }

    public static void Apply(SpriteRenderer renderer, object v) {
        renderer.enabled = true;
        UnityEngine.Object.DestroyImmediate(renderer.GetComponent<GifRenderer>());
        if(v is Sprite spr) {
            renderer.sprite = spr;
        } else if(v is GifImage gif) {
            renderer.sprite = gif.Thumbnail;
            GifRenderer gifRenderer = renderer.gameObject.AddComponent<GifRenderer>();
            gifRenderer.Image = gif;
            gifRenderer.Renderer = renderer;
        } else if(v == null) {
            renderer.sprite = null;
        }
    }

    public static void Init() {
        fileDialog = new VistaOpenFileDialog();
        fileDialog.Multiselect = false;

        dirDialog = new VistaFolderBrowserDialog();
        dirDialog.SelectedPath = GetPath();
    }

    public static string ShowOpenFileDialog() {
        DialogResult result = fileDialog.ShowDialog();
        if(result == DialogResult.OK) {
            try {
                return fileDialog.FileName;
            } catch (Exception e) {
                Main.instance.Log(e.StackTrace);
            }
        }
        return null;
    }

    public static string ShowFolderBrowserDialog() {
        DialogResult result = dirDialog.ShowDialog();
        if(result == DialogResult.OK) {
            try {
                return dirDialog.SelectedPath;
            } catch (Exception e) {
                Main.instance.Log(e.StackTrace);
            }
        }
        return null;
    }

    public static void Load() {
        DirectoryInfo dir = GetPath().CreateIfNotExists();
        foreach(FileInfo file in dir.GetFiles()) {
            try {
                Add(Path.Combine(dir.FullName, file.Name));
            } catch (Exception) { }
        }
    }

    public static void Add(string fileName) {
        if(!File.Exists(fileName))
            throw new ArgumentException("file doesn't exists!");
        string name = new FileInfo(fileName).Name;
        name = name.Substring(0, name.LastIndexOf('.'));
        string first = name;
        for(int i = 1; sprites.ContainsKey(name); i++)
            name = first + i;
        if(fileName.EndsWith(".gif")) {
            sprites.Add(name, new GifImage(fileName));
        } else if(fileName.EndsWith(".gifmeta")) {
            sprites.Add(name, GifImage.Load(fileName));
        } else {
            Sprite sprite = File.ReadAllBytes(fileName).ToSprite();
            sprite.name = name;
            sprites.Add(name, sprite);
        }
    }

    public static bool Remove(string name) {
        if(!sprites.ContainsKey(name))
            return false;
        if(Main.settings.redSelected == name)
            RedSelected = -1;
        if(Main.settings.blueSelected == name)
            BlueSelected = -1;
        if(Main.settings.thirdSelected == name)
            ThirdSelected = -1;
        sprites.Remove(name);
        return true;
    }

    public static bool Remove(int index) {
        if(index < 0 || index >= sprites.Count())
            return false;
        return Remove(sprites.Keys.ElementAt(index));
    }

    public static Texture2D ResizeFix(this Texture2D texture) {
        int targetWidth
            = texture.width >= texture.height
                  ? Size
                  : (int) (FSize / texture.height * texture.width);
        int targetHeight
            = texture.width >= texture.height
                  ? (int) (FSize / texture.width * texture.height)
                  : Size;
        Texture2D result = new Texture2D(targetWidth, targetHeight, texture.format, true);
        Color[] pixels = result.GetPixels(0);
        float incX = 1.0f / targetWidth;
        float incY = 1.0f / targetHeight;
        for(int pixel = 0; pixel < pixels.Length; pixel++) {
            pixels[pixel] = texture.GetPixelBilinear(incX * ((float) pixel % targetWidth), incY * ((float) Mathf.Floor(pixel / targetWidth)));
        }
        result.SetPixels(pixels, 0);
        result.Apply();
        return result;
    }

    public static DirectoryInfo CreateIfNotExists(this string dirName) {
        if(!Directory.Exists(dirName))
            return Directory.CreateDirectory(dirName);
        return new DirectoryInfo(dirName);
    }

    public static Sprite ToSprite(this byte[] data) {
        Texture2D texture = new Texture2D(0, 0);
        if(texture.LoadImage(data)) {
            texture = texture.ResizeFix();
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
        }
        return null;
    }

    public static void Dispose() {
        sprites = new IndexedDictionary<string, object>();
        fileDialog.Dispose();
        fileDialog = null;
        dirDialog.Dispose();
        dirDialog = null;
    }
}