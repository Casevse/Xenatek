/*
 * Types.cs
 * --------
 * Here are defined data types used in multiple places.
*/

using UnityEngine;
using System.Collections.Generic;

// Vector of 2 integers.
[System.Serializable]
public struct Vec2i {

    public int x;
    public int y;

    public Vec2i(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public static Vec2i operator + (Vec2i a, Vec2i b) {
        a.x += b.x;
        a.y += b.y;
        return a;
    }

    public static bool operator ==(Vec2i a, Vec2i b) {
        return (a.x == b.x && a.y == b.y);
    }

    public static bool operator !=(Vec2i a, Vec2i b) {
        return (a.x != b.x || a.y != b.y);
    }

    public static Vec2i Randomize(int xMin, int xMax, int yMin, int yMax) {
        return new Vec2i(Random.Range(xMin, xMax), Random.Range(yMin, yMax));
    }

}

// Enumerator for the directions.
public enum Direction {
    NONE    = 0,
    NORTH   = 1,
    EAST    = 2,
    SOUTH   = 4,
    WEST    = 8
}

// Enumerator for the maze generation mode.
[System.Serializable]
public enum GenerateMode {
    NEWEST, MIDDLE, OLDEST, RANDOM
}

// Auxiliar functions to operate with the directions.
public struct DirectionFuncs {
    private static Dictionary<Direction, int> DirectionX = new Dictionary<Direction, int>() {
        { Direction.NORTH, 0 },
        { Direction.EAST, 1 },
        { Direction.SOUTH, 0 },
        { Direction.WEST, -1 }
    };
    private static Dictionary<Direction, int> DirectionY = new Dictionary<Direction, int>() {
        { Direction.NORTH, 1 },
        { Direction.EAST, 0 },
        { Direction.SOUTH, -1 },
        { Direction.WEST, 0 }
    };
    private static Dictionary<Direction, Direction> OppositeDirection = new Dictionary<Direction, Direction>() {
        { Direction.NORTH, Direction.SOUTH },
        { Direction.EAST, Direction.WEST },
        { Direction.SOUTH, Direction.NORTH },
        { Direction.WEST, Direction.EAST }
    };
    public static int GetX(Direction direction) {
        return DirectionX[direction];
    }
    public static int GetY(Direction direction) {
        return DirectionY[direction];
    }
    public static Direction GetOpposite(Direction direction) {
        return OppositeDirection[direction];
    }
}