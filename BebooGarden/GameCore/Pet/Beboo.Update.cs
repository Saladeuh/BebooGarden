using info.lundin.math;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
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
        CuteBehaviour.Done();
      }
    }
    if (MoveBehaviour.ItsTime())
    {
      if (!Sleeping)
      {
        MoveTowardGoal();
        MoveBehaviour.Done();
      }
    }
    if (GoToSleepOrWakeUpBehaviour.ItsTime())
    {
      if (!Sleeping && Energy <= 0)
      {
        GoAsleep();
      }
      else if (Sleeping && Energy > 2)
      {
        WakeUp();
        GoToSleepOrWakeUpBehaviour.Done();
      }
    }
    if (FancyMoveBehaviour.ItsTime())
    {
      if (!Sleeping && !Racer)
      {
        if (Happy || (!Happy && Game1.Instance.Random.Next(3) == 1))
          WannaGoToRandomPlace();
        FancyMoveBehaviour.Done();
      }
    }
    if (GoingTiredBehaviour.ItsTime())
    {
      if (!Sleeping)
      {
        Energy--;
        GoingTiredBehaviour.Done();
      }
    }
    if (GoingSadBehaviour.ItsTime())
    {
      if (!Sleeping && !Racer)
      {
        Happiness--;
        GoingSadBehaviour.Done();
      }
    }
    if (!Racer && EmotionBehaviour.ItsTime())
    {
      if (Happy && Happiness <= 0)
        BurstInTearrs();
      else if (!Happy && Happiness > 0)
      {
        Task.Run(async () =>
        {
          await Task.Delay(1000);
          BeHappy();
        });
      }
      if (Energy > 5 && Happiness >= 9)
        BeOverexcited();
      else if (Happiness <= 0 && Energy < 5)
        BeFloppy();
      else BeNormal();
      EmotionBehaviour.Done();
    }
    if (CryBehaviour.ItsTime())
    {
      if (!Sleeping && !Racer)
      {
        Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.BebooCrySounds, this);
        CryBehaviour.Done();
      }
    }
    if (SleepingBehaviour.ItsTime())
    {
      if (Sleeping)
      {
        Energy += 0.10f;
        Game1.Instance.SoundSystem.PlayBebooSound(Game1.Instance.SoundSystem.BebooSleepingSounds, this, true, 0.3f);
        CuteBehaviour.Done(); ;
      }
    }
    //+0.1 every 3mn=1lvl/30mn
    if (!Racer && GrowthBehaviour.ItsTime())
    {
      if (Energy >= 2 && Happiness >= 2)
      {
        Age += 0.1f;
        GrowthBehaviour.Done();
      }
    }
  }
}
