using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using BebooGarden.Content;
using BebooGarden.GameCore.World;
using CrossSpeak;
using FmodAudio;
using FmodAudio.DigitalSignalProcessing;

namespace BebooGarden.GameCore.Pet;

public partial class Beboo
{
  private Vector3? _destination;
  private DateTime _lastPetted = DateTime.MinValue;

  private int _petCount;
  private float voicePitch = 1.1f;
  private float age = 1;
  public bool BootsSlippedOn { get; set; } = false;
  public Channel? Channel { get; set; }
  public float VoicePitch
  {
    get => voicePitch; set
    {
      VoiceDsp.SetParameterFloat(0, value);
      voicePitch = value;
    }
  }
  public Dsp VoiceDsp { get; }
  public int SwimLevel { get; set; } = 0;
  public bool Racer { get; set; } = false;
  public BebooType BebooType { get; set; }
  public Beboo(string name, BebooType bebooType, float age, DateTime lastPlayed, int happiness = 3, float energy = 3, int swimLevel = 0, bool racer = false, float voicePitch = 1)
  {
    Racer = racer;
    Position = new Vector3(0, 0, 0);
    Name = name == string.Empty ? "boby" : name;
    BebooType = bebooType;
    VoiceDsp = Game1.Instance.SoundSystem.System.CreateDSPByType(FmodAudio.DigitalSignalProcessing.DSPType.PitchShift);
    VoiceDsp.SetParameterFloat(0, voicePitch);
    SwimLevel = swimLevel;
    bool isSleepingAtStart = !racer && (DateTime.Now.Hour < 8 || DateTime.Now.Hour > 22);
    Sleeping = isSleepingAtStart;
    CuteBehaviour =
      new TimedBehaviour(15000, 25000, !isSleepingAtStart);
    MoveBehaviour =
        new TimedBehaviour(200, 400, !isSleepingAtStart);
    GoToSleepOrWakeUpBehaviour =
        new TimedBehaviour(10000, 150000, true);
    FancyMoveBehaviour =
        new TimedBehaviour(10000, 20000, true);
    GoingTiredBehaviour =
        new TimedBehaviour(60000 * 3, 60000 * 6, !isSleepingAtStart || !racer);
    GoingSadBehaviour =
        new TimedBehaviour(120000, 150000, !racer);
    EmotionBehaviour = new TimedBehaviour(1000, 1500, true);
    CryBehaviour =
      new TimedBehaviour(5000, 15000, false);
    SleepingBehaviour = new(5000, 10000, isSleepingAtStart);
    //+0.1 every 3mn=1lvl/30mn
    GrowthBehaviour = new(3000 * 60, 3000 * 60, !racer);
    TimeSpan elapsedTime = DateTime.Now - lastPlayed;
    Happiness = elapsedTime.TotalHours > 4 ? 3 : happiness;
    Age = age;
    KnowItsName = age > 2;
    switch (age)
    {
      case < 2:
        MaxEnergy = 10;
        MaxHappinness = 10;
        break;
      case < 3:
        MaxEnergy = 15;
        MaxHappinness = 15;
        break;
      case < 4:
        MaxEnergy = 20;
        MaxHappinness = 20;
        break;
    }
    Energy = elapsedTime.TotalHours > 8 ? 5 : energy;
    Energy = elapsedTime.TotalDays > 2? 7 : energy;
    //SpeechRecognizer = new BebooSpeechRecognition(this);
    //SpeechRecognizer.BebooCalled += Call;
  }

  public string Name { get; set; }
  public float Age
  {
    get => age; set
    {
      value = Math.Clamp(value, 1, 10);
      if (age < value) TestLevelUp(age, value);
      age = value;
    }
  }

  private void TestLevelUp(float preview, float updated)
  {
    if (preview >= 1 && preview < 2 && updated >= 2)
    {
      KnowItsName = true;
    }
    else if (preview > 2 && preview < 3 && updated >= 3)
    {
      MaxHappinness = 15;
      MaxEnergy = 15;
    }
    else if (preview > 3 && preview < 4 && updated >= 4)
    {
      MaxEnergy = 20;
      MaxHappinness = 20;
    }
  }

  public float Energy
  {
    get;
    set
    {
      value = Math.Clamp(value, -10, MaxEnergy);
      field = value;
    }
  }

  public int Happiness
  {
    get;
    set
    {
      field = Math.Clamp(value, -10, MaxHappinness);
    }
  }

