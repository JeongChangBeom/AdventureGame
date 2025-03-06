using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get => _instance;
    }

    [SerializeField] private UIManager uiManager;
    public static bool isFirstLoading = true;

    private void Awake()
    {
        _instance = this;

        uiManager = FindObjectOfType<UIManager>();
    }

    private void Start()
    {
        if (!isFirstLoading)
        {
            StartGame();
        }
        else
        {
            uiManager.ChangeState(UIState.Home);
            isFirstLoading = false;
            Time.timeScale = 0f;
        }
    }

    public void StartGame()
    {
        uiManager.SetPlayGame();
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        uiManager.SetGameOver();
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0f;
    }
}
