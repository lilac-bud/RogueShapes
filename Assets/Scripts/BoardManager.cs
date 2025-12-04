using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Xml.Serialization;

public class BoardManager : MonoBehaviour
{
    public class CellData
    {
        public bool Passable;
        public CellObject ContainedObject;
    }
    private Tilemap m_Tilemap;
    private CellData[,] m_BoardData;
    private Grid m_Grid;
    private List<Vector2Int> m_EmptyCellsList;

    public int Width;
    public int Height;
    public Tile[] GroundTiles;
    public Tile[] WallTiles;

    public FoodObject[] FoodPrefabs;
    public int MinFoodCount = 2;
    public int MaxFoodCount = 5;

    public int MinWallCount = 6;
    public int MaxWallCount = 10;
    public WallObject WallPrefab;
    public ExitCellObject ExitCellPrefab;

    public int MinEnemyCount = 1;
    public int MaxEnemyCount = 2;
    public Enemy EnemyPrefab;

    public int MinUpgradeCount = 0;
    public int MaxUpgradeCount = 1;
    public UpgradeObject[] UpgradePrefabs;

    public CameraController CameraController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init()
    {
        if ((GameManager.Instance.CurrentLevel + 1) % 5 == 0)
        {
            Width++;
            Height++;
            MinWallCount++;
            MaxWallCount++;
            MinEnemyCount++;
            MaxEnemyCount++;
            MaxUpgradeCount++;
        }
        CameraController.UpdateCollider(Width, Height);
        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_Grid = GetComponentInChildren<Grid>();
        m_EmptyCellsList = new List<Vector2Int>();
        m_BoardData = new CellData[Width, Height];
        for (int i = 0; i < Height; i++)
            for (int j = 0; j < Width; j++)
            {
                Tile tile;
                m_BoardData[i, j] = new CellData();
                if (i == 0 || j == 0 || i == Height - 1 || j == Width - 1)
                {
                    tile = WallTiles[Random.Range(0, WallTiles.Length)];
                    m_BoardData[i, j].Passable = false;
                }
                else
                {
                    tile = GroundTiles[Random.Range(0, GroundTiles.Length)];
                    m_BoardData[i, j].Passable = true;
                    m_EmptyCellsList.Add(new Vector2Int(i, j));
                }
                SetCellTile(new Vector2Int(i, j), tile);
            }
        m_EmptyCellsList.Remove(new Vector2Int(1, 1));
        Vector2Int endCoord = new(Width - 2, Height - 2);
        AddObject(Instantiate(ExitCellPrefab), endCoord);
        m_EmptyCellsList.Remove(endCoord);
        GenerateWall();
        GenerateUpgrade();
        GenerateEnemy();
        GenerateFood();
    }
    public void Clean()
    {
        if (m_BoardData == null) return;
        for (int y = 0; y < Height; ++y)
            for (int x = 0; x < Width; ++x)
            {
                var cellData = m_BoardData[x, y];
                if (cellData.ContainedObject != null)
                    Destroy(cellData.ContainedObject.gameObject);
                SetCellTile(new Vector2Int(x, y), null);
            }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }
    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x < 0 || cellIndex.x >= Width || cellIndex.y < 0 || cellIndex.y >= Height)
            return null;
        return m_BoardData[cellIndex.x, cellIndex.y];
    }
    void GenerateFood()
    {
        int foodCount = Random.Range(MinFoodCount, MaxFoodCount + 1);
        for (int i = 0; i < foodCount; ++i)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];
            m_EmptyCellsList.RemoveAt(randomIndex);
            FoodObject newFood = Instantiate(FoodPrefabs[Random.Range(0, FoodPrefabs.Length)]);
            AddObject(newFood, coord);
        }
    }
    void GenerateWall()
    {
        int wallCount = Random.Range(MinWallCount, MaxWallCount + 1);
        for (int i = 0; i < wallCount; ++i)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];
            m_EmptyCellsList.RemoveAt(randomIndex);
            WallObject newWall = Instantiate(WallPrefab);
            AddObject(newWall, coord);
        }
    }
    void GenerateUpgrade()
    {
        int upgradeCount = Random.Range(MinUpgradeCount, MaxUpgradeCount + 1);
        for (int i = 0; i < upgradeCount; ++i)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];
            m_EmptyCellsList.RemoveAt(randomIndex);
            UpgradeObject newUpgrade = Instantiate(UpgradePrefabs[Random.Range(0, UpgradePrefabs.Length)]);
            AddObject(newUpgrade, coord);
        }
    }
    void GenerateEnemy()
    {
        int enemyCount = Random.Range(MinEnemyCount, MaxEnemyCount + 1);
        for (int i = 0; i < enemyCount; ++i)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];
            m_EmptyCellsList.RemoveAt(randomIndex);
            Enemy enemy = Instantiate(EnemyPrefab);
            AddObject(enemy, coord);
        }
    }
    
    void AddObject(CellObject obj, Vector2Int coord)
    {
        CellData data = m_BoardData[coord.x, coord.y];
        obj.transform.position = CellToWorld(coord);
        data.ContainedObject = obj;
        obj.Init(coord);
    }
    public void SetCellTile(Vector2Int cellIndex, Tile tile)
    {
        m_Tilemap.SetTile(new Vector3Int(cellIndex.x, cellIndex.y, 0), tile);
    }
    public Tile GetCellTile(Vector2Int cellIndex)
    {
        return m_Tilemap.GetTile<Tile>(new Vector3Int(cellIndex.x, cellIndex.y, 0));
    }
}
