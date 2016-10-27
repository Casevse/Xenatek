using UnityEngine;
using System.Collections;

public class LevelResources : MonoBehaviour {

    // IMPORTANT. This class stores all the resources needed to instantiate when the level is generated.
    // ********** It manages which ones to chose, according to the profile's progress.

    // This GameObject is globally accesible in the scene.
    public static LevelResources instance;

    // Arrays with the different resources.
    public GameObject[] floors;
    public GameObject[] walls;
    public GameObject[] ceilings;
    public GameObject[] doors;
    public GameObject[] enemies;
    public GameObject[] puzzles;
    public GameObject[] missionItems;
    public GameObject[] teleports;
    public GameObject[] weapons;
    public GameObject[] ammoPacks;
    public GameObject[] powerUps;
    public GameObject[] healthPacks;
    // For the minimap.
    public GameObject[] minimapPieces;

    // Enums of the different types of resources. NOTE: This should not have to be necessary.
    public enum FloorTypes {
        RANDOM = -1, BASIC = 0
    }
    public enum WallTypes {
        RANDOM = -1, BASIC = 0, BASIC_DOOR = 1
    }
    public enum CeilingTypes {
        RANDOM = -1, BASIC = 0
    }
    public enum DoorTypes {
        RANDOM = -1, BASIC = 0
    }
    public enum EnemyTypes {
        RANDOM = -1, MOVING_LASER = 0, BUG = 1, TURRET = 2, RESTLESS_BALL = 3, BOT = 4, CHASE_TURRET = 5, CRUSHING = 6
    }
    public enum PuzzleTypes {
        RANDOM = -1, PANELS = 0, JUMP = 1, DARTBOARD = 2, ROTATORY = 3, BAR = 4
    }
    public enum MissionItemTypes {
        ORB = 0, FRAGMENT = 1
    }
    public enum TeleportTypes {
        EXIT = 0
    }
    public enum WeaponTypes {
        RANDOM = -1, SHOTGUN = 0, BAZOOKA = 1, RAY = 2, CLEANER = 3
    }
    public enum AmmoPackTypes {
        RANDOM = -1, SHOTGUN = 0, BAZOOKA = 1, RAY = 2, CLEANER = 3
    }
    public enum PowerUpTypes {
        RANDOM = -1, REGENERATION = 0, TURBO = 1, MEGAPROTECTION = 2, VAMPIBOT = 3, QUICKFIRE = 4
    }
    public enum HealthPackTypes {
        RANDOM = -1, SMALL = 0, MEDIUM = 1, BIG = 2
    }

    // For the minimap.
    public enum MinimapPieceType {
        FLOOR = 0, WALL = 1, NUMBER = 2
    }

    // Max index of each type of resurce.
    private int maxFloorIndex;
    private int maxWallIndex;
    private int maxCeilingIndex;
    private int maxDoorIndex;
    private int maxEnemyIndex;
    private int maxPuzzleIndex;
    private int maxMissionItemIndex;
    private int maxTeleportIndex;
    private int maxWeaponIndex;
    private int maxAmmoPackIndex;
    private int maxPowerUpIndex;
    private int maxHealthPackIndex;

    private void Awake() {
        instance = this;
    }

