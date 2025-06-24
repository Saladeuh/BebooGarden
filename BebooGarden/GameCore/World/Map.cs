using System;
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
  public static Dictionary<MapPreset, Map> Maps { get; private set; }

  public static Map Garden { get; private set; }
  public static Map Snowy { get; private set; }
  public static Map UnderWater { get; private set; }
  public static Map Beach { get; }
  public static Map BasicRace { get; private set; }
  public static Map SnowyRace { get; private set; }

  static Map()
  {
    Garden = new Map(MapPreset.garden, 40, 40,
        [new TreeLine(new Vector2(20, 20), new Vector2(20, -20))],
       [new WaterRectangle(WaterPreset.Lagoon, new Vector3(-20, 5, 0), 10, 10)], FmodAudio.Preset.Plain);
    Snowy = new Map(MapPreset.snowy, 60, 60,
        [new TreeLine(new Vector2(-5, 30), new Vector2(5, 30), 3, [FruitSpecies.Normal, FruitSpecies.Energetic])],
        [], FmodAudio.Preset.Plain);
    UnderWater = new Map(MapPreset.underwater, 40, 40,
        [],
        [], FmodAudio.Preset.UnderWater);
    Beach = new Map(MapPreset.beach, 60, 40,
        [],
        [new WaterRectangle(WaterPreset.Sea, new Vector3(-30, 20, 0), 60, 20)], FmodAudio.Preset.Off);
    
    BasicRace = new Map(MapPreset.basicrace, Race.BASERACELENGTH, 10,
        [],
        [/*new WaterRectangle(position: new Vector3(0, -(Race.BASERACELENGTH / 2) - 10, 0))*/], FmodAudio.Preset.StoneCorridor);
    SnowyRace = new Map(MapPreset.snowyrace, Race.BASERACELENGTH, 10,
        [],
        [], FmodAudio.Preset.Plain);

    Maps =  new Dictionary<MapPreset, Map>{
      { MapPreset.garden, Garden },
      { MapPreset.snowy, Snowy },
      { MapPreset.underwater, UnderWater },
      { MapPreset.basicrace, BasicRace },
      { MapPreset.snowyrace, SnowyRace }
    };
  }

  private int SizeX { get; set; }
  private int SizeY { get; set; }
  public List<TreeLine> TreeLines { get; }
  public List<WaterRectangle> WaterPoints { get; } = [];
  public List<Item.Item> Items { get; set; } = new();
  public bool IsLullabyPlaying { get; set; } = false;
  public bool IsDansePlaying { get; set; } = false;
  public bool IsRaceMap => (this == BasicRace || this == SnowyRace);
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

  public Map(MapPreset preset, int sizeX, int sizeY, List<TreeLine> treeLines, List<WaterRectangle> waterPoints, ReverbProperties reverbPreset)
  {
    this.Preset = preset;
    SizeX = sizeX;
    SizeY = sizeY;
    TreeLines = treeLines;
    WaterPoints = waterPoints;
    TicketPopBehaviour = new(30000 * 60, 60000 * 60, true);
    SnowBallPopBehaviour = new(10000, 15000, this == Snowy);
    BubblePopBehaviour = new(10000, 15000, this == UnderWater);
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
    } while ((tryCounter <= 10 && excludeWater && IsInWater(randPos)) || GetTreeLineAtPosition(randPos) != null);
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
  public bool IsInWater(Vector3 position)
  {
    if (this == UnderWater)
    {
      return true;
    }
    else
    {
      foreach(var waterPoint in WaterPoints)
      {
        if(waterPoint.IsInRectangle(position))
        {
          return true;
        }
      }
    }
    return false;
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
    return (this == Garden || this == Snowy) && Util.IsInSquare(new Vector3(-SizeX / 2, -SizeY / 2, 0), position, 1);
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

  public override int GetHashCode()
  => Preset.GetHashCode();
}