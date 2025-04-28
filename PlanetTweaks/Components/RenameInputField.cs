using DG.Tweening;
using JALib.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PlanetTweaks.Components;

public class RenameInputField : MonoBehaviour {
    public static RenameInputField instance;
    public Canvas canvas;
    public CanvasGroup canvasGroup;
    public InputField inputField;
    private UnityAction<string> onHide;

    private void Awake() {
        if(instance) {
            DestroyImmediate(this);
            return;
        }

        instance = this;
        canvas = GetComponentInChildren<Canvas>();
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        Text desc = canvas.GetComponentInChildren<Text>();
        inputField = canvas.GetComponentInChildren<InputField>();
        Button backgroundButton = canvas.GetComponentInChildren<Button>();

        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        JALocalization localization = Main.instance.Localization;
        desc.text = localization["InputField.Rename"];
        ((Text) inputField.placeholder).text = localization["InputField.EnterName"];
        inputField.onEndEdit.AddListener(_ => Hide());
        backgroundButton.onClick.AddListener(Hide);
    }

    public void Show(string text, UnityAction<string> hideAction) {
        canvasGroup.interactable = canvasGroup.blocksRaycasts = true;
        this.DOKill();
        DOTween.To(() => canvasGroup.alpha, a => canvasGroup.alpha = a, 1, 0.5f).SetTarget(this);
        onHide?.Invoke(inputField.text);
        inputField.text = text;
        onHide = hideAction;
    }

    public void Hide() {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        this.DOKill();
        DOTween.To(() => canvasGroup.alpha, a => canvasGroup.alpha = a, 0, 0.5f).SetTarget(this);
        onHide?.Invoke(inputField.text);
        onHide = null;
    }
}