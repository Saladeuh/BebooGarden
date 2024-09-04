using BebooGarden.Interface;

namespace BebooGarden.Minigame.memory;

internal class Level : Form
{
  private int Retry { get; set; }
  private int NbSounds { get; }
  private int MaxRetry { get; set; }
  private List<(int, CaseState)> Grid { get; set; }
  private SoundSystem SoundSystem { get; set; }
  public static bool Win { get; internal set; } = true;

  public Level(SoundSystem soundSystem, int nbSounds, int maxRetry, string group1, string? group2 = null)
  {
    Visible = false;
    Text = IGlobalActions.GetLocalizedString("boombox");
    NbSounds = nbSounds;
    MaxRetry = maxRetry;
    Grid = new List<(int, CaseState)>();
    FillGridByRandomInt();
    SoundSystem = soundSystem;
    SoundSystem.LoadLevel(nbSounds, group1, group2);
    KeyDown += Keymapper;
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

  private void Keymapper(object? sender, KeyEventArgs e)
  {
    if (BebooGarden.Util.IsKeyDigit(e.KeyCode, out var intKey) && intKey <= NbSounds * 2 && intKey > 0)
    {
      int caseIndex = intKey - 1;
      int soundIndex = Grid[caseIndex].Item1 - 1;
      SoundSystem.PlayQueue(SoundSystem.Sounds[soundIndex], queued: false);
      TryCase(soundIndex + 1, caseIndex);
      if (Grid.All(pair => pair.Item2 == CaseState.Paired))
      {
        IGlobalActions.SayLocalizedString("win");
        SoundSystem.PlayQueue(SoundSystem.JingleWin);
        Release();
        Win = true;
        Close();
      }
      else if (Retry >= MaxRetry)
      {
        IGlobalActions.SayLocalizedString("lose");
        SoundSystem.PlayQueue(SoundSystem.JingleLose);
        Release();
        Win = false;
        Close();
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