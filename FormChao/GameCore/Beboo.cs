using System.Numerics;
using BebooGarden.Interface;

namespace BebooGarden.GameCore;

internal class Beboo
{
  public string Name { get; set; }
  public int Age { get; set; }
  public float Energy { get; set; }
  public int Happiness { get; set; }
  public Mood Mood { get; set; }
  public Vector3 Position { get; set; }
  public SoundSystem SoundSystem { get; set; }
  public BebooBehaviour CuteBehaviour { get; }
  public Vector3? GoalPosition { get; set; }
  public BebooBehaviour GoingTiredBehaviour { get; }
  public BebooBehaviour MoveBehaviour { get; }
  public BebooBehaviour SleepingBehaviour { get; }

  public Beboo(SoundSystem soundSystem)
  {
    Name = "waw";
    Energy = 1;
    Happiness = 0;
    Age = 0;
    Mood = Mood.Happy;
    Position = new Vector3(0, 0, 0);
    SoundSystem = soundSystem;
    CuteBehaviour = new BebooBehaviour(this, 15000, 25000, (Beboo beboo) =>
    {
      beboo.DoCuteThing();
    });
    CuteBehaviour.Start();
    MoveBehaviour = new(this, 200, 400, (Beboo beboo) =>
    {
      beboo.MoveTowardGoal();
    });
    MoveBehaviour.Start();
    BebooBehaviour fancyMoveBehaviour = new(this, 30000, 60000, (Beboo beboo) =>
    {
      beboo.WannaGoToRandomPlace();
    });
    fancyMoveBehaviour.Start();
    GoingTiredBehaviour = new BebooBehaviour(this, 50000, 70000, (Beboo beboo) =>
    {
      if (beboo.Age < 2) beboo.Energy -= 2;
      else beboo.Energy--;
      if(beboo.Energy<=0) GoAsleep();
    });
    GoingTiredBehaviour.Start();
    SleepingBehaviour = new BebooBehaviour(this, 5000, 10000, (Beboo beboo) =>
    {
      beboo.Energy += 0.10f;
      SoundSystem.PlayBebooSound(SoundSystem.BebooSleepingSounds, beboo, 0.3f);
      if (beboo.Energy>=10) WakeUp();  
    });
  }
  public bool MoveTowardGoal()
  {
    if (GoalPosition == null || GoalPosition == Position) return false;
    Vector3 direction = (Vector3)GoalPosition - Position;
    Vector3 directionNormalized = Vector3.Normalize(direction);
    directionNormalized.X = Math.Sign(directionNormalized.X);
    directionNormalized.Y = Math.Sign(directionNormalized.Y);
    Position += directionNormalized;
    bool moved = Position != GoalPosition;
    SoundSystem.PlayBebooSound(SoundSystem.BebooStepSound, this);
    if (!moved) GoalPosition = null;
    return moved;
  }
  public void DoCuteThing()
  {
    SoundSystem.PlayBebooSound(SoundSystem.BebooCuteSounds, this);
  }
  public void WannaGoToRandomPlace()
  {
    var rnd = new Random();
    var randomMove = new Vector3(rnd.Next(-4, 5), rnd.Next(-4, 5), 0);
    GoalPosition = Position + randomMove;
  }
  public void GoAsleep()
  {
    ScreenReader.Output($"{Name} va dormir.");
    GoingTiredBehaviour.Stop();
    MoveBehaviour.Stop();
    CuteBehaviour.Stop();
    SoundSystem.PlayBebooSound(SoundSystem.GrassSound, this);
    SoundSystem.PlayBebooSound(SoundSystem.BebooYawningSounds, this);
    Mood = Mood.Sleeping;
    SleepingBehaviour.Start();
  }
  public void WakeUp()
  {
    if (Mood != Mood.Sleeping) return;
    ScreenReader.Output($"{Name} se réveille.");
    GoingTiredBehaviour.Start();
    MoveBehaviour.Start();
    CuteBehaviour.Start();
    SleepingBehaviour.Stop();
    SoundSystem.PlayBebooSound(SoundSystem.GrassSound, this);
    SoundSystem.PlayBebooSound(SoundSystem.BebooYawningSounds, this);
    Mood = Mood.Happy;
  }
}