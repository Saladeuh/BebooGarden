using System.Numerics;
using BebooGarden.GameCore.World;
using BebooGarden.Interface;

namespace BebooGarden.GameCore.Pet;

internal class Beboo
{
  public string Name { get; set; }
  public int Age { get; set; }
  public float Energy { get; private set; }
  private int _hapiness;
  private int Happiness
  {
    get => _hapiness;
    set
    {
      if (_hapiness > value && value <= 0) BurstInTearrs();
      else if (_hapiness < value && _hapiness <= 0) BeHappy();
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
      if (value.HasValue) _goalPosition= Game.Map.Clamp(value.Value);
      else _goalPosition = null;
    }
  }

  private TimedBehaviour<Beboo> GoingTiredBehaviour { get; }
  private TimedBehaviour<Beboo> MoveBehaviour { get; }
  private TimedBehaviour<Beboo> SadBehaviour { get; }
  private TimedBehaviour<Beboo> SleepingBehaviour { get; }

  public Beboo(string name = "Bob", int age = 0, DateTime lastPlayed = default)
  {
    Position = new Vector3(0, 0, 0);
    Name = name == string.Empty ? "bob" : name;
    Happiness = 5;
    Age = age;
    bool isSleepingAtStart = DateTime.Now.Hour < 8 || DateTime.Now.Hour > 20;
    Mood = isSleepingAtStart ? Mood.Sleeping : Mood.Happy;
    Energy = (DateTime.Now - lastPlayed).Hours > 4 ? 10 : 5;
    CuteBehaviour = new(this, 15000, 25000, beboo =>
    {
      beboo.DoCuteThing();
    });
    if (!isSleepingAtStart) CuteBehaviour.Start();
    MoveBehaviour = new(this, 200, 400, beboo =>
    {
      beboo.MoveTowardGoal();
    });
    if (!isSleepingAtStart) MoveBehaviour.Start();
    TimedBehaviour<Beboo> fancyMoveBehaviour = new(this, 30000, 60000, beboo =>
    {
      beboo.WannaGoToRandomPlace();
    });
    fancyMoveBehaviour.Start();
    GoingTiredBehaviour = new(this, 50000, 70000, beboo =>
    {
      if (beboo.Age < 2) beboo.Energy -= 2;
      else beboo.Energy--;
      if (beboo.Energy <= 0) GoAsleep();
    });
    if (!isSleepingAtStart) GoingTiredBehaviour.Start();
    TimedBehaviour<Beboo> goingDepressedBehaviour = new(this, 120000, 150000, beboo =>
    {
      beboo.Happiness--;
      //if (beboo is { Happiness: <= 0, Mood: Mood.Happy }) beboo.BurstInTearrs();
    });
    goingDepressedBehaviour.Start();
    SadBehaviour = new(this, 5000, 7000, beboo =>
    {
      Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooCrySounds, beboo);
    });
    SleepingBehaviour = new(this, 5000, 10000, beboo =>
    {
      beboo.Energy += 0.10f;
      Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooSleepingSounds, beboo, 0.3f);
      //if (beboo.Energy >= 10) WakeUp();
    });
    if (isSleepingAtStart) SleepingBehaviour.Start();
  }

  private void BurstInTearrs()
  {
    if (Mood == Mood.Sad) return;
    this.Mood = Mood.Sad;
    Game.SoundSystem.MusicTransition(Game.SoundSystem.SadMusicStream, 464375, 4471817, FmodAudio.TimeUnit.PCM, 0.1f);
    this.CuteBehaviour.Stop();
    this.SadBehaviour.Start();
    this.MoveBehaviour.MinMS = 500;
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
    Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooStepSound, this);
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
    if (fruitSpecies==FruitSpecies.Normal)
    {
      Energy ++;
      Happiness ++;
      Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooChewSounds, this, 0.5f);
      Game.SoundSystem.PlayBebooSound(Game.SoundSystem.BebooYumySounds, this);
    }
  }
}