  public Vector3 Position { get; set; }
  public bool Happy { get; private set; } = true;
  public bool Sleeping { get; private set; }
  public bool Panik { get; private set; } = false;
  private TimedBehaviour CuteBehaviour { get; }

  public Vector3? Destination
  {
    get;
    set => _destination = value.HasValue ? Game1.Instance.Map?.Clamp(value.Value) : null;
  }

  private TimedBehaviour GoingTiredBehaviour { get; }
  private TimedBehaviour GoingSadBehaviour { get; }
  public TimedBehaviour EmotionBehaviour { get; private set; }
  private TimedBehaviour MoveBehaviour { get; }
  public TimedBehaviour GoToSleepOrWakeUpBehaviour { get; private set; }
  private TimedBehaviour FancyMoveBehaviour { get; }
  private TimedBehaviour CryBehaviour { get; }
  private TimedBehaviour SleepingBehaviour { get; }
  public TimedBehaviour GrowthBehaviour { get; }
  //public BebooSpeechRecognition SpeechRecognizer { get; }
  public bool KnowItsName { get; internal set; }
  public int MaxEnergy { get; private set; } = 10;
  public int MaxHappinness { get; private set; } = 10;
  public bool Paused { get; private set; }

  private void BurstInTearrs()
  {
    if (!Happy) return;
    Happy = false;
    CrossSpeakManager.Instance.Output(String.Format(GameText.beboo_sadstart, Name));
    Game1.Instance.SoundSystem.PlaySadMusic();
    CuteBehaviour.Stop();
    CryBehaviour.Start();
    MoveBehaviour.MinMS = 800;
    MoveBehaviour.MaxMS = 1000;
  }

  private void BeHappy()
  {
    if (Happy) return;
    Happy = true;
    //IGlobalActions.SayLocalizedString("beboo.happystart", Name);
    //Game1.Instance.UpdateMapMusic();
    // CuteBehaviour.Start();
    CryBehaviour.Stop();
    MoveBehaviour.MinMS = 200;
    MoveBehaviour.MaxMS = 400;
  }

