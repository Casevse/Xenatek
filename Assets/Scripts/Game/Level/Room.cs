using UnityEngine;
using System.Collections;

public class Room : MonoBehaviour {

    public int walls = 0;
    public int x = 0;
    public int y = 0;
    public bool isNull = true;
    public bool isCentral = false;
    // NOTE: -1 does not have a door, 0 door without key, > 0 door with key.
    public int northDoor = -1;
    public int eastDoor = -1;
    public int southDoor = -1;
    public int westDoor = -1;
    public bool isPath = false;
    public int filling = 0;

    // Bit Flags to check the overlap of the elements.
    private enum SpawnPositions {
        NORTH_WEST = 0x1, NORTH = 0x2, NORTH_EAST = 0x4, WEST = 0x8, CENTER = 0x10, EAST = 0x20, SOUTH_WEST = 0x40, SOUTH = 0x80, SOUTH_EAST = 0x100 
    }
    private int spawns = 0;

    // For DEBUG.
    public GameObject debugFloor;
    public GameObject debugLine;
    public GameObject debugWall;
    public GameObject debugCeiling;
    public GameObject debugDoor;

    private Zone zone;

    public void Generate(Zone zone) {
        this.zone = zone;

        // First generate the structural elements.
        GenerateFloor();
        if (walls > 0) {
            GenerateWalls();
        }
        GenerateCeiling();
        GenerateDoors();

        // NOTE: The central part of the room could be reserved only for the mission's objective.
        // NOTE: The filling rooms could be reserved for weapons, ammo, health, and power ups.
        // NOTE: The enemies can appear in whatever room.

        // The rest of the elements are placed in the spawn points.
        GenerateMissionItems();
        GenerateTeleports();
        GenerateItems();    // Weapons, ammo, power ups, health.
        GenerateEnemies();

        // DEBUG.
        // DrawDebugRoom();
    }

    private void GenerateFloor() {
        // At first place, there is only one floor.
        LevelResources.instance.CreateFloor(LevelResources.FloorTypes.BASIC, transform, Vector3.zero);
    }

    private void GenerateWalls() {
        // NOTE: Do not generate for one side if it is a road.
        if ((walls & (int)Direction.NORTH) == 0) {
            LevelResources.instance.CreateWall(LevelResources.WallTypes.BASIC, transform, new Vector3(0.0f, 0.0f, Globals.ROOM_SIZE / 2.0f), Quaternion.identity);
        }
        else if (northDoor >= 0) {
            LevelResources.instance.CreateWall(LevelResources.WallTypes.BASIC_DOOR, transform, new Vector3(0.0f, 0.0f, Globals.ROOM_SIZE / 2.0f), Quaternion.identity);
        }
        if ((walls & (int)Direction.EAST) == 0) {
            LevelResources.instance.CreateWall(LevelResources.WallTypes.BASIC, transform, new Vector3(Globals.ROOM_SIZE / 2.0f, 0.0f, 0.0f), Quaternion.Euler(0.0f, 90.0f, 0.0f));
        }
        else if (eastDoor >= 0) {
            LevelResources.instance.CreateWall(LevelResources.WallTypes.BASIC_DOOR, transform, new Vector3(Globals.ROOM_SIZE / 2.0f, 0.0f, 0.0f), Quaternion.Euler(0.0f, 90.0f, 0.0f));
        }
        if ((walls & (int)Direction.SOUTH) == 0) {
            LevelResources.instance.CreateWall(LevelResources.WallTypes.BASIC, transform, new Vector3(0.0f, 0.0f, -Globals.ROOM_SIZE / 2.0f), Quaternion.Euler(0.0f, 180.0f, 0.0f));
        }
        else if (southDoor >= 0) {
            LevelResources.instance.CreateWall(LevelResources.WallTypes.BASIC_DOOR, transform, new Vector3(0.0f, 0.0f, -Globals.ROOM_SIZE / 2.0f), Quaternion.Euler(0.0f, 180.0f, 0.0f));
        }
        if ((walls & (int)Direction.WEST) == 0) {
            LevelResources.instance.CreateWall(LevelResources.WallTypes.BASIC, transform, new Vector3(-Globals.ROOM_SIZE / 2.0f, 0.0f, 0.0f), Quaternion.Euler(0.0f, 270.0f, 0.0f));
        }
        else if (westDoor >= 0) {
            LevelResources.instance.CreateWall(LevelResources.WallTypes.BASIC_DOOR, transform, new Vector3(-Globals.ROOM_SIZE / 2.0f, 0.0f, 0.0f), Quaternion.Euler(0.0f, 270.0f, 0.0f));
        }
    }

