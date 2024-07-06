using System.Numerics;
using System.Timers;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.Interface;
using BebooGarden.Save;
namespace BebooGarden.GameCore;

internal class Game
{
  public static SoundSystem SoundSystem { get; set; }
  private GlobalActions GlobalActions { get; set; }
  private Dictionary<Keys, bool> KeyState { get; set; }
  private DateTime LastPressedKeyTime { get; set; }
  static readonly System.Windows.Forms.Timer tickTimer = new();
  public Beboo beboo { get; set; }
  public Vector3 PlayerPosition { get; private set; }
  public SortedDictionary<FruitSpecies, int> FruitsBasket { get; set; }
  public static BebooGarden.GameCore.World.Map Map { get; set; }
  public Game(Parameters parameters)
  {
    Map = new(40, 40,
      [new TreeLine(new Vector2(20, 20), new Vector2(20, -20))],
      new Vector3(-15, 0, 0));
    SoundSystem = new SoundSystem(parameters.Volume);
    SoundSystem.LoadMainScreen();
    SoundSystem.LoadMap(Map);
    GlobalActions = new GlobalActions(SoundSystem);
    LastPressedKeyTime = DateTime.Now;
    tickTimer.Tick += new EventHandler(Tick);
    PlayerPosition = new Vector3(0, 0, 0);
    beboo = new(parameters.BebooName, parameters.Age, parameters.LastPayed);
    FruitsBasket = new();
    foreach (FruitSpecies fruitSpecies in Enum.GetValues(typeof(FruitSpecies)))
    {
      FruitsBasket[fruitSpecies] = 0;
    }
    KeyState = new();
    foreach (Keys key in Enum.GetValues(typeof(Keys)))
    {
      KeyState[key] = false;
    }
  }
  public void KeyDownMapper(object sender, KeyEventArgs e)
  {
    if ((DateTime.Now - LastPressedKeyTime).TotalMilliseconds < 150) return;
    else LastPressedKeyTime = DateTime.Now;
    switch (e.KeyCode)
    {
      case System.Windows.Forms.Keys.Left:
      case System.Windows.Forms.Keys.Q:
        MoveOf(new Vector3(-1, 0, 0));
        break;
      case System.Windows.Forms.Keys.Right:
      case System.Windows.Forms.Keys.D:
        MoveOf(new Vector3(1, 0, 0));
        break;
      case System.Windows.Forms.Keys.Up:
      case System.Windows.Forms.Keys.Z:
        if (KeyState[Keys.Space])
        {
          ShakeAtPlayerPosition();
        }
        else MoveOf(new Vector3(0, 1, 0));
        break;
      case System.Windows.Forms.Keys.Down:
      case System.Windows.Forms.Keys.S:
        if (KeyState[Keys.Space])
        {
          ShakeAtPlayerPosition();
        }
        else MoveOf(new Vector3(0, -1, 0));
        break;
      case System.Windows.Forms.Keys.Space:
        if (KeyState[Keys.Space]) break;
        if (Util.IsInSquare(beboo.Position, PlayerPosition, 1))
        {
          if (beboo.Mood == Mood.Sleeping) Whistle();
          else FeedBeboo();
        } else if(Map.GetTreeLineAtPosition(PlayerPosition)!=null) break;
        else Whistle();
        break;
      default:
        GlobalActions.CheckGlobalActions(e.KeyCode);
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
      beboo.Eat(FruitSpecies.Normal);
      FruitsBasket[FruitSpecies.Normal]--;
    }
  }

  private void Whistle()
  {
    SoundSystem.System.Get3DListenerAttributes(0, out Vector3 currentPosition, out _, out _, out _);
    SoundSystem.Whistle();
    beboo.WakeUp();
    beboo.GoalPosition = currentPosition;
  }

  public void MoveOf(Vector3 movement)
  {
    var newPos = Map.Clamp(PlayerPosition + movement);
    if (newPos != PlayerPosition + movement) SoundSystem.WallBouncing();
    PlayerPosition = newPos;
    SoundSystem.MovePlayerTo(newPos);
    SpeakObjectUnderCursor();
  }

  private void SpeakObjectUnderCursor()
  {
    if (Util.IsInSquare(beboo.Position, PlayerPosition, 1))
    {
      ScreenReader.Output(beboo.Name);
    }
    else if (Map.GetTreeLineAtPosition(PlayerPosition) != null)
    {
      ScreenReader.Output("Arbre");
    }
  }

  public void Tick(object? _, EventArgs __)
  {
    SoundSystem.System.Update();
  }

  internal void KeyUpMapper(object? sender, KeyEventArgs e)
  {
    KeyState[e.KeyCode] = false;
  }
}