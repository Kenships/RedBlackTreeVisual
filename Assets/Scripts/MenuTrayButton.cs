using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class MenuTrayButton : MonoBehaviour
{
    [SerializeField] private RectTransform tray;
    [SerializeField] private RectTransform inPos;
    [SerializeField] private RectTransform outPos;
    [SerializeField] private Button[] trayButtons;
    [SerializeField] private float duration = .5f;
    
    private bool _isHidden;
    
    public void ToggleTray()
    {
        Tween.UIAnchoredPosition(
            target: tray,
            endValue: _isHidden ? inPos.anchoredPosition : outPos.anchoredPosition,
            duration: duration,
            ease: Ease.InOutExpo
        );
        
        _isHidden = !_isHidden;

        if (_isHidden)
        {
            foreach (var button in trayButtons)
            {
                button.interactable = false;
            }
        }
        else
        {
            foreach (var button in trayButtons)
            {
                button.interactable = true;
            }
        }
    }
}
