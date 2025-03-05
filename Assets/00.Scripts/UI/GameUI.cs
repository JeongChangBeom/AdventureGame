using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : BaseUI
{
    [SerializeField] private Image uiHealthBar;

    private void Update()
    {
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        uiHealthBar.fillAmount = CharacterManager.Instance.Player.condition.health.GetPercentage();
    }

    protected override UIState GetUIState()
    {
        return UIState.Game;
    }
}
