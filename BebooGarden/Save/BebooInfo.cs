﻿using BebooGarden.GameCore.Pet;

namespace BebooGarden.Save;

public class BebooInfo(string name, float age, int happiness, float energy, int swimLevel, float voice, BebooType bebooType)
{
  public string Name { get; set; } = name;
  public float Age { get; set; } = age;
  public int Happiness { get; set; } = happiness;
  public float Energy { get; set; } = energy;
  public int SwimLevel { get; set; } = swimLevel;
  public float Voice { get; set; } = voice;
  public bool KnowItsName { get; set; } = false;
  public BebooType BebooType { get; set; }=bebooType;
}
