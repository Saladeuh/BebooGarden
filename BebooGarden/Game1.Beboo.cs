using BebooGarden.Content;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.Interface.UI;
using BebooGarden.Minigame;
using BebooGarden.Save;
using CrossSpeak;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BebooGarden;

public partial class Game1
{
  private Beboo? _contester;

  private void SayBebooState()
  {
    if (Map?.Beboos.Count == 0)
    {
      CrossSpeakManager.Instance.Output(GameText.nobeboo);
    }
    if (Map is null) return;
    string allSentences = "";
    foreach (var beboo in Map.Beboos)
    {
      string sentence;
      string name = beboo.Name;
      if (beboo.Sleeping)
      {
        sentence = GameText.beboo_sleep;
      }
      else
      {
        if (beboo.Happiness < 0) sentence = GameText.beboo_verysad;
        else if (beboo.Energy < 0) sentence = GameText.beboo_verytired;
        else if (beboo.Energy < 3) sentence = GameText.beboo_littletired;
        else sentence = beboo.Happiness < 3 ? GameText.beboo_littlesad : (beboo.Energy < 8 ? GameText.beboo_good : GameText.beboo_verygood);
      }
      allSentences += String.Format(sentence, name);
#if DEBUG
      allSentences += $"\n{beboo.Age}";
#endif
      allSentences += "\n";
    }
    CrossSpeakManager.Instance.Output(allSentences);
  }
  private void FeedBeboo()
  {
    Beboo? bebooUnderCursor = BebooUnderCursor();
    if (Save.FruitsBasket == null || bebooUnderCursor == null) return;
    Dictionary<string, FruitSpecies> options = [];
    foreach (KeyValuePair<FruitSpecies, int> fruit in Save.FruitsBasket)
    {
      if (fruit.Value > 0) options.Add(fruit.Key.ToString() + " " + fruit.Value.ToString(), fruit.Key);
    }
    if (options.Count == 1)
    {
      OnFruitSelected(options.First().Value);
    }
    else if (options.Count > 0)
    {
      new ChooseMenu<FruitSpecies>(GameText.ui_chooseitem, options, OnFruitSelected)
       .Show();
    }
  }

  private void OnFruitSelected(FruitSpecies choice)
  {
    if (choice != FruitSpecies.None)
    {
      Beboo? bebooUnderCursor = BebooUnderCursor();
      bebooUnderCursor?.Eat(choice);
      Save.FruitsBasket[choice]--;
    }
  }

  public void ChooseBebooForRace()
  {
    if (Map?.Beboos.Count > 0)
    {
      if (Map.Beboos.Count > 1)
      {
        Dictionary<string, Beboo
          > options = new();
        foreach (Beboo b in Map.Beboos)
          options.Add(b.Name, b);
        new ChooseMenu<Beboo?>(GameText.choosebeboo, options, StartRace)
          .Show();
      }
      else { StartRace(Map?.Beboos[0]); }
    }
    else
    {
      CrossSpeakManager.Instance.Output(GameText.nobeboo);
      SoundSystem.System.PlaySound(SoundSystem.WarningSound);
    }
  }

  private void StartRace(Beboo? contester)
  {
    if (Race.GetRemainingTriesToday() > 0)
    {
      if (contester == null) return;
      Dictionary<string, RaceType> raceTypeOptions = new()
      {
        { GameText.race_simple, RaceType.Base },
      };
      if (Save.Flags.UnlockSnowyMap) raceTypeOptions.Add(GameText.race_snow, RaceType.Snowy);
      RaceType choice = RaceType.Base;
      _contester = contester;
      if (raceTypeOptions.Count > 1)
      {
        new ChooseMenu<RaceType>(GameText.race_chooserace, raceTypeOptions, OnRaceChoosed)
          .Show();
      }
      else
      {
        OnRaceChoosed(choice);
      }
    }
    else
    {
      SoundSystem.System.PlaySound(SoundSystem.WarningSound);
      CrossSpeakManager.Instance.Output(GameText.race_trytommorow);
    }
  }

  private void OnRaceChoosed(RaceType raceType)
  {
    if (Minigame.Race.IsARaceRunning || CurrentPlayingMiniGame != null) return;
    CurrentPlayingMiniGame = new Minigame.Race(raceType, _contester);
    CurrentPlayingMiniGame.Start();
  }
}
