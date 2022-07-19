using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Increase the scale when called, then decrease back, to "blip" this game object
/// </summary>
public class ScaleBlip : MonoBehaviour
{
    private float _scaleUpMax = 1.5f;
    private float _time = 4f;
    private float _maxScale;
    private float _oldScale = 1f;
    private float _currentScale = 1f;
    private bool _scaleUp = false;

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void Update()
    {
        if (_scaleUp)
        {
            if (_currentScale >= _maxScale)
            {
                _scaleUp = false;
            }
            else
            {
                //Debug.Log(_currentScale);
                _currentScale = Mathf.MoveTowards(_currentScale, _scaleUpMax, _time * Time.deltaTime);
                this.transform.localScale = new Vector3(_currentScale, _currentScale, _currentScale);
            }
        }
        else if (_currentScale != _oldScale)
        {
            _currentScale = Mathf.MoveTowards(_currentScale, _oldScale, _time * Time.deltaTime);
            this.transform.localScale = new Vector3(_currentScale, _currentScale, _currentScale);
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void StartScaleBlip()
    {
        if (!_scaleUp)// && _currentScale == _oldScale)
        {
            _oldScale = this.transform.localScale.x;
            _currentScale = this.transform.localScale.x;
            _maxScale = _scaleUpMax * _currentScale;
            _scaleUp = true;
        }
    }
}
