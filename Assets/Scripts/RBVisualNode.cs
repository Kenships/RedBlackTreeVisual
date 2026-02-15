using System;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer))]
public class RBVisualNode : MonoBehaviour, IEquatable<RBVisualNode>, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float defaultFontSize = 20f;
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform lineAnchor;
    [SerializeField] private float lineWidth = 0.1f;
    [SerializeField] private TextMeshProUGUI x;
    public int Key;
    private SpriteRenderer _spriteRenderer;
    private Vector3 size;
    private RedBlackRenderer _redBlackRenderer;
    public bool IsNil;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
    }

    public void Init(bool isRed, int key, float size, RedBlackRenderer redBlackRenderer)
    {
        SetColor(isRed);
        Key = key;
        textMeshPro.text = key.ToString();
        textMeshPro.fontSize = size * defaultFontSize;
        this.size = size * Vector3.one;
        transform.localScale = new Vector3(size, size, size);
        _redBlackRenderer = redBlackRenderer;
    }

    public void SetColor(bool isRed)
    {
        Color target = isRed ? Color.red : Color.black;

        Tween.Color(
            target: _spriteRenderer,
            endValue: target,
            duration: animationDuration,
            ease: Ease.InOutExpo
        );
    }

    public void GoTo(Vector3 pos)
    {
        Tween.Position(
            target: transform,
            endValue: pos,
            duration: animationDuration,
            ease: Ease.InOutExpo
        );
    }

    public void SetParentPos(Vector3 pos)
    {
        Tween.Position(
            target: lineAnchor,
            endValue: pos,
            duration: animationDuration,
            ease: Ease.InOutExpo
        );
    }

    public void SetNil(bool isNil)
    {
        IsNil = isNil;
        x.color = IsNil ? new Color(x.color.r, x.color.g, x.color.b, 255f) : new Color(x.color.r, x.color.g, x.color.b, 0f);
    }
    

    private void Update()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, lineAnchor.position);
    }

    public bool Equals(RBVisualNode other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return base.Equals(other) && Key == other.Key;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Key);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsNil)
            return;
        
        x.color = new Color(x.color.r, x.color.g, x.color.b, 0f);
        
        Tween.Scale(
            target: transform,
            endValue: size,
            duration: 0.2f,
            ease: Ease.InOutExpo
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsNil)
            return;
        
        x.color = new Color(x.color.r, x.color.g, x.color.b, 255f);
        Tween.Scale(
            target: transform,
            endValue: size * 1.05f,
            duration: 0.2f,
            ease: Ease.InOutExpo
        );
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsNil)
            return;
        IsNil = true;
        _redBlackRenderer.Delete(Key);
    }

    public static bool operator ==(RBVisualNode left, RBVisualNode right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(RBVisualNode left, RBVisualNode right)
    {
        return !Equals(left, right);
    }
}
