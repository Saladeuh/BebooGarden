using System.Globalization;
using System.Numerics;
using BebooGarden.GameCore.Item;
using BebooGarden.GameCore.Item.MusicBox;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.Interface;
using BebooGarden.Interface.UI;
using BebooGarden.Save;
namespace BebooGarden.GameCore;

internal class Game : IGlobalActions
{
  public static SoundSystem SoundSystem { get; private set; }
  private Dictionary<Keys, bool> KeyState { get; set; }
  private DateTime LastPressedKeyTime { get; set; }
  static readonly System.Windows.Forms.Timer TickTimer = new();
  private BebooSpeechRecognition BebooSpeechRecognition { get; }
  public Beboo Beboo { get; set; }
  private Vector3 PlayerPosition { get; set; }
  public SortedDictionary<FruitSpecies, int> FruitsBasket { get; set; }
  public static Form GameWindow { get; set; }
  public static Map Map { get; private set; }
  public Flags Flags { get; }
  public string PlayerName { get; }
  public static List<IItem> Inventory { get; set; } = new List<IItem>();
  static Game()
  {
    SoundSystem = new SoundSystem();
  }
  public Game(Form form)
  {
    GameWindow = form;
    var parameters = SaveManager.LoadSave();
    Flags = parameters.Flags;
    Map = new("garden", 40, 40,
    [new TreeLine(new Vector2(20, 20), new Vector2(20, -20))],
    new Vector3(-15, 0, 0));
    if (!Flags.NewGame) Map.TreeLines[0].SetFruitsAfterAWhile(parameters.LastPlayed, parameters.RemainingFruits);
    SoundSystem.Volume = parameters.Volume;
    SoundSystem.LoadMainScreen();
    SoundSystem.LoadMap(Map);
    LastPressedKeyTime = DateTime.Now;
    TickTimer.Tick += Tick;
    TickTimer.Enabled = true;
    PlayerPosition = new Vector3(0, 0, 0);
    Beboo = new(parameters.BebooName, parameters.Age, parameters.LastPlayed, parameters.Energy, parameters.Happiness);
    BebooSpeechRecognition = new(Beboo.Name);
    BebooSpeechRecognition.BebooCalled += Call;
    FruitsBasket = parameters.FruitsBasket;
    if (FruitsBasket == null || FruitsBasket.Count == 0)
    {
      FruitsBasket = [];
      foreach (FruitSpecies fruitSpecies in Enum.GetValues(typeof(FruitSpecies)))
      {
        FruitsBasket[fruitSpecies] = 0;
      }
    }
    KeyState = [];
    foreach (Keys key in Enum.GetValues(typeof(Keys)))
    {
      KeyState[key] = false;
    }
    PlayerName = parameters.PlayerName;
  }

  private void Call(object? sender, EventArgs e)
  {
    if (Beboo.Sleeping) return;
    SoundSystem.System.Get3DListenerAttributes(0, out Vector3 currentPosition, out _, out _, out _);
    Task.Run(async () =>
    {
      await Task.Delay(1000);
      Beboo.WakeUp();
    });
    Beboo.GoalPosition = currentPosition;
  }

