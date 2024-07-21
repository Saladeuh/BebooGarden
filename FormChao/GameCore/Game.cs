using System.Globalization;
using System.Numerics;
using BebooGarden.GameCore.Item;
using BebooGarden.GameCore.Item.MusicBox;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.Interface;
using BebooGarden.Interface.ScriptedScene;
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
  public static Form GameWindow { get; private set; }
  public static Map Map { get; private set; }
  public Flags Flags { get; }
  public string PlayerName { get; }
  public static List<IItem> Inventory { get; set; } = new List<IItem>();
  public IItem? ItemInHand { get; private set; }

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
    Map.Items=parameters.MapItems ?? new();
    MusicBox.AvailableRolls=parameters.UnlockedRolls ?? new();
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
    Inventory=parameters.Inventory;
    //Inventory.Add(new MusicBox());
    Map.AddItem(MusicBox.AllRolls[5], new Vector3(0,5,0));
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
    IItem? itemUnderCursor = Map.GetItemArroundPosition(PlayerPosition);
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
        break;
      case Keys.Enter:
        itemUnderCursor?.Take();
        break;
      case Keys.Escape:
        Dictionary<string, IItem> options = new();
        if (Inventory.Count > 0)
        {
          foreach (var item in Inventory)
          {
            options.Add(GetLocalizedString(item.TranslateKeyName), item);
          }
          ItemInHand=IWindowManager.ShowChoice<IItem>("ui.chooseitem", options);
        }
        else SayLocalizedString("ui.emptyinventory");
        break;
      case Keys.Space:
        if (ItemInHand != null) PutItemInHand();
        else
        {
          if (KeyState[Keys.Space]) break;
          if (Util.IsInSquare(Beboo.Position, PlayerPosition, 1))
          {
            if (Beboo.Sleeping) Whistle();
            else FeedBeboo();
          }
          else if (Map.GetTreeLineAtPosition(PlayerPosition) != null) break;
          else if (itemUnderCursor != null) itemUnderCursor.Action();
          else Whistle();
        }
        break;
      default:
        CheckGlobalActions(e.KeyCode);
        break;
    }
    KeyState[e.KeyCode] = true;
  }

  private void PutItemInHand()
  {
    if (ItemInHand == null) return;
    Map.AddItem(ItemInHand, PlayerPosition);
    SayLocalizedString("ui.itemput",GetLocalizedString( ItemInHand.TranslateKeyName));
    SoundSystem.System.PlaySound(SoundSystem.ItemPutSound);
    Inventory.Remove(ItemInHand);
    ItemInHand = null;
  }

  private void SayBebooState()
  {
    var sentence="";
    if (Beboo.Sleeping) sentence = "beboo.sleep";
    else if (Beboo.Energy < 0) sentence = "beboo.verytired";
    else if (Beboo.Happiness < 0) sentence = "beboo.verysad";
    else if (Beboo.Energy < 5) sentence = "beboo.littletired";
    else if (Beboo.Happiness < 4) sentence = "beboo.littlesad";
    else if (Beboo.Energy < 8) sentence = "beboo.good";
    else sentence = "beboo.verygood";
    SayLocalizedString(sentence, Beboo.Name);
    //ScreenReader.Output($"Energy {Beboo.Energy}, hapiness {Beboo.Happiness}");
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
    TreeLine? treeLine = Map.GetTreeLineAtPosition(PlayerPosition);
    IItem? item = Map.GetItemArroundPosition(PlayerPosition);
    if (Util.IsInSquare(Beboo.Position, PlayerPosition, 1))
    {
      ScreenReader.Output(Beboo.Name);
    }
    else if (treeLine != null)
    {
      SayLocalizedString("trees");
    }
    else if (item != null)
    {
      SayLocalizedString(item.TranslateKeyName);
    }
  }

  private void Tick(object? _, EventArgs __)
  {
    if(Map.IsLullabyPlaying) Beboo.GoAsleep();
    SoundSystem.System.Update();
  }

  internal void KeyUpMapper(object? sender, KeyEventArgs e)
  {
    KeyState[e.KeyCode] = false;
  }

  internal void Close(object? sender, FormClosingEventArgs e)
  {
    var parameters = new SaveParameters(language: (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName),
    volume: SoundSystem.Volume,
    bebooName: Beboo.Name,
    energy: Beboo.Energy,
    happiness: Beboo.Happiness,
    age: Beboo.Age,
    lastPayed: DateTime.Now,
    flags: Flags,
    playerName: PlayerName,
    fruitsBasket: FruitsBasket,
    remainingFruits: Map.TreeLines[0].Fruits,
    inventory: Inventory,
    mapItems: Map.Items,
    unlockedRolls: MusicBox.AvailableRolls
  );
    SaveManager.WriteJson(parameters);
  }
}