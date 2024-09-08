using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Tile",menuName = "Tile Object")]
public class TileObject : ScriptableObject
{
    public ObjectType objectType;
    public string tileName;
    public Color color;
    public Sprite sprite;
}

public enum ObjectType
{
    square = 0,
    circle=1,
    triangle=2,
    hexagon=3

}
