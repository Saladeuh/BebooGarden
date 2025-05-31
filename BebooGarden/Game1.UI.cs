using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.MiniGames;
using BebooGarden.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Myra.Graphics2D.UI;
using System;
using System.Globalization;
using System.Linq;

namespace BebooGarden;

public partial class Game1
{
  private readonly GraphicsDeviceManager _graphics;
  private SpriteBatch _spriteBatch;
  private Desktop _desktop;
  private Panel _gamePanel;
  public int Tickets { get; internal set; }

  private void SwitchToScreen(GameScreen screen)
  {
    _gameState.CurrentScreen = screen;
    switch (screen)
    {
      case GameScreen.First:
        break;
      case GameScreen.MainMenu:
        CreateEscapeMenu();
        _desktop.Root = _escapeMenuPanel;
        //Widget playButton = _mainMenuPanel.FindChildById("playButton");
        //playButton?.SetKeyboardFocus();
        break;
      case GameScreen.game:
        _desktop.Root = _gamePanel;
        break;
    }
  }

  private void UpdateUIState()
  {
  }
  protected override void Draw(GameTime gameTime)
  {
    GraphicsDevice.Clear(Color.Black);

    if (_gameState.CurrentScreen == GameScreen.BasicPractice && !_gameState.IsPaused)
    {
      _spriteBatch.Begin();
      _spriteBatch.End();
    }

    // Make myra interface
    _desktop.Render();

    base.Draw(gameTime);
  }

  internal void ChangeMap(Map map)
  {
    throw new NotImplementedException();
  }

  internal Beboo ChooseBeboo()
  {
    throw new NotImplementedException();
  }

  internal void GainTicket(int v)
  {
    throw new NotImplementedException();
  }
}