  private bool MoveTowardGoal()
  {
    if (Destination == null || Destination == Position || Sleeping) return false;
    Vector3 direction = (Vector3)Destination - Position;
    Vector3 directionNormalized = Vector3.Normalize(direction);
    directionNormalized.X = Math.Sign(directionNormalized.X);
    directionNormalized.Y = Math.Sign(directionNormalized.Y);
    Position += directionNormalized;
    if (BootsSlippedOn && Game1.Instance.Random.Next(2) == 1) Position += directionNormalized;
    if (Game1.Instance.Map?.IsInLake(Position) ?? false)
    {
      Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.BebooStepWaterSound, this, false);
      if (SwimLevel <= 1 || (SwimLevel < 10 && Game1.Instance.Random.Next(SwimLevel) == 1))
      {
        StartPanik();
        Destination = Game1.Instance.Map.GenerateRandomUnoccupedPosition(true);
      }
    }
    else if (Game1.Instance.Map?.Preset == MapPreset.snowy)
    {
      Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.BebooStepSnowSound, this, false);
      if (BootsSlippedOn || Game1.Instance.Random.Next(4) == 1)
      {
        Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.BebooStepSlipSound, this, false);
        Position += Util.DIRECTIONS[Game1.Instance.Random.Next(Util.DIRECTIONS.Length)];
      }
    }
    else if (Game1.Instance.Map?.Preset == MapPreset.snowyrace)
    {
      Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.BebooStepSnowSound, this, false);
      if (BootsSlippedOn || Game1.Instance.Random.Next(4) == 1)
      {
        Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.BebooStepSlipSound, this, false);
        Position += Util.DIRECTIONS[Game1.Instance.Random.Next(Util.DIRECTIONS.Length)] * 2;
      }
    }
    else
    {
      if (BootsSlippedOn) Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.BoingSounds, this, false);
      else Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.BebooStepSound, this, false);
      EndPanik();
    }

    PlayArround();
    if (Game1.Instance.Random.Next(20) == 1) BootsSlippedOn = false;
    bool moved = Position != Destination;
    if (!moved) Destination = null;
    return moved;
  }

  private void PlayArround()
  {
    var proximityBeboos = Game1.Instance.Map?.GetBeboosArround(Position);
    (proximityBeboos ??= []).Remove(this);
    if (Game1.Instance.Random.Next(4) == 1)
    {
      Item.Item? proximityItem = Game1.Instance.Map?.GetItemArroundPosition(Position);
      if (proximityItem != null)
      {
        proximityItem.BebooAction(this);
        Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.BebooFunSounds, this);
        Happiness++;
      }
    }
    if (proximityBeboos.Count > 0 && Game1.Instance.Random.Next(3) == 1)
    {
      InteractWith(proximityBeboos[Game1.Instance.Random.Next(proximityBeboos.Count)]);
      Happiness++;
    }
  }

  private void InteractWith(Beboo friend)
  {
    if (friend.Sleeping) ForceWakeUp(friend);
    else if (Game1.Instance.Map?.IsDansePlaying ?? false) 
      SingWith(friend);
    else
    {
      var rnd = Game1.Instance.Random.Next(5);
      switch (rnd)
      {
        case 1: case 2: SingWith(friend); break;
        case 3: Follow(friend); break;
        case 4: Scare(friend); break;
      }
    }
  }

  private void EndPanik()
  {
    if (!Panik) return;
    Panik = false;
    SwimLevel += 1;
    MoveBehaviour.Restart();
    FancyMoveBehaviour.Restart();
  }

  private void StartPanik()
  {
    if (Panik) return;
    Panik = true;
    Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.BebooScreamSound, this);
    Happiness -= 2;
    Energy -= 2;
    FancyMoveBehaviour.MinMS = 400;
    FancyMoveBehaviour.MaxMS = 400;
    MoveBehaviour.MinMS = 100;
    MoveBehaviour.MaxMS = 100;
  }

  private void DoCuteThing()
  {
    Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.BebooCuteSounds, this);
  }

  private void WannaGoToRandomPlace()
  {
    if (Game1.Instance.Map?.Items.Count > 0 && Game1.Instance.Random.Next(2) == 1)
    {
      Item.Item? targetItem = Game1.Instance.Map?.Items[Game1.Instance.Random.Next(Game1.Instance.Map.Items.Count)];
      if (targetItem != null) Destination = targetItem.Position;
    }
    else if (Game1.Instance.Map?.Beboos.Count > 1 && Game1.Instance.Random.Next(2) == 1)
    {
      var otherBeboos = new List<Beboo>(Game1.Instance.Map.Beboos);
      otherBeboos.Remove(this);
      var targetBeboo = otherBeboos[Game1.Instance.Random.Next(otherBeboos.Count)];
      Destination = targetBeboo.Position;
    }
    else
    {
      Vector3 randomMove = new(Game1.Instance.Random.Next(-4, 5), Game1.Instance.Random.Next(-4, 5), 0);
      Destination = Position + randomMove;
    }
  }

  public void GoAsleep()
  {
    if (Sleeping) return;
    if (SwimLevel >= 10 || Game1.Instance.Map?.Preset == MapPreset.underwater || (!Game1.Instance.Map?.IsInLake(Position) ?? false))
    {
      CrossSpeakManager.Instance.Output(String.Format(GameText.beboo_gosleep, Name));
      GoingTiredBehaviour.Stop();
      MoveBehaviour.Stop();
      FancyMoveBehaviour.Stop();
      CuteBehaviour.Stop();
      GoingSadBehaviour.Stop();
      Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.GrassSound, this);
      Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.BebooYawningSounds, this);
      Sleeping = true;
      SleepingBehaviour.Start();
    }
    else
    {
      Destination = new(0, 0, 0); //TODO do somethin better
    }
  }

  public void WakeUp()
  {
    if (Game1.Instance.Map != null && (!Sleeping || Game1.Instance.Map.IsLullabyPlaying)) return;
    CrossSpeakManager.Instance.Output(String.Format(GameText.beboo_wakeup, Name));
    SleepingBehaviour.Stop();
    GoingTiredBehaviour.Start();
    FancyMoveBehaviour.Start();
    MoveBehaviour.Start(3000);
    CuteBehaviour.Start();
    GoingSadBehaviour.Start();
    Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.GrassSound, this);
    Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.BebooYawningSounds, this);
    Sleeping = false;
    BeHappy();
  }

  public void Eat(FruitSpecies fruitSpecies)
  {
    if (Sleeping) return;
    if (fruitSpecies == FruitSpecies.Normal)
    {
      Energy++;
      Happiness++;
    }
    else if (fruitSpecies == FruitSpecies.Energetic)
    {
      Energy += 3;
      Happiness++;
    }
    else if (fruitSpecies == FruitSpecies.Shrink) VoicePitch += 0.1f;
    else if (fruitSpecies == FruitSpecies.Growth) VoicePitch -= 0.1f;

    Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.BebooChewSounds, this, true, 0.5f);
    Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.BebooYumySounds, this);
  }

  public void GetPetted()
  {
    if ((DateTime.Now - _lastPetted).TotalMilliseconds < 800) return;
    _petCount++;
    _lastPetted = DateTime.Now;
    Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.BebooPetSound, this, false);
    if (_petCount + Game1.Instance.Random.Next(2) >= 4)
    {
      Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.BebooDelightSounds, this);
      if (Happiness <= 7 && Game1.Instance.Random.Next(2) == 1)
      {
        Happiness++;
        Game1.Instance.SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.JingleComplete);
      }

      _petCount = 0;
    }
    //if (Game1.Instance.Random.Next(101) == 1) Game1.Instance.GainTicket(Game1.Instance.Random.Next(3));
  }

  private void BeNormal()
  {
    MoveBehaviour.Restart();
    FancyMoveBehaviour.Restart();
    //CuteBehaviour.Restart();
  }

  private void BeFloppy()
  {
    MoveBehaviour.MinMS = 800;
    MoveBehaviour.MaxMS = 1000;
    FancyMoveBehaviour.MinMS = 40000;
    FancyMoveBehaviour.MaxMS = 70000;
    CuteBehaviour.MinMS = 15000;
    CuteBehaviour.MaxMS = 25000;
  }

  private void BeOverexcited()
  {
    MoveBehaviour.MinMS = 50;
    MoveBehaviour.MaxMS = 150;
    FancyMoveBehaviour.MinMS = 5000;
    FancyMoveBehaviour.MaxMS = 10000;
  }
  public void Pause()
  {
    Paused = true;
    CuteBehaviour.Stop();
    MoveBehaviour.Stop();
    GoingSadBehaviour.Stop();
    CryBehaviour.Stop();
    SleepingBehaviour.Stop();
    GrowthBehaviour.Stop();
  }
  public void Unpause()
  {
    Paused = false;
    if (Sleeping) SleepingBehaviour.Start();
    else
    {
      if (Happy)
      {
        CuteBehaviour.Start();
      }
      else CryBehaviour.Start();
      MoveBehaviour.Start();
    }
    GoingSadBehaviour.Start();
    GrowthBehaviour.Start();
  }
  public void Call(object? sender, EventArgs eventArgs)
  {
    if (Paused || Sleeping || !KnowItsName) return;
    Task.Run(async () =>
    {
      await Task.Delay(1000);
      WakeUp();
    });
    Destination = Game1.Instance.PlayerPosition;
  }
  public void Scare(Beboo friend)
  {
    Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.BebooInteractSounds, this);
    friend.GetScared(this);
  }
  public void ForceWakeUp(Beboo friend)
  {
    Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.BebooInteractSounds, this);
    friend.GetWakeUped(this);
  }
  public void GetScared(Beboo friend)
  {
    StartPanik();
    Task.Run(async () =>
    {
      await Task.Delay(5000);
      EndPanik();
    });
  }
  public void GetWakeUped(Beboo friend)
  {
    Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.BebooSurpriseSounds, this);
    Task.Run(async () =>
    {
      await Task.Delay(2000);
      Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.BebooAngrySounds, this);
      WakeUp();
    });
  }
  public void Follow(Beboo friend)
  {
    Destination = friend.Destination;
  }
  public void SingWith(Beboo friend)
  {
    if (friend.Channel != null && friend.Channel.IsPlaying) return;
    var songsList = SoundSystem.GetBebooSounds(Game1.Instance.SoundSystem.BebooSongSounds, this);
    var songsListFriend = SoundSystem.GetBebooSounds(Game1.Instance.SoundSystem.BebooSongSounds, friend);
    if (songsList.Count > 0 && songsListFriend.Count > 0)
    {
      var randomSong = songsList[Game1.Instance.Random.Next(songsList.Count)];
      var randomSongFriend = songsList[Game1.Instance.Random.Next(songsListFriend.Count)];
      Game1.Instance.SoundSystem.PlayBebooSound(randomSong, this);
      Task.Run(async () =>
      {
        await Task.Delay(100);
        Game1.Instance.SoundSystem.PlayBebooSound(randomSongFriend, friend);
      });
      Happiness++;
      friend.Happiness++;
    }
  }
}