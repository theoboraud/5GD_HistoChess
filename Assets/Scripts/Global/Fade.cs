using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Enums;

/// <summary>
///     Fade on or fade out
/// </summary>
public class Fade : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Image _image;
    private TMP_Text _text;
    private Color _color;
    [SerializeField] private float _alphaIncrement = 6f;
    [SerializeField] private float _desiredAlpha = 1f;

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Init at start
    /// </summary>
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _image = GetComponent<Image>();

        _text = GetComponent<TMP_Text>();

        if (_spriteRenderer != null)
        {
            _color = _spriteRenderer.color;
        }
        if (_image != null)
        {
            _color = _image.color;
        }
        if (_text != null)
        {
            _color = _text.color;
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    private void Update()
    {
        if (_color.a != _desiredAlpha)
        {
            _color.a = Mathf.MoveTowards(_color.a, _desiredAlpha, _alphaIncrement * Time.deltaTime);

            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = _color;
            }
            if (_image != null)
            {
                _image.color = _color;
            }
            if (_text != null)
            {
                _text.color = _color;
            }
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void Appear()
    {
        _desiredAlpha = 1f;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void Disappear()
    {
        _desiredAlpha = 0f;
    }
}
