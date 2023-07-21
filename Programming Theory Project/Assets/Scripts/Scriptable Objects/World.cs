using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used to put all Levels into an array

[CreateAssetMenu (fileName = "World", menuName = "World")]
public class World : ScriptableObject
{
    public Level[] levels;
}
