using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveMode : MonoBehaviour
{
    // Global tool mode the simulation application is in.
    public static ToolMode Mode = ToolMode.None;
}

public enum ToolMode
{
    None,
    Spawner,
    Obstacles
}