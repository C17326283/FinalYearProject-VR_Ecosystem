using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//For getting the highest and lowest points on the planet
public class TerrainMinMaxHeights
{
    public float Min { get; private set; }
    public float Max { get; private set; }

    //constructor
    public TerrainMinMaxHeights()
    {
        Min = float.MaxValue;
        Max = float.MinValue;
    }

    //gets sent all the points from the terrainface class and if theyre higher or lower then set those
    public void AddValue(float v)
    {
        if (v > Max)
        {
            Max = v;
        }

        if (v < Min)
        {
            Min = v;
        }
    }
}
