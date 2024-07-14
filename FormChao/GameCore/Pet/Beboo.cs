using System.Numerics;
using BebooGarden.GameCore.World;
using BebooGarden.Interface;

namespace BebooGarden.GameCore.Pet;

internal class Beboo
{
  public string Name { get; set; }
  public int Age { get; set; }
  private float _energy;
  public float Energy
  {
    get => _energy;
    set
    {
      if (_energy > value && value <= 0) GoAsleep();
      else if (_energy < value && _energy >= 0) Task.Run(async () =>
      {
        await Task.Delay(1000);
        WakeUp();
      });
      _energy = value;
    }
  }
  private int _hapiness;
  public int Happiness
  {
    get => _hapiness;
    private set
    {
      if (_hapiness > value && value <= 0) BurstInTearrs();
      else if (_hapiness <= 0 && value > 0 && Mood != Mood.Happy) Task.Run(async () =>
      {
        await Task.Delay(1000);
        BeHappy();
      });
      _hapiness = value;
    }
  }
  public Mood Mood { get; private set; }
  public Vector3 Position { get; private set; }
  private TimedBehaviour<Beboo> CuteBehaviour { get; }
  private Vector3? _goalPosition;
  public Vector3? GoalPosition
  {
    get => _goalPosition;
    set
    {
      if (value.HasValue) _goalPosition = Game.Map.Clamp(value.Value);
      else _goalPosition = null;
    }
  }

  private TimedBehaviour<Beboo> GoingTiredBehaviour { get; }
  private TimedBehaviour<Beboo> MoveBehaviour { get; }
  private TimedBehaviour<Beboo> SadBehaviour { get; }
  private TimedBehaviour<Beboo> SleepingBehaviour { get; }

  public Beboo(string name, int age, DateTime lastPlayed, float energy)
  {
    Position = new Vector3(0, 0, 0);
    Name = name == string.Empty ? "boby" : name;
    bool isSleepingAtStart = DateTime.Now.Hour < 8 || DateTime.Now.Hour > 20;
    Mood = isSleepingAtStart ? Mood.Sleeping : Mood.Happy;
    CuteBehaviour = new(this, 15000, 25000, beboo =>
    {
      beboo.DoCuteThing();
    }, !isSleepingAtStart);
    MoveBehaviour = new(this, 200, 400, beboo =>
    {
      beboo.MoveTowardGoal();
    }, !isSleepingAtStart);
    TimedBehaviour<Beboo> fancyMoveBehaviour = new(this, 30000, 60000, beboo =>
    {
      beboo.WannaGoToRandomPlace();
    }, true);
    GoingTiredBehaviour = new(this, 50000, 70000, beboo =>
    {
      if (beboo.Age < 2) beboo.Energy -= 2;
      else beboo.Energy--;
    }, !isSleepingAtStart);
    TimedBehaviour<Beboo> goingDepressedBehaviour = new(this, 120000, 150000, beboo =>
    {
      beboo.Happiness--;
    }, true);
    SadBehaviour = new(this, 5000, 15000, beboo =>
    {
      Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooCrySounds, beboo);
    }, false);
    SleepingBehaviour = new(this, 5000, 10000, beboo =>
    {
      beboo.Energy += 0.10f;
      Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooSleepingSounds, beboo, true, 0.3f);
    }, isSleepingAtStart);
    Happiness = 3;
    Age = age;
    Energy = (DateTime.Now - lastPlayed).TotalHours > 4 ? 10 : 5;
  }

  private void BurstInTearrs()
  {
    if (Mood == Mood.Sad) return;
    this.Mood = Mood.Sad;
    Game.SoundSystem.MusicTransition(Game.SoundSystem.SadMusicStream, 464375, 4471817, FmodAudio.TimeUnit.PCM, 0.1f);
    this.CuteBehaviour.Stop();
    this.SadBehaviour.Start();
    this.MoveBehaviour.MinMS = 800;
    this.MoveBehaviour.MaxMS = 1000;
  }

  private void BeHappy()
  {
    if (Mood == Mood.Happy) return;
    this.Mood = Mood.Happy;
    Game.SoundSystem.MusicTransition(Game.SoundSystem.NeutralMusicStream, 12, 88369, FmodAudio.TimeUnit.MS);
    this.CuteBehaviour.Start();
    this.SadBehaviour.Stop();
    this.MoveBehaviour.MinMS = 200;
    this.MoveBehaviour.MaxMS = 400;
  }
  private bool MoveTowardGoal()
  {
    if (GoalPosition == null || GoalPosition == Position) return false;
    Vector3 direction = (Vector3)GoalPosition - Position;
    Vector3 directionNormalized = Vector3.Normalize(direction);
    directionNormalized.X = Math.Sign(directionNormalized.X);
    directionNormalized.Y = Math.Sign(directionNormalized.Y);
    Position += directionNormalized;
    bool moved = Position != GoalPosition;
    Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooStepSound, this, false);
    if (!moved) GoalPosition = null;
    return moved;
  }

  private void DoCuteThing()
  {
    Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooCuteSounds, this);
  }

  private void WannaGoToRandomPlace()
  {
    var rnd = new Random();
    var randomMove = new Vector3(rnd.Next(-4, 5), rnd.Next(-4, 5), 0);
    GoalPosition = Position + randomMove;
  }

  private void GoAsleep()
  {
    if (Mood == Mood.Sleeping) return;
    IGlobalActions.SayLocalizedString("beboo.gosleep", this.Name);
    GoingTiredBehaviour.Stop();
    MoveBehaviour.Stop();
    CuteBehaviour.Stop();
    Game.SoundSystem.PlayBebooSound(Game.SoundSystem.GrassSound, this);
    Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooYawningSounds, this);
    Mood = Mood.Sleeping;
    SleepingBehaviour.Start();
  }
  public void WakeUp()
  {
    if (Mood != Mood.Sleeping) return;
    IGlobalActions.SayLocalizedString("beboo.wakeup", this.Name);
    GoingTiredBehaviour.Start();
    MoveBehaviour.Start(3000);
    CuteBehaviour.Start();
    SleepingBehaviour.Stop();
    Game.SoundSystem.PlayBebooSound(Game.SoundSystem.GrassSound, this);
    Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooYawningSounds, this);
    Mood = Mood.Happy;
  }
  public void Eat(FruitSpecies fruitSpecies)
  {
    if (Mood == Mood.Sleeping) return;
    if (fruitSpecies == FruitSpecies.Normal)
    {
      Energy++;
      Happiness++;
      Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooChewSounds, this, true, 0.5f);
      Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooYumySounds, this);
    }
  }
  private DateTime _lastPetted = DateTime.MinValue;

  private int _petCount = 0;
  public void GetPetted()
  {
    if ((DateTime.Now - _lastPetted).TotalMilliseconds < 500) return;
    _petCount++;
    _lastPetted = DateTime.Now;
    Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooPetSound, this, false);
    var rnd = new Random();
    if (_petCount + rnd.Next(2)>= 5){
      Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooDelightSounds, this);
      if (Happiness <= 5 && rnd.Next(4) == 3) { Happiness++;
        Game.SoundSystem.System.PlaySound(Game.SoundSystem.JingleComplete);
      }
      _petCount = 0;
    }
  }
}