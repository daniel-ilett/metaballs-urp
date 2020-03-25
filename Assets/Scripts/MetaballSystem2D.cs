using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MetaballSystem2D
{
    private static List<Metaball2D> metaballs;

    static MetaballSystem2D()
    {
        metaballs = new List<Metaball2D>();
    }

    public static void Add(Metaball2D metaball)
    {
        metaballs.Add(metaball);
    }

    public static List<Metaball2D> Get()
    {
        return metaballs;
    }

    public static void Remove(Metaball2D metaball)
    {
        metaballs.Remove(metaball);
    }
}
