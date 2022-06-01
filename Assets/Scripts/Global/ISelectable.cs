using UnityEngine;
using System.Collections;

/// <summary>
///     Interface for selectable entity in the game
/// </summary>
public interface ISelectableEntity
{
    void OnMouseDown();
    void Select();
    void Unselect();
}
