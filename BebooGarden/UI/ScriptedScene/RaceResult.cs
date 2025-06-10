using BebooGarden.Content;
using BebooGarden.GameCore;
using BebooGarden.GameCore.Pet;
using BebooGarden.Minigame;
using CrossSpeak;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace BebooGarden.UI.ScriptedScene;

internal class RaceResult : IScriptedScene
{
  private (int, double) _third;
  private (int, double) _second;
  private (int, double) _first;
  private RaceType _raceType;
  private Beboo _mainBeboo;
  private DateTime _timer = DateTime.Now;
  private int _currentStep = 0;
  private bool _isComplete = false;

  // Flags pour s'assurer que chaque action n'est exécutée qu'une fois
  private bool _step0Executed = false;
  private bool _step1Executed = false;
  private bool _step2Executed = false;
  private bool _step3Executed = false;

  // Durées pour chaque étape (en millisecondes)
  private readonly int[] _stepDurations = { 500, 1000, 1500, 500 };

  public RaceResult((int, double) third, (int, double) second, (int, double) first, GameCore.Pet.Beboo mainBeboo, RaceType raceType)
  {
    _third = third;
    _second = second;
    _first = first;
    _raceType = raceType;
_mainBeboo = mainBeboo;
  }

  public void Update(GameTime gameTime)
  {
    if (_isComplete) return;

    double elapsedMs = (DateTime.Now - _timer).TotalMilliseconds;

    switch (_currentStep)
    {
      case 0: // Début - jouer le son d'arrêt
        if (!_step0Executed)
        {
          Game1.Instance.SoundSystem.PlayCinematic(Game1.Instance.SoundSystem.RaceStopSound, false);
          _step0Executed = true;
        }
        if (elapsedMs >= _stepDurations[0])
        {
          NextStep();
        }
        break;

      case 1: // Annoncer la troisième place
        if (!_step1Executed)
        {
          Game1.Instance.SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.RaceBadSound);
          CrossSpeakManager.Instance.Output(String.Format(GameText.race_third, Game1.Instance.Map.Beboos[_third.Item1].Name));
          _step1Executed = true;
        }
        if (elapsedMs >= _stepDurations[1])
        {
          NextStep();
        }
        break;

      case 2: // Annoncer la deuxième place
        if (!_step2Executed)
        {
          Game1.Instance.SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.RaceGoodSound);
          CrossSpeakManager.Instance.Output(String.Format(GameText.race_second, Game1.Instance.Map.Beboos[_second.Item1].Name, _second.Item2));
          _step2Executed = true;
        }
        if (elapsedMs >= _stepDurations[2])
        {
          NextStep();
        }
        break;

      case 3: // Annoncer la première place
        if (!_step3Executed)
        {
          Game1.Instance.SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.RaceGoodSound);
          CrossSpeakManager.Instance.Output(String.Format(GameText.race_first, Game1.Instance.Map.Beboos[_first.Item1].Name, _first.Item2));
          _step3Executed = true;
        }
        if (elapsedMs >= _stepDurations[3])
        {
          Game1.Instance.Map?.Beboos[1].Pause();
          Game1.Instance.Map?.Beboos[2].Pause();
          Game1.Instance.Map?.Beboos.Clear();
          Game1.Instance.SoundSystem.PlayCinematic(Game1.Instance.SoundSystem.CinematicRaceEnd);
          Game1.Instance.LoadBackedMap();
          Game1.Instance.ChangeMapMusic();
          _mainBeboo.Position = new(0, 0, 0);
          _mainBeboo.Destination = new(0, 0, 0);
          Race.IsARaceRunning = false;
          if (_first.Item1 == 0)
          {
            switch (_raceType)
            {
              case RaceType.Base: Game1.Instance.GainTicket(2); break;
              case RaceType.Snowy: Game1.Instance.GainTicket(3); break;
            }
            Race.TotalWin++;
            _mainBeboo.Happiness += 2;
            _mainBeboo.Energy -= 1;
          }
          else
          {
            _mainBeboo.Happiness--;
            _mainBeboo.Energy -= 2;
          }
          _isComplete = true;
          Close();
        }
        break;
    }
  }

  private void NextStep()
  {
    _currentStep++;
    _timer = DateTime.Now; // Reset le timer pour la prochaine étape
  }

  public void Show()
  {
    Game1.Instance.SwitchToScreen(GameScreen.ScriptedScene);
    Game1.Instance._scriptedScene = this;

    // Réinitialiser tous les états
    _timer = DateTime.Now;
    _currentStep = 0;
    _isComplete = false;
    _step0Executed = false;
    _step1Executed = false;
    _step2Executed = false;
    _step3Executed = false;
  }

  private void Close()
  {
    Game1.Instance.SwitchToScreen(GameScreen.game);
    Game1.Instance._scriptedScene = null;
  }
}