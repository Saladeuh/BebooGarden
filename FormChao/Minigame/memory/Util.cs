namespace BebooGarden.Minigame.memory;

public static class Util
{
  public static IEnumerable<int> IndexesWhere<T>(this IEnumerable<T> source, Func<T, bool> predicate)
  {
    int index = 0;
    foreach (T element in source)
    {
      if (predicate(element))
      {
        yield return index;
      }
      index++;
    }
  }
}
