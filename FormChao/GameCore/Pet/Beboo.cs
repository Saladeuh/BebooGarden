using System.Numerics;
using BebooGarden.Interface;

namespace BebooGarden.GameCore.Pet;

internal class Beboo
{
    public string Name { get; set; }
    public int Age { get; set; }
    public float Energy { get; set; }
    public int Happiness { get; set; }
    public Mood Mood { get; set; }
    public Vector3 Position { get; set; }
    public SoundSystem SoundSystem { get; set; }
    public TimedBehaviour<Beboo> CuteBehaviour { get; }
    public Vector3? GoalPosition { get; set; }
    public TimedBehaviour<Beboo> GoingTiredBehaviour { get; }
    public TimedBehaviour<Beboo> MoveBehaviour { get; }
    public TimedBehaviour<Beboo> SleepingBehaviour { get; }

    public Beboo(SoundSystem soundSystem, string name = "Bob", int age = 0, DateTime lastPlayed = default)
    {
        SoundSystem = soundSystem;
        Position = new Vector3(0, 0, 0);
        Name = name;
        Happiness = 0;
        Age = age;
        bool isSleepingAtStart = DateTime.Now.Hour < 8 || DateTime.Now.Hour > 20;
        if (isSleepingAtStart) Mood = Mood.Sleeping;
        else Mood = Mood.Happy;
        if ((DateTime.Now - lastPlayed).Hours > 4) Energy = 10;
        else Energy = 1;
        CuteBehaviour = new(this, 15000, 25000, (Beboo beboo) =>
        {
            beboo.DoCuteThing();
        });
        if (!isSleepingAtStart) CuteBehaviour.Start();
        MoveBehaviour = new(this, 200, 400, (Beboo beboo) =>
        {
            beboo.MoveTowardGoal();
        });
        if (!isSleepingAtStart) MoveBehaviour.Start();
        TimedBehaviour<Beboo> fancyMoveBehaviour = new(this, 30000, 60000, (Beboo beboo) =>
        {
            beboo.WannaGoToRandomPlace();
        });
        fancyMoveBehaviour.Start();
        GoingTiredBehaviour = new(this, 50000, 70000, (Beboo beboo) =>
        {
            if (beboo.Age < 2) beboo.Energy -= 2;
            else beboo.Energy--;
            if (beboo.Energy <= 0) GoAsleep();
        });
        if (!isSleepingAtStart) GoingTiredBehaviour.Start();
        SleepingBehaviour = new(this, 5000, 10000, (Beboo beboo) =>
        {
            beboo.Energy += 0.10f;
            SoundSystem.PlayBebooSound(SoundSystem.BebooSleepingSounds, beboo, 0.3f);
            //if (beboo.Energy >= 10) WakeUp();
        });
        if (isSleepingAtStart) SleepingBehaviour.Start();
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
        MoveBehaviour.Start(3000);
        CuteBehaviour.Start();
        SleepingBehaviour.Stop();
        SoundSystem.PlayBebooSound(SoundSystem.GrassSound, this);
        SoundSystem.PlayBebooSound(SoundSystem.BebooYawningSounds, this);
        Mood = Mood.Happy;
    }
}