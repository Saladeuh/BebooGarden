using System.Globalization;
using System.Numerics;
using BebooGarden.GameCore.Item;
using BebooGarden.GameCore.Item.MusicBox;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.Interface;
using BebooGarden.Interface.ScriptedScene;
using BebooGarden.Interface.Shop;
using BebooGarden.Save;
using FmodAudio;
using Timer = System.Windows.Forms.Timer;


namespace BebooGarden.GameCore;

internal class Game : IGlobalActions

{
  public static readonly Timer TickTimer = new();

  private bool _lastArrowWasUp;

  static Game()
  {
    SoundSystem = new SoundSystem();
    KeyState = [];
    foreach (Keys key in Enum.GetValues(typeof(Keys))) KeyState[key] = false;
  }

  public Game(Form1 form)
  {
    GameWindow = form;
    Parameters = SaveManager.LoadSave();
    Flags = Parameters.Flags;
    Map = Map.Maps[MapPresets.garden];
    Map.Items = Parameters.MapItems ?? [];
    MusicBox.AvailableRolls = Parameters.UnlockedRolls ?? [];
    SoundSystem.Volume = Parameters.Volume;
    SoundSystem.LoadMainScreen(Flags.NewGame);
    if (!Flags.NewGame)
    {
      PlayerPosition = new Vector3(0, 0, 0);
      Map.TreeLines[0].SetFruitsAfterAWhile(Parameters.LastPlayed, Parameters.RemainingFruits);
      Beboos[0] = new Beboo(Parameters.BebooName, Parameters.Age, Parameters.LastPlayed, Parameters.Happiness);
    }
    else
    {
      PlayerPosition = new Vector3(-2, 0, 0);
      Map.AddItem(new Egg(Parameters.FavoredColor), new(2,0,0));
    }

    SoundSystem.LoadMap(Map);
    if(Flags.NewGame) Welcome.AfterGarden();
    LastPressedKeyTime = DateTime.Now;
    TickTimer.Tick += Tick;
    TickTimer.Enabled = true;
    FruitsBasket = Parameters.FruitsBasket;
    FruitsBasket[FruitSpecies.Shrink] = 12;
    if (FruitsBasket == null || FruitsBasket.Count == 0)
    {
      FruitsBasket = [];
      foreach (FruitSpecies fruitSpecies in Enum.GetValues(typeof(FruitSpecies))) FruitsBasket[fruitSpecies] = 0;
    }

    PlayerName = Parameters.PlayerName;
    Tickets = Parameters.Tickets;
    Inventory = Parameters.Inventory;
    Inventory.Add(new TicketPack(2));
    Inventory.Add(new Duck());
  }

  public static SoundSystem SoundSystem { get; }
  public static Dictionary<Keys, bool> KeyState { get; private set; }
  private DateTime LastPressedKeyTime { get; set; }
  public static Beboo[] Beboos { get; set; } = new Beboo[3];
  private Vector3 PlayerPosition { get; set; }
  public SortedDictionary<FruitSpecies, int>? FruitsBasket { get; set; }
  public static Form1? GameWindow { get; private set; }
  public static Random Random { get; set; } = new();
  public SaveParameters Parameters { get; }
  public static Map? Map { get; private set; }
  public static Flags Flags { get; set; }
  public string PlayerName { get; }
  public static List<Item.Item> Inventory { get; set; } = [];
  public static int Tickets { get; set; } = 0;
  public Item.Item? ItemInHand { get; private set; }

  public static void GainTicket(int amount)
  {
    if (amount > 0)
    {
      Tickets += amount;
      SayLocalizedString("gainticket", amount);
      SoundSystem.System.PlaySound(SoundSystem.MenuOk2Sound);
      if (!Flags.UnlockShop)
      {
        Flags.UnlockShop = true;
        SoundSystem.System.PlaySound(SoundSystem.JingleComplete);
        ShopUnlock.Run();
      }
    }
  }
  public static void Call(object? sender, EventArgs eventArgs)
  {
    if (Beboos[0] == null || Beboos[0].Sleeping) return;
    SoundSystem.System.Get3DListenerAttributes(0, out var currentPosition, out _, out _, out _);
    Task.Run(async () =>
    {
      await Task.Delay(1000);
      Beboos[0]?.WakeUp();
    });
    var beboo = Beboos[0];
    if (beboo != null) beboo.GoalPosition = currentPosition;
  }

