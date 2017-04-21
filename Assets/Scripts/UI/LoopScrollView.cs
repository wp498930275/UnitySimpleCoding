using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// 循环滚动列表，支持横着滚、竖着滚，仅支持单行单列
/// 原理：把Cell放到ScrollRect的Content上面，根据Content的位置计算Cell的位置
/// </summary>
[RequireComponent(typeof(ScrollRect))]
public class LoopScrollView : MonoBehaviour {

    UnityAction<int, GameObject> UpdateCellEvent;

    enum Direction {
        Horizontal,
        Vertical,
    }

    [SerializeField]
    Vector2 mCellSize;

    [SerializeField]
    float mSpace;

    [SerializeField]
    Direction mDirection;

    [SerializeField]
    GameObject mCellPrefab;


    private int mCellCount = 0;
    private ScrollRect mScrollRect;
    private int mShowingIndexBegin, mShowingIndexEnd;
    private int mCellStartIndex = 0;
    private List<GameObject> mCreatedCells = new List<GameObject>();

    private float CellBegin {
        get {
            if (mDirection == Direction.Horizontal) {
                return mShowingIndexBegin * (mCellSize.x + mSpace);
            } else if (mDirection == Direction.Vertical) {
                return -mShowingIndexBegin * (mCellSize.y + mSpace);
            } else {
                return 0;
            }
        }
    }

    private float CellEnd {
        get {
            if (mDirection == Direction.Horizontal) {
                return mShowingIndexEnd * (mCellSize.x + mSpace) + mCellSize.x;
            } else if (mDirection == Direction.Vertical) {
                return -mShowingIndexEnd * (mCellSize.y + mSpace) - mCellSize.y;
            } else {
                return 0;
            }
        }
    }

    public int CellCount {
        get { return mCellCount; }
        private set {
            mCellCount = value;
            for (int i = 0; i < mCreatedCells.Count; i++) {
                mCreatedCells[i].gameObject.SetActive(i < mCellCount);
            }
            if (mDirection == Direction.Horizontal) {
                Vector2 originContentSize = mScrollRect.content.sizeDelta;
                originContentSize.x = mCellCount * mCellSize.x + (mCellCount - 1) * mSpace;
                mScrollRect.content.sizeDelta = originContentSize;
            } else if (mDirection == Direction.Vertical) {
                Vector2 originContentSize = mScrollRect.content.sizeDelta;
                originContentSize.y = mCellCount * mCellSize.y + (mCellCount - 1) * mSpace;
                mScrollRect.content.sizeDelta = originContentSize;
            }
            UpdateCellValues();
        }
    }

    void Awake() {
        mScrollRect = GetComponent<ScrollRect>();
        //设置prefab的中心点在左上角
        RectTransform rt = mCellPrefab.transform as RectTransform;
        rt.pivot = new Vector2(0, 1);
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        mCellPrefab.gameObject.SetActive(false);
        InstantiateCells();
    }

    void Update() {
        //需要更新
        //下方元素上移
        while (true) {
            float contentBegin = -mScrollRect.content.anchoredPosition.y;
            float contentEnd = -mScrollRect.content.anchoredPosition.y - (mScrollRect.transform as RectTransform).sizeDelta.y;
            if (mDirection == Direction.Horizontal) {
                contentBegin = -mScrollRect.content.anchoredPosition.x;
                contentEnd = -mScrollRect.content.anchoredPosition.x + (mScrollRect.transform as RectTransform).sizeDelta.x;
                if (CellBegin > contentBegin && mShowingIndexBegin > 0) {
                    CellEndMoveToStart();
                } else if (CellEnd < contentEnd && mShowingIndexEnd < CellCount - 1) {
                    CellStartMoveToEnd();
                } else {
                    break;
                }
            } else if (mDirection == Direction.Vertical) {
                contentBegin = -mScrollRect.content.anchoredPosition.y;
                contentEnd = -mScrollRect.content.anchoredPosition.y - (mScrollRect.transform as RectTransform).sizeDelta.y;
                if (CellBegin < contentBegin && mShowingIndexBegin > 0) {
                    CellEndMoveToStart();
                } else if (CellEnd > contentEnd && mShowingIndexEnd < CellCount - 1) {
                    CellStartMoveToEnd();
                } else {
                    break;
                }
            } else {
                break;
            }
        }
    }

