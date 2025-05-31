using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using BebooGarden.GameCore;
using BebooGarden.GameCore.Pet;
using Microsoft.Xna.Framework.Input;

namespace BebooGarden;

public static class Util
{
  public static readonly Vector3[] DIRECTIONS = [new(0, 1, 0), new(1, 0, 0), new(-1, 0, 0), new(0, -1, 0)];
  public static readonly string[] Colors =
      ["pink", "red", "orange", "yellow", "green", "blue", "indigo", "violet", "none"];

  public static bool IsInSquare(Vector3 otherPoint, Vector3 center, int halfSideSize)
  {
    bool isInX = Math.Abs(otherPoint.X - center.X) <= halfSideSize;
    bool isInY = Math.Abs(otherPoint.Y - center.Y) <= halfSideSize;
    return isInX && isInY;
  }

  public static IEnumerable<string> SplitToLines(this string input)
  {
    if (input == null) yield break;

    using StringReader reader = new(input);
    while (reader.ReadLine() is { } line) yield return line;
  }
  public static bool IsKeyDigit(Keys key, out int keyInt)
  {
    return int.TryParse(key.ToString().Replace("NumPad", "").Replace("D", ""), out keyInt);
  }
  public static BebooType GetRandomBebooType()
  {
    var bebooTypes = Enum.GetValues(typeof(BebooType));
    return (BebooType)bebooTypes.GetValue(Game1.Instance.Random.Next(1, bebooTypes.Length));
  }

  public static BebooType GetBebooTypeByColor(string color)
  {
    return color switch
    {
      "none" => BebooType.Base,
      "pink" => BebooType.Pink,
      "red" => BebooType.Red,
      "orange" => BebooType.Orange,
      "yellow" => BebooType.Yellow,
      "green" => BebooType.Green,
      "blue" => BebooType.Blue,
      "indigo" => BebooType.Indigo,
      "violet" => BebooType.Violet,
      _ => BebooType.Base,
    };
  }
}