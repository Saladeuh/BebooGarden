using System.Globalization;
using System.Numerics;
using System.Threading.Channels;
using System.Windows.Input;
using System.Windows.Interop;
using BebooGarden.GameCore.Item;
using BebooGarden.GameCore.Item.MusicBox;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.Interface;
using BebooGarden.Interface.EscapeMenu;
using BebooGarden.Interface.ScriptedScene;
using BebooGarden.Interface.Shop;
using BebooGarden.Minigame;
using BebooGarden.Save;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using Timer = System.Windows.Forms.Timer;


namespace BebooGarden.GameCore;

internal partial class Game : IGlobalActions
{
  public static readonly Timer TickTimer = new();

  private bool _lastArrowWasUp;

  static Game()
  {
    SoundSystem = new SoundSystem();
  }

  public Game(Form1 form)
  {
    GameWindow = form;
    Parameters = SaveManager.LoadSave();
    Flags = Parameters.Flags;
    if (Parameters.RaceScores != null) Race.RaceScores = Parameters.RaceScores;
    Race.TotalWin = Parameters.RaceTotalWin;
    Race.TodayTries = Parameters.LastPlayed.Day == DateTime.Now.Day ? Parameters.RaceTodayTries : 0;
    Flags.UnlockEggInShop = Flags.UnlockUnderwaterMap || Flags.UnlockSnowyMap || Flags.UnlockEggInShop;
    try
    {
      //if (Parameters.CurrentMap != MapPreset.basicrace && Parameters.CurrentMap != MapPreset.snowyrace)
        Map = Map.Maps[Parameters.CurrentMap];
      //else
        //Map = Map.Maps[MapPreset.garden];
    }
    catch (Exception) { Map = Map.Maps[MapPreset.garden]; }
    MusicBox.AvailableRolls = Parameters.UnlockedRolls ?? [];
    SoundSystem.Volume = Parameters.Volume;
    SoundSystem.LoadMainScreen();
    if (!Flags.NewGame)
    {
      PlayerPosition = new Vector3(0, 0, 0);
      foreach (Map map in Map.Maps.Values)
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
        try
        {
          foreach (BebooInfo bebooInfo in Parameters.MapInfos[map.Preset].BebooInfos)
          {
            if (bebooInfo.Name != "bob" && bebooInfo.Name != "boby")
            {
              Beboo beboo = new(bebooInfo.Name, bebooInfo.Age, Parameters.LastPlayed, bebooInfo.Happiness, bebooInfo.Energy, bebooInfo.SwimLevel, false, bebooInfo.Voice);
              map.Beboos.Add(beboo);
              if (map != Map) beboo.Pause();
            }
          }
        }
        catch (KeyNotFoundException _) { }
      }
    }
    else
    {
      PlayerPosition = new Vector3(-2, 0, 0);
      Map.AddItem(new Egg(Parameters.FavoredColor), new(2, 0, 0));
    }
    SoundSystem.LoadMap(Map);
    if (Flags.NewGame) Welcome.AfterGarden();
    else UpdateMapMusic();
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
  }


  public static SoundSystem SoundSystem { get; }
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
  public static bool WASD = InputLanguage.CurrentInputLanguage.Culture.TwoLetterISOLanguageName != "fr";
  public static void GainTicket(int amount)
  {
    if (amount > 0)
    {
      Tickets += amount;
      SayLocalizedString("gainticket", amount);
      SoundSystem.System.PlaySound(SoundSystem.MenuOk2Sound);
      if (!Flags.UnlockShop && Map.Preset != MapPreset.snowyrace && Map.Preset != MapPreset.basicrace)
      {
        Flags.UnlockShop = true;
        SoundSystem.System.PlaySound(SoundSystem.JingleComplete);
        ShopUnlock.Run();
      }
    }
  }
  public static void Call(object? sender, EventArgs eventArgs)
  {
    SoundSystem.System.Get3DListenerAttributes(0, out Vector3 currentPosition, out _, out _, out _);
    foreach (Beboo beboo in Map.Beboos)
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
    if (Race.IsARaceRunning || (DateTime.Now - LastPressedKeyTime).TotalMilliseconds < 150) return;
    LastPressedKeyTime = DateTime.Now;
    Item.Item? itemUnderCursor = Map?.GetItemArroundPosition(PlayerPosition);
    Map?.IsInLake(PlayerPosition);
    Keys key = e.KeyCode;
    if (key is Keys.Left
      || WASD && key is Keys.A
      || !WASD && key is Keys.Q)
    {
      MoveOf(new Vector3(-1, 0, 0));
    }
    else if (key is Keys.Right or Keys.D)
    {
      MoveOf(new Vector3(1, 0, 0));
    }
    else if (key is Keys.Up 
      || WASD && key is Keys.W
      ||!WASD && key is Keys.Z)
    {
      if ((Keyboard.GetKeyStates(Key.Enter) & KeyStates.Down) > 0)
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
    }
    else if (key is Keys.Down
      || WASD && key is Keys.S
      ||!WASD && key is Keys.S)
    {
      if ((Keyboard.GetKeyStates(Key.Enter) & KeyStates.Down) > 0)
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
    }
    else if (key == Keys.F)
    {
      SayBebooState();
    }
    else if (key == Keys.G)
    {
      SayBasketState();
    }
    else if (key == Keys.T)
    {
      SayTickets();
    }
    else if (key == Keys.Enter)
    {
      if (itemUnderCursor != null && itemUnderCursor.IsTakable) itemUnderCursor.Take();
      else if (!Race.IsARaceRunning && Flags.UnlockShop && (Map?.IsArroundShop(PlayerPosition) ?? false)) new Shop().Show();
      else if (!Race.IsARaceRunning && Flags.UnlockSnowyMap && (Map?.IsArroundMapPath(PlayerPosition) ?? false))
        TravelBetwieen(MapPreset.garden, MapPreset.snowy);
      else if (!Race.IsARaceRunning && Flags.UnlockUnderwaterMap && (Map?.IsArroundMapUnderWater(PlayerPosition) ?? false))
        TravelBetwieen(MapPreset.garden, MapPreset.underwater);
      else if (!Race.IsARaceRunning && Map?.Beboos.Count > 0 && (Map?.IsArroundRaceGate(PlayerPosition) ?? false))
        StartRace();
    }
    else if (key == Keys.Escape)
    {
      new EscapeMenu().Show();
    }
    else if (key == Keys.Space)
    {
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
    }
    else
    {
      CheckGlobalActions(key);
    }
  }

  private static void StartRace()
  {
    if (Race.GetRemainingTriesToday() > 0)
    {
      Beboo? contester;
      if (Map.Beboos.Count > 1)
      {
        Dictionary<string, int?> options = new();
        for (int i = 0; i < Map.Beboos.Count; i++)
          options.Add(Map.Beboos[i].Name, i);
        int? choiceId = IWindowManager.ShowChoice<int?>("race.chooseracer", options);
        contester = choiceId != null ? Map.Beboos[choiceId.Value] : null;
      }
      else contester = Map?.Beboos[0];
      if (contester == null) return;
      Dictionary<string, RaceType> raceTypeOptions = new()
      {
        {GetLocalizedString("race.simple"), RaceType.Base },
      };
      if (Flags.UnlockSnowyMap) raceTypeOptions.Add(GetLocalizedString("race.snow"), RaceType.Snowy);
      RaceType choice = RaceType.Base;
      if (raceTypeOptions.Count > 1)
      {
        choice = IWindowManager.ShowChoice<RaceType>(GetLocalizedString("race.chooserace"), raceTypeOptions);
      }
      if (choice != RaceType.None) new Minigame.Race(choice, contester).Start();
    }
    else
    {
      Game.SoundSystem.System.PlaySound(Game.SoundSystem.WarningSound);
      SayLocalizedString("race.trytommorow");
    }
  }

  private void TravelBetwieen(MapPreset a, MapPreset b)
  {
    Map richedMap = Map;
    if (Map.Preset == a) richedMap = Map.Maps[b];
    else if (Map.Preset == b) richedMap = Map.Maps[a];
    List<Beboo> beboosHere = BeboosUnderCursor(2);
    foreach (Beboo transferedBeboo in beboosHere)
    {
      Map?.Beboos.Remove(transferedBeboo);
      transferedBeboo.Position = new(0, 0, 0);
      richedMap.Beboos.Add(transferedBeboo);
    }
    ChangeMap(richedMap);
    UpdateMapMusic();
    PlayerPosition = new(0, 0, 0);
  }

  public static Beboo? BebooUnderCursor()
  {
    foreach (Beboo beboo in Map.Beboos)
    {
      if (Util.IsInSquare(beboo.Position, PlayerPosition, 1)) return beboo;
    }
    return null;
  }
  public List<Beboo> BeboosUnderCursor(int squareSide = 1)
  {
    List<Beboo> beboos = new();
    foreach (Beboo beboo in Map.Beboos)
    {
      if (Util.IsInSquare(beboo.Position, PlayerPosition, squareSide)) beboos.Add(beboo);
    }
    return beboos;
  }
  private static void SayTickets()
  {
    SayLocalizedString("tickets", Tickets);
  }

  private void TryPutItemInHand()
  {
    if (ItemInHand == null) return;
    bool waterProof = ItemInHand?.IsWaterProof ?? false;
    bool inWater = Map?.IsInLake(PlayerPosition) ?? false;
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
    foreach (Beboo beboo in Map?.Beboos)
    {
      string sentence;
      string name = beboo.Name;
      if (beboo.Sleeping)
      {
        sentence = "beboo.sleep";
      }
      else
      {
        if (beboo.Happiness < 0) sentence = "beboo.verysad";
        else if (beboo.Energy < 0) sentence = "beboo.verytired";
        else if (beboo.Energy < 3) sentence = "beboo.littletired";
        else sentence = beboo.Happiness < 3 ? "beboo.littlesad" : beboo.Energy < 8 ? "beboo.good" : "beboo.verygood";
        SayLocalizedString(sentence, name);
#if DEBUG
        ScreenReader.Output(beboo.Age.ToString());
#endif
      }
    }
  }

  private void SayBasketState()
  {
    var fruits = 0;
    foreach (var fruistCount in FruitsBasket.Values) fruits += fruistCount;
    if (FruitsBasket != null) SayLocalizedString("ui.basket", fruits);
  }

  private void ShakeOrPetAtPlayerPosition()
  {
    TreeLine? treeLine = Map?.GetTreeLineAtPosition(PlayerPosition);
    Beboo? bebooUnderCursor = BebooUnderCursor();
    if (treeLine != null)
    {
      FruitSpecies? dropped = treeLine.Shake();
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
    Beboo? bebooUnderCursor = BebooUnderCursor();
    if (FruitsBasket == null || bebooUnderCursor == null) return;
    Dictionary<string, FruitSpecies> options = [];
    foreach (KeyValuePair<FruitSpecies, int> fruit in FruitsBasket)
    {
      if (fruit.Value > 0) options.Add(GetLocalizedString(fruit.Key.ToString()) + " " + fruit.Value.ToString(), fruit.Key);
    }
    FruitSpecies choice = IWindowManager.ShowChoice("ui.chooseitem", options);
    if (choice != FruitSpecies.None)
    {
      bebooUnderCursor.Eat(choice);
      FruitsBasket[choice]--;
    }
  }

  private void Whistle()
  {
    SoundSystem.System.Get3DListenerAttributes(0, out Vector3 currentPosition, out _, out _, out _);
    SoundSystem.Whistle();
    foreach (Beboo beboo in Map?.Beboos)
    {
      if (Map?.Beboos.Count <= 1 || Random.Next(3) == 1)
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
    Vector3 newPos = Map.Clamp(PlayerPosition + movement);
    if (newPos != PlayerPosition + movement) Game.SoundSystem.System.PlaySound(Game.SoundSystem.WallSound);
    else SoundSystem.PlayCursorSound();
    PlayerPosition = newPos;
    SoundSystem.MovePlayerTo(newPos);
    if (Map.IsInLake(newPos) && Map.Preset != MapPreset.underwater) SayLocalizedString("water");
    if (Flags.UnlockShop && (Map?.IsArroundShop(PlayerPosition) ?? false)) SayLocalizedString("shop");
    else if (Flags.UnlockSnowyMap && (Map?.IsArroundMapPath(PlayerPosition) ?? false)) SayLocalizedString("path");
    else if (Map?.IsArroundRaceGate(PlayerPosition) ?? false) SayLocalizedString("race.gate", Race.GetRemainingTriesToday());
    else if (Flags.UnlockUnderwaterMap && (Map?.IsArroundMapUnderWater(PlayerPosition) ?? false)) SayLocalizedString("underwater");
    SpeakObjectUnderCursor();
  }

  private static void SpeakObjectUnderCursor()
  {
    TreeLine? treeLine = Map?.GetTreeLineAtPosition(PlayerPosition);
    Item.Item? item = Map?.GetItemArroundPosition(PlayerPosition);
    Beboo? bebooUnderCursor = BebooUnderCursor();
    if (bebooUnderCursor != null) ScreenReader.Output(bebooUnderCursor.Name);
    if (treeLine != null)
    {
      if (treeLine.Fruits == treeLine.FruitPerHour)
        SayLocalizedString("trees.full");
      else if (treeLine.Fruits == 0)
        SayLocalizedString("trees.empty");
      else if (treeLine.Fruits <= treeLine.FruitPerHour / 2)
        SayLocalizedString("trees.soonempty");
      else if (treeLine.Fruits >= treeLine.FruitPerHour / 2)
        SayLocalizedString("trees.soonfull");
    }
    else if (item != null) SayLocalizedString(item.Name);
  }


  public static void Pause()
  {
    foreach (Beboo beboo in Map?.Beboos) beboo.Pause();
    SoundSystem.DisableAmbiTimer();
    if (Map == null) return;
    SoundSystem.Pause(Map);
  }
  public static void Unpause()
  {
    foreach (Beboo beboo in Map?.Beboos) beboo.Unpause();
    SoundSystem.EnableAmbiTimer();
    if (Map == null) return;
    SoundSystem.Unpause(Map);
  }
  internal void Close(object? sender, FormClosingEventArgs e)
  {
    if (Race.IsARaceRunning)
    {
      e.Cancel = true;
      SoundSystem.System.PlaySound(SoundSystem.WarningSound);
      return;
    }
    Map?.Items.RemoveAll(item => typeof(Roll) == item.GetType());
    Dictionary<MapPreset, MapInfo> mapInfos = [];
    foreach (Map map in Map.Maps.Values)
    {
      int fruits = 0;
      if (map.TreeLines.Count > 0) fruits = map.TreeLines[0].Fruits;
      List<BebooInfo> bebooInfos = new();
      foreach (Beboo beboo in map.Beboos)
      {
        if (!beboo.Racer) bebooInfos.Add(new(beboo.Name, beboo.Age, beboo.Happiness, beboo.Energy, beboo.SwimLevel, beboo.VoicePitch));
      }
      MapInfo mapInfo = new(map.Items, fruits, bebooInfos);
      mapInfos.Add(map.Preset, mapInfo);
    }
    SaveParameters parameters = new(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName,
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
       mapInfos: mapInfos,
       raceScores: Race.RaceScores,
       raceTodayTries: Race.TodayTries,
       raceTotalWin: Race.TotalWin
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