    // Calculated the max GameObject of each type to return, because it depends on the profile's progress.
    public void CalculateLimits(float percentage) {
        maxFloorIndex = floors.Length;
        maxWallIndex = walls.Length;
        maxCeilingIndex = ceilings.Length;
        maxDoorIndex = doors.Length;

        // Enemies.
        if (percentage < 0.1f) {
            maxEnemyIndex = (int)EnemyTypes.BUG + 1;
        }
        else if (percentage < 0.2f) {
            maxEnemyIndex = (int)EnemyTypes.TURRET + 1;
        }
        else if (percentage < 0.3f) {
            maxEnemyIndex = (int)EnemyTypes.RESTLESS_BALL + 1;
        }
        else if (percentage < 0.4f) {
            maxEnemyIndex = (int)EnemyTypes.BOT + 1;
        }
        else if (percentage < 0.5f) {
            maxEnemyIndex = (int)EnemyTypes.CHASE_TURRET + 1;
        }
        else {
            maxEnemyIndex = (int)EnemyTypes.CRUSHING + 1;
        }

        // Puzzles.
        if (percentage < 0.1f) {
            maxPuzzleIndex = 0;
        }
        else if (percentage < 0.25f) {
            maxPuzzleIndex = (int)PuzzleTypes.PANELS + 1;
        }
        else if (percentage < 0.4f) {
            maxPuzzleIndex = (int)PuzzleTypes.JUMP + 1;
        }
        else if (percentage < 0.55f) {
            maxPuzzleIndex = (int)PuzzleTypes.DARTBOARD + 1;
        }
        else if (percentage < 0.7f) {
            maxPuzzleIndex = (int)PuzzleTypes.ROTATORY + 1;
        }
        else {
            maxPuzzleIndex = (int)PuzzleTypes.BAR + 1;
        }

        maxMissionItemIndex = missionItems.Length;
        maxTeleportIndex = teleports.Length;

        // Weapons.
        if (percentage < 0.15f) {
            maxWeaponIndex = 0;
            maxAmmoPackIndex = 0;
        }
        if (percentage < 0.25f) {
            maxWeaponIndex = (int)WeaponTypes.SHOTGUN + 1;
            maxAmmoPackIndex = (int)AmmoPackTypes.SHOTGUN + 1;
        }
        else if (percentage < 0.35f) {
            maxWeaponIndex = (int)WeaponTypes.BAZOOKA + 1;
            maxAmmoPackIndex = (int)AmmoPackTypes.BAZOOKA + 1;
        }
        else if (percentage < 0.45f) {
            maxWeaponIndex = (int)WeaponTypes.RAY + 1;
            maxAmmoPackIndex = (int)AmmoPackTypes.RAY + 1;
        }
        else {
            maxWeaponIndex = (int)WeaponTypes.CLEANER + 1;
            maxAmmoPackIndex = (int)AmmoPackTypes.CLEANER + 1;
        }

        // Power Ups.
        if (percentage < 0.15f) {
            maxPowerUpIndex = 0;
        }
        else if (percentage < 0.3f) {
            maxPowerUpIndex = (int)PowerUpTypes.REGENERATION + 1;
        }
        else if (percentage < 0.45f) {
            maxPowerUpIndex = (int)PowerUpTypes.TURBO + 1;
        }
        else if (percentage < 0.6f) {
            maxPowerUpIndex = (int)PowerUpTypes.MEGAPROTECTION + 1;
        }
        else if (percentage < 0.75f) {
            maxPowerUpIndex = (int)PowerUpTypes.VAMPIBOT + 1;
        }
        else {
            maxPowerUpIndex = (int)PowerUpTypes.QUICKFIRE + 1;
        }

        maxHealthPackIndex = healthPacks.Length;
    }

    public GameObject CreateFloor(FloorTypes type, Transform parent, Vector3 position) {
        GameObject floor = null;
        if (floors.Length > 0) {
            floor = SearchResource(floors, (int)type, maxFloorIndex);
            if (floor) {
                floor.transform.parent = parent;
                floor.transform.localPosition = position;
            }
        }
        return floor;
    }

    public GameObject CreateWall(WallTypes type, Transform parent, Vector3 position, Quaternion rotation) {
        GameObject wall = null;
        if (walls.Length > 0) {
            wall = SearchResource(walls, (int)type, maxWallIndex);
            if (wall) {
                wall.transform.parent = parent;
                wall.transform.localPosition = position;
                wall.transform.localRotation = rotation;
            }
        }
        
        return wall;
    }

    public GameObject CreateCeiling(CeilingTypes type, Transform parent, Vector3 position) {
        GameObject ceiling = null;
        if (ceilings.Length > 0) {
            ceiling = SearchResource(ceilings, (int)type, maxCeilingIndex);
            if (ceiling) {
                ceiling.transform.parent = parent;
                ceiling.transform.localPosition = position;
            }
        }
        return ceiling;
    }

    public GameObject CreateDoor(DoorTypes type, Transform parent, Vector3 position, Quaternion rotation, int region) {
        GameObject door = null;
        if (doors.Length > 0) {
            door = SearchResource(doors, (int)type, maxDoorIndex);
            if (door) {
                door.transform.parent = parent;
                door.transform.localPosition = position;
                door.transform.localRotation = rotation;
            }
        }
        return door;
    }

