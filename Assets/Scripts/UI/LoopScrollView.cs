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

    Vector2 mLastContentPos;
    void Update() {
        //需要更新
        //下方元素上移
        Vector2 contentPos = mScrollRect.content.anchoredPosition;
        bool needUpdateAll = false;
        if (mDirection == Direction.Horizontal) {
            if (Mathf.Abs(contentPos.x - mLastContentPos.x) >= (mCellSize.x + mSpace) * mCreatedCells.Count) {
                needUpdateAll = true;
            }
        } else if (mDirection == Direction.Vertical) {
            if (Mathf.Abs(contentPos.y - mLastContentPos.y) >= (mCellSize.y + mSpace) * mCreatedCells.Count) {
                needUpdateAll = true;
            }
        }
        //如果某一帧移动的距离过大，更新全部单元格
        //移动距离小,逐个更新
        if (needUpdateAll) {
            UpdateAllCells();
        } else {
            while (true) {
                float contentBegin, contentEnd;
                if (mDirection == Direction.Horizontal) {
                    contentBegin = -contentPos.x;
                    contentEnd = -contentPos.x + (mScrollRect.transform as RectTransform).sizeDelta.x;
                    if (CellBegin > contentBegin && mShowingIndexBegin > 0) {
                        CellEndMoveToStart();
                    } else if (CellEnd < contentEnd && mShowingIndexEnd < CellCount - 1) {
                        CellStartMoveToEnd();
                    } else {
                        break;
                    }
                } else if (mDirection == Direction.Vertical) {
                    contentBegin = -contentPos.y;
                    contentEnd = -contentPos.y - (mScrollRect.transform as RectTransform).sizeDelta.y;
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
        mLastContentPos = mScrollRect.content.anchoredPosition;
    }

    //获得Content在当前位置，实际上应该显示Cell在数据中的索引
    int GetNowCellBeginIndex() {
        if (mDirection == Direction.Horizontal) {
            float contentBegin = -mScrollRect.content.anchoredPosition.x;
            return Mathf.Clamp(Mathf.FloorToInt(contentBegin / (mCellSize.x + mSpace)), 0, CellCount - mCreatedCells.Count);
        } else if (mDirection == Direction.Vertical) {
            float contentBegin = mScrollRect.content.anchoredPosition.y;
            return Mathf.Clamp(Mathf.FloorToInt(contentBegin / (mCellSize.y + mSpace)), 0, CellCount - mCreatedCells.Count);
        } else {
            return 0;
        }
    }

    //更新所有Cell的位置
    void UpdateAllCells() {
        mCellStartIndex = 0;
        mShowingIndexBegin = GetNowCellBeginIndex();
        mShowingIndexEnd = mShowingIndexBegin + mCreatedCells.Count - 1;
        for (int i = 0, index = mShowingIndexBegin; i < mCreatedCells.Count; i++, index++) {
            UpdateCell(index, i);
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
        SetCellPosition(dataIndex, mCreatedCells[createdIndex].transform as RectTransform);
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
