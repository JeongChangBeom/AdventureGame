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

    [Header("Condition")]
    public Condition health;

    private void Start()
    {
        CharacterManager.Instance.Player.condition.uiManager = this;
    }

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
