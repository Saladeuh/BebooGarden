using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace chaos;

internal class Bebou
{
  public string Name { get; set; }
  public int energy { get; set; }
  public int Happiness { get; set; }
  public Mood mood { get; set; }
  public Vector3 position { get; set; }
}

internal enum Mood
{
  Happy,
  Angry,
  Sad,
}