  public void KeyDownMapper(object sender, KeyEventArgs e)
  {
    if ((DateTime.Now - LastPressedKeyTime).TotalMilliseconds < 150) return;
    LastPressedKeyTime = DateTime.Now;
    var itemUnderCursor = Map?.GetItemArroundPosition(PlayerPosition);
    Map?.IsInLake(PlayerPosition);
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
        else
        {
          MoveOf(new Vector3(0, 1, 0));
        }
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
        else
        {
          MoveOf(new Vector3(0, -1, 0));
        }

        break;
      case Keys.F:
        SayBebooState();
        break;
      case Keys.G:
        SayBasketState();
        break;
      case Keys.T:
        SayTickets();
        break;
      case Keys.Enter:
        if (itemUnderCursor != null && itemUnderCursor.IsTakable) itemUnderCursor.Take();
        else if (Flags.UnlockShop && (Map?.IsArrundShop(PlayerPosition) ?? false)) new Shop().Show();
        break;
      case Keys.Escape:
        Dictionary<string, Item.Item> options = [];
        if (Inventory.Count > 0)
        {
          foreach (var item in Inventory)
          {
            int occurences = Inventory.FindAll(x => x.Name == item.Name).Count;
            string text = GetLocalizedString("inventory.item", item.Name, occurences);
            if (options.Keys.ToList().Find(x => x.Contains(item.Name)) == null && occurences == 1) options.Add(item.Name, item);
            else if (options.Keys.ToList().Find(x => x.Contains(text)) == null)
              options.Add(text, item);
          }
          ItemInHand = IWindowManager.ShowChoice("ui.chooseitem", options);
        }
        else
        {
          SayLocalizedString("ui.emptyinventory");
        }
        break;
      case Keys.F1:
        new Minigame.Race(60).Start();
        break;
      case Keys.Space:
        if (ItemInHand != null)
        {
          TryPutItemInHand();
        }
        else
        {
          if (KeyState[Keys.Space]) break;
          if (Beboos[0] != null && Util.IsInSquare(Beboos[0].Position, PlayerPosition, 1))
          {
            if (Beboos[0].Sleeping) Whistle();
            else FeedBeboo();
          }
          else if (Map?.GetTreeLineAtPosition(PlayerPosition) != null)
          {
          }
          else if (itemUnderCursor != null)
          {
            itemUnderCursor.Action();
          }
          else
          {
            Whistle();
          }
        }
        break;
      default:
        CheckGlobalActions(e.KeyCode);
        break;
    }

