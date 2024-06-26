using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Manager : MonoBehaviour
{
    [SerializeField] Tile mTilePrefab;
    float mTileSize = 50;
    [SerializeField] int mTileCountX = 10;
    int mTileCountY = 10;
    [SerializeField] int mFieldSpacing = 50;
    [SerializeField] int mFieldOffset = 80;
    [SerializeField] int mTotalBoomCount = 20;
    [SerializeField] GridLayoutGroup mTilesRoot;
    [SerializeField] GameObject mGameOverImage;
    [SerializeField] GameObject mGameClearImage;

    [SerializeField] TextMeshProUGUI labelCountFlags;
    [SerializeField] TextMeshProUGUI labelCountBooms;

    Tile[] mTiles;
    Tile[] mBoomTiles;
    int mDiggedTileCount;
    int countFlags = 0;

    bool mIsSettingBoom = false;
    public bool IsSettingBoom() => mIsSettingBoom;


    private void Start()
    {
        GenerateTiles();
        labelCountFlags.text = "0";
        labelCountBooms.text = mTotalBoomCount.ToString();
    }

    //タイルを設置
    void GenerateTiles()
    {
        RectTransform mTilerootRect = mTilesRoot.transform as RectTransform;
        Vector2 rootSize = new Vector2(mTilerootRect.rect.width, mTilerootRect.rect.height);

        mTileSize = (rootSize.x - mFieldSpacing * (mTileCountX - 1)) / mTileCountX;
        mTileCountY = (int)(rootSize.y / mTileSize);

        mTilesRoot.cellSize = new Vector2(mTileSize, mTileSize);
        mTilesRoot.spacing = new Vector2(mFieldSpacing, mFieldSpacing);

        mTiles = new Tile[mTileCountX * mTileCountY];

        for (int i = 0; i < mTiles.Length; i++)
        {
            Tile tile = Instantiate(mTilePrefab, mTilesRoot.transform);

            tile.Initialize(this, i);
            mTiles[i] = tile;
        }
    }

    public void SetTileData(Tile selectedTile)
    {
        SetBooms(selectedTile);
        SetCounts();
        mIsSettingBoom = true;
        selectedTile.OnDigged();
    }

    //地雷を設置
    void SetBooms(Tile selectedTile)
    {
        List<Tile> safeTiles = new List<Tile>();

        // タップしたタイルとその周りをsafeTileから除く
        foreach (Tile tile in mTiles)
        {
            bool isExcluded = tile == selectedTile;
            foreach (var aroundExcludedTileIndex in GetAroundIndices(selectedTile.Index))
            {
                if (tile.Index == aroundExcludedTileIndex)
                {
                    isExcluded = true;
                }
            }
            if (!isExcluded)
            {
                safeTiles.Add(tile);
            }
        }

        // 爆弾マスの設定
        mBoomTiles = new Tile[mTotalBoomCount];

        for (int i = 0; i < mTotalBoomCount; i++)
        {
            int index = Random.Range(0, safeTiles.Count);
            Tile boomTile = safeTiles[index];

            mBoomTiles[i] = boomTile;
            safeTiles.Remove(boomTile);

            boomTile.SetBoom();
        }
    }

    void SetCounts()
    {
        int[] boomCountEach = new int[mTiles.Length];

        foreach (var tile in mBoomTiles)
        {
            int[] aroundTileIndices = GetAroundIndices(tile.Index);
            foreach (var aroundIndex in aroundTileIndices)
                boomCountEach[aroundIndex] = boomCountEach[aroundIndex] + 1;
        }

        for (int i = 0; i < boomCountEach.Length; i++)
        {
            int boomCount = boomCountEach[i];
            if (boomCount != 0 && mTiles[i].TileType != TileType.BOOM)
                mTiles[i].SetCount(boomCount);
        }
    }

    //隣のタイルを開く
    public void DigAround(int index)
    {
        foreach (var aroundTileIndex in GetAroundIndices(index))
            mTiles[aroundTileIndex].OnDigged();
    }

    int[] GetAroundIndices(int index)
    {
        List<int> result = new List<int>(8);

        int index0 = index - mTileCountX - 1;
        if (index0 >= 0 && index0 % mTileCountX != mTileCountX - 1)
            result.Add(index0);

        int index1 = index - mTileCountX;
        if (index1 >= 0)
            result.Add(index1);

        int index2 = index - mTileCountX + 1;
        if (index2 >= 0 && index2 % mTileCountX != 0)
            result.Add(index2);

        int index3 = index - 1;
        if (index3 >= 0 && index3 % mTileCountX != mTileCountX - 1)
            result.Add(index3);

        int index4 = index + 1;
        if (index4 % mTileCountX != 0)
            result.Add(index4);

        int index5 = index + mTileCountX - 1;
        if (index5 < mTiles.Length && index5 % mTileCountX != mTileCountX - 1)
            result.Add(index5);

        int index6 = index + mTileCountX;
        if (index6 < mTiles.Length)
            result.Add(index6);

        int index7 = index + mTileCountX + 1;
        if (index7 < mTiles.Length && index7 % mTileCountX != 0)
            result.Add(index7);

        return result.ToArray();
    }

    //ゲームオーバー
    public void GameOver()
    {
        Instantiate(mGameOverImage, mTilesRoot.transform.parent.transform);

        foreach (var tile in mTiles)
        {
            tile.GameOverCheck();
        }
    }

    public void CountDiggedTile()
    {
        mDiggedTileCount++;
        if (mDiggedTileCount == mTileCountX * mTileCountY - mTotalBoomCount)
            GameClear();
    }

    //ゲームクリア
    void GameClear()
    {
        Instantiate(mGameClearImage, mTilesRoot.transform.parent.transform);
    }
}