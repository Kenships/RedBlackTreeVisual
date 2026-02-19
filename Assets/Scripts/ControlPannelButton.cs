using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private RectTransform tray;
    [SerializeField] private CanvasGroup trayImage;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private CanvasGroup tutorialImage;
    [SerializeField] private CanvasGroup exitImage;

    [Header("Animation Config")]
    [SerializeField] private float trayAnimationDuration = .5f;
    [SerializeField] private float buttonFadeDuration = .3f;

    private float _trayHeight;
    private float _trayWidth;

    private bool _isOpen;
    
    private void Start()
    {
        _trayHeight = tray.rect.height;
        _trayWidth = tray.rect.width;
        
        tray.sizeDelta = new Vector2(_trayWidth, _trayWidth);
        trayImage.alpha = 0f;

        tutorialButton.interactable = false;
        exitButton.interactable = false;
        
        tutorialImage.alpha = 0f;
        exitImage.alpha = 0f;
    }

    public void Toggle()
    {
        if (_isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    private void Open()
    {
        _isOpen = true;
        Tween.UISizeDelta(
            target: tray,
            endValue: new Vector2(_trayWidth, _trayHeight),
            duration: trayAnimationDuration,
            Ease.InOutExpo
        ).Group(
            Tween.Alpha(
                target: trayImage,
                endValue: 1f,
                duration: trayAnimationDuration,
                Ease.InOutExpo
            )
        ).Chain(
            Tween.Alpha(
                target: tutorialImage,
                endValue: 1f,
                duration: buttonFadeDuration,
                Ease.InOutExpo
            ).Group(
                Tween.Alpha(
                    target: exitImage,
                    endValue: 1f,
                    duration: buttonFadeDuration,
                    Ease.InOutExpo
                )
            )
        ).OnComplete(() =>
                     {
                         tutorialButton.interactable = true;
                         exitButton.interactable = true;
                     });
    }

    private void Close()
    {
        _isOpen = false;
        tutorialButton.interactable = false;
        exitButton.interactable = false;
        
        Tween.Alpha(
            target: tutorialImage,
            endValue: 0f,
            duration: buttonFadeDuration,
            Ease.InOutExpo
        ).Group(
            Tween.Alpha(
                target: exitImage,
                endValue: 0f,
                duration: buttonFadeDuration,
                Ease.InOutExpo
            )
        ).Chain(
            Tween.UISizeDelta(
                target: tray,
                endValue: new Vector2(_trayWidth, _trayWidth),
                duration: trayAnimationDuration,
                Ease.InOutExpo
            ).Group(
                Tween.Alpha(
                    target: trayImage,
                    endValue: 0f,
                    duration: trayAnimationDuration,
                    Ease.InOutExpo
                )
            )
        );
    }
}
