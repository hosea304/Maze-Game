using UnityEngine;
using System.Collections.Generic;

public enum Difficulty { Easy, Medium, Hard, Extreme }

public class MazeGenerator : MonoBehaviour
{
    [Header("Difficulty Settings")]
    public Difficulty difficulty;
    public Vector2Int gridSize;
    public GameObject wallPrefab;

    public Transform floorParent; // Parent untuk tempatkan lantai yang ada di hierarki

    public GameObject triggerPrefab; // Prefab untuk trigger (EntranceExitTrigger)
    public MazeTimer mazeTimer; // Referensi ke script MazeTimer

    private Cell[,] grid;
    private Vector2Int entrance;
    private Vector2Int exit;

    void Start()
    {
        SetMazeSizeBasedOnDifficulty();
        GenerateMaze();
    }

    void SetMazeSizeBasedOnDifficulty()
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                gridSize = new Vector2Int(10, 10);
                break;
            case Difficulty.Medium:
                gridSize = new Vector2Int(15, 15);
                break;
            case Difficulty.Hard:
                gridSize = new Vector2Int(20, 20);
                break;
            case Difficulty.Extreme:
                gridSize = new Vector2Int(25, 25);
                break;
        }

        // Set difficulty pada MazeTimer
        if (mazeTimer != null)
        {
            mazeTimer.SetDifficulty(difficulty);
        }
        else
        {
            Debug.LogError("MazeTimer reference is not set in MazeGenerator.");
        }
    }

    void GenerateMaze()
    {
        grid = new Cell[gridSize.x, gridSize.y];
        CreateCells();
        GeneratePath();
        CreateEntrancesAndExits();
        InstantiateMaze();
        InstantiateTriggers(); // Tambahkan ini
    }

    void CreateCells()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                grid[x, y] = new Cell(new Vector2Int(x, y));
                grid[x, y].hasWallTop = true;
                grid[x, y].hasWallRight = true;
                grid[x, y].hasWallBottom = true;
                grid[x, y].hasWallLeft = true;
            }
        }
    }

    void GeneratePath()
    {
        Stack<Cell> stack = new Stack<Cell>();
        Cell startCell = grid[0, 0];
        startCell.isPath = true;
        stack.Push(startCell);

        while (stack.Count > 0)
        {
            Cell currentCell = stack.Peek();
            List<Cell> neighbors = GetUnvisitedNeighbors(currentCell);

            if (neighbors.Count > 0)
            {
                Cell neighbor = neighbors[Random.Range(0, neighbors.Count)];
                RemoveWall(currentCell, neighbor);
                neighbor.isPath = true;
                stack.Push(neighbor);
            }
            else
            {
                stack.Pop();
            }
        }
    }

    List<Cell> GetUnvisitedNeighbors(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1), // Up
            new Vector2Int(1, 0), // Right
            new Vector2Int(0, -1), // Down
            new Vector2Int(-1, 0) // Left
        };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighborPos = cell.position + direction;

            if (IsInBounds(neighborPos) && !grid[neighborPos.x, neighborPos.y].isPath)
            {
                neighbors.Add(grid[neighborPos.x, neighborPos.y]);
            }
        }

        return neighbors;
    }

    void RemoveWall(Cell current, Cell neighbor)
    {
        Vector2Int direction = neighbor.position - current.position;

        if (direction == new Vector2Int(0, 1)) // Up
        {
            current.hasWallTop = false;
            neighbor.hasWallBottom = false;
        }
        else if (direction == new Vector2Int(1, 0)) // Right
        {
            current.hasWallRight = false;
            neighbor.hasWallLeft = false;
        }
        else if (direction == new Vector2Int(0, -1)) // Down
        {
            current.hasWallBottom = false;
            neighbor.hasWallTop = false;
        }
        else if (direction == new Vector2Int(-1, 0)) // Left
        {
            current.hasWallLeft = false;
            neighbor.hasWallRight = false;
        }
    }

    void CreateEntrancesAndExits()
    {
        entrance = new Vector2Int(0, 0);
        exit = new Vector2Int(gridSize.x - 1, gridSize.y - 1);

        // Open entrance
        grid[entrance.x, entrance.y].hasWallLeft = false; // Left wall of entrance
        grid[entrance.x, entrance.y].hasWallBottom = false; // Bottom wall of entrance

        // Open exit
        grid[exit.x, exit.y].hasWallRight = false; // Right wall of exit
        grid[exit.x, exit.y].hasWallTop = false; // Top wall of exit
    }

    void InstantiateMaze()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Cell cell = grid[x, y];
                if (cell.hasWallTop) InstantiateWall(new Vector3(x * 2, 0, y * 2 + 1));
                if (cell.hasWallRight) InstantiateWall(new Vector3(x * 2 + 1, 0, y * 2), Quaternion.Euler(0, 90, 0));
                if (cell.hasWallBottom) InstantiateWall(new Vector3(x * 2, 0, y * 2 - 1));
                if (cell.hasWallLeft) InstantiateWall(new Vector3(x * 2 - 1, 0, y * 2), Quaternion.Euler(0, 90, 0));

                // Abaikan instansiasi untuk lantai
            }
        }
    }

    void InstantiateWall(Vector3 position, Quaternion rotation = default)
    {
        Instantiate(wallPrefab, position, rotation);
    }

    bool IsInBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < gridSize.x && position.y >= 0 && position.y < gridSize.y;
    }

    // Method untuk instansiasi trigger
    void InstantiateTriggers()
    {
        // Instantiate Entrance Trigger
        Vector3 entrancePos = new Vector3(entrance.x * 2, 0.5f, entrance.y * 2);
        GameObject entranceTrigger = Instantiate(triggerPrefab, entrancePos, Quaternion.identity);
        entranceTrigger.transform.localScale = new Vector3(2f, 1f, 2f); // Set scale x dan z menjadi 2
        MazeTrigger entranceMazeTrigger = entranceTrigger.GetComponent<MazeTrigger>();
        entranceMazeTrigger.triggerType = MazeTrigger.TriggerType.Entrance;
        entranceMazeTrigger.mazeTimer = mazeTimer;

        // Instantiate Exit Trigger
        Vector3 exitPos = new Vector3(exit.x * 2, 0.5f, exit.y * 2);
        GameObject exitTrigger = Instantiate(triggerPrefab, exitPos, Quaternion.identity);
        exitTrigger.transform.localScale = new Vector3(2f, 1f, 2f); // Set scale x dan z menjadi 2
        MazeTrigger exitMazeTrigger = exitTrigger.GetComponent<MazeTrigger>();
        exitMazeTrigger.triggerType = MazeTrigger.TriggerType.Exit;
        exitMazeTrigger.mazeTimer = mazeTimer;
    }
    public class Cell
    {
        public Vector2Int position;
        public bool isPath = false;
        public bool hasWallTop = true;
        public bool hasWallRight = true;
        public bool hasWallBottom = true;
        public bool hasWallLeft = true;

        public Cell(Vector2Int position)
        {
            this.position = position;
        }
    }
}
