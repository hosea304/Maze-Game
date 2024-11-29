using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    [Header("Basic Settings")]
    [SerializeField] private Difficulty difficulty = Difficulty.Medium;
    [SerializeField] private GameObject wallPrefab;

    [Header("Difficulty Presets")]
    [SerializeField] private DifficultySettings easySettings = new DifficultySettings(10, 10, 0.2f, 0.1f, 2f);
    [SerializeField] private DifficultySettings mediumSettings = new DifficultySettings(15, 15, 0.3f, 0.15f, 2.5f);
    [SerializeField] private DifficultySettings hardSettings = new DifficultySettings(20, 20, 0.4f, 0.2f, 3f);

    [System.Serializable]
    public class DifficultySettings
    {
        public int width = 15;
        public int height = 15;
        public float complexityFactor = 0.3f;
        public float extraPathProbability = 0.15f;
        public float wallHeight = 2f;

        public DifficultySettings(int w, int h, float cf, float epp, float wh)
        {
            width = w;
            height = h;
            complexityFactor = cf;
            extraPathProbability = epp;
            wallHeight = wh;
        }
    }

    [Header("Customization")]
    [SerializeField] private Vector3 startPosition = Vector3.zero;
    [SerializeField] private float wallLength = 2f;
    [SerializeField] private bool addEntranceExit = true;
    [SerializeField] private Material entranceMaterial;
    [SerializeField] private Material exitMaterial;
    [SerializeField] private Material wallMaterial;

    private Cell[,] maze;
    private Vector3 mazeOrigin;
    private List<GameObject> allWalls = new List<GameObject>();
    private DifficultySettings currentSettings;

    private class Cell
    {
        public bool visited = false;
        public bool[] walls = new bool[4] { true, true, true, true }; // Top, Right, Bottom, Left
        public int distanceFromStart = 0;
        public bool isDeadEnd = false;
        public bool isPath = false;
    }

    void Start()
    {
        SetDifficulty(difficulty);
        mazeOrigin = transform.position + startPosition;
        GenerateMaze();
    }

    public void SetDifficulty(Difficulty newDifficulty)
    {
        difficulty = newDifficulty;
        switch (difficulty)
        {
            case Difficulty.Easy:
                currentSettings = easySettings;
                break;
            case Difficulty.Medium:
                currentSettings = mediumSettings;
                break;
            case Difficulty.Hard:
                currentSettings = hardSettings;
                break;
        }

        UpdatePlatformSize();
    }

    void UpdatePlatformSize()
    {
        // Menyesuaikan ukuran platform dengan ukuran maze
        float platformSize = currentSettings.width * wallLength;
        Transform platform = transform.Find("Platform");
        if (platform != null)
        {
            platform.localScale = new Vector3(platformSize, 1, platformSize);
        }
    }

    public void RegenerateMaze()
    {
        foreach (GameObject wall in allWalls)
        {
            if (wall != null)
                Destroy(wall);
        }
        allWalls.Clear();

        GenerateMaze();
    }

    void GenerateMaze()
    {
        InitializeMaze();

        int startX = currentSettings.width / 2;
        int startY = currentSettings.height / 2;

        GeneratePath(startX, startY);
        AddComplexity();

        if (addEntranceExit)
        {
            CreateMainPath();
            CreateEntranceAndExit();
        }

        CreateWalls();
    }

    void InitializeMaze()
    {
        maze = new Cell[currentSettings.width, currentSettings.height];
        for (int x = 0; x < currentSettings.width; x++)
        {
            for (int y = 0; y < currentSettings.height; y++)
            {
                maze[x, y] = new Cell();
            }
        }
    }

    void GeneratePath(int x, int y)
    {
        maze[x, y].visited = true;

        List<int> directions = new List<int> { 0, 1, 2, 3 };
        ShuffleList(directions);

        int[] dx = { 0, 1, 0, -1 };
        int[] dy = { 1, 0, -1, 0 };

        foreach (int direction in directions)
        {
            int newX = x + dx[direction];
            int newY = y + dy[direction];

            if (IsValidCell(newX, newY) && !maze[newX, newY].visited)
            {
                if (Random.value > currentSettings.complexityFactor)
                {
                    maze[x, y].walls[direction] = false;
                    maze[newX, newY].walls[(direction + 2) % 4] = false;
                    maze[newX, newY].distanceFromStart = maze[x, y].distanceFromStart + 1;
                    GeneratePath(newX, newY);
                }
                else
                {
                    maze[newX, newY].isDeadEnd = true;
                    maze[newX, newY].visited = true;
                }
            }
        }
    }

    void AddComplexity()
    {
        for (int x = 1; x < currentSettings.width - 1; x++)
        {
            for (int y = 1; y < currentSettings.height - 1; y++)
            {
                if (Random.value < currentSettings.extraPathProbability)
                {
                    int direction = Random.Range(0, 4);
                    int newX = x + (direction == 1 ? 1 : direction == 3 ? -1 : 0);
                    int newY = y + (direction == 0 ? 1 : direction == 2 ? -1 : 0);

                    if (IsValidCell(newX, newY))
                    {
                        maze[x, y].walls[direction] = false;
                        maze[newX, newY].walls[(direction + 2) % 4] = false;
                    }
                }
            }
        }
    }

    void CreateMainPath()
    {
        // Difficulty affects path straightness
        float straightPathProbability = difficulty switch
        {
            Difficulty.Easy => 0.7f,    // More straight paths
            Difficulty.Medium => 0.5f,   // Mixed
            Difficulty.Hard => 0.3f,     // More winding paths
            _ => 0.5f
        };

        int currentX = 0;
        int currentY = 0;
        int targetX = currentSettings.width - 1;
        int targetY = currentSettings.height - 1;

        while (currentX < targetX || currentY < targetY)
        {
            maze[currentX, currentY].isPath = true;

            if (currentX < targetX && Random.value < straightPathProbability)
            {
                maze[currentX, currentY].walls[1] = false;
                maze[currentX + 1, currentY].walls[3] = false;
                currentX++;
            }
            else if (currentY < targetY)
            {
                maze[currentX, currentY].walls[0] = false;
                maze[currentX, currentY + 1].walls[2] = false;
                currentY++;
            }
        }
    }

    void CreateWalls()
    {
        for (int x = 0; x < currentSettings.width; x++)
        {
            for (int y = 0; y < currentSettings.height; y++)
            {
                Vector3 cellPosition = mazeOrigin + new Vector3(x * wallLength, 0, y * wallLength);

                if (maze[x, y].walls[0])
                    CreateWall(cellPosition, Quaternion.Euler(0, 0, 0));
                if (maze[x, y].walls[1])
                    CreateWall(cellPosition, Quaternion.Euler(0, 90, 0));
                if (maze[x, y].walls[2] && y == 0)
                    CreateWall(cellPosition, Quaternion.Euler(0, 180, 0));
                if (maze[x, y].walls[3] && x == 0)
                    CreateWall(cellPosition, Quaternion.Euler(0, 270, 0));

                if ((x == 0 && y == 0) || (x == currentSettings.width - 1 && y == currentSettings.height - 1))
                    CreatePillar(cellPosition);
            }
        }
    }

    void CreateWall(Vector3 position, Quaternion rotation)
    {
        GameObject wall = Instantiate(wallPrefab, position, rotation);
        wall.transform.parent = transform;

        Vector3 scale = wall.transform.localScale;
        scale.y = currentSettings.wallHeight;
        wall.transform.localScale = scale;

        allWalls.Add(wall);

        // Pengecekan yang lebih toleran untuk entrance dan exit
        if (Vector3.Distance(position, mazeOrigin) < 0.1f && Mathf.Abs(rotation.eulerAngles.y - 270) < 0.1f)
        {
            SetWallMaterial(wall, entranceMaterial);
        }
        else if (Vector3.Distance(position, mazeOrigin + new Vector3((currentSettings.width - 1) * wallLength, 0, (currentSettings.height - 1) * wallLength)) < 0.1f
                 && Mathf.Abs(rotation.eulerAngles.y - 90) < 0.1f)
        {
            SetWallMaterial(wall, exitMaterial);
        }
        else
        {
            SetWallMaterial(wall, wallMaterial);
        }
    }

    void CreateEntranceAndExit()
    {
        // Create entrance (bottom-left)
        maze[0, 0].walls[3] = false;

        // Create exit (top-right)
        maze[currentSettings.width - 1, currentSettings.height - 1].walls[1] = false;
    }

    // Helper methods
    bool IsValidCell(int x, int y)
    {
        return x >= 0 && x < currentSettings.width && y >= 0 && y < currentSettings.height;
    }

    void CreatePillar(Vector3 position)
    {
        GameObject pillar = Instantiate(wallPrefab, position, Quaternion.identity);
        pillar.transform.parent = transform;

        Vector3 scale = new Vector3(wallLength * 0.2f, currentSettings.wallHeight, wallLength * 0.2f);
        pillar.transform.localScale = scale;

        allWalls.Add(pillar);
    }

    void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    void SetWallMaterial(GameObject wall, Material material)
    {
        if (material != null && wall.GetComponent<Renderer>() != null)
            wall.GetComponent<Renderer>().material = material;
    }
}