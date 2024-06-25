using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BebooGarden.GameCore;

internal class Beboo
{
    public string Name { get; set; }
    public int Energy { get; set; }
    public int Happiness { get; set; }
    public Mood Mood { get; set; }
    public Vector3 Position { get; set; }
    public SoundSystem SoundSystem { get; set; }
    public Beboo(SoundSystem soundSystem)
    {
        Name = "waw";
        Energy = 10;
        Happiness = 0;
        Mood = Mood.Happy;
        Position = new Vector3(0, 0, 0);
        SoundSystem = soundSystem;
    }
    public void MoveTo(Vector3 destination)
    {
        Position = destination;
    }
    public void DoCuteThing()
    {
        SoundSystem.PlayCuteSound(this);
    }
}

internal enum Mood
{
    Happy,
    Angry,
    Sad,
}