using BebooGarden.Save;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Myra;
using Myra.Graphics2D.UI;

namespace BebooGarden;

public partial class Game1
{
  public SoundEffect UIConfirmSound { get => _UiConfirmSound; private set => _UiConfirmSound = value; }
  public SoundEffect UIGoodSound { get; private set; }
  public SoundEffect UIVictorySound { get; private set; }
  public SoundEffect UIFailSound { get; private set; }
  public SoundEffect UIViewScrollSound { get; private set; }
  public SoundEffect UIBackSound { get; private set; }
  private SoundEffect _UiConfirmSound;
  private Song _titleScreenSong;
  private Song _brailleTableViewSong;
  private Song _basicPracticeSong;
  protected override void LoadContent()
  {
    _spriteBatch = new SpriteBatch(GraphicsDevice);
    Save = SaveManager.LoadSave();
    MediaPlayer.Volume = 0.3f;
    SoundEffect.MasterVolume = 1f;
    MyraEnvironment.Game = this;
    _desktop = new Desktop
    {
      HasExternalTextInput = true
    };
    Window.TextInput += (s, a) =>
    {
      _desktop.OnChar(a.Character);
    };
    /*  CreateMainMenu();
    CreateSettingsUI();
    CreateFirstScreen();
    CreateTableToLearnScreen();
    SwitchToScreen(Save.Flags.EmptySave ? GameScreen.First : GameScreen.MainMenu);*/
  }
}