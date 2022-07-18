using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

/// <summary>
///     When the mouse is left a given time on an unit, the tooltip should appear
/// </summary>
public class UnitTooltip : MonoBehaviour
{

    // References
    public static UnitTooltip instance;
    [SerializeField] private List<TraitTooltip> _traitTooltips = new List<TraitTooltip>();
    private UnitReference _unitReference;

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Init instance
    /// </summary>
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }

        instance = this;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Init variables
    /// </summary>
    public void InitUnit(UnitReference unitReference)
    {
        _unitReference = unitReference;

        for (int i = 0; i < _unitReference.traits.Count; i++)
        {
            Trait trait = _unitReference.traits[i];

            _traitTooltips[i].InitTrait(trait);
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void EnableTooltip(bool enable)
    {
        if (GameManager.instance.GetPlanificationMode())
        {
            for (int i = 0; i < _unitReference.traits.Count; i++)
            {
                _traitTooltips[i].gameObject.SetActive(enable);
            }
        }
    }
}
