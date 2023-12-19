using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveMode : MonoBehaviour
{
    public static ToolMode Mode = ToolMode.None;
}

public enum ToolMode
{
    None,
    Spawner
}