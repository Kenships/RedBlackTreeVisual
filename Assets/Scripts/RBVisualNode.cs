using System;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer))]
public class RBVisualNode : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform lineAnchor;
    [SerializeField] private TextMeshProUGUI x;
    
    [Header("Style")]
    [SerializeField] private float defaultFontSize = 20f;
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private float lineWidth = 0.1f;
    
    [Header("Debug Values")]
    [field: SerializeField] public int Key{get; private set;}
    [field: SerializeField] public bool IsNil { get; private set; }
    
    private SpriteRenderer _spriteRenderer;
    private RedBlackRenderer _redBlackRenderer;
    
    private Vector3 _size;
    private bool _moveStarted;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        //Initialize Line Values
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        
    }

    public void Init(bool isRed, int key, float size, RedBlackRenderer redBlackRenderer)
    {
        //Initialize Values
        SetColor(isRed);
        Key = key;
        _size = size * Vector3.one;
        _redBlackRenderer = redBlackRenderer;
        
        //Configure text
        valueText.text = key.ToString();
        valueText.fontSize = size * defaultFontSize;
        
        x.fontSize = size * defaultFontSize;
        
        transform.localScale = new Vector3(size, size, size);
        
    }

    public void SetColor(bool isRed)
    {
        Color target = isRed ? Color.red : Color.black;

        if (_spriteRenderer.color == target)
        {
            return;
        }

        Tween.Color(
            target: _spriteRenderer,
            endValue: target,
            duration: animationDuration,
            ease: Ease.InOutExpo
        );
    }

    public void GoTo(Vector3 pos)
    {
        if (transform.position == pos)
        {
            return;
        }

        _moveStarted = true;
        Tween.Position(
            target: transform,
            endValue: pos,
            duration: animationDuration,
            ease: Ease.InOutExpo
        );
    }

    public void SetParentPos(Vector3 pos)
    {
        if (lineAnchor.position == pos && !_moveStarted)
        {
            lineAnchor.position = pos;
            return;
        }

        _moveStarted = false;
        Tween.Position(
            target: lineAnchor,
            endValue: pos,
            duration: animationDuration,
            ease: Ease.InOutExpo,
            startDelay: 0.05f
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
            endValue: _size,
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
            endValue: _size * 1.05f,
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
}
