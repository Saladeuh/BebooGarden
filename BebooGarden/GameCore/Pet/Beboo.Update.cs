using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BebooGarden.GameCore.Pet;

public partial class Beboo
{
  public void Update(GameTime gameTime)
  {
    if (Paused) return;
    if (CuteBehaviour.ItsTime())
    {
      if (!Sleeping)
      {
        DoCuteThing();
        CuteBehaviour.Timer = DateTime.Now;
      }
    }
    if (MoveBehaviour.ItsTime())
    {
      if (!Sleeping)
      {
        MoveTowardGoal();
        MoveBehaviour.Timer = DateTime.Now;
      }
    }
    if (FancyMoveBehaviour.ItsTime())
    {
      if (!Sleeping)
      {
        if (Happy || (!Happy && Game1.Instance.Random.Next(3) == 1))
          WannaGoToRandomPlace();
        FancyMoveBehaviour.Timer = DateTime.Now;
      }
    }
    if (GoingTiredBehaviour.ItsTime())
    {
      if (!Sleeping)
      {
        Energy--;
        GoingTiredBehaviour.Timer = DateTime.Now;
      }
    }
    if (GoingSadBehaviour.ItsTime())
    {
      if (!Sleeping)
      {
        Happiness--;
        GoingSadBehaviour.Timer = DateTime.Now;
      }
    }
    if (CryBehaviour.ItsTime())
    {
      if (!Sleeping)
      {
        Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.BebooCrySounds, this);
        CryBehaviour.Timer = DateTime.Now;
      }
    }
    if (SleepingBehaviour.ItsTime())
    {
      if (Sleeping)
      {
        Energy += 0.10f;
        Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.BebooSleepingSounds, this, true, 0.3f);
        CuteBehaviour.Timer = DateTime.Now;
      }
    }
    //+0.1 every 3mn=1lvl/30mn
    if (GrowthBehaviour.ItsTime())
    {
      if (Energy >= 2 && Happiness >= 2)
      {
        Age += 0.1f;
        GrowthBehaviour.Timer = DateTime.Now;
      }
    }
  }
}
