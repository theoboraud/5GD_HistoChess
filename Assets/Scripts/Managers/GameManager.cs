using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     The GameManager class manages the game flow (states, game phases...) and references used in the game (settings, units...)
/// </summary>
public class GameManager : MonoBehaviour
{
    // GameManager static instance
    public static GameManager instance;

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
        Init();
    }

    /// <summary>
    ///     Init class variables
    /// </summary>
    private void Init()
    {
        // TODO: Add all variable to init and their init value
    }
}