    //最后面的item -> 最前面
    void CellEndMoveToStart() {
        //最后面Cell在mCreatedCells中的Index
        int index = GetCellIndexByShowingOrder(mCreatedCells.Count - 1);
        mCellStartIndex--;
        if (mCellStartIndex < 0) {
            mCellStartIndex += mCreatedCells.Count;
        }
        mCellStartIndex %= mCreatedCells.Count;
        mShowingIndexBegin--;
        mShowingIndexEnd--;
        UpdateCell(mShowingIndexBegin, index);
    }

    //最前面的Item -> 最后面
    void CellStartMoveToEnd() {
        //最前面Cell在mCreatedCells中的Index
        int index = GetCellIndexByShowingOrder(0);
        mCellStartIndex++;
        mCellStartIndex %= mCreatedCells.Count;
        mShowingIndexBegin++;
        mShowingIndexEnd++;
        UpdateCell(mShowingIndexEnd, index);
    }

    //根据显示顺序获得Cell的Index
    int GetCellIndexByShowingOrder(int showingOrder) {
        showingOrder += mCellStartIndex;
        showingOrder %= mCreatedCells.Count;
        return showingOrder;
    }

    /// <summary>
    /// 更新Cell
    /// </summary>
    /// <param name="dataIndex">cell在数据中的Index</param>
    /// <param name="createdIndex">cell在创建列表中的Index</param>
    void UpdateCell(int dataIndex, int createdIndex) {
        SetCellPosition(dataIndex, createdIndex);
        if (UpdateCellEvent != null) {
            UpdateCellEvent(dataIndex, mCreatedCells[createdIndex]);
        }
    }

    void UpdateCellValues() {
        for (int i = mShowingIndexBegin, j = 0; i <= mShowingIndexEnd; i++, j++) {
            int index = GetCellIndexByShowingOrder(j);
            if (UpdateCellEvent != null) {
                UpdateCellEvent(i, mCreatedCells[index]);
            }
        }
    }

    void SetCellPosition(int dataIndex, int createdIndex) {
        RectTransform rt = mCreatedCells[createdIndex].transform as RectTransform;
        SetCellPosition(dataIndex, rt);
    }

    void SetCellPosition(int dataIndex, RectTransform rt) {
        if (mDirection == Direction.Horizontal) {
            rt.anchoredPosition = new Vector2(dataIndex * (mCellSize.x + mSpace), 0);
        } else if (mDirection == Direction.Vertical) {
            rt.anchoredPosition = new Vector2(0, -dataIndex * (mCellSize.y + mSpace));
        }
    }

    //实例化Cells
    void InstantiateCells() {
        int instantiateCellCount = 0;
        if (mDirection == Direction.Horizontal) {
            RectTransform rt = mScrollRect.transform as RectTransform;
            instantiateCellCount = Mathf.CeilToInt(rt.sizeDelta.x / (mCellSize.x + mSpace)) + 2;
        } else if (mDirection == Direction.Vertical) {
            RectTransform rt = mScrollRect.transform as RectTransform;
            instantiateCellCount = Mathf.CeilToInt(rt.sizeDelta.y / (mCellSize.y + mSpace)) + 2;
        }
        for (int i = 0; i < instantiateCellCount; i++) {
            GameObject cell = Instantiate(mCellPrefab, mScrollRect.content);
            cell.gameObject.name = "cell_" + i.ToString();
            RectTransform rt = cell.transform as RectTransform;
            SetCellPosition(i, rt);
            mCreatedCells.Add(cell);
        }
        mShowingIndexBegin = 0;
        mShowingIndexEnd = instantiateCellCount - 1;
    }

    /// <summary>
    /// 设置LoopScrollView的值
    /// </summary>
    /// <param name="cellCount">cell的数量</param>
    /// <param name="updateCellEvent">cell的更新方法</param>
    public void SetValue(int cellCount, UnityAction<int, GameObject> updateCellEvent) {
        //设置CellCount的时候回执行UpdateCellEvent，所以在这里面先给UpdateCellEvent赋值
        UpdateCellEvent = updateCellEvent;
        CellCount = cellCount;
    }

    public void GotoIndex(int index) {
    }
}
