using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoopScrollViewTest : MonoBehaviour {

    [SerializeField]
    LoopScrollView mVertical, mHorizontal;

    [SerializeField]
    int mCellCount = 3;
    void Start () {
        mVertical.SetValue(mCellCount, (int index, GameObject o) => {
            o.GetComponentInChildren<Text>().text = index.ToString();
        });
        mHorizontal.SetValue(mCellCount, (int index, GameObject o) => {
            o.GetComponentInChildren<Text>().text = index.ToString();
        });
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            mCellCount++;
            mVertical.SetValue(mCellCount, (int index, GameObject o) => {
                o.GetComponentInChildren<Text>().text = index.ToString();
            });
            mHorizontal.SetValue(mCellCount, (int index, GameObject o) => {
                o.GetComponentInChildren<Text>().text = index.ToString();
            });
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            mCellCount--;
            mVertical.SetValue(mCellCount, (int index, GameObject o) => {
                o.GetComponentInChildren<Text>().text = index.ToString();
            });
            mHorizontal.SetValue(mCellCount, (int index, GameObject o) => {
                o.GetComponentInChildren<Text>().text = index.ToString();
            });
        }
    }
}
