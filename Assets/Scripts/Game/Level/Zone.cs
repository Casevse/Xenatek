using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Zone : MonoBehaviour {

    public Room roomPrefab; // NOTE: This two variables can be a waste of memory, because of the zones have the same width and height.
    public int width = 4;   // This can be static. All the zones have the same width.
    public int height = 4;  // This can be static. All the zones have the same height.
    public int x = 0;
    public int y = 0;
    public int walls = 0;           // By default, it does not connect with any room.
    public bool isPath = false;     // By default, is not a path.
    public bool isStart = false;    // By default, is not the start zone.
    public bool isEnd = false;      // By default, is not the end zone.
    public int key = 0;             // By default, it does not contain a key.
    public int region = 0;          // By default, it does not is a part of a region.
    public int order = 0;           // By default, it does not have an order inside the region.
    public int northDoor = 0;       // By default, it does not have a north door.
    public int eastDoor = 0;        // By default, it does not have a east door.
    public int southDoor = 0;       // By default, it does not have a south door.
    public int westDoor = 0;        // By default, it does not have a west door.
    
    // References to the neighbor zones. By default are null, and we only reference them there is not a wall.
    public Zone northZone = null;
    public Zone eastZone = null;
    public Zone southZone = null;
    public Zone westZone = null;

    // For DEBUG.
    public GameObject debugFloor;
    public GameObject debugLine;
    public GameObject debugNumber;

    // References to the importent rooms.
    public Room centralRoom = null;
    public Room northRoom = null;
    public Room eastRoom = null;
    public Room southRoom = null;
    public Room westRoom = null;

    private Room[,] rooms;

    // Auxiliar variables.
    private float widthDiv2;
    private float widthDiv4;
    private float widthDiv8;
    private float heightDiv2;
    private float heightDiv4;
    private float heightDiv8;

    public void Generate() {
        // NOTE: We don't need to instance each room, because some of them does not exist.
        widthDiv2 = width / 2.0f;
        widthDiv4 = width / 4.0f;
        widthDiv8 = width / 8.0f;
        heightDiv2 = height / 2.0f;
        heightDiv4 = height / 4.0f;
        heightDiv8 = height / 8.0f;

        // Initialize the room matrix.
        rooms = new Room[width, height];

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                Room roomClone = Instantiate(roomPrefab) as Room;
                roomClone.name = "Room (" + i + ", " + j + ")";
                roomClone.transform.parent = transform;
                roomClone.transform.localPosition = new Vector3(i * Globals.ROOM_SIZE, 0.0f, j * Globals.ROOM_SIZE);
                roomClone.x = i;
                roomClone.y = j;
                rooms[i, j] = roomClone;
            }
        }

        // First chose the middle room, and the side ones in case there is not a wall with the neighbor zone.
        ChooseImportantRooms();
        
        // Create paths between the important rooms, following different strategies.
        int rand = Random.Range(0, 3);
        switch (rand) {
            case 0:
                GenerateCorridorZone();     break;
            case 1:
                GenerateTriangularZone();   break;
            case 2:
                GenerateRectangularZone();  break;
        }

        GenerateFilling();

        // Finally, generate the content of each room.
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                // If the room is still null, delete the GameObject. If not, generate it.
                if (rooms[i, j].isNull) {
                    Destroy(rooms[i, j].gameObject);
                }
                else {
                    // Pass the zone as a parameter and place the puzzles checking the order inside the same region.
                    rooms[i, j].Generate(this);
                }
            }
        }

        // NOTE: Better to make this public and call it from the level. Pass a parameters if we want to draw also the rooms.
        // DrawDebugZone();
    }

    private void ChooseImportantRooms() {
        // NOTE: A room can have different types at the same time.
        Vec2i centralRoomPoint = Vec2i.Randomize(Mathf.RoundToInt(widthDiv4 + widthDiv8), Mathf.RoundToInt(widthDiv2 + widthDiv8), Mathf.RoundToInt(heightDiv4 + heightDiv8), Mathf.RoundToInt(heightDiv2 + heightDiv8));
        rooms[centralRoomPoint.x, centralRoomPoint.y].isCentral = true;
        rooms[centralRoomPoint.x, centralRoomPoint.y].isNull = false;
        centralRoom = rooms[centralRoomPoint.x, centralRoomPoint.y];

        // IMPORTANT NOTE: Always check the south and west zone to connect rooms between zones. This is the order that the loop follows.
        if ((walls & (int)Direction.NORTH) != 0) {
            Vec2i northRoomPoint = Vec2i.Randomize(Mathf.RoundToInt(widthDiv4), Mathf.RoundToInt(widthDiv2 + widthDiv4), Mathf.RoundToInt(height - 1), Mathf.RoundToInt(height - 1));
            rooms[northRoomPoint.x, northRoomPoint.y].walls |= (int)Direction.NORTH;
            rooms[northRoomPoint.x, northRoomPoint.y].northDoor = northDoor;
            rooms[northRoomPoint.x, northRoomPoint.y].isNull = false;
            northRoom = rooms[northRoomPoint.x, northRoomPoint.y];
        }
        if ((walls & (int)Direction.EAST) != 0) {
            Vec2i eastRoomPoint = Vec2i.Randomize(Mathf.RoundToInt(width - 1), Mathf.RoundToInt(width - 1), Mathf.RoundToInt(heightDiv4), Mathf.RoundToInt(heightDiv2 + heightDiv4));
            rooms[eastRoomPoint.x, eastRoomPoint.y].walls |= (int)Direction.EAST;
            rooms[eastRoomPoint.x, eastRoomPoint.y].eastDoor = eastDoor;
            rooms[eastRoomPoint.x, eastRoomPoint.y].isNull = false;
            eastRoom = rooms[eastRoomPoint.x, eastRoomPoint.y];
        }
        if ((walls & (int)Direction.SOUTH) != 0) {
            Vec2i southRoomPoint;
            if (southZone != null) {    // Because of the loop's order, check if the south zone has a door towards the north zone.
                southRoomPoint = new Vec2i(southZone.northRoom.x, 0);
            }
            else {
                southRoomPoint = Vec2i.Randomize(Mathf.RoundToInt(widthDiv4), Mathf.RoundToInt(widthDiv2 + widthDiv4), Mathf.RoundToInt(height - 1), Mathf.RoundToInt(height - 1));
            }
            rooms[southRoomPoint.x, southRoomPoint.y].walls |= (int)Direction.SOUTH;
            rooms[southRoomPoint.x, southRoomPoint.y].southDoor = southDoor;
            rooms[southRoomPoint.x, southRoomPoint.y].isNull = false;
            southRoom = rooms[southRoomPoint.x, southRoomPoint.y];
        }
        if ((walls & (int)Direction.WEST) != 0) {
            Vec2i westRoomPoint;
            if (westZone != null) {    // Because of the loop's order, check if the west zone has a door towards the east zone.
                westRoomPoint = new Vec2i(0, westZone.eastRoom.y);
            }
            else {
                westRoomPoint = Vec2i.Randomize(Mathf.RoundToInt(widthDiv4), Mathf.RoundToInt(widthDiv2 + widthDiv4), Mathf.RoundToInt(height - 1), Mathf.RoundToInt(height - 1));
            }
            rooms[westRoomPoint.x, westRoomPoint.y].walls |= (int)Direction.WEST;
            rooms[westRoomPoint.x, westRoomPoint.y].westDoor = westDoor;
            rooms[westRoomPoint.x, westRoomPoint.y].isNull = false;
            westRoom = rooms[westRoomPoint.x, westRoomPoint.y];
        }
    }

    // Distribute the rooms with the "PASILLOS" strategy.
    private void GenerateCorridorZone() {
        // For each room with door we need to generate a path towards the central room.
        if (northRoom != null) {
            GeneratePathBetweenRooms(new Vec2i(northRoom.x, northRoom.y), new Vec2i(centralRoom.x, centralRoom.y));
        }
        if (eastRoom != null) {
            GeneratePathBetweenRooms(new Vec2i(eastRoom.x, eastRoom.y), new Vec2i(centralRoom.x, centralRoom.y));
        }
        if (southRoom != null) {
            GeneratePathBetweenRooms(new Vec2i(southRoom.x, southRoom.y), new Vec2i(centralRoom.x, centralRoom.y));
        }
        if (westRoom != null) {
            GeneratePathBetweenRooms(new Vec2i(westRoom.x, westRoom.y), new Vec2i(centralRoom.x, centralRoom.y));
        }
    }

    // Distribute the rooms with the "TRIANGULOS" strategy.
    private void GenerateTriangularZone() {
        if (northRoom != null) {
            GeneratePathBetweenRooms(new Vec2i(northRoom.x, northRoom.y), new Vec2i(centralRoom.x, centralRoom.y));
            if (eastRoom != null) {
                GeneratePathBetweenRooms(new Vec2i(northRoom.x, northRoom.y), new Vec2i(eastRoom.x, eastRoom.y));
            }
        }
        if (eastRoom != null) {
            GeneratePathBetweenRooms(new Vec2i(eastRoom.x, eastRoom.y), new Vec2i(centralRoom.x, centralRoom.y));
            if (southRoom != null) {
                GeneratePathBetweenRooms(new Vec2i(eastRoom.x, eastRoom.y), new Vec2i(southRoom.x, southRoom.y));
            }
        }
        if (southRoom != null) {
            GeneratePathBetweenRooms(new Vec2i(southRoom.x, southRoom.y), new Vec2i(centralRoom.x, centralRoom.y));
            if (westRoom != null) {
                GeneratePathBetweenRooms(new Vec2i(southRoom.x, southRoom.y), new Vec2i(westRoom.x, westRoom.y));
            }
        }
        if (westRoom != null) {
            GeneratePathBetweenRooms(new Vec2i(westRoom.x, westRoom.y), new Vec2i(centralRoom.x, centralRoom.y));
            if (northRoom) {
                GeneratePathBetweenRooms(new Vec2i(westRoom.x, westRoom.y), new Vec2i(northRoom.x, northRoom.y));
            }
        }

        FloodBetweenRooms();
    }

    // Distribute the rooms with the "CUADRADOS" strategy.
    private void GenerateRectangularZone() {
        Vec2i minPoint = Vec2i.Randomize(Mathf.RoundToInt(widthDiv4 - widthDiv8), Mathf.RoundToInt(widthDiv4 + widthDiv8), Mathf.RoundToInt(heightDiv4 - heightDiv8), Mathf.RoundToInt(heightDiv4 + heightDiv8));
        Vec2i maxPoint = Vec2i.Randomize(Mathf.RoundToInt(widthDiv2 + widthDiv4 - widthDiv8), Mathf.RoundToInt(widthDiv2 + widthDiv4 + widthDiv8), Mathf.RoundToInt(heightDiv2 + heightDiv4 - heightDiv8), Mathf.RoundToInt(heightDiv2 + heightDiv4 + heightDiv8));

        // We check if the rectangle is large enough.
        if (northRoom != null) {
            if (northRoom.x < minPoint.x) {
                minPoint.x = northRoom.x;
            }
            else if (northRoom.x > maxPoint.x) {
                maxPoint.x = northRoom.x;
            }
        }
        if (eastRoom != null) {
            if (eastRoom.y < minPoint.y) {
                minPoint.y = eastRoom.y;
            }
            else if (eastRoom.y > maxPoint.y) {
                maxPoint.y = eastRoom.y;
            }
        }
        if (southRoom != null) {
            if (southRoom.x < minPoint.x) {
                minPoint.x = southRoom.x;
            }
            else if (southRoom.x > maxPoint.x) {
                maxPoint.x = southRoom.x;
            }
        }
        if (westRoom != null) {
            if (westRoom.y < minPoint.y) {
                minPoint.y = westRoom.y;
            }
            else if (westRoom.y > maxPoint.y) {
                maxPoint.y = westRoom.y;
            }
        }

        // Create the rectangle.
        // NOTE: With <= has an out of bounds exception when the width and the height of the zone is 1. Check that case also.
        for (int i = minPoint.x; i <= maxPoint.x; i++) {
            for (int j = minPoint.y; j <= maxPoint.y; j++) {
                if (i < width && j < height) {
                    if (rooms[i, j].isNull) {
                        rooms[i, j].isPath = true;
                        rooms[i, j].isNull = false;
                    }
                    // Open way through the walls. Following the order of the loop.
                    if (i > minPoint.x) {
                        rooms[i, j].walls |= (int)Direction.WEST;
                        rooms[i - 1, j].walls |= (int)Direction.EAST;
                    }
                    if (j > minPoint.y) {
                        rooms[i, j].walls |= (int)Direction.SOUTH;
                        rooms[i, j - 1].walls |= (int)Direction.NORTH;
                    }
                }
            }
        }

        // Trace the path from the exit rooms to the closer rectangle border.
        if (northRoom != null) {
            GeneratePathBetweenRooms(new Vec2i(northRoom.x, northRoom.y), new Vec2i(northRoom.x, maxPoint.y));
        }
        if (eastRoom != null) {
            GeneratePathBetweenRooms(new Vec2i(eastRoom.x, eastRoom.y), new Vec2i(maxPoint.x, eastRoom.y));
        }
        if (southRoom != null) {
            GeneratePathBetweenRooms(new Vec2i(southRoom.x, southRoom.y), new Vec2i(southRoom.x, minPoint.y));
        }
        if (westRoom != null) {
            GeneratePathBetweenRooms(new Vec2i(westRoom.x, westRoom.y), new Vec2i(minPoint.x, westRoom.y));
        }

    }

    // Generate a path between 2 rooms in a simple and direct way.
    private void GeneratePathBetweenRooms(Vec2i start, Vec2i end) {
        // Choose the directions towards the end room.
        Direction directionX = (end.x > start.x) ? Direction.EAST : Direction.WEST;
        Direction directionY = (end.y > start.y) ? Direction.NORTH : Direction.SOUTH;

        Vec2i current = start;
        Vec2i last = current;

        while (current != end) {
            int axis = Random.Range(0, 2);
            if (axis == 0 && current.x == end.x) {
                axis = 1;
            }
            else if (axis == 1 && current.y == end.y) {
                axis = 0;
            }
            if (axis == 0) {
                // Take an X direction.
                current.x += (directionX == Direction.EAST) ? 1 : -1;
                // Connect the rooms.
                if (current.x >= 0 && current.x < width && current.y >= 0 && current.y < height) {
                    rooms[last.x, last.y].walls |= (int)directionX;
                    rooms[current.x, current.y].walls |= (int)DirectionFuncs.GetOpposite(directionX);
                }
            }
            else {
                // Take the Y direction.
                current.y += (directionY == Direction.NORTH) ? 1 : -1;
                // Connect the rooms.
                if (current.x >= 0 && current.x < width && current.y >= 0 && current.y < height) {
                    rooms[last.x, last.y].walls |= (int)directionY;
                    rooms[current.x, current.y].walls |= (int)DirectionFuncs.GetOpposite(directionY);
                }
            }
            if (current != end) {
                rooms[current.x, current.y].isPath = true;
                rooms[current.x, current.y].isNull = false;
            }
            last = current;
        }
    }

    // Fill with paths the zone between the min and max room of each column. This function can be expensive.
    private void FloodBetweenRooms() {
        int lastMin = -1;
        int lastMax = -1;
        for (int i = 0; i < width; i++) {
            int min = -1, max = -1;
            // Search the min and max points...
            for (int j = 0; j < height; j++) {
                if (!rooms[i, j].isNull) {
                    if (min == -1) {
                        min = j;
                    }
                    else {
                        max = j;
                    }
                }
            }
            // ...and then flood between those points.
            if (max != -1) {
                for (int j = min; j <= max; j++) {
                    if (rooms[i, j].isNull) {
                        rooms[i, j].isPath = true;
                        rooms[i, j].isNull = false;
                    }
                    if (i > 0) {
                        if (j >= lastMin && j <= lastMax) {
                            // Open ways through the wall.
                            rooms[i, j].walls |= (int)Direction.WEST;
                            rooms[i - 1, j].walls |= (int)Direction.EAST;
                        }
                        if (j > min) {
                            rooms[i, j].walls |= (int)Direction.SOUTH;
                            rooms[i, j - 1].walls |= (int)Direction.NORTH;
                        }
                    }
                }
            }
            lastMin = min;
            lastMax = max;
        }
    }

    // Generate filling rooms.
    private void GenerateFilling() {
        List<Room> fillingRooms = new List<Room>();

        // NOTE: If a room has walls == 15, it means that nothing can be added to the room.
        
        // In the first iteration exists a possibility of creating a filling room touching some of the path rooms.
        // Store the rooms in a list to continue iterating later.
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (rooms[i, j].isPath && rooms[i, j].walls < 15) { // 15 means that is not completely surrounded.
                    for (Direction dir = Direction.NORTH; dir <= Direction.WEST; dir += (int)dir) {
                        int x = i + DirectionFuncs.GetX(dir);
                        int y = j + DirectionFuncs.GetY(dir);
                        if (x >= 0 && x < width && y >= 0 && y < height) {
                            if (rooms[x, y].isNull) {
                                if (Random.value > 0.5f) {
                                    rooms[x, y].filling = 1;
                                    rooms[x, y].isNull = false;
                                    rooms[i, j].walls |= (int)dir;
                                    rooms[x, y].walls |= (int)DirectionFuncs.GetOpposite(dir);
                                    fillingRooms.Add(rooms[x, y]);
                                }
                            }
                        }
                    }
                }
            }
        }

        // IMPORTANT: This is similar to the Drunkard Walk algorithm (Random Walk).
        int iterations = Random.Range(0, 4);
        int filling = 2;
        while (iterations > 0) {
            List<Room> nextFillingRooms = new List<Room>();
            for (int i = 0; i < fillingRooms.Count; i++) {
                if (fillingRooms[i].walls < 15) {
                    for (Direction dir = Direction.NORTH; dir <= Direction.WEST; dir += (int)dir) {
                        int x = fillingRooms[i].x + DirectionFuncs.GetX(dir);
                        int y = fillingRooms[i].y + DirectionFuncs.GetY(dir);
                        if (x >= 0 && x < width && y >= 0 && y < height) {
                            if (rooms[x, y].isNull) {
                                if (Random.value > 0.5f) {
                                    rooms[x, y].filling = filling;
                                    rooms[x, y].isNull = false;
                                    rooms[fillingRooms[i].x, fillingRooms[i].y].walls |= (int)dir;
                                    rooms[x, y].walls |= (int)DirectionFuncs.GetOpposite(dir);
                                    nextFillingRooms.Add(rooms[x, y]);
                                }
                            }
                        }
                    }
                }
            }
            fillingRooms = nextFillingRooms;
            filling++;
            iterations--;
        }

    }

    // Generates the piece of the minimap.
    public void GenerateMinimapPiece(Transform minimapTransform) {
        // Place the floor.
        GameObject floorClone = LevelResources.instance.CreateMinimapPiece(
            LevelResources.MinimapPieceType.FLOOR,
            minimapTransform,
            new Vector3(x * Globals.ROOM_SIZE, 0.0f, y * Globals.ROOM_SIZE),
            Quaternion.Euler(90.0f, 0.0f, 0.0f)
        );
        floorClone.name = "Zone(" + x + ", " + y + ")";

        // Place the walls.
        if ((walls & (int)Direction.NORTH) == 0) {
            GameObject lineClone = LevelResources.instance.CreateMinimapPiece(
                LevelResources.MinimapPieceType.WALL,
                minimapTransform,
                new Vector3(x * Globals.ROOM_SIZE, 0.25f, y * Globals.ROOM_SIZE + Globals.WALL_OFFSET),
                Quaternion.Euler(90.0f, 0.0f, 0.0f)
                );
        }
        if ((walls & (int)Direction.EAST) == 0) {
            GameObject lineClone = LevelResources.instance.CreateMinimapPiece(
                LevelResources.MinimapPieceType.WALL,
                minimapTransform,
                new Vector3(x * Globals.ROOM_SIZE + Globals.WALL_OFFSET, 0.25f, y * Globals.ROOM_SIZE),
                Quaternion.Euler(90.0f, 90.0f, 0.0f)
            );
        }
        if ((walls & (int)Direction.SOUTH) == 0) {
            GameObject lineClone = LevelResources.instance.CreateMinimapPiece(
                LevelResources.MinimapPieceType.WALL,
                minimapTransform,
                new Vector3(x * Globals.ROOM_SIZE, 0.25f, y * Globals.ROOM_SIZE - Globals.WALL_OFFSET),
                Quaternion.Euler(90.0f, 0.0f, 0.0f)
            );
        }
        if ((walls & (int)Direction.WEST) == 0) {
            GameObject lineClone = LevelResources.instance.CreateMinimapPiece(
                LevelResources.MinimapPieceType.WALL,
                minimapTransform,
                new Vector3(x * Globals.ROOM_SIZE - Globals.WALL_OFFSET, 0.25f, y * Globals.ROOM_SIZE),
                Quaternion.Euler(90.0f, 90.0f, 0.0f)
            );
        }
        // Place the number of the region.
        GameObject numberClone = LevelResources.instance.CreateMinimapPiece(
            LevelResources.MinimapPieceType.NUMBER,
            minimapTransform,
            new Vector3(x * Globals.ROOM_SIZE, 0.5f, y * Globals.ROOM_SIZE),
            Quaternion.Euler(90.0f, 0.0f, 0.0f)
        );
        numberClone.GetComponent<TextMesh>().text = region.ToString();
    }

    // Shows the zone with basic forms. For DEBUG.
    private void DrawDebugZone() {
        // Place the floor.
        GameObject floorClone = Instantiate(debugFloor, new Vector3(x * width * Globals.ROOM_SIZE, 0.0f, y * height * Globals.ROOM_SIZE), Quaternion.identity) as GameObject;
        floorClone.transform.parent = transform;
        floorClone.transform.localScale = new Vector3(floorClone.transform.localScale.x * width, 0.25f, floorClone.transform.localScale.z * height);
        if (isEnd) {
            floorClone.GetComponent<Renderer>().material.color = Color.red;
        }
        else if (isStart) {
            floorClone.GetComponent<Renderer>().material.color = Color.green;
        }
        else if (isPath) {
            floorClone.GetComponent<Renderer>().material.color = Color.cyan;
        }
        // Place the walls.
        if ((walls & (int)Direction.NORTH) == 0) {
            GameObject lineClone = Instantiate(debugLine, new Vector3(x * width * Globals.ROOM_SIZE, 0.25f, y * height * Globals.ROOM_SIZE + height * Globals.WALL_OFFSET), Quaternion.identity) as GameObject;
            lineClone.transform.parent = transform;
            lineClone.transform.localScale = new Vector3(lineClone.transform.localScale.x * width, 0.25f, lineClone.transform.localScale.z * height);
        }
        if ((walls & (int)Direction.EAST) == 0) {
            GameObject lineClone = Instantiate(debugLine, new Vector3(x * width * Globals.ROOM_SIZE + width * Globals.WALL_OFFSET, 0.25f, y * height * Globals.ROOM_SIZE), Quaternion.Euler(0.0f, 90.0f, 0.0f)) as GameObject;
            lineClone.transform.parent = transform;
            lineClone.transform.localScale = new Vector3(lineClone.transform.localScale.x * height, 0.25f, lineClone.transform.localScale.z * width);
        }
        if ((walls & (int)Direction.SOUTH) == 0) {
            GameObject lineClone = Instantiate(debugLine, new Vector3(x * width * Globals.ROOM_SIZE, 0.25f, y * height * Globals.ROOM_SIZE - height * Globals.WALL_OFFSET), Quaternion.identity) as GameObject;
            lineClone.transform.parent = transform;
            lineClone.transform.localScale = new Vector3(lineClone.transform.localScale.x * width, 0.25f, lineClone.transform.localScale.z * height);
        }
        if ((walls & (int)Direction.WEST) == 0) {
            GameObject lineClone = Instantiate(debugLine, new Vector3(x * width * Globals.ROOM_SIZE - width * Globals.WALL_OFFSET, 0.25f, y * height * Globals.ROOM_SIZE), Quaternion.Euler(0.0f, 90.0f, 0.0f)) as GameObject;
            lineClone.transform.parent = transform;
            lineClone.transform.localScale = new Vector3(lineClone.transform.localScale.x * height, 0.25f, lineClone.transform.localScale.z * width);
        }
        if (key > 0) {
            GameObject numberClone = Instantiate(debugNumber, new Vector3(x * width * Globals.ROOM_SIZE, 0.25f, y * height * Globals.ROOM_SIZE), Quaternion.Euler(90.0f, 0.0f, 0.0f)) as GameObject;
            if (isEnd) {
                numberClone.GetComponent<TextMesh>().color = Color.yellow;    // This is the exit.
            }
            else {
                numberClone.GetComponent<TextMesh>().color = Color.blue;
            }
            numberClone.transform.parent = transform;
            numberClone.GetComponent<TextMesh>().text = key.ToString();
            numberClone.GetComponent<TextMesh>().characterSize = 4 * height;
        }
        else {
            GameObject numberClone = Instantiate(debugNumber, new Vector3(x * width * Globals.ROOM_SIZE, 0.25f, y * height * Globals.ROOM_SIZE), Quaternion.Euler(90.0f, 0.0f, 0.0f)) as GameObject;
            numberClone.GetComponent<TextMesh>().color = new Color(1.0f, 0.5f, 0.0f, 1.0f);
            numberClone.transform.parent = transform;
            numberClone.GetComponent<TextMesh>().text = region.ToString();
            numberClone.GetComponent<TextMesh>().characterSize = 4 * height;
        }
        if (northDoor > 0) {
            GameObject numberClone = Instantiate(debugNumber, new Vector3(x * width * Globals.ROOM_SIZE, 0.25f, y * height * Globals.ROOM_SIZE + height * Globals.NUMBER_OFFSET), Quaternion.Euler(90.0f, 0.0f, 0.0f)) as GameObject;
            numberClone.transform.parent = transform;
            numberClone.GetComponent<TextMesh>().color = Color.black;
            numberClone.GetComponent<TextMesh>().text = northDoor.ToString();
            numberClone.GetComponent<TextMesh>().characterSize = 4 * height;
        }
        if (eastDoor > 0) {
            GameObject numberClone = Instantiate(debugNumber, new Vector3(x * width * Globals.ROOM_SIZE + width * Globals.NUMBER_OFFSET, 0.25f, y * height * Globals.ROOM_SIZE), Quaternion.Euler(90.0f, 0.0f, 0.0f)) as GameObject;
            numberClone.transform.parent = transform;
            numberClone.GetComponent<TextMesh>().color = Color.black;
            numberClone.GetComponent<TextMesh>().text = eastDoor.ToString();
            numberClone.GetComponent<TextMesh>().characterSize = 4 * height;
        }
        if (southDoor > 0) {
            GameObject numberClone = Instantiate(debugNumber, new Vector3(x * width * Globals.ROOM_SIZE, 0.25f, y * height * Globals.ROOM_SIZE - height * Globals.NUMBER_OFFSET), Quaternion.Euler(90.0f, 0.0f, 0.0f)) as GameObject;
            numberClone.transform.parent = transform;
            numberClone.GetComponent<TextMesh>().color = Color.black;
            numberClone.GetComponent<TextMesh>().text = southDoor.ToString();
            numberClone.GetComponent<TextMesh>().characterSize = 4 * height;
        }
        if (westDoor > 0) {
            GameObject numberClone = Instantiate(debugNumber, new Vector3(x * width * Globals.ROOM_SIZE - width * Globals.NUMBER_OFFSET, 0.25f, y * height * Globals.ROOM_SIZE), Quaternion.Euler(90.0f, 0.0f, 0.0f)) as GameObject;
            numberClone.transform.parent = transform;
            numberClone.GetComponent<TextMesh>().color = Color.black;
            numberClone.GetComponent<TextMesh>().text = westDoor.ToString();
            numberClone.GetComponent<TextMesh>().characterSize = 4 * height;
        }
    }

    // Activate the zone and its neighbors.
    public void ActivateZone() {
        if (northZone) {
            northZone.gameObject.SetActive(true);
        }
        if (eastZone) {
            eastZone.gameObject.SetActive(true);
        }
        if (southZone) {
            southZone.gameObject.SetActive(true);
        }
        if (westZone) {
            westZone.gameObject.SetActive(true);
        }
        this.gameObject.SetActive(true);
    }

    // Deactivate the zone and its neighbors.
    public void DeactivateZone() {
        if (northZone) {
            northZone.gameObject.SetActive(false);
        }
        if (eastZone) {
            eastZone.gameObject.SetActive(false);
        }
        if (southZone) {
            southZone.gameObject.SetActive(false);
        }
        if (westZone) {
            westZone.gameObject.SetActive(false);
        }
        this.gameObject.SetActive(false);
    }

}
