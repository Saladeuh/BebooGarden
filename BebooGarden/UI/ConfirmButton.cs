﻿using AccessibleMyraUI;
using Myra.Graphics2D.UI;
using System;
namespace BebooGarden.UI;

public class ConfirmButton : AccessibleButton
{
  public ConfirmButton(string text, int width = 0, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left)
      : base(text, width, horizontalAlignment)
  {
    KeyboardFocusChanged += OnFocusChanged;
    Click += OnButtonClick;
  }

  private void OnFocusChanged(object? sender, EventArgs e)
  {
    if (IsKeyboardFocused)
      Game1.Instance.SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.MenuBipSound);
  }

  private void OnButtonClick(object sender, System.EventArgs e)
  {
    Game1.Instance.SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.MenuOkSound);
  }
}