    private void GenerateCeiling() {
        LevelResources.instance.CreateCeiling(LevelResources.CeilingTypes.BASIC, transform, new Vector3(0.0f, Globals.HALF_ROOM_SIZE, 0.0f));
    }

    private void GenerateDoors() {
        // NOTE: 25 % change to generate a puzzle for the door.
        bool putPuzzle = (Random.value < 0.25f) ? true : false;

        // NOTE: All this code could be compressed with methods.

        if (northDoor >= 0) {
            GenerateDoor(new Vector3(0.0f, 0.0f, Globals.ROOM_SIZE / 2.0f), Quaternion.identity, putPuzzle, northDoor, zone.northZone.region, zone.northZone.order);
        }
        if (eastDoor >= 0) {
            GenerateDoor(new Vector3(Globals.ROOM_SIZE / 2.0f, 0.0f, 0.0f), Quaternion.Euler(0.0f, 90.0f, 0.0f), putPuzzle, eastDoor, zone.eastZone.region, zone.eastZone.order);
        }
        if (southDoor >= 0) {
            GenerateDoor(new Vector3(0.0f, 0.0f, -Globals.ROOM_SIZE / 2.0f), Quaternion.Euler(0.0f, 180.0f, 0.0f), putPuzzle, southDoor, zone.southZone.region, zone.southZone.order);
        }
        if (westDoor >= 0) {
            GenerateDoor(new Vector3(-Globals.ROOM_SIZE / 2.0f, 0.0f, 0.0f), Quaternion.Euler(0.0f, 270.0f, 0.0f), putPuzzle, westDoor, zone.westZone.region, zone.westZone.order);
        }
    }

    private void GenerateDoor(Vector3 position, Quaternion rotation, bool putPuzzle, int doorKey, int nextRegion, int nextOrder) {
        GameObject door = LevelResources.instance.CreateDoor(LevelResources.DoorTypes.BASIC, transform, position, rotation, doorKey);
        if (doorKey > 0) {
            door.GetComponent<Door>().Close();
            // The door always corresponds to one less that the next region.
            MissionController.instance.RegisterRegionDoor(door.GetComponent<Door>(), nextRegion - 1);
            door.GetComponent<Door>().ShowRegionText(nextRegion);
        }
        else {
            door.GetComponent<Door>().Open();
            if (zone.region > nextRegion) {
                door.GetComponent<Door>().ShowRegionText(nextRegion);
            }
        }
        if (putPuzzle && zone.region == nextRegion && zone.order < nextOrder) {
            LevelResources.instance.CreatePuzzle(LevelResources.PuzzleTypes.RANDOM, door.GetComponent<Door>(), transform, position, rotation);
        }
    }

    private void GenerateMissionItems() {
        Mission mission = MissionController.instance.GetMissionByRegion(zone.region);
        if (mission is OrbMission) {
            if (isCentral && zone.key > 0) {
                Vector3 position = GetAvailableSpawnPosition(SpawnPositions.CENTER);    // Because of the generation order, this should not return null.
                GameObject orb = LevelResources.instance.CreateMissionItem(LevelResources.MissionItemTypes.ORB, transform, position);
                mission.RegisterMissionElement(orb);
            }
        }
        else if (mission is GatherMission) {
            bool generate = true;
            if (mission.RequirementIsReached()) {
                generate = (Random.value > 0.75f) ? true : false;
            }
            if (generate) {
                do {
                    Vector3 position = GetAvailableSpawnPosition(SpawnPositions.CENTER);
                    position.y += 0.75f;
                    GameObject fragment = LevelResources.instance.CreateMissionItem(LevelResources.MissionItemTypes.FRAGMENT, transform, position);
                    mission.RegisterMissionElement(fragment);
                } while (mission.RequirementIsReached() == false);
            }
        }
    }

