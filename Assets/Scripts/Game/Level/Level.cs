using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour {
    
    public int width = 4;
    public int height = 4;
    public GenerateMode generateMode = GenerateMode.NEWEST;
    public Zone zonePrefab;

    private Zone[,] zones;
    private Vector2 zoneSize;
    private Vec2i start;
    private Vec2i end;
    private Transform minimapTransform;

    public void Generate() {

        zoneSize = new Vector2(Globals.ROOM_SIZE * XenatekManager.xenatek.zoneWidth, Globals.ROOM_SIZE * XenatekManager.xenatek.zoneHeight);

        // Initialize the zones matrix.
        zones = new Zone[width, height];

        // Create the zone instances.
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                Zone zoneClone = Instantiate(zonePrefab, new Vector3(i * zoneSize.x, 0.0f, j * zoneSize.y), Quaternion.identity) as Zone;
                zoneClone.name = "Zone (" + i + ", " + j + ")";
                zoneClone.transform.parent = transform;
                zoneClone.x = i;
                zoneClone.y = j;
                zoneClone.width = XenatekManager.xenatek.zoneWidth;
                zoneClone.height = XenatekManager.xenatek.zoneHeight;
                zones[i, j] = zoneClone;
            }
        }

        // Generate the zones maze.
        GenerateMaze();

        // Choose the start and the end zone of the level. Can be the same room.
        start = Vec2i.Randomize(0, width, 0, height);
        end = Vec2i.Randomize(0, width, 0, height);
        zones[start.x, start.y].isStart = true;
        zones[end.x, end.y].isEnd = true;

        // Solve the maze.
        SolveMaze(ref zones[start.x, start.y]);

        // Define what zones have doors and/or keys.
        int region = 0;
        int order = 0;
        CreateDoorsAndKeys(ref zones[start.x, start.y], Direction.NONE, ref region, ref order);

        // Now the missions can be generated.
        // The last region's number is the same as the number of missions to generate.
        MissionController.instance.GenerateMissions(region);

        // Assign references to the neighbor zones that does not have a wall between them.
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if ((zones[i, j].walls & (int)Direction.NORTH) != 0) {
                    zones[i, j].northZone = zones[i, j + 1];
                }
                if ((zones[i, j].walls & (int)Direction.EAST) != 0) {
                    zones[i, j].eastZone = zones[i + 1, j];
                }
                if ((zones[i, j].walls & (int)Direction.SOUTH) != 0) {
                    zones[i, j].southZone = zones[i, j - 1];
                }
                if ((zones[i, j].walls & (int)Direction.WEST) != 0) {
                    zones[i, j].westZone = zones[i - 1, j];
                }
            }
        }

        // Place the minimap in a place where can't be seen.
        minimapTransform.position = new Vector3(-width * XenatekManager.xenatek.zoneWidth * Globals.ROOM_SIZE * 2.0f, 0.0f, 0.0f);

        // Finally, generate each zone content.
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                zones[i, j].Generate();
                // By default the zone is deactivated.
                zones[i, j].gameObject.SetActive(false);
                zones[i, j].GenerateMinimapPiece(minimapTransform);
            }
        }

        // Activate the first zone and its neighbors.
        zones[start.x, start.y].ActivateZone();

    }

    // Generate the maze using the Growing Tree algorithm.
    private void GenerateMaze() {
        int x = Random.Range(0, width);
        int y = Random.Range(0, height);

        List<Zone> cells = new List<Zone>();
        cells.Add(zones[x, y]);

        Direction[] directions = { Direction.NORTH, Direction.EAST, Direction.SOUTH, Direction.WEST };

        while (cells.Count > 0) {
            int index = ChooseIndex(cells.Count);
            x = cells[index].x;
            y = cells[index].y;

            Utils.Shuffle<Direction>(directions);

            foreach (Direction direction in directions) {
                int newX = x + DirectionFuncs.GetX(direction);
                int newY = y + DirectionFuncs.GetY(direction);
                if (newX >= 0 && newX < width && newY >= 0 && newY < height && zones[newX, newY].walls == 0) {
                    zones[x, y].walls |= (int)direction;
                    zones[newX, newY].walls |= (int)DirectionFuncs.GetOpposite(direction);
                    cells.Add(zones[newX, newY]);
                    index = -1;
                    break;
                }
            }
            if (index > -1) {
                cells.RemoveAt(index);
            }
        }

    }

    // Return the index to continue populating the maze. Here can be implemented different methods.
    private int ChooseIndex(int ceil) {
        int index = 0;
        switch (generateMode) {
            case GenerateMode.NEWEST:
                index = ceil - 1;
                break;
            case GenerateMode.MIDDLE:
                index = ceil >> 1;
                break;
            case GenerateMode.OLDEST:
                index = 0;
                break;
            case GenerateMode.RANDOM:
                index = Random.Range(0, ceil);
                break;
        }
        return index;
    }

    // Returns the maze, making the zones that belong to the road.
    private bool SolveMaze(ref Zone zone) {
        if (zone.x == end.x && zone.y == end.y) {
            zone.isPath = true;
            return true;
        }
        if (zone.isPath) {
            return false;
        }
        zone.isPath = true;
        for (Direction dir = Direction.NORTH; dir <= Direction.WEST; dir += (int)dir) {
            if ((zone.walls & (int)dir) != 0) { // If does not exist a wall.
                if (SolveMaze(ref zones[zone.x + DirectionFuncs.GetX(dir), zone.y + DirectionFuncs.GetY(dir)])) {
                    return true;
                }
            }
        }
        zone.isPath = false;
        return false;
    }

    // Marks the zones that have keys and/or doors.
    private void CreateDoorsAndKeys(ref Zone zone, Direction from, ref int region, ref int order) {
        List<Direction> dirs = GetAvailableDirections(zone, from);
        if (dirs.Count == 0) {
            // Put a key.
            zone.key = ++region;
            zone.region = region;
            zone.order = ++order;
            order = 0;
        }
        else {
            // Continue iterating.
            int lastRegion = region;
            foreach (Direction dir in dirs) {
                if (lastRegion != region && dirs.Count > 1) {
                    // Put a door.
                    lastRegion = region;
                    switch (dir) {
                        case Direction.NORTH:
                            zone.northDoor = region;
                            break;
                        case Direction.EAST:
                            zone.eastDoor = region;
                            break;
                        case Direction.SOUTH:
                            zone.southDoor = region;
                            break;
                        case Direction.WEST:
                            zone.westDoor = region;
                            break;
                    }
                }

                if (zone.region == 0) {
                    zone.region = region + 1;
                    zone.order = ++order;
                }
                CreateDoorsAndKeys(ref zones[zone.x + DirectionFuncs.GetX(dir), zone.y + DirectionFuncs.GetY(dir)], DirectionFuncs.GetOpposite(dir), ref region, ref order);
            }
        }
    }

    // Return a list with the available directions to continue navigating through the maze.
    private List<Direction> GetAvailableDirections(Zone zone, Direction from) {
        List<Direction> dirs = new List<Direction>();

        Direction lastDirection = Direction.NONE;
        for (Direction dir = Direction.NORTH; dir <= Direction.WEST; dir += (int)dir) {
            if (dir != from) {
                if ((zone.walls & (int)dir) != 0) { // If does not exist a wall.
                    if (zones[zone.x + DirectionFuncs.GetX(dir), zone.y + DirectionFuncs.GetY(dir)].isPath) {  // Always take the last the zone that leads to the last zone.
                        lastDirection = dir;
                    }
                    else {
                        dirs.Add(dir);
                    }
                }
            }
        }

        // Shuffle to not always take the same direction.
        if (dirs.Count > 1) {
            Utils.Shuffle<Direction>(dirs);
        }

        if (lastDirection != Direction.NONE) {
            dirs.Add(lastDirection);
        }

        return dirs;
    }

    // Returns the initial zone.
    public Zone GetStartZone() {
        return zones[start.x, start.y];
    }

    // Change the zone, activate and deactivate whatever necessary.
    public void ChangeZone(Vec2i newZone, Vec2i oldZone) {
        zones[oldZone.x, oldZone.y].DeactivateZone();
        zones[newZone.x, newZone.y].ActivateZone();
    }

    // Return a zone according to the coordinates.
    public Zone GetZoneByCoords(Vec2i coords) {
        return zones[coords.x, coords.y];
    }

    // Assign the minimap's transform.
    public void SetMinimapTransform(Transform transform) {
        minimapTransform = transform;
    }

}