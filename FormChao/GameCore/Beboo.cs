using System.Numerics;
using BebooGarden.Interface;

namespace BebooGarden.GameCore;

internal class Beboo
{
  public string Name { get; set; }
  public int Energy { get; set; }
  public int Happiness { get; set; }
  public Mood Mood { get; set; }
  public Vector3 Position { get; set; }
  public SoundSystem SoundSystem { get; set; }
  public Vector3? GoalPosition { get; set; }
  public Beboo(SoundSystem soundSystem)
  {
    Name = "waw";
    Energy = 10;
    Happiness = 0;
    Mood = Mood.Happy;
    Position = new Vector3(0, 0, 0);
    SoundSystem = soundSystem;
    BebooBehaviour cuteBehaviour = new(this, 5000, 20000, (Beboo beboo) =>
    {
      beboo.DoCuteThing();
    });
    cuteBehaviour.Start();
    BebooBehaviour moveBehaviour = new(this, 200, 400, (Beboo beboo) =>
    {
      beboo.MoveTowardGoal();
    });
    moveBehaviour.Start();
    BebooBehaviour fancyMoveBehaviour = new(this, 30000, 60000, (Beboo beboo) =>
    {
      beboo.WannaGoToRandomPlace();
    });
    fancyMoveBehaviour.Start();
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
    if(!moved) GoalPosition = null;
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
    Mood=Mood.Sleeping;
  }
}