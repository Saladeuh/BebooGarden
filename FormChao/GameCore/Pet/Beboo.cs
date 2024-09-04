using System.Numerics;
using BebooGarden.GameCore.World;
using BebooGarden.Interface;
using FmodAudio;
using FmodAudio.DigitalSignalProcessing;

namespace BebooGarden.GameCore.Pet;

public class Beboo
{
  private float _energy;
  private Vector3? _goalPosition;
  private int _hapiness;
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
  public BebooType BebooType { get; set; }=BebooType.Green;
  public Beboo(string name, float age, DateTime lastPlayed, int happiness = 3, float energy = 3, int swimLevel = 0, bool racer = false, float voicePitch = 1)
  {
    Racer = racer;
    Position = new Vector3(0, 0, 0);
    Name = name == string.Empty ? "boby" : name;
    VoiceDsp = Game.SoundSystem.System.CreateDSPByType(FmodAudio.DigitalSignalProcessing.DSPType.PitchShift);
    VoiceDsp.SetParameterFloat(0, voicePitch);
    SwimLevel = swimLevel;
    bool isSleepingAtStart = !racer && (DateTime.Now.Hour < 8 || DateTime.Now.Hour > 20);
    Sleeping = isSleepingAtStart;
    CuteBehaviour =
        new TimedBehaviour<Beboo>(this, 15000, 25000, beboo => { beboo.DoCuteThing(); }, !isSleepingAtStart);
    MoveBehaviour =
        new TimedBehaviour<Beboo>(this, 200, 400, beboo => { beboo.MoveTowardGoal(); }, !isSleepingAtStart);
    FancyMoveBehaviour =
        new TimedBehaviour<Beboo>(this, 10000, 30000, beboo => { beboo.WannaGoToRandomPlace(); }, true);
    GoingTiredBehaviour =
        new TimedBehaviour<Beboo>(this, 60000*3, 60000*6, beboo => { beboo.Energy--; }, !isSleepingAtStart || !racer);
    GoingDepressedBehaviour =
        new TimedBehaviour<Beboo>(this, 120000, 150000, beboo => { beboo.Happiness--; }, !racer);
    SadBehaviour = new TimedBehaviour<Beboo>(this, 5000, 15000,
        beboo => { Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooCrySounds, this); }, false);
    SleepingBehaviour = new(this, 5000, 10000, beboo =>
    {
      beboo.Energy += 0.10f;
      Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooSleepingSounds, this, true, 0.3f);
    }, isSleepingAtStart);
    //+0.1 every 3mn=1lvl/30mn
    GrowthBehaviour = new(this, 3000 * 60, 3000 * 60, beboo =>
    {
      if (beboo.Energy >= 2 && beboo.Happiness >= 2)
      {
        beboo.Age += 0.1f;
      }
    }, !racer);
    TimeSpan elapsedTime = DateTime.Now - lastPlayed;
    Happiness = elapsedTime.TotalHours > 4 ? 3 : happiness;
    Happiness = elapsedTime.TotalDays > 2 ? 0 : happiness;
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
    Energy = elapsedTime.TotalHours > 8 ? 3 : 10;
    Energy = elapsedTime.TotalDays > 2 ? 1 : 5;
    SpeechRecognizer = new BebooSpeechRecognition(this);
    SpeechRecognizer.BebooCalled += Call;
  }

  public string Name { get; }
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
    get => _energy;
    set
    {
      value = Math.Clamp(value, -10, MaxEnergy);
      if (_energy > value && ((Happy && value <= -2) || (!Happy && value <= -5))) GoAsleep();
      else if (_energy < value && _energy >= 2)
        Task.Run(async () =>
        {
          await Task.Delay(1000);
          WakeUp();
        });
      _energy = value;
    }
  }

  public int Happiness
  {
    get => _hapiness;
    set
    {
      value = Math.Clamp(value, -10, MaxHappinness);
      if (_hapiness > value && value <= 0) BurstInTearrs();
      else if (_hapiness <= 0 && value > 0)
        Task.Run(async () =>
        {
          await Task.Delay(1000);
          BeHappy();
        });
      if (value >= 9 && Energy > 5) BeOverexcited();
      else if (value <= 0 && Energy < 5) BeFloppy();
      else BeNormal();
      _hapiness = value;
    }
  }

  public Vector3 Position { get; set; }
  public bool Happy { get; private set; } = true;
  public bool Sleeping { get; private set; }
  public bool Panik { get; private set; } = false;
  private TimedBehaviour<Beboo> CuteBehaviour { get; }

  public Vector3? GoalPosition
  {
    get => _goalPosition;
    set => _goalPosition = value.HasValue ? Game.Map?.Clamp(value.Value) : null;
  }

  private TimedBehaviour<Beboo> GoingTiredBehaviour { get; }
  private TimedBehaviour<Beboo> GoingDepressedBehaviour { get; }
  private TimedBehaviour<Beboo> MoveBehaviour { get; }
  private TimedBehaviour<Beboo> FancyMoveBehaviour { get; }
  private TimedBehaviour<Beboo> SadBehaviour { get; }
  private TimedBehaviour<Beboo> SleepingBehaviour { get; }
  public TimedBehaviour<Beboo> GrowthBehaviour { get; }
  public BebooSpeechRecognition SpeechRecognizer { get; }
  public bool KnowItsName { get; internal set; }
  public int MaxEnergy { get; private set; } = 10;
  public int MaxHappinness { get; private set; } = 10;
  public bool Paused { get; private set; }

  private void BurstInTearrs()
  {
    if (!Happy) return;
    Happy = false;
    IGlobalActions.SayLocalizedString("beboo.sadstart", Name);
    Game.SoundSystem.PlaySadMusic();
    CuteBehaviour.Stop();
    SadBehaviour.Start();
    MoveBehaviour.MinMS = 800;
    MoveBehaviour.MaxMS = 1000;
  }

  private void BeHappy()
  {
    if (Happy) return;
    Happy = true;
    IGlobalActions.SayLocalizedString("beboo.happystart", Name);
    Game.UpdateMapMusic();
    CuteBehaviour.Start();
    SadBehaviour.Stop();
    MoveBehaviour.MinMS = 200;
    MoveBehaviour.MaxMS = 400;
  }

  private bool MoveTowardGoal()
  {
    if (GoalPosition == null || GoalPosition == Position || Sleeping) return false;
    Vector3 direction = (Vector3)GoalPosition - Position;
    Vector3 directionNormalized = Vector3.Normalize(direction);
    directionNormalized.X = Math.Sign(directionNormalized.X);
    directionNormalized.Y = Math.Sign(directionNormalized.Y);
    Position += directionNormalized;
    if (BootsSlippedOn && Game.Random.Next(2) == 1) Position += directionNormalized;
    if (Game.Map?.IsInLake(Position) ?? false)
    {
      Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooStepWaterSound, this, false);
      if (SwimLevel <= 1) StartPanik();
      else if (SwimLevel < 10 && Game.Random.Next(SwimLevel) == 1) StartPanik();
    }
    else if (Game.Map?.Preset == MapPreset.snowy)
    {
      Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooStepSnowSound, this, false);
      if (BootsSlippedOn || Game.Random.Next(9) == 1)
      {
        Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooStepSlipSound, this, false);
        Position += Util.DIRECTIONS[Game.Random.Next(Util.DIRECTIONS.Length)];
      }
    }
    else if (Game.Map?.Preset == MapPreset.snowyrace)
    {
      Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooStepSnowSound, this, false);
      if (BootsSlippedOn || Game.Random.Next(4) == 1)
      {
        Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooStepSlipSound, this, false);
        Position += Util.DIRECTIONS[Game.Random.Next(Util.DIRECTIONS.Length)] * 2;
      }
    }
    else
    {
      if (BootsSlippedOn) Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BoingSounds, this, false);
      else Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooStepSound, this, false);
      EndPanik();
    }

    PlayArround();
    if (Game.Random.Next(20) == 1) BootsSlippedOn = false;
    bool moved = Position != GoalPosition;
    if (!moved) GoalPosition = null;
    return moved;
  }

  private void PlayArround()
  {
    if (Happy && Game.Random.Next(4) == 1)
    {
      Item.Item? proximityItem = Game.Map?.GetItemArroundPosition(Position);
      if (proximityItem != null)
      {
        proximityItem.BebooAction(this);
        Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooFunSounds, this);
        Happiness++;
      }
    }
    if (Happy && Game.Random.Next(3) == 1)
    {
      var proximityBeboos = Game.Map?.GetBeboosArround(Position);
      proximityBeboos.Remove(this);
      InteractWith(proximityBeboos[Game.Random.Next(proximityBeboos.Count)]);
      Happiness++;
    }
  }

  private void InteractWith(Beboo friend)
  {
    if (friend.Sleeping) ForceWakeUp(friend);
    else
    {
      var rnd = Game.Random.Next(5);
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
    FancyMoveBehaviour.MinMS = 2000;
    FancyMoveBehaviour.MaxMS = 40000;
    MoveBehaviour.MinMS = 200;
    MoveBehaviour.MaxMS = 400;
    MoveBehaviour.Restart();
    FancyMoveBehaviour.Restart();
  }

  private void StartPanik()
  {
    if (Panik) return;
    Panik = true;
    Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooScreamSound, this);
    Happiness -= 2;
    Energy -= 2;
    FancyMoveBehaviour.MinMS = 400;
    FancyMoveBehaviour.MaxMS = 400;
    MoveBehaviour.MinMS = 100;
    MoveBehaviour.MaxMS = 100;
    MoveBehaviour.Restart();
    FancyMoveBehaviour.Restart();
  }

  private void DoCuteThing()
  {
    Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooCuteSounds, this);
  }

  private void WannaGoToRandomPlace()
  {
    if (Game.Random.Next(3) == 1)
    {
      Item.Item? targetItem = Game.Map?.Items[Game.Random.Next(Game.Map.Items.Count)];
      if (targetItem != null) GoalPosition = targetItem.Position;
    }
    else if (Game.Map?.Beboos.Count > 1 && Game.Random.Next(3) == 1)
    {
      var otherBeboos = new List<Beboo>(Game.Map.Beboos);
      otherBeboos.Remove(this);
      var targetBeboo = otherBeboos[Game.Random.Next(otherBeboos.Count)];
      GoalPosition = targetBeboo.Position;
    }
    else
    {
      Vector3 randomMove = new(Game.Random.Next(-4, 5), Game.Random.Next(-4, 5), 0);
      GoalPosition = Position + randomMove;
    }
  }

  public void GoAsleep()
  {
    if (Sleeping) return;
    if (SwimLevel >= 10 || Game.Map.Preset == MapPreset.underwater || (Game.Map != null && !Game.Map.IsInLake(Position)))
    {
      IGlobalActions.SayLocalizedString("beboo.gosleep", Name);
      GoingTiredBehaviour.Stop();
      MoveBehaviour.Stop();
      FancyMoveBehaviour.Stop();
      CuteBehaviour.Stop();
      GoingDepressedBehaviour.Stop();
      Game.SoundSystem.PlayBebooSound(Game.SoundSystem.GrassSound, this);
      Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooYawningSounds, this);
      Sleeping = true;
      SleepingBehaviour.Start();
    }
    else
    {
      GoalPosition = new(0, 0, 0); //TODO do somethin better
    }
  }

  public void WakeUp()
  {
    if (Game.Map != null && (!Sleeping || Game.Map.IsLullabyPlaying)) return;
    IGlobalActions.SayLocalizedString("beboo.wakeup", Name);
    SleepingBehaviour.Stop();
    GoingTiredBehaviour.Start();
    FancyMoveBehaviour.Start();
    MoveBehaviour.Start(3000);
    CuteBehaviour.Start();
    GoingDepressedBehaviour.Start();
    Game.SoundSystem.PlayBebooSound(Game.SoundSystem.GrassSound, this);
    Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooYawningSounds, this);
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

    Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooChewSounds, this, true, 0.5f);
    Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooYumySounds, this);
  }

  public void GetPetted()
  {
    if ((DateTime.Now - _lastPetted).TotalMilliseconds < 700) return;
    _petCount++;
    _lastPetted = DateTime.Now;
    Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooPetSound, this, false);
    if (_petCount + Game.Random.Next(2) >= 6)
    {
      Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooDelightSounds, this);
      if (Happiness <= 7 && Game.Random.Next(2) == 1)
      {
        Happiness++;
        Game.SoundSystem.System.PlaySound(Game.SoundSystem.JingleComplete);
      }

      _petCount = 0;
    }
    if (Game.Random.Next(101) == 1) Game.GainTicket(Game.Random.Next(3));
  }

  private void BeNormal()
  {
    MoveBehaviour.MinMS = 200;
    MoveBehaviour.MaxMS = 400;
    FancyMoveBehaviour.MinMS = 30000;
    FancyMoveBehaviour.MaxMS = 60000;
    CuteBehaviour.MinMS = 15000;
    CuteBehaviour.MaxMS = 25000;
    MoveBehaviour.Restart();
    FancyMoveBehaviour.Restart();
    CuteBehaviour.Restart();
  }

  private void BeFloppy()
  {
    MoveBehaviour.MinMS = 800;
    MoveBehaviour.MaxMS = 1000;
    FancyMoveBehaviour.MinMS = 40000;
    FancyMoveBehaviour.MaxMS = 70000;
    CuteBehaviour.MinMS = 15000;
    CuteBehaviour.MaxMS = 25000;
    MoveBehaviour.Restart();
    FancyMoveBehaviour.Restart();
    CuteBehaviour.Restart();
  }

  private void BeOverexcited()
  {
    MoveBehaviour.MinMS = 50;
    MoveBehaviour.MaxMS = 150;
    FancyMoveBehaviour.MinMS = 5000;
    FancyMoveBehaviour.MaxMS = 10000;
    CuteBehaviour.MinMS = 10000;
    CuteBehaviour.MaxMS = 20000;
    MoveBehaviour.Restart();
    FancyMoveBehaviour.Restart();
    CuteBehaviour.Restart();
  }
  public void Pause()
  {
    Paused = true;
    CuteBehaviour.Stop();
    MoveBehaviour.Stop();
    GoingDepressedBehaviour.Stop();
    SadBehaviour.Stop();
    SleepingBehaviour.Stop();
    GrowthBehaviour.Stop();
  }
  public void Unpause()
  {
    Paused = false;
    if (Sleeping) SleepingBehaviour.Start();
    else
    {
      if (Happy) CuteBehaviour.Start();
      else SadBehaviour.Start();
      MoveBehaviour.Start();
    }
    GoingDepressedBehaviour.Start();
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
    GoalPosition = Game.PlayerPosition;
  }
  public void Scare(Beboo friend)
  {
    Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooInteractSounds, this);
    friend.GetScared(this);
  }
  public void ForceWakeUp(Beboo friend)
  {
    Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooInteractSounds, this);
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
    Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooSurpriseSounds, this);
    Task.Run(async () =>
    {
      await Task.Delay(2000);
      Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooAngrySounds, this);
      WakeUp();
    });
  }
  public void Follow(Beboo friend)
  {
    GoalPosition = friend.GoalPosition;
  }
  public void SingWith(Beboo friend)
  {
    if (friend.Channel != null && friend.Channel.IsPlaying) return;
    var randomSong = Game.SoundSystem.BebooSongSounds[BebooType][Game.Random.Next(Game.SoundSystem.BebooSongSounds.Count)];
    Game.SoundSystem.PlayBebooSound(randomSong, this);
    Task.Run(async () =>
    {
      await Task.Delay(100);
      Game.SoundSystem.PlayBebooSound(randomSong, friend);
    });
    Happiness++;
    friend.Happiness++;
  }
}