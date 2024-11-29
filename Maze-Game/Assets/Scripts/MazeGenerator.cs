using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Extreme
    }

    [Header("Basic Settings")]
    [SerializeField] private Difficulty difficulty = Difficulty.Medium;
    [SerializeField] private GameObject wallPrefab;

    [Header("Difficulty Presets")]
    [SerializeField] private DifficultySettings easySettings = new DifficultySettings(10, 10, 0.2f, 0.1f, 2f);
    [SerializeField] private DifficultySettings mediumSettings = new DifficultySettings(15, 15, 0.3f, 0.15f, 2.5f);
    [SerializeField] private DifficultySettings hardSettings = new DifficultySettings(20, 20, 0.4f, 0.2f, 3f);
    [SerializeField] private DifficultySettings extremeSettings = new DifficultySettings(25, 25, 0.7f, 0.05f, 3.5f);

    [System.Serializable]
    public class DifficultySettings
    {
        public int width = 15;
        public int height = 15;
        public float complexityFactor = 0.3f;
        public float extraPathProbability = 0.15f;
        public float wallHeight = 2f;
        public float deadEndProbability = 0.3f;

        public DifficultySettings(int w, int h, float cf, float epp, float wh)
        {
            width = w;
            height = h;
            complexityFactor = cf;
            extraPathProbability = epp;
            wallHeight = wh;
            deadEndProbability = cf * 0.5f;
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
            case Difficulty.Extreme:
                currentSettings = extremeSettings;
                break;
        }
        UpdatePlatformSize();
    }

    void UpdatePlatformSize()
    {
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
        // Memastikan pintu masuk terbuka sebelum mulai generate
        InitializeMaze();
        ForceOpenEntrance();

        bool validMaze = false;
        int attempts = 0;
        const int maxAttempts = 10;

        while (!validMaze && attempts < maxAttempts)
        {
            InitializeMaze();

            // Langsung buka pintu masuk setelah inisialisasi
            ForceOpenEntrance();

            int startX = currentSettings.width / 2;
            int startY = currentSettings.height / 2;

            GeneratePath(startX, startY);
            AddComplexity();

            if (addEntranceExit)
            {
                CreateMainPath();
                CreateEntranceAndExit();

                validMaze = ValidatePath();
                ForceOpenEntrance();
            }
            else
            {
                validMaze = true;
            }

            attempts++;

            if (!validMaze && attempts < maxAttempts)
            {
                Debug.Log($"Attempt {attempts}: Invalid maze, regenerating...");
            }
        }

        if (!validMaze)
        {
            Debug.LogWarning("Failed to generate valid maze. Forcing main path.");
            ForceMainPath();
        }

        ForceOpenEntrance();
        CreateWalls();
        RemoveEntranceWall();
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

    private void ForceOpenEntrance()
    {
        // Hapus semua dinding di pintu masuk
        maze[0, 0].walls[3] = false; // Left wall (entrance)
        maze[0, 0].isPath = true;

        if (currentSettings.width > 1)
        {
            maze[1, 0].walls[3] = false;
            maze[1, 0].isPath = true;
        }
    }

    private void RemoveEntranceWall()
    {
        Vector3 entrancePosition = mazeOrigin;

        foreach (GameObject wall in allWalls.ToArray())
        {
            if (wall != null)
            {
                Vector3 wallPosition = wall.transform.position;
                float distanceToEntrance = Vector3.Distance(wallPosition, entrancePosition);
                float angleY = wall.transform.rotation.eulerAngles.y;

                if (distanceToEntrance < wallLength * 0.5f &&
                    (Mathf.Abs(angleY - 270f) < 5f || Mathf.Abs(angleY - (-90f)) < 5f))
                {
                    Debug.Log("Removing entrance wall");
                    allWalls.Remove(wall);
                    Destroy(wall);
                }
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

        if (difficulty == Difficulty.Extreme)
        {
            foreach (int direction in directions)
            {
                int newX = x + dx[direction];
                int newY = y + dy[direction];

                if (IsValidCell(newX, newY) && !maze[newX, newY].visited)
                {
                    if (Random.value > currentSettings.deadEndProbability)
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

                        if (Random.value < 0.5f)
                        {
                            CreateFakePath(newX, newY);
                        }
                    }
                }
            }
        }
        else
        {
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
    }

    void CreateFakePath(int startX, int startY)  // Mengubah parameter names
    {
        int maxFakeLength = 3;
        int currentLength = 0;
        int lastDirection = -1;
        int currentX = startX;  // Menggunakan variabel baru
        int currentY = startY;  // Menggunakan variabel baru

        while (currentLength < maxFakeLength)
        {
            List<int> availableDirections = new List<int>();
            int[] dx = { 0, 1, 0, -1 };
            int[] dy = { 1, 0, -1, 0 };

            for (int i = 0; i < 4; i++)
            {
                if (i != (lastDirection + 2) % 4)
                {
                    int nextX = currentX + dx[i];  // Menggunakan nama berbeda
                    int nextY = currentY + dy[i];  // Menggunakan nama berbeda
                    if (IsValidCell(nextX, nextY) && !maze[nextX, nextY].visited)
                    {
                        availableDirections.Add(i);
                    }
                }
            }

            if (availableDirections.Count == 0) break;

            int direction = availableDirections[Random.Range(0, availableDirections.Count)];
            int nextCellX = currentX + dx[direction];  // Menggunakan nama yang berbeda
            int nextCellY = currentY + dy[direction];  // Menggunakan nama yang berbeda

            maze[currentX, currentY].walls[direction] = false;
            maze[nextCellX, nextCellY].walls[(direction + 2) % 4] = false;
            maze[nextCellX, nextCellY].visited = true;
            maze[nextCellX, nextCellY].isDeadEnd = true;

            currentX = nextCellX;
            currentY = nextCellY;
            lastDirection = direction;
            currentLength++;
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
        if (difficulty == Difficulty.Extreme)
        {
            CreateExtremePath();
        }
        else
        {
            float straightPathProbability = difficulty switch
            {
                Difficulty.Easy => 0.7f,
                Difficulty.Medium => 0.5f,
                Difficulty.Hard => 0.3f,
                Difficulty.Extreme => 0.2f,
                _ => 0.5f
            };

            int currentX = 0;
            int currentY = 0;
            int targetX = currentSettings.width - 1;
            int targetY = currentSettings.height - 1;

            while (currentX < targetX || currentY < targetY)
            {
                maze[currentX, currentY].isPath = true;

                if (currentX < targetX && (Random.value < straightPathProbability || currentY == targetY))
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
            maze[targetX, targetY].isPath = true;
        }
    }

    void CreateExtremePath()
    {
        int currentX = 0;
        int currentY = 0;
        int targetX = currentSettings.width - 1;
        int targetY = currentSettings.height - 1;

        while (currentX != targetX || currentY != targetY)
        {
            maze[currentX, currentY].isPath = true;

            List<(int dx, int dy)> possibleMoves = new List<(int dx, int dy)>();

            if (currentX < targetX) possibleMoves.Add((1, 0));
            if (currentY < targetY) possibleMoves.Add((0, 1));

            if (Random.value < 0.3f)
            {
                if (currentX > 0) possibleMoves.Add((-1, 0));
                if (currentY > 0) possibleMoves.Add((0, -1));
            }

            if (possibleMoves.Count == 0) break;

            var move = possibleMoves[Random.Range(0, possibleMoves.Count)];
            int newX = currentX + move.dx;
            int newY = currentY + move.dy;

            if (IsValidCell(newX, newY))
            {
                if (move.dx > 0)
                {
                    maze[currentX, currentY].walls[1] = false;
                    maze[newX, newY].walls[3] = false;
                }
                else if (move.dx < 0)
                {
                    maze[currentX, currentY].walls[3] = false;
                    maze[newX, newY].walls[1] = false;
                }
                else if (move.dy > 0)
                {
                    maze[currentX, currentY].walls[0] = false;
                    maze[newX, newY].walls[2] = false;
                }
                else
                {
                    maze[currentX, currentY].walls[2] = false;
                    maze[newX, newY].walls[0] = false;
                }

                currentX = newX;
                currentY = newY;
            }
        }

        maze[targetX, targetY].isPath = true;
    }

    private bool ValidatePath()
    {
        bool[,] visited = new bool[currentSettings.width, currentSettings.height];
        return HasValidPath(0, 0, visited);
    }

    private bool HasValidPath(int x, int y, bool[,] visited)
    {
        if (x == currentSettings.width - 1 && y == currentSettings.height - 1)
            return true;

        visited[x, y] = true;

        int[] dx = { 0, 1, 0, -1 }; // Top, Right, Bottom, Left
        int[] dy = { 1, 0, -1, 0 };

        for (int i = 0; i < 4; i++)
        {
            if (!maze[x, y].walls[i])
            {
                int newX = x + dx[i];
                int newY = y + dy[i];

                if (IsValidCell(newX, newY) && !visited[newX, newY])
                {
                    if (HasValidPath(newX, newY, visited))
                        return true;
                }
            }
        }

        return false;
    }

    private void ForceMainPath()
    {
        int currentX = 0;
        int currentY = 0;

        while (currentX < currentSettings.width - 1 || currentY < currentSettings.height - 1)
        {
            if (currentX < currentSettings.width - 1)
            {
                maze[currentX, currentY].walls[1] = false;
                maze[currentX + 1, currentY].walls[3] = false;
                maze[currentX, currentY].isPath = true;
                currentX++;
            }
            else if (currentY < currentSettings.height - 1)
            {
                maze[currentX, currentY].walls[0] = false;
                maze[currentX, currentY + 1].walls[2] = false;
                maze[currentX, currentY].isPath = true;
                currentY++;
            }
        }

        maze[currentSettings.width - 1, currentSettings.height - 1].isPath = true;
    }

    void CreateWalls()
    {
        for (int x = 0; x < currentSettings.width; x++)
        {
            for (int y = 0; y < currentSettings.height; y++)
            {
                Vector3 cellPosition = mazeOrigin + new Vector3(x * wallLength, 0, y * wallLength);

                // Skip pembuatan dinding kiri untuk pintu masuk
                if (x == 0 && y == 0)
                {
                    // Hanya buat dinding atas dan kanan
                    if (maze[x, y].walls[0]) // top
                        CreateWall(cellPosition, Quaternion.Euler(0, 0, 0));
                    if (maze[x, y].walls[1]) // right
                        CreateWall(cellPosition, Quaternion.Euler(0, 90, 0));
                    continue;
                }

                // Normal wall creation for other cells
                if (maze[x, y].walls[0]) // top
                    CreateWall(cellPosition, Quaternion.Euler(0, 0, 0));
                if (maze[x, y].walls[1]) // right
                    CreateWall(cellPosition, Quaternion.Euler(0, 90, 0));
                if (maze[x, y].walls[2] && y == 0) // bottom
                    CreateWall(cellPosition, Quaternion.Euler(0, 180, 0));
                if (maze[x, y].walls[3] && x == 0 && y != 0) // left (skip for entrance)
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

        // Set appropriate material based on position
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
        maze[0, 0].isPath = true;

        if (currentSettings.width > 1)
        {
            maze[1, 0].walls[3] = false;
            maze[1, 0].isPath = true;
        }

        // Create exit (top-right)
        int exitX = currentSettings.width - 1;
        int exitY = currentSettings.height - 1;
        maze[exitX, exitY].walls[1] = false;
        maze[exitX, exitY].isPath = true;
    }

    void CreatePillar(Vector3 position)
    {
        GameObject pillar = Instantiate(wallPrefab, position, Quaternion.identity);
        pillar.transform.parent = transform;

        Vector3 scale = new Vector3(wallLength * 0.2f, currentSettings.wallHeight, wallLength * 0.2f);
        pillar.transform.localScale = scale;

        allWalls.Add(pillar);
        SetWallMaterial(pillar, wallMaterial);
    }

    bool IsValidCell(int x, int y)
    {
        return x >= 0 && x < currentSettings.width && y >= 0 && y < currentSettings.height;
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