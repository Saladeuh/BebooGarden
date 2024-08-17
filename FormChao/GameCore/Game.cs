using System.Globalization;
using System.Numerics;
using BebooGarden.GameCore.Item;
using BebooGarden.GameCore.Item.MusicBox;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.Interface;
using BebooGarden.Interface.EscapeMenu;
using BebooGarden.Interface.ScriptedScene;
using BebooGarden.Interface.Shop;
using BebooGarden.Save;
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
    Flags.UnlockSnowyMap = true;
    try { Map = Map.Maps[Parameters.CurrentMap]; } catch (Exception e) { Map = Map.Maps[MapPreset.garden]; }
    MusicBox.AvailableRolls = Parameters.UnlockedRolls ?? [];
    SoundSystem.Volume = Parameters.Volume;
    SoundSystem.LoadMainScreen(Flags.NewGame);
    if (!Flags.NewGame)
    {
      PlayerPosition = new Vector3(0, 0, 0);
      foreach (var map in Map.Maps.Values)
      {
        if (map.TreeLines.Count > 0) map.TreeLines[0].SetFruitsAfterAWhile(Parameters.LastPlayed, Parameters.MapInfos[map.Preset].RemainingFruits);
        try
        {
          map.Items = Parameters.MapInfos[map.Preset].Items;
        }
        catch
        {
          map.Items = new();
        }
        foreach (var bebooInfo in Parameters.MapInfos[map.Preset].BebooInfos)
        {
          var beboo = new Beboo(bebooInfo.Name, bebooInfo.Age, Parameters.LastPlayed, bebooInfo.Happiness, bebooInfo.SwimLevel);
          map.Beboos.Add(beboo);
          if (map != Map) beboo.Pause();
        }
      }
    }
    else
    {
      PlayerPosition = new Vector3(-2, 0, 0);
      Map.AddItem(new Egg(Parameters.FavoredColor), new(2, 0, 0));
      Map.AddItem(new Egg(Parameters.FavoredColor), new(-2, 0, 0));
    }
    SoundSystem.LoadMap(Map);
    UpdateMapMusic();
    if (Flags.NewGame) Welcome.AfterGarden();
    LastPressedKeyTime = DateTime.Now;
    TickTimer.Tick += Tick;
    TickTimer.Enabled = true;
    FruitsBasket = Parameters.FruitsBasket;
    if (FruitsBasket == null || FruitsBasket.Count == 0)
    {
      FruitsBasket = [];
      foreach (FruitSpecies fruitSpecies in Enum.GetValues(typeof(FruitSpecies))) FruitsBasket[fruitSpecies] = 0;
    }

    PlayerName = Parameters.PlayerName;
    Tickets = Parameters.Tickets;
    Inventory = Parameters.Inventory;
    Inventory.Add(new Duck());
  }


  public static SoundSystem SoundSystem { get; }
  public static Dictionary<Keys, bool> KeyState { get; private set; }
  private DateTime LastPressedKeyTime { get; set; }
  public static Vector3 PlayerPosition { get; private set; }
  public SortedDictionary<FruitSpecies, int>? FruitsBasket { get; set; }
  public static Form1? GameWindow { get; private set; }
  public static Random Random { get; set; } = new();
  public SaveParameters Parameters { get; }
  public static Map? Map { get; private set; }
  public static Flags Flags { get; set; }
  public string PlayerName { get; }
  public static List<Item.Item> Inventory { get; set; } = [];
  public static int Tickets { get; set; } = 0;
  public static Item.Item? ItemInHand { get; set; }

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
    SoundSystem.System.Get3DListenerAttributes(0, out var currentPosition, out _, out _, out _);
    foreach (var beboo in Map.Beboos)
    {
      if (beboo.Sleeping || !beboo.KnowItsName) continue;
      Task.Run(async () =>
      {
        await Task.Delay(1000);
        beboo.WakeUp();
      });
      beboo.GoalPosition = currentPosition;
    }
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
        if (KeyState[Keys.Enter])
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
        if (KeyState[Keys.Enter])
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
        else if (Flags.UnlockShop && (Map?.IsArroundShop(PlayerPosition) ?? false)) new Shop().Show();
        else if (Flags.UnlockSnowyMap && (Map?.IsArrundMapPath(PlayerPosition) ?? false))
        {
          Map richedMap;
          if (Map.Preset == MapPreset.snowy) richedMap = Map.Maps[MapPreset.garden];
          else richedMap = Map.Maps[MapPreset.snowy];
          var beboosHere = BeboosUnderCursor(2);
          foreach (var transferedBeboo in beboosHere)
          {
            Map?.Beboos.Remove(transferedBeboo);
            transferedBeboo.Position = new(0, 0, 0);
            richedMap.Beboos.Add(transferedBeboo);
          }
          ChangeMap(richedMap);
          UpdateMapMusic();
          PlayerPosition = new(0, 0, 0);
        }
        else if ((Map?.IsArroundRaceGate(PlayerPosition) ?? false))
        {
          new Minigame.Race(Minigame.Race.BASERACELENGTH, Map?.Beboos[0]).Start();
        }
        break;
      case Keys.Escape:
        new EscapeMenu().Show();
        break;
      case Keys.F1:
        break;
      case Keys.Space:
        if (ItemInHand != null)
        {
          TryPutItemInHand();
        }
        else
        {
          Beboo? bebooUnderCursor = BebooUnderCursor();
          if (bebooUnderCursor != null)
          {
            if (bebooUnderCursor.Sleeping) Whistle();
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
  public static Beboo? BebooUnderCursor()
  {
    foreach (var beboo in Map.Beboos)
    {
      if (Util.IsInSquare(beboo.Position, PlayerPosition, 1)) return beboo;
    }
    return null;
  }
  public List<Beboo> BeboosUnderCursor(int squareSide = 1)
  {
    var beboos = new List<Beboo>();
    foreach (var beboo in Map.Beboos)
    {
      if (Util.IsInSquare(beboo.Position, PlayerPosition, squareSide)) beboos.Add(beboo);
    }
    return beboos;
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
    if (Map?.Beboos.Count == 0)
    {
      SayLocalizedString("nobeboo");
    }
    foreach (var beboo in Map?.Beboos)
    {
      string sentence;
      if (beboo.Sleeping) sentence = "beboo.sleep";
      else if (beboo.Energy < 0) sentence = "beboo.verytired";
      else if (beboo.Happiness < 0) sentence = "beboo.verysad";
      else if (beboo.Energy < 5) sentence = "beboo.littletired";
      else if (beboo.Happiness < 4) sentence = "beboo.littlesad";
      else if (beboo.Energy < 8) sentence = "beboo.good";
      else sentence = "beboo.verygood";
      var name = beboo.Name;
      SayLocalizedString(sentence, name);
#if DEBUG
      ScreenReader.Output(beboo.Age.ToString());
#endif
    }
  }

  private void SayBasketState()
  {
    if (FruitsBasket != null) SayLocalizedString("ui.basket", FruitsBasket.Count);
  }

  private void ShakeOrPetAtPlayerPosition()
  {
    var treeLine = Map?.GetTreeLineAtPosition(PlayerPosition);
    var bebooUnderCursor = BebooUnderCursor();
    if (treeLine != null)
    {
      var dropped = treeLine.Shake();
      if (dropped != null)
        if (FruitsBasket != null)
          if (FruitsBasket.TryGetValue(dropped.Value, out _)) FruitsBasket[dropped.Value]++;
          else FruitsBasket[dropped.Value] = 1;
    }
    else if (bebooUnderCursor != null)
    {
      bebooUnderCursor.GetPetted();
    }
  }

  private void FeedBeboo()
  {
    var bebooUnderCursor = BebooUnderCursor();
    if (FruitsBasket == null || bebooUnderCursor == null) return;
    Dictionary<string, FruitSpecies> options = [];
    foreach (var fruit in FruitsBasket)
    {
      if (fruit.Value > 0) options.Add(GetLocalizedString(fruit.Key.ToString())+" "+fruit.Value.ToString(), fruit.Key);
    }
    if (options.Count > 0)
    {
      var choice = IWindowManager.ShowChoice("ui.chooseitem", options);
      if (choice != FruitSpecies.None)
      {
        bebooUnderCursor.Eat(choice);
        FruitsBasket[choice]--;
      }
    }
  }

  private void Whistle()
  {
    SoundSystem.System.Get3DListenerAttributes(0, out var currentPosition, out _, out _, out _);
    SoundSystem.Whistle();
    foreach (var beboo in Map?.Beboos)
    {
      if (Map?.Beboos.Count<=1 || Random.Next(3) == 1)
      {
        Task.Run(async () =>
        {
          await Task.Delay(Game.Random.Next(1000, 2000));
          beboo.WakeUp();
        });
        beboo.GoalPosition = currentPosition;
      }
    }
  }

  public static void MoveOf(Vector3 movement)
  {
    if (Map == null) return;
    var newPos = Map.Clamp(PlayerPosition + movement);
    if (newPos != PlayerPosition + movement) Game.SoundSystem.System.PlaySound(Game.SoundSystem.WallSound);
    PlayerPosition = newPos;
    SoundSystem.MovePlayerTo(newPos);
    if (Map.IsInLake(newPos)) SayLocalizedString("water");
    else if (Flags.UnlockShop && (Map?.IsArroundShop(PlayerPosition) ?? false)) SayLocalizedString("shop");
    else if (Flags.UnlockSnowyMap && (Map?.IsArrundMapPath(PlayerPosition) ?? false)) SayLocalizedString("path");
    else if ((Map?.IsArroundRaceGate(PlayerPosition) ?? false)) SayLocalizedString("racegate");
    SpeakObjectUnderCursor();
  }

  private static void SpeakObjectUnderCursor()
  {
    var treeLine = Map?.GetTreeLineAtPosition(PlayerPosition);
    var item = Map?.GetItemArroundPosition(PlayerPosition);
    var bebooUnderCursor = BebooUnderCursor();
    if (bebooUnderCursor != null) ScreenReader.Output(bebooUnderCursor.Name);
    if (treeLine != null)
      SayLocalizedString("trees");
    else if (item != null) SayLocalizedString(item.Name);
  }
  private void Tick(object? _, EventArgs __)
  {
    foreach (var beboo in Map?.Beboos)
    {
      if (Map.IsLullabyPlaying) beboo.GoAsleep();
      if (beboo.Age >= 2 && !Flags.VoiceRecoPopupPrinted)
      {
        Flags.VoiceRecoPopupPrinted = true;
        UnlockVoiceRecognition.Run(beboo.Name);
      }
      else if (beboo.Age >= 3 && !Flags.UnlockSnowyMap)
      {
        Flags.UnlockSnowyMap = true;
        UnlockSnowyMap.Run();
      }
      if(beboo.SwimLevel>=10 && !Flags.UnlockPerfectSwimming)
      {
        Flags.UnlockPerfectSwimming = true;
        UnlockSwimming.Run(beboo.Name);
      }
    }
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
    foreach (var beboo in Map?.Beboos) beboo.Pause();
    SoundSystem.DisableAmbiTimer();
    if (Map == null) return;
    SoundSystem.Pause(Map);
  }
  public static void Unpause()
  {
    foreach (var beboo in Map?.Beboos) beboo.Unpause();
    SoundSystem.EnableAmbiTimer();
    if (Map == null) return;
    SoundSystem.Unpause(Map);
  }
  internal void Close(object? sender, FormClosingEventArgs e)
  {
    Map?.Items.RemoveAll(item => typeof(Roll) == item.GetType());
    Dictionary<MapPreset, MapInfo> mapInfos = [];
    foreach (var map in Map.Maps.Values)
    {
      var fruits = 0;
      if (map.TreeLines.Count > 0) fruits = map.TreeLines[0].Fruits;
      var bebooInfos = new List<BebooInfo>();
      foreach (var beboo in map.Beboos)
      {
        bebooInfos.Add(new(beboo.Name, beboo.Age, beboo.Happiness, beboo.Energy, beboo.SwimLevel));
      }
      var mapInfo = new MapInfo(map.Items, fruits, bebooInfos);
      mapInfos.Add(map.Preset, mapInfo);
    }
    var parameters = new SaveParameters(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName,
       SoundSystem.Volume,
       lastPayed: DateTime.Now,
       flags: Flags,
       playerName: PlayerName,
       fruitsBasket: FruitsBasket ?? [],
       inventory: Inventory,
       tickets: Tickets,
       unlockedRolls: MusicBox.AvailableRolls,
       favoredColor: Parameters.FavoredColor,
       currentMap: Map?.Preset ?? MapPreset.garden,
       mapInfos: mapInfos
   );
    SaveManager.WriteJson(parameters);
  }
  private static Map? _backedMap;
  internal static void ChangeMap(Map map, bool backup = true)
  {
    if (Game.Map != null) Game.SoundSystem.Pause(Game.Map);
    if (backup) _backedMap = Map;
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
  public static void UpdateMapMusic()
  {
    if (Map != null) SoundSystem.PlayMapMusic(Map);
  }
}