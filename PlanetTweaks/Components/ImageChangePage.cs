using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ByteSheep.Events;
using DG.Tweening;
using JALib.Core;
using JALib.Tools;
using PlanetTweaks.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityModManagerNet;

namespace PlanetTweaks.Components;

public class ImageChangePage : MonoBehaviour {
    public static ImageChangePage instance;

    public static Sprite pageBtnNormal;
    public static Sprite pageBtnEntered;
    public static Sprite pageBtnDisabled;
    private const int Size = 50;

    private static bool NeedColor(int i, int j) => j >= Size / 2 - 1 - i / 2 && j <= Size / 2 + i / 2;

    public static void Init() {
        // 페이지 버튼 그리기
        {
            Color lightGray = new(0.9f, 0.9f, 0.9f);
            Texture2D texture = new(Size, Size);
            for(int i = 0; i < Size; i++) for(int j = 0; j < Size; j++) texture.SetPixel(j, i, NeedColor(i, j) ? Color.white : Color.clear);
            texture.Apply();
            pageBtnNormal = Sprite.Create(texture, new Rect(0, 0, Size, Size), new Vector2(0.5f, 0.5f));
            texture = new Texture2D(Size, Size);
            for(int i = 0; i < Size; i++) for(int j = 0; j < Size; j++) texture.SetPixel(j, i, NeedColor(i, j) ? lightGray : Color.clear);
            texture.Apply();
            pageBtnEntered = Sprite.Create(texture, new Rect(0, 0, Size, Size), new Vector2(0.5f, 0.5f));
            texture = new Texture2D(Size, Size);
            for(int i = 0; i < Size; i++) for(int j = 0; j < Size; j++) texture.SetPixel(j, i, NeedColor(i, j) ? Color.gray : Color.clear);
            texture.Apply();
            pageBtnDisabled = Sprite.Create(texture, new Rect(0, 0, Size, Size), new Vector2(0.5f, 0.5f));
        }
        // 왼쪽 페이지 버튼
        events.Add(new Rect(18, 129, 43, 44), new ButtonEvent(
            delegate {
                if(instance.page == 0) return;
                instance.leftPageBtn.sprite = pageBtnEntered;
            },
            delegate {
                if(instance.page == 0) return;
                instance.leftPageBtn.sprite = pageBtnNormal;
            },
            delegate {
                if(instance.page == 0) return;
                instance.ChangePage(instance.page - 1);
            }));
        // 오른쪽 페이지 버튼
        events.Add(new Rect(1232, 129, 43, 44), new ButtonEvent(
            delegate {
                instance.rightPageBtn.sprite = pageBtnEntered;
            },
            delegate {
                instance.rightPageBtn.sprite = pageBtnNormal;
            },
            delegate {
                instance.ChangePage(instance.page + 1);
            }));
        // 메인메뉴로 나가기
        events.Add(new Rect(1920 - 400, 1080 - 150, 400, 150), new ButtonEvent(
            delegate {
                scrFloor floor = PlanetTweakFloorController.instance.exitFloor;
                floor.DOKill();
                floor.transform.DOScale(new Vector3(0.55f, 0.55f), 0.5f);
            },
            delegate {
                scrFloor floor = PlanetTweakFloorController.instance.exitFloor;
                floor.DOKill();
                floor.transform.DOScale(new Vector3(0.5f, 0.5f), 0.5f);
            },
            delegate {
                scrController.instance.SetValue("exitingToMainMenu", true);
                Destroy();
                GCS.sceneToLoad = GCNS.sceneLevelSelect;
                scrUIController.instance.WipeToBlack(WipeDirection.StartsFromRight, delegate {
                    DOTween.KillAll();
                    SceneManager.LoadScene("scnLoading");
                });
            }));
        // 공 바꾸기
        events.Add(new Rect(1920 - 615, 1080 - 950, 600, 600), new ButtonEvent(
            delegate {
                PlanetTweakFloorController.instance.planetFloor.transform.DOScale(new Vector3(1, 1), 0.5f);
            },
            delegate {
                PlanetTweakFloorController.instance.planetFloor.transform.DOScale(new Vector3(0.8f, 0.8f), 0.5f);
            },
            delegate {
                instance.changing = true;
                scrUIController.instance.WipeToBlack(WipeDirection.StartsFromRight, delegate {
                    instance.changing = false;
                    PlanetarySystem planetarySystem = scrController.instance.planetarySystem;
                    planetarySystem.chosenPlanet = scrController.instance.planetarySystem.chosenPlanet.next;
                    planetarySystem.chosenPlanet.transform.LocalMoveXY(-15, -3);
                    planetarySystem.chosenPlanet.transform.position = new Vector3(-15, -3);
                    JALocalization localization = Main.instance.Localization;
                    instance.planetText.text = (planetarySystem.chosenPlanet.isRed   ? $"<color={ColorUtils.GetRealColor(true).Hex()}>{localization["Setting.RedColor"]}</color>" :
                                                !planetarySystem.chosenPlanet.isExtra ? $"<color={ColorUtils.GetRealColor(false).Hex()}>{localization["Setting.BlueColor"]}</color>" :
                                                                                       $"<color={ColorUtils.GetRealThirdColor().Hex()}>{localization["Setting.ThirdColor"]}</color>")
                        + ' ' + localization["ChangePage.Selected"];
                    instance.UpdateFloorIcons();
                    scrUIController.instance.WipeFromBlack();
                });
            }));
        // 이미지 타일들
        for(int i = 0; i < 6; i++) {
            for(int j = 0; j < 4; j++) {
                int copyI = i;
                int copyJ = j;
                if(i == 5 && j == 3)
                    events.Add(new Rect(79 + i * 194 + (i > 1 ? i > 4 ? 2 : 1 : 0), 112 + j * 238 + (j > 1 ? -1 : 0), 164, 164), new ButtonEvent(
                        delegate {
                            scrFloor floor = PlanetTweakFloorController.instance.floors[copyJ * 6 + copyI].floor;
                            floor.transform.DOKill();
                            floor.transform.DOScale(new Vector3(0.9f, 0.9f), 0.5f);
                        },
                        delegate {
                            scrFloor floor = PlanetTweakFloorController.instance.floors[copyJ * 6 + copyI].floor;
                            floor.transform.DOKill();
                            floor.transform.DOScale(new Vector3(0.8f, 0.8f), 0.5f);
                        },
                        delegate {
                            string file = Sprites.ShowOpenFileDialog();
                            try {
                                Sprites.Add(file);
                                instance.UpdateFloorIcons();
                            } catch (Exception e) {
                                Main.instance.Log("wrong file '" + file + "'!");
                                Main.instance.Log(e.StackTrace);
                            }
                        }));
                else {
                    events.Add(new Rect(79 + i * 194 + (i > 1 ? i > 4 ? 2 : 1 : 0), 112 + j * 238 + (j > 1 ? -1 : 0), 164, 164), new ButtonEvent(
                        delegate {
                            PlanetSettingFloor settingFloor = PlanetTweakFloorController.instance.floors[copyJ * 6 + copyI];
                            scrFloor floor = settingFloor.floor;
                            SpriteRenderer sprite = settingFloor.icon;
                            if(!sprite.sprite) return;
                            floor.transform.DOKill();
                            floor.transform.DOScale(new Vector3(1, 1), 0.5f).OnComplete(delegate {
                                int index = instance.page * 23 + copyJ * 6 + copyI;
                                if((scrController.instance.planetarySystem.chosenPlanet.isRed ? Sprites.RedSelected :
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
                        },
                        delegate {
                            PlanetSettingFloor settingFloor = PlanetTweakFloorController.instance.floors[copyJ * 6 + copyI];
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
                        },
                        delegate {
                            PlanetSettingFloor settingFloor = PlanetTweakFloorController.instance.floors[copyJ * 6 + copyI];
                            scrFloor floor = settingFloor.floor;
                            SpriteRenderer sprite = settingFloor.icon;
                            if(!sprite.sprite) return;
                            floor.transform.DOComplete();
                            settingFloor.preview.gameObject.SetActive(false);
                            sprite.transform.DOComplete();
                            int index = instance.page * 23 + copyJ * 6 + copyI;
                            if(Input.GetMouseButtonUp(0))
                                if((scrController.instance.planetarySystem.chosenPlanet.isRed ? Sprites.RedSelected : !scrController.instance.planetarySystem.chosenPlanet.isExtra ? Sprites.BlueSelected : Sprites.ThirdSelected) == index) {
                                    if(scrController.instance.planetarySystem.chosenPlanet.isRed) Sprites.RedSelected = -1;
                                    else if(!scrController.instance.planetarySystem.chosenPlanet.isExtra) Sprites.BlueSelected = -1;
                                    else Sprites.ThirdSelected = -1;
                                } else {
                                    if(scrController.instance.planetarySystem.chosenPlanet.isRed) Sprites.RedSelected = index;
                                    else if(!scrController.instance.planetarySystem.chosenPlanet.isExtra) Sprites.BlueSelected = index;
                                    else Sprites.ThirdSelected = index;
                                }
                            else if(Input.GetMouseButtonUp(1)) {
                                Sprites.Remove(index);
                                if(scrController.instance.planetarySystem.chosenPlanet.isRed) Sprites.RedPreview = null;
                                else if(!scrController.instance.planetarySystem.chosenPlanet.isExtra) Sprites.BluePreview = null;
                                else Sprites.ThirdPreview = null;
                            } else return;
                            instance.UpdateFloorIcons();
                        }));

                    events.Add(new Rect(79 + i * 194 + (i > 1 ? i > 4 ? 2 : 1 : 0), 112 + j * 238 + (j > 1 ? -1 : 0) + 168, 164, 40), new ButtonEvent(
                        delegate {
                            TextMesh text = PlanetTweakFloorController.instance.floors[copyJ * 6 + copyI].nameText;
                            text.DOKill();
                            DOTween.To(() => text.color, c => text.color = c, Color.yellow, 0.5f).SetTarget(text);
                        },
                        delegate {
                            TextMesh text = PlanetTweakFloorController.instance.floors[copyJ * 6 + copyI].nameText;
                            text.DOKill();
                            DOTween.To(() => text.color, c => text.color = c, Color.white, 0.5f).SetTarget(text);
                        },
                        delegate {
                            TextMesh text = PlanetTweakFloorController.instance.floors[copyJ * 6 + copyI].nameText;
                            int index = instance.page * 23 + copyJ * 6 + copyI;
                            if(index >= Sprites.sprites.Count) return;
                            instance.input = true;
                            RenameInputField.instance.Show(text.text, s => {
                                instance.input = false;
                                if(s.Trim().IsNullOrEmpty() || s == text.text) return;
                                s = Path.GetInvalidPathChars().Aggregate(s, (cur, c) => cur.Replace(c.ToString(), "_"));
                                string first = s;
                                for(int k = 1; Sprites.sprites.ContainsKey(s); k++) s = first + k;
                                if(index == Sprites.RedSelected) Main.settings.redSelected = s;
                                if(index == Sprites.BlueSelected) Main.settings.blueSelected = s;
                                text.text = s;
                                Sprites.sprites.Replace(index, s, Sprites.sprites.ElementAt(index).Value);
                            });
                        }));
                }
            }
        }
    }

    public static Color floorColor = new(0.78f, 0.78f, 0.886f);

    public void UpdateFloorIcons() {
        PlanetSettingFloor[] settingFloors = PlanetTweakFloorController.instance.floors;
        for(int i = 0; i < settingFloors.Length - 1; i++) {
            PlanetSettingFloor cur = PlanetTweakFloorController.instance.floors[i];
            Sprites.Apply(cur.icon, null);
            cur.nameText.text = null;
            cur.floor.SetTileColor((scrController.instance.planetarySystem.chosenPlanet.isRed ? Sprites.RedSelected :
                                    !scrController.instance.planetarySystem.chosenPlanet.isExtra ? Sprites.BlueSelected : Sprites.ThirdSelected) == i + page * 23 ? Color.yellow : floorColor);
        }
        for(int i = 0; i < settingFloors.Length - 1; i++) {
            if(i + page * 23 >= Sprites.sprites.Count) break;
            KeyValuePair<string, object> pair = Sprites.sprites.ElementAt(i + page * 23);
            PlanetSettingFloor cur = PlanetTweakFloorController.instance.floors[i];
            Sprites.Apply(cur.icon, pair.Value);
            cur.nameText.text = pair.Key;
        }
        pageText.text = string.Format(Main.instance.Localization["ChangePage.Page"], page + 1);
        leftPageBtn.sprite = page == 0 ? pageBtnDisabled : new Rect(18, 86, 43, 44).Contains(Event.current.mousePosition) ? pageBtnEntered : pageBtnNormal;
    }

    public void ChangePage(int page) {
        if(page == this.page) return;
        this.page = page;
        changing = true;
        scrFloor floor = null;
        Tween fade = null;
        for(int i = 0; i < 6; i++)
            for(int j = 0; j < 4; j++) {
                if(i == 5 && j == 3) {
                    fade.OnComplete(delegate {
                        UpdateFloorIcons();
                        for(i = 0; i < 6; i++)
                            for(j = 0; j < 4; j++) {
                                if(i == 5 && j == 3) {
                                    fade.OnComplete(delegate {
                                        changing = false;
                                    });
                                    break;
                                }
                                PlanetSettingFloor settingFloor = PlanetTweakFloorController.instance.floors[j * 6 + i];
                                floor = settingFloor.floor;
                                FloorRenderer fr2 = floor.floorRenderer;
                                fr2.color = fr2.color.WithAlpha(0);
                                fade = DOTween.To(() => fr2.color, c => fr2.color = c, fr2.color.WithAlpha(1), 0.5f);
                                settingFloor.nameRenderer.material.DOFade(1, 0.5f);
                                settingFloor.icon.material.DOFade(1, 0.5f);
                            }
                    });
                    break;
                }
                PlanetSettingFloor settingFloor = PlanetTweakFloorController.instance.floors[j * 6 + i];
                floor = settingFloor.floor;
                FloorRenderer fr = floor.floorRenderer;
                fade = DOTween.To(() => fr.color, c => fr.color = c, fr.color.WithAlpha(0), 0.5f);
                settingFloor.nameRenderer.material.DOFade(0, 0.5f);
                settingFloor.icon.material.DOFade(0, 0.5f);
            }
    }

    //private bool propertyPage = false;
    private bool input;
    private bool changing;

    private int page;

    public TextMesh planetText;
    public TextMesh pageText;
    public SpriteRenderer leftPageBtn;
    public SpriteRenderer rightPageBtn;

    public void Awake() {
        if(instance) Destroy(instance.gameObject);
        instance = this;
        JALocalization localization = Main.instance.Localization;
        planetText = new GameObject().AddComponent<TextMesh>();
        planetText.richText = true;
        planetText.text = $"<color={ColorUtils.GetRealColor(true).Hex()}>{localization["Setting.RedColor"]}</color> {localization["ChangePage.Selected"]}";
        planetText.SetLocalizedFont();
        planetText.fontSize = 100;
        planetText.anchor = TextAnchor.UpperCenter;
        planetText.transform.position = new Vector3(-15.05f, -4.25f);
        planetText.transform.ScaleXY(0.045f, 0.045f);
        TextMesh exit = new GameObject().AddComponent<TextMesh>();
        exit.text = localization["ChangePage.Exit"];
        exit.SetLocalizedFont();
        exit.fontSize = 100;
        exit.transform.position = new Vector3(-15.2f, -5.29f);
        exit.transform.ScaleXY(0.05f, 0.05f);

        pageText = new GameObject().AddComponent<TextMesh>();
        pageText.text = string.Format(localization["ChangePage.Page"], 1);
        pageText.SetLocalizedFont();
        pageText.fontSize = 100;
        pageText.transform.position = new Vector3(-22, -1.23f);
        pageText.transform.ScaleXY(0.02f, 0.02f);
        leftPageBtn = new GameObject().AddComponent<SpriteRenderer>();
        leftPageBtn.sprite = pageBtnNormal;
        leftPageBtn.transform.Rotate(0, 0, -90);
        leftPageBtn.transform.position = new Vector3(-22.26f, -1.7f);
        leftPageBtn.transform.ScaleXY(0.4f, 0.4f);
        rightPageBtn = new GameObject().AddComponent<SpriteRenderer>();
        rightPageBtn.sprite = pageBtnNormal;
        rightPageBtn.transform.Rotate(0, 0, 90);
        rightPageBtn.transform.position = new Vector3(-16.64f, -1.7f);
        rightPageBtn.transform.ScaleXY(0.4f, 0.4f);
        instance.UpdateFloorIcons();
    }

    private static readonly Dictionary<Rect, ButtonEvent> events = new();

    public void OnGUI() {
        if(scrController.instance.paused) return;
        HandleButtonEvent();
    }

    public static void Destroy() {
        if(instance) Destroy(instance);
        instance = null;
    }

    public void HandleButtonEvent() {
        Vector2 mouse = Event.current.mousePosition;
        foreach(KeyValuePair<Rect, ButtonEvent> pair in events) {
            try {
                Rect rect = pair.Key.Fix();
                ButtonEvent btnEvent = pair.Value;
                if(!btnEvent.Entered && rect.Contains(mouse)) {
                    if(!input && !changing && !UnityModManager.UI.Instance.Opened) {
                        btnEvent.Entered = true;
                        btnEvent.OnEntered.Invoke();
                    }
                } else if(btnEvent.Entered && !rect.Contains(mouse)) {
                    btnEvent.Entered = false;
                    btnEvent.OnExited.Invoke();
                }
                if(!input && !changing && !UnityModManager.UI.Instance.Opened && GUI.Button(rect, "", GUIStyle.none))
                    btnEvent.OnClicked.Invoke();
            } catch (Exception) { }
        }
    }

    private class ButtonEvent {
        public bool Entered { get; set; }
        public QuickAction OnEntered { get; }
        public QuickAction OnExited { get; }
        public QuickAction OnClicked { get; }

        public ButtonEvent(QuickAction onEntered, QuickAction onExited, QuickAction onClicked) {
            Entered = false;
            OnEntered = onEntered;
            OnExited = onExited;
            OnClicked = onClicked;
        }
    }
}