    public GameObject CreateEnemy(EnemyTypes type, Transform parent, Vector3 position) {
        GameObject enemy = null;
        if (enemies.Length > 0) {
            enemy = SearchResource(enemies, (int)type, maxEnemyIndex);
            if (enemy) {
                enemy.transform.parent = parent;
                enemy.transform.localPosition = position;
            }
        }
        return enemy;
    }

    public GameObject CreatePuzzle(PuzzleTypes type, Door door, Transform parent, Vector3 position, Quaternion rotation) {
        GameObject puzzle = null;
        if (puzzles.Length > 0) {
            puzzle = SearchResource(puzzles, (int)type, maxPuzzleIndex);
            if (puzzle) {
                puzzle.transform.parent = parent;
                puzzle.transform.localPosition = position;
                puzzle.transform.localRotation = rotation;
                puzzle.GetComponent<PuzzleController>().SetDoor(door);
                door.Close();
            }
        }
        return puzzle;
    }

    public GameObject CreateMissionItem(MissionItemTypes type, Transform parent, Vector3 position) {
        GameObject missionItem = null;
        if (missionItems.Length > 0) {
            missionItem = SearchResource(missionItems, (int)type, maxMissionItemIndex);
            if (missionItem) {
                missionItem.transform.parent = parent;
                missionItem.transform.localPosition = position;
            }
        }
        return missionItem;
    }

    public GameObject CreateTeleport(TeleportTypes type, Transform parent, Vector3 position) {
        GameObject teleport = null;
        if (teleports.Length > 0) {
            teleport = SearchResource(teleports, (int)type, maxTeleportIndex);
            if (teleport) {
                teleport.transform.parent = parent;
                teleport.transform.localPosition = position;
            }
        }
        return teleport;
    }

    public GameObject CreateWeapon(WeaponTypes type, Transform parent, Vector3 position) {
        GameObject weapon = null;
        if (weapons.Length > 0) {
            weapon = SearchResource(weapons, (int)type, maxWeaponIndex);
            if (weapon) {
                weapon.transform.parent = parent;
                weapon.transform.localPosition = position;
            }
        }
        return weapon;
    }

    public GameObject CreateAmmoPack(AmmoPackTypes type, Transform parent, Vector3 position) {
        GameObject ammoPack = null;
        if (ammoPacks.Length > 0) {
            ammoPack = SearchResource(ammoPacks, (int)type, maxAmmoPackIndex);
            if (ammoPack) {
                ammoPack.transform.parent = parent;
                ammoPack.transform.localPosition = position;
            }
        }
        return ammoPack;
    }

    public GameObject CreatePowerUp(PowerUpTypes type, Transform parent, Vector3 position) {
        GameObject powerUp = null;
        if (powerUps.Length > 0) {
            powerUp = SearchResource(powerUps, (int)type, maxPowerUpIndex);
            if (powerUp) {
                powerUp.transform.parent = parent;
                powerUp.transform.localPosition = position;
            }
        }
        return powerUp;
    }

    public GameObject CreateHealthPack(HealthPackTypes type, Transform parent, Vector3 position) {
        GameObject healthPack = null;
        if (healthPacks.Length > 0) {
            healthPack = SearchResource(healthPacks, (int)type, maxHealthPackIndex);
            if (healthPack) {
                healthPack.transform.parent = parent;
                healthPack.transform.localPosition = position;
            }
        }
        return healthPack;
    }

    // For the minimap.
    public GameObject CreateMinimapPiece(MinimapPieceType type, Transform parent, Vector3 position, Quaternion rotation) {
        GameObject minimapPiece = null;
        if (minimapPieces.Length > 0) {
            minimapPiece = SearchResource(minimapPieces, (int)type, minimapPieces.Length);
            if (minimapPiece) {
                minimapPiece.transform.parent = parent;
                minimapPiece.transform.localPosition = position;
                minimapPiece.transform.localRotation = rotation;

                if (type == MinimapPieceType.NUMBER) {
                    minimapPiece.transform.Translate(0.0f, 7.5f, 0.0f);
                }
            }
        }
        return minimapPiece;
    }

    private GameObject SearchResource(GameObject[] array, int index, int maxIndex) {
        GameObject resource = null;
        if (index < maxIndex && maxIndex > 0) {
            if (index == -1) {
                index = Random.Range(0, maxIndex);
            }
            resource = Instantiate(array[index]) as GameObject;
        }
        return resource;
    }

}