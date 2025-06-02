using AccessibleMyraUI;
using Microsoft.Xna.Framework;
using Myra.Events;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BebooGarden.UI;

internal class FancyTextField : AccessibleTextBox
{
  private int _maxLength;
  private bool _nameFormat;

  public FancyTextField(int maxLength, bool nameFormat=false, string text = "", int width = 200) : base(text, width)
  {
    _maxLength = maxLength;
    _nameFormat = nameFormat;
    ValueChanging += OnValueChanged;
  }

  private void OnValueChanged(object? sender, ValueChangingEventArgs<string> e)
  {
    bool incorrect = false;
    if (e.OldValue.Length > e.NewValue.Length)
    {
      Game1.Instance.SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.MenuKeyDeleteSound);
    }
    else if (e.NewValue.Length > _maxLength ||
             (_nameFormat && !Regex.IsMatch(e.NewValue, @"^[a-zA-Z0-9\p{L}\p{M}_-]+$") &&
              e.NewValue != string.Empty))
    {
      Game1.Instance.SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.MenuKeyFullSound);
      e.Cancel = true;
    }
    else if (!incorrect)
    {
      Game1.Instance.SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.MenuKeySound);
    }
  }
}
