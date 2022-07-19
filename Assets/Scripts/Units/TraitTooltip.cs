using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Enums;

/// <summary>
///     When the mouse is left a given time on an unit, the tooltip should appear
/// </summary>
public class TraitTooltip : MonoBehaviour
{
    // Variables
    private TraitData _traitData;

    // References
    [SerializeField] private SpriteRenderer _iconSpriteRenderer;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptiveText;

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Init the icon and the string to correspond to the given trait
    /// </summary>
    public void InitTrait(Trait trait)
    {
        _traitData = GameManager.instance.GetTraitData(trait);
        _iconSpriteRenderer.sprite = _traitData.sprite;
        _nameText.text = _traitData.name;
        _descriptiveText.text = _traitData.text;
    }
}
