using Microsoft.Xna.Framework;
using System;
using BebooGarden.GameCore;
using CrossSpeak;
using BebooGarden.Content;

namespace BebooGarden.UI.ScriptedScene;

internal class RaceResult : IScriptedScene
{
  private (int, double) _third;
  private (int, double) _second;
  private (int, double) _first;

  private DateTime _timer = DateTime.Now;
  private int _currentStep = 0;
  private bool _isComplete = false;

  // Durées pour chaque étape (en millisecondes)
  private readonly int[] _stepDurations = { 500, 1000, 1500, 500 };

  public RaceResult((int, double) third, (int, double) second, (int, double) first)
  {
    _third = third;
    _second = second;
    _first = first;
  }

  public void Update(GameTime gameTime)
  {
    if (_isComplete) return;

    double elapsedMs = (DateTime.Now - _timer).TotalMilliseconds;

    switch (_currentStep)
    {
      case 0: // Début - jouer le son d'arrêt
        if (elapsedMs == 0) // Premier passage
        {
          Game1.Instance.SoundSystem.PlayCinematic(Game1.Instance.SoundSystem.RaceStopSound, false);
        }
        if (elapsedMs >= _stepDurations[0])
        {
          NextStep();
        }
        break;

      case 1: // Annoncer la troisième place
        if (elapsedMs == 0) // Premier passage de cette étape
        {
          Game1.Instance.SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.RaceBadSound);
          CrossSpeakManager.Instance.Output(String.Format(GameText.race_third, Game1.Instance.Map.Beboos[_third.Item1].Name));
        }
        if (elapsedMs >= _stepDurations[1])
        {
          NextStep();
        }
        break;

      case 2: // Annoncer la deuxième place
        if (elapsedMs == 0) // Premier passage de cette étape
        {
          Game1.Instance.SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.RaceGoodSound);
          CrossSpeakManager.Instance.Output(String.Format(GameText.race_second, Game1.Instance.Map.Beboos[_second.Item1].Name, _second.Item2));
        }
        if (elapsedMs >= _stepDurations[2])
        {
          NextStep();
        }
        break;

      case 3: // Annoncer la première place
        if (elapsedMs == 0) // Premier passage de cette étape
        {
          Game1.Instance.SoundSystem.System.PlaySound(Game1.Instance.SoundSystem.RaceGoodSound);
          CrossSpeakManager.Instance.Output(String.Format(GameText.race_first, Game1.Instance.Map.Beboos[_first.Item1].Name, _first.Item2));
        }
        if (elapsedMs >= _stepDurations[3])
        {
          _isComplete = true;
          Close();        }
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
    _timer = DateTime.Now;
    _currentStep = 0;
    _isComplete = false;
  }

  private void Close()
  {
    Game1.Instance.SwitchToScreen(GameScreen.game);
    Game1.Instance._scriptedScene = null;
  }
}