    KeyState[e.KeyCode] = true;
  }

  private static void SayTickets()
  {
    SayLocalizedString(Tickets.ToString());
  }

  private void TryPutItemInHand()
  {
    if (ItemInHand == null) return;
    var waterProof = (ItemInHand?.IsWaterProof ?? false);
    bool inWater = (Map?.IsInLake(PlayerPosition) ?? false);
    if (inWater && !waterProof)
    {
      SoundSystem.System.PlaySound(SoundSystem.WarningSound);
      SayLocalizedString("ui.warningwater");
    }
    else
    {
      if (ItemInHand != null)
      {
        Map?.AddItem(ItemInHand, PlayerPosition);
        SayLocalizedString("ui.itemput", ItemInHand.Name);
        if (inWater) SoundSystem.System.PlaySound(SoundSystem.ItemPutWaterSound);
        else SoundSystem.System.PlaySound(SoundSystem.ItemPutSound);
        Inventory.Remove(ItemInHand);
      }

      ItemInHand = null;
    }
  }
  private static void SayBebooState()
  {
    if (Beboos[0] == null) return;
    string sentence;
    if (Beboos[0].Sleeping) sentence = "beboo.sleep";
    else if (Beboos[0]?.Energy < 0) sentence = "beboo.verytired";
    else if (Beboos[0]?.Happiness < 0) sentence = "beboo.verysad";
    else if (Beboos[0]?.Energy < 5) sentence = "beboo.littletired";
    else if (Beboos[0]?.Happiness < 4) sentence = "beboo.littlesad";
    else if (Beboos[0]?.Energy < 8) sentence = "beboo.good";
    else sentence = "beboo.verygood";
    var name = Beboos[0]?.Name;
    if (name != null) SayLocalizedString(sentence, name);
    //ScreenReader.Output($"Energy {Beboo.Energy}, hapiness {Beboo.Happiness}");
  }

  private void SayBasketState()
  {
    if (FruitsBasket != null) SayLocalizedString("ui.basket", FruitsBasket[FruitSpecies.Normal]);
  }

  private void ShakeOrPetAtPlayerPosition()
  {
    var treeLine = Map?.GetTreeLineAtPosition(PlayerPosition);
    if (treeLine != null)
    {
      var dropped = treeLine.Shake();
      if (dropped != null)
        if (FruitsBasket != null)
          FruitsBasket[dropped.Value]++;
    }
    else if (Beboos[0] != null && Util.IsInSquare(Beboos[0].Position, PlayerPosition, 1)) ;
    {
      Beboos[0]?.GetPetted();
    }
  }

  private void FeedBeboo()
  {
    if (Beboos[0] == null) return;
    if (FruitsBasket == null) return;
    Dictionary<string, FruitSpecies> options = [];
    foreach (var fruit in FruitsBasket)
    {
      if (fruit.Value > 0) options.Add(GetLocalizedString($"{fruit.Key.ToString()} {fruit.Value}"), fruit.Key);
    }
    if (options.Count > 0)
    {
      var choice = IWindowManager.ShowChoice("ui.chooseitem", options);
      if (choice != null && choice!=FruitSpecies.None)
      {
        Beboos[0]?.Eat(choice);
        FruitsBasket[choice]--;
      }
    }
  }

  private void Whistle()
  {
    SoundSystem.System.Get3DListenerAttributes(0, out var currentPosition, out _, out _, out _);
    SoundSystem.Whistle();
    Task.Run(async () =>
    {
      await Task.Delay(1000);
      Beboos[0]?.WakeUp();
    });
    if (Beboos[0] != null) Beboos[0].GoalPosition = currentPosition;
  }

  private void MoveOf(Vector3 movement)
  {
    if (Map == null) return;
    var newPos = Map.Clamp(PlayerPosition + movement);
    if (newPos != PlayerPosition + movement) SoundSystem.WallBouncing();
    PlayerPosition = newPos;
    SoundSystem.MovePlayerTo(newPos);
    if (Map.IsInLake(newPos)) SayLocalizedString("water");
    else if (Flags.UnlockShop && (Map?.IsArrundShop(PlayerPosition) ?? false)) SayLocalizedString("shop");
    SpeakObjectUnderCursor();
  }

  private void SpeakObjectUnderCursor()
  {
    var treeLine = Map?.GetTreeLineAtPosition(PlayerPosition);
    var item = Map?.GetItemArroundPosition(PlayerPosition);
    foreach (var beboo in Beboos)
    {
      if (beboo != null && Util.IsInSquare(beboo.Position, PlayerPosition, 1))
        ScreenReader.Output(beboo.Name);
    }
    if (treeLine != null)
      SayLocalizedString("trees");
    else if (item != null) SayLocalizedString(item.Name);
  }

  private void Tick(object? _, EventArgs __)
  {
    if (Map != null && Map.IsLullabyPlaying) Beboos[0]?.GoAsleep();
    SoundSystem.System.Update();
  }

  public static void KeyUpMapper(object? sender, KeyEventArgs e)
  {
    KeyState[e.KeyCode] = false;
  }
  public static void ResetKeyState()
  {
    KeyState = [];
    foreach (Keys key in Enum.GetValues(typeof(Keys))) KeyState[key] = false;
  }

  public static void Pause()
  {
    Beboos[0].Pause();
    SoundSystem.DisableAmbiTimer();
    if (Map == null) return;
    SoundSystem.Pause(Map);
  }
  public static void Unpause()
  {
    Beboos[0].Unpause();
    SoundSystem.EnableAmbiTimer();
    if (Map == null) return;
    SoundSystem.Unpause(Map);
  }
  internal void Close(object? sender, FormClosingEventArgs e)
  {
    Map?.Items.RemoveAll(item => typeof(Roll) == item.GetType());
    var parameters = new SaveParameters(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName,
       SoundSystem.Volume,
       Beboos[0]?.Name ?? "",
       energy: Beboos[0]?.Energy ?? 5,
       happiness: Beboos[0]?.Happiness ?? 5,
       age: Beboos[0]?.Age ?? 0,
       lastPayed: DateTime.Now,
       flags: Flags,
       playerName: PlayerName,
       fruitsBasket: FruitsBasket ?? [],
       remainingFruits: Map?.TreeLines[0].Fruits ?? 0,
       inventory: Inventory,
       tickets: Tickets,
       mapItems: Map?.Items ?? [],
       unlockedRolls: MusicBox.AvailableRolls,
       favoredColor: Parameters.FavoredColor
   );
    parameters.Flags.NewGame = Game.Beboos == null;
    SaveManager.WriteJson(parameters);
  }
  private static Map? _backedMap;
  internal static void ChangeMap(Map map)
  {
    if (Game.Map != null) Game.SoundSystem.Pause(Game.Map);
    _backedMap = Map;
    Map = map;
    SoundSystem.LoadMap(map);
  }
  internal static void LoadBackedMap()
  {
    if (_backedMap == null) return;
    SoundSystem.Pause(Map);
    Map = _backedMap;
    _backedMap = null;
    SoundSystem.Unpause(Map);
  }
}