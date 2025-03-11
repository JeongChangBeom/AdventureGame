using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : BaseUI
{
    [SerializeField] private Image uiHealthBar;
    [SerializeField] private Image uiStaminaBar;

    private void Update()
    {
        UpdateHealthBar();
        UpdateStaminaBar();
    }

    public void UpdateHealthBar()
    {
        uiHealthBar.fillAmount = CharacterManager.Instance.Player.condition.health.GetPercentage();
    }

    public void UpdateStaminaBar()
    {
        uiStaminaBar.fillAmount = CharacterManager.Instance.Player.condition.stamina.GetPercentage();
    }

    protected override UIState GetUIState()
    {
        return UIState.Game;
    }
}
