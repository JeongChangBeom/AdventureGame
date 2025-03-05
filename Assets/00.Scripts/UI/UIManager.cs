using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIState
{
    Home,
    Game,
    GameOver,
}

public class UIManager : MonoBehaviour
{
    BaseUI[] UIArr;

    private UIState curState;

    private void Awake()
    {
        UIArr = GetComponentsInChildren<BaseUI>(true);

        foreach(var ui in UIArr)
        {
            ui.Init(this);
        }
    }

    public void SetPlayGame()
    {
        ChangeState(UIState.Game);
    }

    public void SetGameOver()
    {
        ChangeState(UIState.GameOver);
    }


    public void ChangeState(UIState state)
    {
        curState = state;

        foreach (var ui in UIArr)
        {
            ui.SetActive(curState);
        }
    }
}
