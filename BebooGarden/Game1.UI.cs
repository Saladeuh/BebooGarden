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
  private Panel _mainMenuPanel;
  private Panel _gamePanel;
  private Panel _settingsPanel;

  public int Tickets { get; internal set; }

  private void SwitchToScreen(GameScreen screen)
  {
    /*
    _gameState.CurrentScreen = screen;
    switch (screen)
    {
      case GameScreen.First:
        _desktop.Root = _firstScreenPanel;
        Widget tipsLabel = _firstScreenPanel.FindChildById("tipsLabel");
        tipsLabel?.SetKeyboardFocus();
        break;
      case GameScreen.MainMenu:
        _desktop.Root = _mainMenuPanel;
        Widget playButton = _mainMenuPanel.FindChildById("playButton");
        playButton?.SetKeyboardFocus();
        if (MediaPlayer.Queue.ActiveSong != _titleScreenSong)
          MediaPlayer.Play(_titleScreenSong);
        break;
      case GameScreen.BrailleTableView:
        CreateBrailleTableView(culture);
        _desktop.Root = _brailleTableViewPanels[culture];
        UpdateUIState();
        MediaPlayer.Play(_brailleTableViewSong);
        break;
      case GameScreen.BasicPractice:
        MediaPlayer.Play(_basicPracticeSong);
        CreateBasicPracticeUI(culture);
        CurrentPlayingMiniGame = new BasicPractice(culture, Save.Flags.FirstPlayBasicPractice);
        _desktop.Root = _basicPracticePanels[culture];
        UpdateUIState();
        break;
      case GameScreen.WordPractice:
        MediaPlayer.Play(_basicPracticeSong);
        CreateWordPracticeUI(culture);
        CurrentPlayingMiniGame = new WordPractice(culture, Save.Flags.FirstPlayBasicPractice);
        _desktop.Root = _wordPracticePanels[culture];
        UpdateUIState();
        break;
      case GameScreen.ChoicePractice:
        MediaPlayer.Play(_basicPracticeSong);
        CreateChoicePracticeUI(culture);
        CurrentPlayingMiniGame = new ChoicePractice(culture, Save.Flags.FirstPlayChoicePractice);
        _desktop.Root = _choicePracticePanels[culture];
        UpdateUIState();
        break;
      case GameScreen.Settings:
        _desktop.Root = _settingsPanel;
        Widget volumeSlider = _settingsPanel.FindChildById("volumeSlider");
        volumeSlider?.SetKeyboardFocus();
        UpdateUIState();
        break;
    }
    */
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

  internal void UpdateMapMusic()
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
