using BebooGarden.Content;
using BebooGarden.Interface;
using CrossSpeak;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BebooGarden.Minigame.memory;

internal class Level
{
  private int Retry { get; set; }
  private int NbSounds { get; }
  private int MaxRetry { get; set; }
  private List<(int, CaseState)> Grid { get; set; }
  private SoundSystem SoundSystem { get; set; }
  public bool Win { get; internal set; } = true;
  public bool Ended { get; private set; }

  public Level(SoundSystem soundSystem, int nbSounds, int maxRetry, string group1, string? group2 = null)
  {
    NbSounds = nbSounds;
    MaxRetry = maxRetry;
    Grid = new List<(int, CaseState)>();
    FillGridByRandomInt();
    SoundSystem = soundSystem;
    SoundSystem.LoadLevel(nbSounds, group1, group2);
  }

  private void FillGridByRandomInt()
  {
    var rnd = new Random();
    var randomDisposition = Enumerable.Range(1, NbSounds).Concat(Enumerable.Range(1, NbSounds)).OrderBy(_ => rnd.Next()).ToArray();
    foreach (int n in randomDisposition)
    {
      Grid.Add((n, CaseState.None));
    }
  }

  public void Update(GameTime gameTime, KeyboardState currentKeyboardState)
  {
    if (Ended) return;
    if (Game1.Instance.IsKeyPressed(currentKeyboardState, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D6, Keys.D7, Keys.D8, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5, Keys.NumPad6, Keys.NumPad7, Keys.NumPad8))
    {
      var key = currentKeyboardState.GetPressedKeys()[0];
      if (BebooGarden.Util.IsKeyDigit(key, out int keyInt) && keyInt <= NbSounds * 2 && keyInt > 0)
      {
        int caseIndex = keyInt - 1;
        int soundIndex = Grid[caseIndex].Item1 - 1;
        SoundSystem.PlayQueue(SoundSystem.Sounds[soundIndex], queued: false);
        TryCase(soundIndex + 1, caseIndex);
        if (Grid.All(pair => pair.Item2 == CaseState.Paired))
        {
          CrossSpeakManager.Instance.Output(GameText.win);
          SoundSystem.PlayQueue(SoundSystem.JingleWin);
          Release();
          Win = true;
          Ended = true;
        }
        else if (Retry >= MaxRetry)
        {
          CrossSpeakManager.Instance.Output(GameText.lose);
          SoundSystem.PlayQueue(SoundSystem.JingleLose);
          Release();
          Win = false;
          Ended = true;
        }
      }
      else if (Game1.Instance.IsKeyPressed(currentKeyboardState, Keys.F4) && Game1.Instance.IsKeyPressed(currentKeyboardState, Keys.LeftAlt, Keys.RightAlt))
      {
        Win = false;
        Ended = true;
      }
    }
  }

  private void TryCase(int soundIndex, int caseIndex)
  {
    var caseIndexTouched = Grid.IndexesWhere(o => o.Item1 == soundIndex && o.Item2 == CaseState.Touched).ToList();
    var touchedCases = Grid.IndexesWhere(o => o.Item2 == CaseState.Touched).ToList();
    if (Grid[caseIndex].Item2 == CaseState.Paired || caseIndexTouched.Count == 1 && caseIndexTouched.Contains(caseIndex))
    {
      SoundSystem.PlayQueue(SoundSystem.JingleError);
    }
    else if (caseIndexTouched.Count == 1 && !caseIndexTouched.Contains(caseIndex))
    {
      Grid[caseIndexTouched[0]] = (soundIndex, CaseState.Paired);
      Grid[caseIndex] = (soundIndex, CaseState.Paired);
      SoundSystem.PlayQueue(SoundSystem.JingleCaseWin);
    }
    else if (touchedCases.Count == 1)
    {
      Grid[touchedCases[0]] = (Grid[touchedCases[0]].Item1, CaseState.None);
      Retry++;
      SoundSystem.PlayQueue(SoundSystem.JingleCaseLose);
    }
    else if (caseIndexTouched.Count == 0)
    {
      Grid[caseIndex] = (Grid[caseIndex].Item1, CaseState.Touched);
    }
  }
  public void Release()
  {
    Task.WaitAll(SoundSystem.tasks.ToArray());
    SoundSystem.FreeRessources();
    //SoundSystem.System.Release();
  }
}