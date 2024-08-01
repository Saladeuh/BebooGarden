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
using Timer = System.Windows.Forms.Timer;

namespace BebooGarden.GameCore;

internal class Game : IGlobalActions
{
  private static readonly Timer TickTimer = new();

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
    Map = new Map("garden", 40, 40,
        [new TreeLine(new Vector2(20, 20), new Vector2(20, -20))],
        new Vector3(-15, 0, 0))
    {
      Items = Parameters.MapItems ?? []
    };
    MusicBox.AvailableRolls = Parameters.UnlockedRolls ?? [];
    SoundSystem.Volume = Parameters.Volume;
    SoundSystem.LoadMainScreen();
    if (!Flags.NewGame)
    {
      Map.TreeLines[0].SetFruitsAfterAWhile(Parameters.LastPlayed, Parameters.RemainingFruits);
      Beboo = new Beboo(Parameters.BebooName, Parameters.Age, Parameters.LastPlayed, Parameters.Happiness);
    }
    else
    {
      Map.AddItem(new Egg(Parameters.FavoredColor), PlayerPosition);
    }

    SoundSystem.LoadMap(Map);
    LastPressedKeyTime = DateTime.Now;
    TickTimer.Tick += Tick;
    TickTimer.Enabled = true;
    PlayerPosition = new Vector3(0, 0, 0);
    FruitsBasket = Parameters.FruitsBasket;
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
  public static Dictionary<Keys, bool> KeyState { get; }
  private DateTime LastPressedKeyTime { get; set; }
  public static Beboo? Beboo { get; set; }
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
    if (Beboo == null || Beboo.Sleeping) return;
    SoundSystem.System.Get3DListenerAttributes(0, out var currentPosition, out _, out _, out _);
    Task.Run(async () =>
    {
      await Task.Delay(1000);
      Beboo.WakeUp();
    });
    Beboo.GoalPosition = currentPosition;
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
        else if (Flags.UnlockShop && (Map?.IsArrundShop(PlayerPosition)??false)) new Shop().Show();
        break;
      case Keys.Escape:
        Dictionary<string, Item.Item> options = [];
        if (Inventory.Count() > 0)
        {
          foreach (var item in Inventory)
          {
            int occurences = Inventory.FindAll(x => x.Name == item.Name).Count();
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
        break;
      case Keys.Space:
        if (ItemInHand != null)
        {
          TryPutItemInHand();
        }
        else
        {
          if (KeyState[Keys.Space]) break;
          if (Beboo != null && Util.IsInSquare(Beboo.Position, PlayerPosition, 1))
          {
            if (Beboo.Sleeping) Whistle();
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

  private void SayTickets()
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
      Map?.AddItem(ItemInHand, PlayerPosition);
      SayLocalizedString("ui.itemput", ItemInHand.Name);
      if (inWater) SoundSystem.System.PlaySound(SoundSystem.ItemPutWaterSound);
      else SoundSystem.System.PlaySound(SoundSystem.ItemPutSound);
      Inventory.Remove(ItemInHand);
      ItemInHand = null;
    }
  }
  private static void SayBebooState()
  {
    if (Beboo == null) return;
    string sentence;
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
    else
    {
      Beboo?.GetPetted();
    }
  }

  private void FeedBeboo()
  {
    if (Beboo == null) return;
    if (FruitsBasket != null && FruitsBasket[FruitSpecies.Normal] > 0)
    {
      Beboo.Eat(FruitSpecies.Normal);
      FruitsBasket[FruitSpecies.Normal]--;
    }
  }

  private void Whistle()
  {
    SoundSystem.System.Get3DListenerAttributes(0, out var currentPosition, out _, out _, out _);
    SoundSystem.Whistle();
    Task.Run(async () =>
    {
      await Task.Delay(1000);
      Beboo?.WakeUp();
    });
    if (Beboo != null) Beboo.GoalPosition = currentPosition;
  }

  private void MoveOf(Vector3 movement)
  {
    if (Map == null) return;
    var newPos = Map.Clamp(PlayerPosition + movement);
    if (newPos != PlayerPosition + movement) SoundSystem.WallBouncing();
    PlayerPosition = newPos;
    SoundSystem.MovePlayerTo(newPos);
    if (Map.IsInLake(newPos)) SayLocalizedString("water");
    else if (Flags.UnlockShop && (Map?.IsArrundShop(PlayerPosition)??false)) SayLocalizedString("shop");
    SpeakObjectUnderCursor();
  }

  private void SpeakObjectUnderCursor()
  {
    var treeLine = Map?.GetTreeLineAtPosition(PlayerPosition);
    var item = Map?.GetItemArroundPosition(PlayerPosition);
    if (Beboo != null && Util.IsInSquare(Beboo.Position, PlayerPosition, 1))
      ScreenReader.Output(Beboo.Name);
    else if (treeLine != null)
      SayLocalizedString("trees");
    else if (item != null) SayLocalizedString(item.Name);
  }

  private void Tick(object? _, EventArgs __)
  {
    if (Map != null && Map.IsLullabyPlaying) Beboo?.GoAsleep();
    SoundSystem.System.Update();
  }

  public static void KeyUpMapper(object? sender, KeyEventArgs e)
  {
    KeyState[e.KeyCode] = false;
  }
  public static void Pause()
  {
    Beboo.Pause();
    SoundSystem.DisableAmbiTimer();
    SoundSystem.Pause(Map);
  }
  public static void Unpause()
  {
    Beboo.Unpause();
    SoundSystem.EnableAmbiTimer();
    SoundSystem.Unpause(Map);
  }
  internal void Close(object? sender, FormClosingEventArgs e)
  {
    Map?.Items.RemoveAll(item => typeof(Roll) == item.GetType());
     var parameters = new SaveParameters(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName,
        SoundSystem.Volume,
        Beboo?.Name ?? "",
        energy: Beboo?.Energy ?? 5,
        happiness: Beboo?.Happiness ?? 5,
        age: Beboo?.Age ?? 0,
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
    parameters.Flags.NewGame = Game.Beboo == null;
    SaveManager.WriteJson(parameters);
  }
}