using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum TileType { EMPTY, BOOM, COUNT }
public class Tile : MonoBehaviour
{
    enum MarkState { NO_MARK, FLAG }
    public TileType TileType { get { return mTileType; } }
    public int Index { get { return mIndex; } }

    Manager mManager;
    TileType mTileType = TileType.EMPTY;
    MarkState mMarkState = MarkState.NO_MARK;
    int mIndex;
    bool mIsDigged = false;

    [SerializeField] GameObject mCover;
    [SerializeField] GameObject mFlag;
    [SerializeField] GameObject mRedBG;
    [SerializeField] GameObject mRedCross;
    [SerializeField] Image mCount;
    [SerializeField] GameObject mBoom;
    [SerializeField] Image touchArea;
    [SerializeField] Sprite[] mCountSprites = new Sprite[8];

    public void Initialize(Manager manager, int index)
    {
        mManager = manager;
        mIndex = index;
    }

    //タイルを掘る
    public void OnDigged()
    {
        if (mIsDigged || mMarkState == MarkState.FLAG)
            return;

        if (!mManager.IsSettingBoom())
        {
            mManager.SetTileData(this);
            return;
        }

        mIsDigged = true;

        mCover.SetActive(false);

        switch (mTileType)
        {
            case TileType.EMPTY:
                //マネージャーに隣接タイルを掘ってもらう 
                mManager.DigAround(mIndex);
                mManager.CountDiggedTile();
                break;
            case TileType.BOOM:
                //マネージャーにゲームオーバー処理をしてもらう
                mRedBG.SetActive(true);
                mManager.GameOver();
                break;
            case TileType.COUNT:
                mManager.CountDiggedTile();
                break;
        }
    }

    float tapTime = 0;
    bool isDown = false;

    public void OnPointerDown()
    {
        isDown = true;
        Debug.Log("push");
    }

    private void Update()
    {
        if (isDown)
        {
            tapTime += Time.deltaTime;
            if (tapTime >= 0.5f)
            {
                SetMark();
                isDown = false;
            }
        }
    }

    public void OnPointerUp()
    {
        isDown = false;
        Debug.Log(tapTime);

        if (tapTime < 0.5f)
            OnDigged();

        tapTime = 0;
    }

    //周りの地雷数を設定/表示
    public void SetCount(int count)
    {
        mTileType = TileType.COUNT;

        //カウントを設定
        mCount.gameObject.SetActive(true);
        mCount.sprite = mCountSprites[count - 1];
    }

    public void SetBoom()
    {
        mTileType = TileType.BOOM;
        mBoom.SetActive(true);
    }

    //マークを付ける
    public void SetMark()
    {
        if (mIsDigged)
            return;
        Debug.Log(mMarkState);
        switch (mMarkState)
        {
            case MarkState.NO_MARK:
                mFlag.SetActive(true);
                break;
            case MarkState.FLAG:
                mFlag.SetActive(false);
                break;
            default:
                break;
        }

        mMarkState++;
        int markStateLength = System.Enum.GetNames(typeof(MarkState)).Length;
        if ((int)mMarkState == markStateLength)
            mMarkState -= markStateLength;
    }

    public void GameOverCheck()
    {
        if (mMarkState == MarkState.FLAG && mTileType != TileType.BOOM)
            mRedCross.SetActive(true);
        else if (mMarkState != MarkState.FLAG && mTileType == TileType.BOOM)
            mCover.SetActive(false);
    }
}