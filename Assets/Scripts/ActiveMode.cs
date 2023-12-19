using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveMode : MonoBehaviour
{
    public static ToolMode Mode = ToolMode.None;

    public static void Default() => Mode = ToolMode.None;
    public static void SpawnerMode() => Mode = ToolMode.Spawner;
}

public enum ToolMode
{
    None,
    Spawner
}