    private void GenerateTeleports() {
        if (isCentral && zone.isEnd) {
            Vector3 position = GetAvailableSpawnPosition(SpawnPositions.CENTER);    // Because of the generation order, this should not return null.
            GameObject exitTeleport = LevelResources.instance.CreateTeleport(LevelResources.TeleportTypes.EXIT, transform, position);
            exitTeleport.GetComponent<Teleport>().Close();
            MissionController.instance.RegisterExitTeleport(exitTeleport.GetComponent<Teleport>());
        }
    }

    private void GenerateItems() {
        // Generate them in the filling zones.
        if (filling > 0 && Random.value > 0.75f) {
            Vector3 position = GetAvailableSpawnPosition(SpawnPositions.CENTER);
            position.y = 1.0f;
            switch (Random.Range(0, 4)) {
                case 0:
                    LevelResources.instance.CreateWeapon(LevelResources.WeaponTypes.RANDOM, transform, position);
                    break;
                case 1:
                    LevelResources.instance.CreateAmmoPack(LevelResources.AmmoPackTypes.RANDOM, transform, position);
                    break;
                case 2:
                    LevelResources.instance.CreatePowerUp(LevelResources.PowerUpTypes.RANDOM, transform, position);
                    break;
                case 3:
                    LevelResources.instance.CreateHealthPack(LevelResources.HealthPackTypes.RANDOM, transform, position);
                    break;
            }
        }
    }

    private void GenerateEnemies() {
        float random = Random.value;
        bool isMassacre = MissionController.instance.GetMissionByRegion(zone.region) is MassacreMission;

        // If the zone has the massacre mission, put at least one enemy.
        if (isMassacre) {
            if (MissionController.instance.GetMissionByRegion(zone.region).RequirementIsReached() == false) {
                random = 1.0f;
            }
        }

        if (random > 0.75f) {
            Vector3 position = GetAvailableSpawnPosition(SpawnPositions.CENTER);
            GameObject enemy = LevelResources.instance.CreateEnemy(LevelResources.EnemyTypes.RANDOM, transform, position);

            // This depends one the enemy type. NOTE: GetComponent could be slow. This could be improved in the LevelResources class.
            if (enemy.GetComponent<MovingLaser>()) {
                // MOVING LASER.
                MovingLaser laser = enemy.GetComponent<MovingLaser>();
                int axes = Random.Range(0, 3);  // 0 - horizontal, 1 - vertical, 2 - both.
                if (axes == 0) {
                    laser.horizontalSpeed = 1.0f + Random.value;
                }
                else if (axes == 1) {
                    laser.verticalSpeed = 1.0f + Random.value;
                }
                else if (axes == 2) {
                    laser.horizontalSpeed = 1.0f + Random.value;
                    laser.verticalSpeed = 1.0f + Random.value; ;
                }

                // NOTE: In any case, only the room walls matter, so the spawn position is not needed.
                if ((walls & (int)Direction.NORTH) == 0) {
                    enemy.transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                    if (axes == 0) {
                        enemy.transform.localPosition = new Vector3(0.0f, Globals.QUARTER_ROOM_SIZE * 0.5f, Globals.HALF_ROOM_SIZE - 0.25f);
                    }
                    else {
                        enemy.transform.localPosition = new Vector3(0.0f, Globals.QUARTER_ROOM_SIZE, Globals.HALF_ROOM_SIZE - 0.25f);
                    }
                }
                else if ((walls & (int)Direction.EAST) == 0) {
                    enemy.transform.localRotation = Quaternion.Euler(0.0f, 270.0f, 0.0f);
                    if (axes == 0) {
                        enemy.transform.localPosition = new Vector3(Globals.HALF_ROOM_SIZE - 0.25f, Globals.QUARTER_ROOM_SIZE * 0.5f, 0.0f);
                    }
                    else {
                        enemy.transform.localPosition = new Vector3(Globals.HALF_ROOM_SIZE - 0.25f, Globals.QUARTER_ROOM_SIZE, 0.0f);
                    }
                }
                else if ((walls & (int)Direction.SOUTH) == 0) {
                    if (axes == 0) {
                        enemy.transform.localPosition = new Vector3(0.0f, Globals.QUARTER_ROOM_SIZE * 0.5f, -Globals.HALF_ROOM_SIZE + 0.25f);
                    }
                    else {
                        enemy.transform.localPosition = new Vector3(0.0f, Globals.QUARTER_ROOM_SIZE, -Globals.HALF_ROOM_SIZE + 0.25f);
                    }
                }
                else if ((walls & (int)Direction.WEST) == 0) {
                    enemy.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                    if (axes == 0) {
                        enemy.transform.localPosition = new Vector3(-Globals.HALF_ROOM_SIZE + 0.25f, Globals.QUARTER_ROOM_SIZE * 0.5f, 0.0f);
                    }
                    else {
                        enemy.transform.localPosition = new Vector3(-Globals.HALF_ROOM_SIZE + 0.25f, Globals.QUARTER_ROOM_SIZE, 0.0f);
                    }
                }
                else {
                    enemy.transform.localPosition = new Vector3(0.0f, Globals.HALF_ROOM_SIZE - 0.25f, 0.0f);
                    enemy.transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                }
            }
            else if (enemy.GetComponent<Turret>() || enemy.GetComponent<ChaseTurret>()) {
                // TURRET. CHASE TURRET.
                if (Random.value > 0.5f) {
                    enemy.transform.localPosition = new Vector3(position.x, Globals.HALF_ROOM_SIZE - 0.35f, position.z);
                    enemy.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
                }
                else {
                    enemy.transform.localPosition = new Vector3(position.x, 0.35f, position.z);
                }
            }
            else {
                // BUG. RESTLESS BALL. BOT. CRUSHING.
                enemy.transform.localPosition = new Vector3(position.x, 1.0f, position.z);
            }

            // Register the enemy if the zone has the massacre mission.
            if (isMassacre) {
                MissionController.instance.GetMissionByRegion(zone.region).RegisterMissionElement(enemy);
            }
        }
    }