  bool _lastArrowWasUp = false;
  public void KeyDownMapper(object sender, KeyEventArgs e)
  {
    if ((DateTime.Now - LastPressedKeyTime).TotalMilliseconds < 150) return;
    else LastPressedKeyTime = DateTime.Now;
    switch (e.KeyCode)
    {
      case Keys.Left:
      case Keys.Q:
        MoveOf(new Vector3(-1, 0, 0));
        break;
      case Keys.Right:
      case Keys.D:
        MoveOf(new Vector3(1, 0, 0));
        break;
      case Keys.Up:
      case Keys.Z:
        if (KeyState[Keys.Space])
        {
          if (!_lastArrowWasUp)
          {
            ShakeOrPetAtPlayerPosition();
            _lastArrowWasUp = true;
          }
        }
        else MoveOf(new Vector3(0, 1, 0));
        break;
      case Keys.Down:
      case Keys.S:
        if (KeyState[Keys.Space])
        {
          if (_lastArrowWasUp)
          {
            ShakeOrPetAtPlayerPosition();
            _lastArrowWasUp = false;
          }
        }
        else MoveOf(new Vector3(0, -1, 0));
        break;
      case Keys.F:
        SayBebooState();
        SoundSystem.System.GetChannelsPlaying(out int channel, out int real);
        //ScreenReader.Output($"{channel} {real}");
        break;
      case Keys.Enter:
        var textmenu = new TextForm("titre", 12, true);
        //GameWindow.Hide();
        //textmenu.Show();
        textmenu.ShowDialog(GameWindow);
        break;
      case Keys.Escape:
        var menu = new ChooseMenu<string>("test", new Dictionary<string, string>
        {
          { "test", "b" },
          { "waw", "a" }
        });
        menu.FormClosing += EscapeMenuResultHandle;
        //GameWindow.Hide();
        //menu.Show();
        menu.ShowDialog(GameWindow);
        break;
      case Keys.Space:
        if (KeyState[Keys.Space]) break;
        if (Util.IsInSquare(Beboo.Position, PlayerPosition, 1))
        {
          if (Beboo.Sleeping) Whistle();
          else FeedBeboo();
        }
        else if (Map.GetTreeLineAtPosition(PlayerPosition) != null) break;
        else Whistle();
        break;
      default:
        CheckGlobalActions(e.KeyCode);
        break;
    }
    KeyState[e.KeyCode] = true;
  }

  private void EscapeMenuResultHandle(object? sender, FormClosingEventArgs e)
  {
    ChooseMenu<string> form = sender as ChooseMenu<string>;
    ScreenReader.Output(form.Result);
  }

  private void SayBebooState()
  {
    ScreenReader.Output($"Energy {Beboo.Energy}, hapiness {Beboo.Happiness}");
  }

  private void ShakeOrPetAtPlayerPosition()
  {
    var treeLine = Map.GetTreeLineAtPosition(PlayerPosition);
    if (treeLine != null)
    {
      var dropped = treeLine.Shake();
      if (dropped != null) FruitsBasket[dropped.Value]++;
    }
    else Beboo.GetPetted();
  }

  private void FeedBeboo()
  {
    if (FruitsBasket[FruitSpecies.Normal] > 0)
    {
      Beboo.Eat(FruitSpecies.Normal);
      FruitsBasket[FruitSpecies.Normal]--;
    }
  }

  private void Whistle()
  {
    SoundSystem.System.Get3DListenerAttributes(0, out Vector3 currentPosition, out _, out _, out _);
    SoundSystem.Whistle();
    Task.Run(async () =>
    {
      await Task.Delay(1000);
      Beboo.WakeUp();
    });
    Beboo.GoalPosition = currentPosition;
  }

  private void MoveOf(Vector3 movement)
  {
    var newPos = Map.Clamp(PlayerPosition + movement);
    if (newPos != PlayerPosition + movement) SoundSystem.WallBouncing();
    PlayerPosition = newPos;
    SoundSystem.MovePlayerTo(newPos);
    SpeakObjectUnderCursor();
  }

  private void SpeakObjectUnderCursor()
  {
    if (Util.IsInSquare(Beboo.Position, PlayerPosition, 1))
    {
      ScreenReader.Output(Beboo.Name);
    }
    else if (Map.GetTreeLineAtPosition(PlayerPosition) != null)
    {
      SayLocalizedString("trees");
    }
  }

  private void Tick(object? _, EventArgs __)
  {
    SoundSystem.System.Update();
  }

  internal void KeyUpMapper(object? sender, KeyEventArgs e)
  {
    KeyState[e.KeyCode] = false;
  }

  internal void Close(object? sender, FormClosingEventArgs e)
  {
    var parameters = new SaveParameters(language: (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName ?? "en"),
  volume: SoundSystem.Volume,
  bebooName: Beboo.Name,
  energy: Beboo.Energy,
  happiness: Beboo.Happiness,
  age: Beboo.Age,
  lastPayed: DateTime.Now,
  flags: Flags,
  playerName: PlayerName,
  fruitsBasket: FruitsBasket,
  remainingFruits: Map.TreeLines[0].Fruits
  );
    SaveManager.WriteJson(parameters);
  }
}