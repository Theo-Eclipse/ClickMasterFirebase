using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NameGenerator
{
    private static string[] names = new string[] {"Alex", "David", "Salamon", "Sam", "John","Theo", "Gal"};

    private static string[] family_names = new string[] { "Vazovski", "Linderman", "Black", "Eclipse", "Rosenfield", "Tomahavk", "Liberman" };
    public static string GetName() => string.Format("{0} {1}", names[Random.Range(0, names.Length)], family_names[Random.Range(0, family_names.Length)]);
}
