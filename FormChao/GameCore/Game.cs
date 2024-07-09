using System.Numerics;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.Interface;
using BebooGarden.Save;
namespace BebooGarden.GameCore;

internal class Game : IGlobalActions
{
  public static SoundSystem SoundSystem { get; private set; }
  private Dictionary<Keys, bool> KeyState { get; set; }
  private DateTime LastPressedKeyTime { get; set; }
  static readonly System.Windows.Forms.Timer TickTimer = new();
  public Beboo Beboo { get; set; }
  private Vector3 PlayerPosition { get; set; }
  private SortedDictionary<FruitSpecies, int> FruitsBasket { get; set; }
  public static Map Map { get; private set; }
  public Game(Parameters parameters)
  {
    Map = new(40, 40,
      [new TreeLine(new Vector2(20, 20), new Vector2(20, -20))],
      new Vector3(-15, 0, 0));
    SoundSystem = new SoundSystem(parameters.Volume);
    SoundSystem.LoadMainScreen();
    SoundSystem.LoadMap(Map);
    LastPressedKeyTime = DateTime.Now;
    TickTimer.Tick += Tick;
    PlayerPosition = new Vector3(0, 0, 0);
    Beboo = new(parameters.BebooName, parameters.Age, parameters.LastPayed);
    FruitsBasket = [];
    foreach (FruitSpecies fruitSpecies in Enum.GetValues(typeof(FruitSpecies)))
    {
      FruitsBasket[fruitSpecies] = 0;
    }
    KeyState = [];
    foreach (Keys key in Enum.GetValues(typeof(Keys)))
    {
      KeyState[key] = false;
    }
  }
  bool _lastArrowWasUp=false;
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
            ShakeAtPlayerPosition();
            _lastArrowWasUp = true;
          }
        }
        else MoveOf(new Vector3(0, 1, 0));
        break;
      case Keys.Down:
      case Keys.S:
        if (KeyState[Keys.Space])
        {
          if (_lastArrowWasUp) {
            ShakeAtPlayerPosition();
            _lastArrowWasUp = false;
          }
        }
        else MoveOf(new Vector3(0, -1, 0));
        break;
      case Keys.Space:
        if (KeyState[Keys.Space]) break;
        if (Util.IsInSquare(Beboo.Position, PlayerPosition, 1))
        {
          if (Beboo.Mood == Mood.Sleeping) Whistle();
          else FeedBeboo();
        } else if(Map.GetTreeLineAtPosition(PlayerPosition)!=null) break;
        else Whistle();
        break;
      default:
        CheckGlobalActions(e.KeyCode);
        break;
    }
    KeyState[e.KeyCode] = true;
  }

  private void ShakeAtPlayerPosition()
  {
    var treeLine = Map.GetTreeLineAtPosition(PlayerPosition);
    if (treeLine != null)
    {
      var dropped = treeLine.Shake();
      if (dropped != null) FruitsBasket[dropped.Value]++;
    }
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
    Beboo.WakeUp();
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
}