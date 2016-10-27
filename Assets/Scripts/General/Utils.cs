/*
 * Utils.cs
 * --------
 * Here are defined functions used in multiple places.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Utils : MonoBehaviour {

    // C# random number generator.
    public static System.Random randomGenerator = new System.Random(1);

    // Shuffles an array.
    public static void Shuffle<T>(T[] array) {
        int count = array.Length;
        for (int i = 0; i < count; i++) {
            // It is needed the C# generator, because the Unity max value is inclusive, and that brokes this algorithm.
            int random = i + (int)(randomGenerator.NextDouble() * (count - i));
            T tmp = array[random];
            array[random] = array[i];
            array[i] = tmp;
        }
    }

    // Shuffles a list.
    public static void Shuffle<T>(List<T> list) {
        int count = list.Count;
        for (int i = 0; i < count; i++) {
            // It is needed the C# generator, because the Unity max value is inclusive, and that brokes this algorithm.
            int random = i + (int)(randomGenerator.NextDouble() * (count - i));
            T tmp = list[random];
            list[random] = list[i];
            list[i] = tmp;
        }
    }

    // Cancels the first vector components that are null in the second vector.
    // Example: v1(2,2,2) y v2(0,1,0) returns v3(0,2,0).
    public static Vector3 NullifyVector3Axes(Vector3 v1, Vector3 v2) {
        Vector3 newVector = new Vector3();
        newVector.x = (v2.x == 0.0f) ? 0.0f : v1.x;
        newVector.y = (v2.y == 0.0f) ? 0.0f : v1.y;
        newVector.z = (v2.z == 0.0f) ? 0.0f : v1.z;
        return newVector;
    }

}