    private void DrawDebugRoom() {
        // Place the floor.
        GameObject floorClone = Instantiate(debugFloor) as GameObject;
        floorClone.transform.parent = transform;
        floorClone.transform.localPosition = new Vector3(0.0f, 0.1f, 0.0f);
        if (isPath) {
            floorClone.GetComponent<Renderer>().material.color = Color.cyan;
        }
        if (isCentral) {
            floorClone.GetComponent<Renderer>().material.color = Color.red;
        }
        if (filling > 0) {
            floorClone.GetComponent<Renderer>().material.color = Color.yellow;
        }
        if (northDoor == 0 || eastDoor == 0 || southDoor == 0 || westDoor == 0) {
            floorClone.GetComponent<Renderer>().material.color = Color.green;
        }
        if (northDoor > 0 || eastDoor > 0 || southDoor > 0 || westDoor > 0) {
            floorClone.GetComponent<Renderer>().material.color = Color.magenta;
        }

        // Place the walls.
        if (walls > 0) {
            if ((walls & (int)Direction.NORTH) == 0) {
                GameObject lineClone = Instantiate(debugWall) as GameObject;
                lineClone.transform.parent = transform;
                lineClone.transform.localPosition = new Vector3(0.0f, 0.3f, Globals.ROOM_SIZE / 2.0f);
            }
            else if (northDoor >= 0) {
                GameObject doorClone = Instantiate(debugDoor) as GameObject;
                doorClone.transform.parent = transform;
                doorClone.transform.localPosition = new Vector3(0.0f, 0.3f, Globals.ROOM_SIZE / 2.0f);
            }
            if ((walls & (int)Direction.EAST) == 0) {
                GameObject lineClone = Instantiate(debugWall) as GameObject;
                lineClone.transform.parent = transform;
                lineClone.transform.localPosition = new Vector3(Globals.ROOM_SIZE / 2.0f, 0.3f, 0.0f);
                lineClone.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
            }
            else if (eastDoor >= 0) {
                GameObject doorClone = Instantiate(debugDoor) as GameObject;
                doorClone.transform.parent = transform;
                doorClone.transform.localPosition = new Vector3(Globals.ROOM_SIZE / 2.0f, 0.3f, 0.0f);
                doorClone.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
            }
            if ((walls & (int)Direction.SOUTH) == 0) {
                GameObject lineClone = Instantiate(debugWall) as GameObject;
                lineClone.transform.parent = transform;
                lineClone.transform.localPosition = new Vector3(0.0f, 0.3f, -Globals.ROOM_SIZE / 2.0f);
            }
            else if (southDoor >= 0) {
                GameObject doorClone = Instantiate(debugDoor) as GameObject;
                doorClone.transform.parent = transform;
                doorClone.transform.localPosition = new Vector3(0.0f, 0.3f, -Globals.ROOM_SIZE / 2.0f);
            }
            if ((walls & (int)Direction.WEST) == 0) {
                GameObject lineClone = Instantiate(debugWall) as GameObject;
                lineClone.transform.parent = transform;
                lineClone.transform.localPosition = new Vector3(-Globals.ROOM_SIZE / 2.0f, 0.3f, 0.0f);
                lineClone.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
            }
            else if (westDoor >= 0) {
                GameObject doorClone = Instantiate(debugDoor) as GameObject;
                doorClone.transform.parent = transform;
                doorClone.transform.localPosition = new Vector3(-Globals.ROOM_SIZE / 2.0f, 0.3f, 0.0f);
                doorClone.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
            }
        }
    }

