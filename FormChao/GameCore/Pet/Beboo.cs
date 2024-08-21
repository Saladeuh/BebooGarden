using System.Numerics;
using BebooGarden.GameCore.World;
using BebooGarden.Interface;
using BebooGarden.Interface.ScriptedScene;
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
  public Beboo(string name, float age, DateTime lastPlayed, int happiness = 5, int swimLevel = 0, bool racer = false, float voicePitch = 1)
  {
    Position = new Vector3(0, 0, 0);
    Name = name == string.Empty ? "boby" : name;
    VoiceDsp = Game.SoundSystem.System.CreateDSPByType(FmodAudio.DigitalSignalProcessing.DSPType.PitchShift);
    VoiceDsp.SetParameterFloat(0, voicePitch);
    SwimLevel = swimLevel;
    var isSleepingAtStart = !racer && (DateTime.Now.Hour < 8 || DateTime.Now.Hour > 20);
    Sleeping = isSleepingAtStart;
    CuteBehaviour =
        new TimedBehaviour<Beboo>(this, 15000, 25000, beboo => { beboo.DoCuteThing(); }, !isSleepingAtStart);
    MoveBehaviour =
        new TimedBehaviour<Beboo>(this, 200, 400, beboo => { beboo.MoveTowardGoal(); }, !isSleepingAtStart);
    FancyMoveBehaviour =
        new TimedBehaviour<Beboo>(this, 10000, 30000, beboo => { beboo.WannaGoToRandomPlace(); }, true);
    GoingTiredBehaviour =
        new TimedBehaviour<Beboo>(this, 50000, 70000, beboo => { beboo.Energy--; }, !isSleepingAtStart || !racer);
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
    var elapsedTime = DateTime.Now - lastPlayed;
    Happiness = elapsedTime.TotalHours > 4 ? 5 : happiness;
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
    Energy = elapsedTime.TotalHours > 8 ? 5 : 10;
    SpeechRecognizer = new BebooSpeechRecognition(name);
    SpeechRecognizer.BebooCalled += Game.Call;
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
      if (value >= 8) BeOverexited();
      else if (value <= 0) BeFloppy();
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
  private BebooSpeechRecognition SpeechRecognizer { get; }
  public bool KnowItsName { get; internal set; }
  public int MaxEnergy { get; private set; } = 10;
  public int MaxHappinness { get; private set; } = 10;

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
    var direction = (Vector3)GoalPosition - Position;
    var directionNormalized = Vector3.Normalize(direction);
    directionNormalized.X = Math.Sign(directionNormalized.X);
    directionNormalized.Y = Math.Sign(directionNormalized.Y);
    Position += directionNormalized;
    if (BootsSlippedOn && Game.Random.Next(2) == 1) Position += directionNormalized;
    if (Game.Map?.IsInLake(Position) ?? false)
    {
      Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooStepWaterSound, this, false);
      if (SwimLevel < 1) StartPanik();
      else if (SwimLevel < 10 && Game.Random.Next(SwimLevel) == 1) StartPanik();
    }
    else if (Game.Map?.Preset == MapPreset.snowy)
    {
      Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooStepSnowSound, this, false);
    }
    else
    {
      if (BootsSlippedOn) Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BoingSounds, this, false);
      else Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooStepSound, this, false);
      EndPanik();
    }

    PlayArround();
    if (Game.Random.Next(20) == 1) BootsSlippedOn = false;
    var moved = Position != GoalPosition;
    if (!moved) GoalPosition = null;
    return moved;
  }

  private void PlayArround()
  {
    if (Happy && Game.Random.Next(4) == 1)
    {
      var proximityItem = Game.Map?.GetItemArroundPosition(Position);
      if (proximityItem != null)
      {
        proximityItem.BebooAction(this);
        Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooFunSounds, this);
        Happiness++;
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
      var targetItem = Game.Map?.Items[Game.Random.Next(Game.Map.Items.Count)];
      if(targetItem != null) GoalPosition=targetItem.Position;
    }
    else
    {
      var randomMove = new Vector3(Game.Random.Next(-4, 5), Game.Random.Next(-4, 5), 0);
      GoalPosition = Position + randomMove;
    }
  }

  public void GoAsleep()
  {
    if (Sleeping) return;
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

  private void BeOverexited()
  {
    MoveBehaviour.MinMS = 50;
    MoveBehaviour.MaxMS = 150;
    FancyMoveBehaviour.MinMS = 5000;
    FancyMoveBehaviour.MaxMS = 10000;
    CuteBehaviour.MinMS = 10000;
    CuteBehaviour.MaxMS = 20000;
  }
  public void Pause()
  {
    CuteBehaviour.Stop();
    MoveBehaviour.Stop();
    GoingDepressedBehaviour.Stop();
    SadBehaviour.Stop();
    SleepingBehaviour.Stop();
    GrowthBehaviour.Stop();
  }
  public void Unpause()
  {
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
}