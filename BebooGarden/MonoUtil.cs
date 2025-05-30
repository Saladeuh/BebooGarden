using Microsoft.Xna.Framework.Input;
using SharpHook.Native;
using System.Collections.Generic;

namespace BebooGarden;

public static class MonoUtil
{
  private static readonly Dictionary<KeyCode, Keys> KEYCODEMAP = new()
  {
    // Lettres
    { KeyCode.VcA, Keys.A },
    { KeyCode.VcB, Keys.B },
    { KeyCode.VcC, Keys.C },
    { KeyCode.VcD, Keys.D },
    { KeyCode.VcE, Keys.E },
    { KeyCode.VcF, Keys.F },
    { KeyCode.VcG, Keys.G },
    { KeyCode.VcH, Keys.H },
    { KeyCode.VcI, Keys.I },
    { KeyCode.VcJ, Keys.J },
    { KeyCode.VcK, Keys.K },
    { KeyCode.VcL, Keys.L },
    { KeyCode.VcM, Keys.M },
    { KeyCode.VcN, Keys.N },
    { KeyCode.VcO, Keys.O },
    { KeyCode.VcP, Keys.P },
    { KeyCode.VcQ, Keys.Q },
    { KeyCode.VcR, Keys.R },
    { KeyCode.VcS, Keys.S },
    { KeyCode.VcT, Keys.T },
    { KeyCode.VcU, Keys.U },
    { KeyCode.VcV, Keys.V },
    { KeyCode.VcW, Keys.W },
    { KeyCode.VcX, Keys.X },
    { KeyCode.VcY, Keys.Y },
    { KeyCode.VcZ, Keys.Z },
    
    // Chiffres de la rangée supérieure
    { KeyCode.Vc0, Keys.D0 },
    { KeyCode.Vc1, Keys.D1 },
    { KeyCode.Vc2, Keys.D2 },
    { KeyCode.Vc3, Keys.D3 },
    { KeyCode.Vc4, Keys.D4 },
    { KeyCode.Vc5, Keys.D5 },
    { KeyCode.Vc6, Keys.D6 },
    { KeyCode.Vc7, Keys.D7 },
    { KeyCode.Vc8, Keys.D8 },
    { KeyCode.Vc9, Keys.D9 },
    
    // Pavé numérique
    { KeyCode.VcNumPad0, Keys.NumPad0 },
    { KeyCode.VcNumPad1, Keys.NumPad1 },
    { KeyCode.VcNumPad2, Keys.NumPad2 },
    { KeyCode.VcNumPad3, Keys.NumPad3 },
    { KeyCode.VcNumPad4, Keys.NumPad4 },
    { KeyCode.VcNumPad5, Keys.NumPad5 },
    { KeyCode.VcNumPad6, Keys.NumPad6 },
    { KeyCode.VcNumPad7, Keys.NumPad7 },
    { KeyCode.VcNumPad8, Keys.NumPad8 },
    { KeyCode.VcNumPad9, Keys.NumPad9 },
    { KeyCode.VcNumPadAdd, Keys.Add },
    { KeyCode.VcNumPadSubtract, Keys.Subtract },
    { KeyCode.VcNumPadMultiply, Keys.Multiply },
    { KeyCode.VcNumPadDivide, Keys.Divide },
    { KeyCode.VcNumPadDecimal, Keys.Decimal },
    
    // Touches de contrôle
    { KeyCode.VcEnter, Keys.Enter },
    { KeyCode.VcSpace, Keys.Space },
    { KeyCode.VcBackspace, Keys.Back },
    { KeyCode.VcTab, Keys.Tab },
    { KeyCode.VcEscape, Keys.Escape },
    
    // Touches de fonction
    { KeyCode.VcF1, Keys.F1 },
    { KeyCode.VcF2, Keys.F2 },
    { KeyCode.VcF3, Keys.F3 },
    { KeyCode.VcF4, Keys.F4 },
    { KeyCode.VcF5, Keys.F5 },
    { KeyCode.VcF6, Keys.F6 },
    { KeyCode.VcF7, Keys.F7 },
    { KeyCode.VcF8, Keys.F8 },
    { KeyCode.VcF9, Keys.F9 },
    { KeyCode.VcF10, Keys.F10 },
    { KeyCode.VcF11, Keys.F11 },
    { KeyCode.VcF12, Keys.F12 },
    
    // Touches de navigation
    { KeyCode.VcLeft, Keys.Left },
    { KeyCode.VcRight, Keys.Right },
    { KeyCode.VcUp, Keys.Up },
    { KeyCode.VcDown, Keys.Down },
    { KeyCode.VcHome, Keys.Home },
    { KeyCode.VcEnd, Keys.End },
    { KeyCode.VcPageUp, Keys.PageUp },
    { KeyCode.VcPageDown, Keys.PageDown },
    
    // Touches modificatrices
    { KeyCode.VcLeftShift, Keys.LeftShift },
    { KeyCode.VcRightShift, Keys.RightShift },
    { KeyCode.VcLeftControl, Keys.LeftControl },
    { KeyCode.VcRightControl, Keys.RightControl },
    { KeyCode.VcLeftAlt, Keys.LeftAlt },
    { KeyCode.VcRightAlt, Keys.RightAlt },
    
    // Touches spéciales
    { KeyCode.VcInsert, Keys.Insert },
    { KeyCode.VcDelete, Keys.Delete },
    { KeyCode.VcPrintScreen, Keys.PrintScreen },
    { KeyCode.VcScrollLock, Keys.Scroll },
    { KeyCode.VcPause, Keys.Pause },
    { KeyCode.VcNumLock, Keys.NumLock },
    { KeyCode.VcCapsLock, Keys.CapsLock },
    
    // Ponctuation et symboles
    // { KeyCode.Vc, Keys.OemTilde },        // ~ (tilde)
    { KeyCode.VcMinus, Keys.OemMinus },        // - (moins)
    //{ KeyCode.VcPl, Keys.OemPlus },          // = (égal)
    { KeyCode.VcOpenBracket, Keys.OemOpenBrackets }, // [
    { KeyCode.VcCloseBracket, Keys.OemCloseBrackets }, // ]
    { KeyCode.VcBackslash, Keys.OemPipe },          // \ (backslash)
    { KeyCode.VcSemicolon, Keys.OemSemicolon }, // ; (point-virgule)
    { KeyCode.VcQuote, Keys.OemQuotes },      // ' (apostrophe)
    { KeyCode.VcComma, Keys.OemComma },        // , (virgule)
    { KeyCode.VcPeriod, Keys.OemPeriod },      // . (point)
    { KeyCode.VcSlash, Keys.OemQuestion },  // / (slash)
    
    // Touches Windows
  };

  public static Keys ConvertKeyCodeToMonogameKey(KeyCode keyCode)
  {
    return KEYCODEMAP.ContainsKey(keyCode) ? KEYCODEMAP[keyCode] : Keys.None;
  }
}