    Vector3 GetAvailableSpawnPosition(SpawnPositions preference) {
        Vector3 position = new Vector3(-1.0f, -1.0f, -1.0f);    // El valor inicial indica que no hay una posición disponible.
        SpawnPositions spawnChosed = 0;

        // First check if the preference is available.
        if ((spawns & (int)preference) == 0) {
            spawnChosed = preference;
            spawns |= (int)spawnChosed;
        }
        else {
            // If not, search for one available.
            // NOTE: This could be shuffled to not always follow the same order.
            for (SpawnPositions spawn = SpawnPositions.NORTH_WEST; spawn <= SpawnPositions.SOUTH_EAST; spawn += (int)spawn) {
                if ((spawns & (int)spawn) == 0) {
                    spawnChosed = spawn;
                    spawns |= (int)spawnChosed;
                    break;
                }
            }
        }

        if (spawnChosed > 0) {
            switch (spawnChosed) {
                case SpawnPositions.NORTH_WEST:
                    position = new Vector3(-Globals.QUARTER_ROOM_SIZE, 0.0f, Globals.QUARTER_ROOM_SIZE);
                    break;
                case SpawnPositions.NORTH:
                    position = new Vector3(0.0f, 0.0f, Globals.QUARTER_ROOM_SIZE);
                    break;
                case SpawnPositions.NORTH_EAST:
                    position = new Vector3(Globals.QUARTER_ROOM_SIZE, 0.0f, Globals.QUARTER_ROOM_SIZE);
                    break;
                case SpawnPositions.WEST:
                    position = new Vector3(-Globals.QUARTER_ROOM_SIZE, 0.0f, 0.0f);
                    break;
                case SpawnPositions.CENTER:
                    position = Vector3.zero;
                    break;
                case SpawnPositions.EAST:
                    position = new Vector3(Globals.QUARTER_ROOM_SIZE, 0.0f, 0.0f);
                    break;
                case SpawnPositions.SOUTH_WEST:
                    position = new Vector3(-Globals.QUARTER_ROOM_SIZE, 0.0f, -Globals.QUARTER_ROOM_SIZE);
                    break;
                case SpawnPositions.SOUTH:
                    position = new Vector3(0.0f, 0.0f, -Globals.QUARTER_ROOM_SIZE);
                    break;
                case SpawnPositions.SOUTH_EAST:
                    position = new Vector3(Globals.QUARTER_ROOM_SIZE, 0.0f, -Globals.QUARTER_ROOM_SIZE);
                    break;
            }
        }

        return position;
    }

}