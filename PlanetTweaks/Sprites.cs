using Ookii.Dialogs;
using PlanetTweaks.Components;
using PlanetTweaks.Utils;
using System;
using System.Collections.Generic;
using System.IO;
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
        SpriteRenderer renderer = PlanetTweaksPlanetController.GetInstance().GetPlanetRenderer(planet);
        return renderer ?? planet.AddRenderer();
    }

    public static SpriteRenderer AddRenderer(this scrPlanet planet) {
        SpriteRenderer renderer = new GameObject("PlanetTweaksRenderer") {
            transform = {
                parent = planet.transform,
                position = planet.transform.position
            }
        }.AddComponent<RendererController>().renderer;
        Apply(renderer, planet.isRed    ? RedSprite :
                        !planet.isExtra ? BlueSprite : ThirdSprite);
        return renderer;
    }

    public static int Size => 100;
    public static float FSize => Size;

    private static object redPreview;

    public static object RedPreview {
        get => redPreview;

        set {
            redPreview = value;
            scrPlanet planet = scrController.instance?.planetRed;
            if(!planet) return;
            Apply(planet, value ?? RedSprite);
        }
    }

    private static object bluePreview;

    public static object BluePreview {
        get => bluePreview;

        set {
            bluePreview = value;
            scrPlanet planet = scrController.instance?.planetBlue;
            if(!planet) return;
            Apply(planet, value ?? BlueSprite);
        }
    }

    private static object thirdPreview;

    public static object ThirdPreview {
        get => thirdPreview;

        set {
            thirdPreview = value;
            scrPlanet planet = scrController.instance?.planetGreen;
            if(!planet) return;
            Apply(planet, value ?? ThirdSprite);
        }
    }

    public static object RedSprite { get; private set; }
    public static object BlueSprite { get; private set; }
    public static object ThirdSprite { get; private set; }

    public static int RedSelected {
        get {
            if(Main.settings.redSelected == null) return -1;
            if(sprites.ContainsKey(Main.settings.redSelected)) return sprites.Keys.IndexOf(Main.settings.redSelected);
            Main.settings.redSelected = null;
            return -1;
        }

        set {
            scrPlanet planet = scrController.instance?.planetRed;

            if(value < 0) {
                Main.settings.redSelected = null;
                RedSprite = null;
                Apply(planet, null);
                return;
            }
            if(value >= sprites.Count) return;
            KeyValuePair<string, object> pair = sprites.ElementAt(value);
            if(Main.settings.redSelected != pair.Key) {
                Main.settings.redSelected = pair.Key;
                Main.instance.SaveSetting();
            }
            RedSprite = pair.Value;

            if(!planet) return;
            Apply(planet, RedSprite);
        }
    }

    public static int BlueSelected {
        get {
            if(Main.settings.blueSelected == null) return -1;
            if(sprites.ContainsKey(Main.settings.blueSelected)) return sprites.Keys.IndexOf(Main.settings.blueSelected);
            Main.settings.blueSelected = null;
            return -1;
        }

        set {
            scrPlanet planet = scrController.instance?.planetBlue;
            if(value < 0) {
                Main.settings.blueSelected = null;
                BlueSprite = null;
                Apply(planet, null);
                return;
            }
            if(value >= sprites.Count) return;
            KeyValuePair<string, object> pair = sprites.ElementAt(value);
            if(Main.settings.blueSelected != pair.Key) {
                Main.settings.blueSelected = pair.Key;
                Main.instance.SaveSetting();
            }
            BlueSprite = pair.Value;

            if(!planet) return;
            Apply(planet, BlueSprite);
        }
    }

    public static int ThirdSelected {
        get {
            if(Main.settings.thirdSelected == null) return -1;
            if(sprites.ContainsKey(Main.settings.thirdSelected)) return sprites.Keys.IndexOf(Main.settings.thirdSelected);
            Main.settings.thirdSelected = null;
            return -1;
        }

        set {
            scrPlanet planet = scrController.instance?.planetGreen;
            if(value < 0) {
                Main.settings.thirdSelected = null;
                ThirdSprite = null;
                Apply(planet, null);
                return;
            }
            if(value >= sprites.Count) return;
            KeyValuePair<string, object> pair = sprites.ElementAt(value);
            if(Main.settings.thirdSelected != pair.Key) {
                Main.settings.thirdSelected = pair.Key;
                Main.instance.SaveSetting();
            }
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
        if(v is GifImage gif) {
            renderer.sprite = gif.Thumbnail;
            renderer.gameObject.GetOrAddComponent<GifRenderer>().SetData(gif, renderer);
        } else {
            UnityEngine.Object.DestroyImmediate(renderer.GetComponent<GifRenderer>());
            renderer.sprite = v as Sprite;
        }
    }

    public static void Init() {
        DirectoryInfo dir = GetPath().CreateIfNotExists();
        foreach(FileInfo file in dir.GetFiles()) {
            try {
                Add(Path.Combine(dir.FullName, file.Name));
            } catch (Exception e) {
                Main.instance.Log("Failed to load sprite: " + file.Name);
                Main.instance.NativeLog(e);
            }
        }
        
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
                Main.instance.NativeLog(e.StackTrace);
            }
        }
        return null;
    }

    public static void Add(string fileName) {
        if(!File.Exists(fileName))
            throw new ArgumentException("file doesn't exists!");
        string name = Path.GetFileNameWithoutExtension(fileName);
        string defaultFolder = GetPath();
        if(defaultFolder != Path.GetDirectoryName(fileName)) {
            string first = name;
            for(int i = 1; sprites.ContainsKey(name); i++) name = first + i;
            File.Copy(fileName, Path.Combine(defaultFolder, name + Path.GetExtension(fileName)));
        }
        if(fileName.EndsWith(".gif")) sprites.Add(name, new GifImage(fileName));
        else if(fileName.EndsWith(".gifmeta")) sprites.Add(name, GifImage.Load(fileName));
        else {
            Sprite sprite = File.ReadAllBytes(fileName).ToSprite();
            sprite.name = name;
            sprites.Add(name, sprite);
        }
    }

    public static void Remove(string name) {
        if(!sprites.ContainsKey(name)) return;
        if(Main.settings.redSelected == name) RedSelected = -1;
        if(Main.settings.blueSelected == name) BlueSelected = -1;
        if(Main.settings.thirdSelected == name) ThirdSelected = -1;
        sprites.Remove(name);
    }

    public static void Remove(int index) {
        if(index < 0 || index >= sprites.Count) return;
        Remove(sprites.Keys[index]);
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
        Texture2D result = new(targetWidth, targetHeight, texture.format, true);
        Color[] pixels = result.GetPixels(0);
        float incX = 1.0f / targetWidth;
        float incY = 1.0f / targetHeight;
        for(int pixel = 0; pixel < pixels.Length; pixel++)
            pixels[pixel] = texture.GetPixelBilinear(incX * (pixel % targetWidth), incY * Mathf.Floor((float) pixel / targetWidth));
        result.SetPixels(pixels, 0);
        result.Apply();
        return result;
    }

    public static DirectoryInfo CreateIfNotExists(this string dirName) {
        return !Directory.Exists(dirName) ? Directory.CreateDirectory(dirName) : new DirectoryInfo(dirName);
    }

    public static Sprite ToSprite(this byte[] data) {
        Texture2D texture = new(0, 0);
        if(!texture.LoadImage(data)) return null;
        texture = texture.ResizeFix();
        Rect rect = new(0, 0, texture.width, texture.height);
        return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
    }

    public static void Dispose() {
        sprites = new IndexedDictionary<string, object>();
        fileDialog.Dispose();
        fileDialog = null;
        dirDialog.Dispose();
        dirDialog = null;
    }
}