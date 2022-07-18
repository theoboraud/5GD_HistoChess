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
    private Unit _unit;

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
    public void InitUnit(Unit unit)
    {
        _unit = unit;

        for (int i = 0; i < _unit.traits.Count; i++)
        {
            Trait trait = _unit.traits[i];

            _traitTooltips[i].InitTrait(trait);
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void EnableTooltip(bool enable)
    {
        for (int i = 0; i < _unit.traits.Count; i++)
        {
            _traitTooltips[i].gameObject.SetActive(enable);
        }
    }
}
