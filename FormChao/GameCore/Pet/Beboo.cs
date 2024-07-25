using System.Numerics;
using BebooGarden.GameCore.World;
using BebooGarden.Interface;
using FmodAudio;

namespace BebooGarden.GameCore.Pet;

public class Beboo
{
  private float _energy;
  private Vector3? _goalPosition;
  private int _hapiness;
  private DateTime _lastPetted = DateTime.MinValue;

  private int _petCount;

  public Beboo(string name, int age, DateTime lastPlayed, int happiness = 5)
  {
    Position = new Vector3(0, 0, 0);
    Name = name == string.Empty ? "boby" : name;
    var isSleepingAtStart = DateTime.Now.Hour < 8 || DateTime.Now.Hour > 20;
    Sleeping = isSleepingAtStart;
    CuteBehaviour =
        new TimedBehaviour<Beboo>(this, 15000, 25000, beboo => { beboo.DoCuteThing(); }, !isSleepingAtStart);
    MoveBehaviour =
        new TimedBehaviour<Beboo>(this, 200, 400, beboo => { beboo.MoveTowardGoal(); }, !isSleepingAtStart);
    FancyMoveBehaviour =
        new TimedBehaviour<Beboo>(this, 20000, 40000, beboo => { beboo.WannaGoToRandomPlace(); }, true);
    GoingTiredBehaviour =
        new TimedBehaviour<Beboo>(this, 50000, 70000, beboo => { beboo.Energy--; }, !isSleepingAtStart);
    GoingDepressedBehaviour =
        new TimedBehaviour<Beboo>(this, 120000, 150000, beboo => { beboo.Happiness--; }, true);
    SadBehaviour = new TimedBehaviour<Beboo>(this, 5000, 15000,
        beboo => { Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooCrySounds, Position); }, false);
    SleepingBehaviour = new TimedBehaviour<Beboo>(this, 5000, 10000, beboo =>
    {
      beboo.Energy += 0.10f;
      Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooSleepingSounds, Position, true, 0.3f);
    }, isSleepingAtStart);
    var elapsedTime = DateTime.Now - lastPlayed;
    Happiness = elapsedTime.TotalHours > 4 ? 5 : happiness;
    Age = age;
    Energy = elapsedTime.TotalHours > 8 ? 5 : 10;
    SpeechRecognizer = new BebooSpeechRecognition(name);
    SpeechRecognizer.BebooCalled += Game.Call;
  }

  public string Name { get; set; }
  public int Age { get; set; }

  public float Energy
  {
    get => _energy;
    private set
    {
      value = Math.Clamp(value, -10, 10);
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
    private set
    {
      value = Math.Clamp(value, -10, 10);
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

  public Vector3 Position { get; private set; }
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
  private BebooSpeechRecognition SpeechRecognizer { get; }

  private void BurstInTearrs()
  {
    if (!Happy) return;
    Happy = false;
    IGlobalActions.SayLocalizedString("beboo.sadstart", Name);
    Game.SoundSystem.MusicTransition(Game.SoundSystem.SadMusicStream, 464375, 4471817, TimeUnit.PCM, 0.1f);
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
    Game.SoundSystem.MusicTransition(Game.SoundSystem.NeutralMusicStream, 12, 88369, TimeUnit.MS);
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
    if (Game.Map?.isInLake(Position) ?? false)
    {
      Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooStepWaterSound, Position, false);
      StartPanik();
    }
    else
    {
      Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooStepSound, Position, false);
      EndPanik();
    }
    var moved = Position != GoalPosition;
    if (!moved) GoalPosition = null;
    return moved;
  }

  private void EndPanik()
  {
    if (!Panik) return;
    Panik = false;
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
    Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooScreamSound, Position);
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
    Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooCuteSounds, Position);
  }

  private void WannaGoToRandomPlace()
  {
    var rnd = new Random();
    var randomMove = new Vector3(rnd.Next(-4, 5), rnd.Next(-4, 5), 0);
    GoalPosition = Position + randomMove;
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
    Game.SoundSystem.PlayBebooSound(Game.SoundSystem.GrassSound, Position);
    Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooYawningSounds, Position);
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
    Game.SoundSystem.PlayBebooSound(Game.SoundSystem.GrassSound, Position);
    Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooYawningSounds, Position);
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
      Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooChewSounds, Position, true, 0.5f);
      Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooYumySounds, Position);
    }
  }

  public void GetPetted()
  {
    if ((DateTime.Now - _lastPetted).TotalMilliseconds < 700) return;
    _petCount++;
    _lastPetted = DateTime.Now;
    Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooPetSound, Position, false);
    var rnd = new Random();
    if (_petCount + rnd.Next(2) >= 6)
    {
      Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooDelightSounds, Position);
      if (Happiness <= 7 && rnd.Next(2) == 1)
      {
        Happiness++;
        Game.SoundSystem.System.PlaySound(Game.SoundSystem.JingleComplete);
      }

      _petCount = 0;
    }
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
}