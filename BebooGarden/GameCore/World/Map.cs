﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using BebooGarden.GameCore.Item;
using BebooGarden.GameCore.Pet;
using BebooGarden.Minigame;
using FmodAudio;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

namespace BebooGarden.GameCore.World;

public class Map
{
  public static Dictionary<MapPreset, Map> Maps = new()
  {
    {MapPreset.garden, new Map(MapPreset.garden, 40, 40,
        [new TreeLine(new Vector2(20, 20), new Vector2(20, -20))],
       new Vector3(-15, 0, 0), FmodAudio.Preset.Plain) },
    {MapPreset.snowy, new Map(MapPreset.snowy, 60, 60,
        [new TreeLine(new Vector2(-5,30), new Vector2(5, 30), 3, [FruitSpecies.Normal, FruitSpecies.Energetic])],
        new Vector3(-100, 0, 0), FmodAudio.Preset.Plain) },
    {MapPreset.underwater, new Map(MapPreset.underwater, 40, 40,
        [],
        new Vector3(0, 0, 0), FmodAudio.Preset.UnderWater) },
    {MapPreset.basicrace, new Map(MapPreset.basicrace, Race.BASERACELENGTH, 10,
        [],
        new Vector3(0, -(Race.BASERACELENGTH / 2) - 10, 0), FmodAudio.Preset.StoneCorridor) },
        {MapPreset.snowyrace, new Map(MapPreset.snowyrace, Race.BASERACELENGTH, 10,
        [],
        null, FmodAudio.Preset.Plain) }
  };
  private int SizeX { get; set; }
  private int SizeY { get; set; }
  public List<TreeLine> TreeLines { get; }
  public Vector3? WaterPoint { get; }
  public List<Item.Item> Items { get; set; } = new();
  public bool IsLullabyPlaying { get; set; } = false;
  public bool IsDansePlaying { get; set; } = false;
  [JsonIgnore]
  public List<Channel> WaterChannels { get; set; } = new();
  [JsonIgnore]
  public List<Channel> TreesChannels { get; set; } = new();
  [JsonIgnore]
  public Channel? BackgroundChannel { get; set; }
  public ReverbProperties ReverbPreset { get; set; }
  public MapPreset Preset { get; }
  public List<Beboo> Beboos { get; set; } = new();
  public bool Paused { get; internal set; }
  private TimedBehaviour TicketPopBehaviour { get; set; }
  private TimedBehaviour SnowBallPopBehaviour { get; set; }
  private TimedBehaviour BubblePopBehaviour { get; set; }

  public Map(MapPreset preset, int sizeX, int sizeY, List<TreeLine> treeLines, Vector3? waterPoint, ReverbProperties reverbPreset)
  {
    this.Preset = preset;
    SizeX = sizeX;
    SizeY = sizeY;
    TreeLines = treeLines;
    WaterPoint = waterPoint;
    TicketPopBehaviour = new(30000 * 60, 60000 * 60, true);
    SnowBallPopBehaviour = new(10000, 15000, this.Preset == MapPreset.snowy);
    BubblePopBehaviour = new(10000, 15000, this.Preset == MapPreset.underwater);
    ReverbPreset = reverbPreset;
  }

  private void PopTicketPack()
  {
    if (!Items.OfType<TicketPack>().Any())
    {
      Vector3 randPos = GenerateRandomUnoccupedPosition();
      AddItem(new TicketPack(Game1.Instance.Random.Next(4)), randPos);
    }
  }

  public Vector3 GenerateRandomUnoccupedPosition(bool excludeWater = true)
  {
    int tryCounter = 0;
    Vector3 randPos;
    do
    {
      randPos = new Vector3(Game1.Instance.Random.Next(-SizeX / 2, SizeX / 2), Game1.Instance.Random.Next(-SizeY / 2, SizeY / 2), 0);
      tryCounter++;
    } while ((tryCounter <= 10 && excludeWater && IsInLake(randPos)) || GetTreeLineAtPosition(randPos) != null);
    return randPos;
  }

  public Vector3 Clamp(Vector3 value)
  {
    float x = Math.Clamp(value.X, SizeX / 2 * -1, SizeX / 2);
    float y = Math.Clamp(value.Y, SizeY / 2 * -1, SizeY / 2);
    float z = value.Z;
    Vector3 newPos = new(x, y, z);
    return newPos;
  }
  public bool IsInLake(Vector3 position)
  {
    return Preset == MapPreset.underwater || WaterPoint != null && Util.IsInSquare(position, WaterPoint.Value, 5);
  }

  public TreeLine? GetTreeLineAtPosition(Vector3 position)
  {
    return TreeLines.FirstOrDefault(
        treeLine => treeLine != null && treeLine.IsOnLine(position),
        null);
  }

  public bool AddItem(Item.Item item, Vector3 position)
  {
    if (GetTreeLineAtPosition(position) != null) return false;
    Items.Add(item);
    item.Position = position;
    if (Paused) item.Pause();
    return true;
  }


  public Item.Item? GetItemArroundPosition(Vector3 position)
  {
    return Items == null || Items.Count == 0
      ? null
      : Items.FirstOrDefault(item => item != null && item.Position != null && Util.IsInSquare(item.Position.Value, position, 1),
            null);
  }
  public List<Beboo> GetBeboosArround(Vector3 position)
  {
    return Beboos.FindAll(beboo => beboo != null && beboo.Position != null && Util.IsInSquare(beboo.Position, position, 1));
  }
  public bool IsArroundShop(Vector3 position)
  {
    return Util.IsInSquare(new Vector3(SizeX / 2, -SizeY / 2, 0), position, 1);
  }
  public bool IsArroundMapPath(Vector3 position)
  {
    return (Preset == MapPreset.garden || Preset == MapPreset.snowy) && Util.IsInSquare(new Vector3(-SizeX / 2, -SizeY / 2, 0), position, 1);
  }
  public bool IsArroundMapUnderWater(Vector3 position)
  {
    switch (Preset)
    {
      case MapPreset.garden:
        return Util.IsInSquare(new Vector3(-SizeX / 2, 0, 0), position, 1);
      case MapPreset.underwater:
        return Util.IsInSquare(new Vector3(SizeX / 2, 0, 0), position, 1);
      default: return false;
    }
  }
  public bool IsArroundRaceGate(Vector3 position)
  {
    return Util.IsInSquare(new Vector3(-SizeX / 2, SizeY / 2, 0), position, 1);
  }

  public override bool Equals(object? obj)
  {
    return obj is Map map &&
           Preset == map.Preset;
  }
  public void Update(GameTime gameTime)
  {
    foreach (var trelline in TreeLines.ToList())
    {
      trelline.Update(gameTime);
    }
    if (TicketPopBehaviour.ItsTime())
    {
      PopTicketPack();
      TicketPopBehaviour.Done();
    }
    if (SnowBallPopBehaviour.ItsTime())
    {
      List<Item.Item> snowBalls = this.Items.FindAll(x => x is SnowBall);
      if (snowBalls.Count < 10)
      {
        Vector3 randPos = GenerateRandomUnoccupedPosition();
        AddItem(new SnowBall(), randPos);
      }
      SnowBallPopBehaviour.Done();
    }
    if (BubblePopBehaviour.ItsTime())
    {
      List<Item.Item> bubbles = this.Items.FindAll(x => x is Bubble);
      if (bubbles.Count < 15)
      {
        Vector3 randPos = GenerateRandomUnoccupedPosition(false);
        AddItem(new Bubble(), randPos);
      }
      BubblePopBehaviour.Done();
    }
  }
}