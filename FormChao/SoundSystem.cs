using System.Diagnostics;
using System.Numerics;
using System.Timers;
using BebooGarden.GameCore;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using FmodAudio;
using FmodAudio.Base;
using FmodAudio.DigitalSignalProcessing;
using Timer = System.Timers.Timer;

namespace BebooGarden;

internal class SoundSystem
{
  public const string CONTENTFOLDER = "Content/";

  private static Timer? _ambiTimer;
  private readonly List<Task> Tasks = new();
  public Vector3 Up = new(0, 0, 1), Forward = new(0, 1, 0);

  public SoundSystem(float initialVolume = 0.5f)
  {
    //Creates the FmodSystem object
    System = Fmod.CreateSystem();
    //System object Initialization
    System.Init(4095, InitFlags._3D_RightHanded | InitFlags.Vol0_Becomes_Virtual);
    var advancedSettings = new AdvancedSettings
    {
      Vol0VirtualVol = 0.01f
    };
    System.SetAdvancedSettings(advancedSettings);
    System.Set3DSettings(1.0f, 1.0f, 1.0f);
    Volume = initialVolume;
    //Set the distance Units (Meters/Feet etc)
    System.Set3DListenerAttributes(0, new Vector3(0, 0, 0), default, Forward, Up);
    _ambiTimer = new Timer(2000);
    _ambiTimer.Elapsed += onAmbiTimer;
  }

  public FmodSystem System { get; }
  public List<Sound> AmbiSounds { get; set; } = new();

  public float Volume
  {
    get => System.MasterSoundGroup.GetValueOrDefault().Volume;
    set => System.MasterSoundGroup.GetValueOrDefault().Volume = value;
  }

  public List<Sound> BebooCuteSounds { get; private set; }
  public List<Sound> BebooSleepSounds { get; }
  public List<Sound> BebooYawningSounds { get; private set; }
  public List<Sound> BebooChewSounds { get; private set; }
  public Sound WhistleSound { get; set; }
  public Sound TreesShakeSound { get; private set; }
  public Sound WallSound { get; private set; }
  public List<Sound> BebooSleepingSounds { get; private set; }
  public Sound BebooStepSound { get; private set; }
  public Sound BebooStepWaterSound { get; private set; }
  public Sound BebooScreamSound { get; private set; }
  public Sound ItemPutSound { get; private set; }
  public Sound ItemTakeSound { get; private set; }
  public Sound ItemPutWaterSound { get; private set; }
  public Sound ItemDuckSound { get; private set; }
  public Sound ItemTicketPackSound { get; private set; }
  public List<Sound> EggKrakSounds { get; private set; }
  public Sound GrassSound { get; private set; }
  public Sound MenuBipSound { get; private set; }
  public Sound MenuKeySound { get; private set; }
  public Sound MenuKeyDeleteSound { get; private set; }
  public Sound MenuKeyFullSound { get; private set; }
  public Sound MenuReturnSound { get; private set; }
  public Sound WarningSound { get; private set; }
  public Sound MenuOkSound { get; private set; }
  public Sound MenuOk2Sound { get; private set; }
  public Sound WaterSound { get; private set; }
  public Sound TreeWindSound { get; private set; }
  public Sound JingleComplete { get; private set; }
  public Sound JingleStar { get; private set; }
  public List<Sound> BebooYumySounds { get; private set; }
  private SortedDictionary<FruitSpecies, Sound> FruitsSounds { get; set; }
  public Sound NeutralMusicStream { get; private set; }
  public Channel? Music { get; private set; }
  public Sound SadMusicStream { get; private set; }
  public Sound ShopMusicStream { get; private set; }
  public Sound SnowyMusicStream { get; private set; }
  public Sound RaceMusicStream { get; private set; }
  public Sound RaceStopSound { get; private set; }
  public Sound RaceGoodSound { get; private set; }
  public Sound RaceBadSound { get; private set; }
  public List<Sound> BebooPetSound { get; private set; }
  public List<Sound> BebooCrySounds { get; private set; }
  public List<Sound> BebooDelightSounds { get; private set; }
  public Sound JingleStar2 { get; private set; }
  public Sound JingleWaw { get; private set; }
  public Sound JingleLittleStar { get; private set; }
  public Sound UpSound { get; private set; }
  public Sound DownSound { get; private set; }
  public Sound ShopSound { get; private set; }
  public Sound CinematicHatch { get; private set; }
  public Sound CinematicElevator { get; private set; }
  public Sound CinematicRaceStart { get; private set; }
  public Sound CinematicRaceEnd { get; private set; }
  public List<Sound> BebooFunSounds { get; private set; }
  public Reverb3D Reverb { get; private set; }

  private void LoadSoundsInList(string[] files, List<Sound> sounds, string prefixe = "")
  {
    foreach (var file in files)
    {
      var sound = System.CreateSound(CONTENTFOLDER + prefixe + file,
          Mode._3D | Mode._3D_LinearSquareRolloff);
      sounds.Add(sound);
    }
  }

  public void LoadMainScreen(bool startMusic)
  {
    NeutralMusicStream = System.CreateStream(CONTENTFOLDER + "music/neutral.mp3", Mode.Loop_Normal);
    Music = System.PlaySound(NeutralMusicStream, paused: startMusic)!;
    Music.SetLoopPoints(TimeUnit.MS, 12, TimeUnit.MS, 88369);
    Music.Volume = 0.5f;
    SadMusicStream = System.CreateStream(CONTENTFOLDER + "music/Depressed.mp3", Mode.Loop_Normal);
    ShopMusicStream = System.CreateStream(CONTENTFOLDER + "music/Boutique.mp3", Mode.Loop_Normal);
    SnowyMusicStream = System.CreateStream(CONTENTFOLDER + "music/snowy.mp3", Mode.Loop_Normal);
    LoadRace();
    WaterSound = System.CreateStream(CONTENTFOLDER + "sounds/WaterCalmWide.wav",
        Mode.Loop_Normal | Mode._3D | Mode._3D_InverseTaperedRolloff);
    TreeWindSound = System.CreateStream(CONTENTFOLDER + "sounds/Wind_Trees_Cattails_Fienup_001.mp3",
        Mode.Loop_Normal | Mode._3D | Mode._3D_InverseTaperedRolloff);
    JingleComplete = System.CreateStream(CONTENTFOLDER + "music/coplete.wav");
    JingleStar = System.CreateStream(CONTENTFOLDER + "music/star.wav");
    JingleStar2 = System.CreateStream(CONTENTFOLDER + "music/star2.wav");
    JingleWaw = System.CreateStream(CONTENTFOLDER + "music/waw.wav");
    JingleLittleStar = System.CreateStream(CONTENTFOLDER + "music/LittleStar.wav");
    UpSound = System.CreateStream(CONTENTFOLDER + "sounds/menu/up.wav");
    DownSound = System.CreateStream(CONTENTFOLDER + "sounds/menu/down.wav");
    BebooCuteSounds = new List<Sound>();
    LoadSoundsInList(["ouou.wav", "ouou2.wav", "agougougou.wav"], BebooCuteSounds, "sounds/beboo/");
    BebooYawningSounds = new List<Sound>();
    LoadSoundsInList(["baille.wav", "baille2.wav"], BebooYawningSounds, "sounds/beboo/");
    BebooSleepingSounds = new List<Sound>();
    LoadSoundsInList(["ronfle.wav", "dodo.wav"], BebooSleepingSounds, "sounds/beboo/");
    BebooChewSounds = new List<Sound>();
    LoadSoundsInList(["crunch.wav", "crunch2.wav", "EatingApple.wav"], BebooChewSounds, "sounds/beboo/");
    BebooYumySounds = new List<Sound>();
    LoadSoundsInList(["miam.wav", "miam2.wav", "miam3.wav"], BebooYumySounds, "sounds/beboo/");
    BebooDelightSounds = new List<Sound>();
    LoadSoundsInList(["rourou.wav", "rourou2.wav", "rourou3.wav"], BebooDelightSounds, "sounds/beboo/");
    BebooPetSound = new List<Sound>();
    LoadSoundsInList(["pet.wav", "pet2.wav"], BebooPetSound, "sounds/character/");
    BebooCrySounds = new List<Sound>();
    LoadSoundsInList(["trist.wav", "trist2.wav"], BebooCrySounds, "sounds/beboo/");
    BebooFunSounds = new List<Sound>();
    LoadSoundsInList(["rir.wav", "rir2.wav"], BebooFunSounds, "sounds/beboo/");
    WhistleSound = System.CreateSound(CONTENTFOLDER + "sounds/character/se_sys_whistle_1p.wav", Mode.Unique);
    TreesShakeSound = System.CreateSound(CONTENTFOLDER + "sounds/character/Tree_Shake.wav");
    FruitsSounds = new SortedDictionary<FruitSpecies, Sound>
    {
      [FruitSpecies.Normal] = System.CreateSound(CONTENTFOLDER + "sounds/character/fruit.wav", Mode.Unique)
    };
    WallSound = System.CreateSound(CONTENTFOLDER + "sounds/wall.wav", Mode.Unique);
    BebooStepSound = System.CreateSound(CONTENTFOLDER + "sounds/beboo/step.wav",
        Mode._3D | Mode._3D_LinearSquareRolloff);
    BebooStepWaterSound = System.CreateSound(CONTENTFOLDER + "sounds/buble4.wav",
        Mode._3D | Mode._3D_LinearSquareRolloff);
    BebooScreamSound = System.CreateSound(CONTENTFOLDER + "sounds/beboo/cri.wav",
        Mode._3D | Mode._3D_LinearSquareRolloff);
    CinematicHatch = System.CreateStream(CONTENTFOLDER + "cinematic/hatch.wav");
    CinematicElevator = System.CreateStream(CONTENTFOLDER + "cinematic/elevator.mp3");
    CinematicRaceStart = System.CreateStream(CONTENTFOLDER + "cinematic/race.mp3");
    CinematicRaceEnd = System.CreateStream(CONTENTFOLDER + "cinematic/Return.mp3");
    LoadItemSound();
    GrassSound = System.CreateSound(CONTENTFOLDER + "sounds/grass_rustle.wav",
        Mode._3D | Mode._3D_LinearSquareRolloff | Mode.Unique);
    LoadMenuSounds();
  }

  private void LoadRace()
  {
    RaceMusicStream = System.CreateStream(CONTENTFOLDER + "music/race.mp3", Mode.Loop_Normal);
    RaceStopSound = System.CreateSound(CONTENTFOLDER + "sounds/race/stop.mp3",
        Mode._3D | Mode._3D_LinearSquareRolloff);
    RaceGoodSound = System.CreateSound(CONTENTFOLDER + "sounds/race/good.wav",
        Mode._3D | Mode._3D_LinearSquareRolloff);
    RaceBadSound = System.CreateSound(CONTENTFOLDER + "sounds/race/bad.wav",
        Mode._3D | Mode._3D_LinearSquareRolloff);
  }

  private void LoadItemSound()
  {
    ItemPutSound = System.CreateSound(CONTENTFOLDER + "sounds/character/item.wav", Mode.Unique);
    ItemTakeSound = System.CreateSound(CONTENTFOLDER + "sounds/pwik.wav", Mode.Unique);
    ItemPutWaterSound = System.CreateSound(CONTENTFOLDER + "sounds/character/itemwaterfall.wav", Mode.Unique);
    ItemDuckSound = System.CreateSound(CONTENTFOLDER + "sounds/kwak.wav", Mode._3D | Mode._3D_LinearSquareRolloff | Mode.Unique);
    ItemTicketPackSound = System.CreateSound(CONTENTFOLDER + "sounds/ticket.wav", Mode._3D | Mode._3D_LinearSquareRolloff | Mode.Unique);
    EggKrakSounds = new List<Sound>();
    LoadSoundsInList(["krak.wav", "krak2.wav"], EggKrakSounds, "sounds/egg/");
  }

  public void LoadMenuSounds()
  {
    MenuBipSound = System.CreateSound(CONTENTFOLDER + "sounds/menu/boup.wav", Mode.Unique);
    MenuOkSound = System.CreateSound(CONTENTFOLDER + "sounds/menu/ok.wav", Mode.Unique);
    MenuOk2Sound = System.CreateSound(CONTENTFOLDER + "sounds/menu/ok2.wav", Mode.Unique);
    MenuKeySound = System.CreateSound(CONTENTFOLDER + "sounds/menu/key.wav", Mode.Unique);
    MenuKeyDeleteSound = System.CreateSound(CONTENTFOLDER + "sounds/menu/keydelete.wav", Mode.Unique);
    MenuKeyFullSound = System.CreateSound(CONTENTFOLDER + "sounds/menu/keyfull.wav", Mode.Unique);
    MenuReturnSound = System.CreateSound(CONTENTFOLDER + "sounds/menu/return.wav", Mode.Unique);
    WarningSound = System.CreateSound(CONTENTFOLDER + "sounds/menu/warn.wav", Mode.Unique);
    ShopSound = System.CreateStream(CONTENTFOLDER + "sounds/menu/shop.wav");
  }

  public void LoadMap(Map map)
  {
    Channel waterChannel = System.PlaySound(WaterSound, paused: true)!;
    waterChannel.SetLoopPoints(TimeUnit.MS, 2780, TimeUnit.MS, 17796);
    waterChannel.Set3DAttributes(map.WaterPoint + new Vector3(0, 0, 10), default, default);
    waterChannel.Set3DMinMaxDistance(30f, 35f);
    waterChannel.Volume = 0.1f;
    waterChannel.Paused = false;
    map.WaterChannels.Clear();
    map.WaterChannels.Add(waterChannel);
    if (map.TreeLines.Count > 0)
    {
      Channel treeChannel = System.PlaySound(TreeWindSound, paused: true)!;
      treeChannel.SetLoopPoints(TimeUnit.PCM, 668725, TimeUnit.PCM, 2961327);
      var treePosVector2 = map.TreeLines[0].X;
      var treePosVector3 = new Vector3(treePosVector2.X, treePosVector2.Y, 10);
      treeChannel.Set3DAttributes(treePosVector3, default, default);
      treeChannel.Set3DMinMaxDistance(60f, 60f);
      treeChannel.Set3DConeSettings(200, 2001, 0.3f);
      treeChannel.Set3DConeOrientation(new Vector3(1, 0, 0));
      treeChannel.Volume = 0.2f;
      treeChannel.Paused = false;
      map.TreesChannels.Clear();
      map.TreesChannels.Add(treeChannel);
    }
    //winter: 1922069 6508548
    Sound sound = System.CreateStream(CONTENTFOLDER + "sounds/Grass_Shake.wav");
    map.BackgroundChannel = System.PlaySound(sound, paused: false)!;
    map.BackgroundChannel.SetLoopPoints(TimeUnit.MS, 678, TimeUnit.MS, 6007);
    map.BackgroundChannel.Volume = 0.5f;
    LoadAmbiSounds();
    foreach (var item in map.Items) item.SoundLoopTimer?.Start();
    Reverb = System.CreateReverb3D();
    Reverb.SetProperties(map.ReverbPreset);
    Reverb.Set3DAttributes(new Vector3(0, 0, 0), 0f, 500f);
  }

  public void LoadAmbiSounds()
  {
    var files = Directory.GetFiles(CONTENTFOLDER + "Sounds/birds/", "*.*");
    foreach (var file in files)
    {
      var sound = System.CreateSound(file, Mode._3D | Mode._3D_LinearSquareRolloff);
      AmbiSounds.Add(sound);
    }

    if (_ambiTimer != null) _ambiTimer.Enabled = true;
  }

  private void onAmbiTimer(object? sender, ElapsedEventArgs e)
  {
    var sound = AmbiSounds[Game.Random.Next(AmbiSounds.Count())];
    Channel channel = System.PlaySound(sound, paused: false)!;
    channel.Set3DAttributes(new Vector3(Game.Random.Next(-20, 20), Game.Random.Next(-20, 20), 5f), default, default);
    channel.Set3DMinMaxDistance(0, 35);
    _ambiTimer?.Dispose();
    _ambiTimer = new Timer(Game.Random.Next(4000, 8000));
    _ambiTimer.Elapsed += onAmbiTimer;
    _ambiTimer.Enabled = true;
  }

  public void PlayQueue(Sound sound, bool queued = true)
  {
    if (queued)
      Tasks.Add(Task.Factory.StartNew(() =>
      {
        int real;
        do
        {
          System.GetChannelsPlaying(out _, out real);
        } while (real > 2);

        Channel? channel = System.PlaySound(sound, paused: false);
        if (channel != null)
        {
          while (channel.IsPlaying) Thread.Sleep(5);
          try
          {
            channel.Stop();
          }
          catch
          {
          }
        }
      }));
    else
      Tasks.Add(Task.Factory.StartNew(() =>
      {
        Channel? channel = System.PlaySound(sound, paused: false);
        if (channel != null)
        {
          while (channel.IsPlaying) Thread.Sleep(5);
          channel.Stop();
        }
      }));
  }

  public void MovePlayerTo(Vector3 newPos)
  {
    System.Set3DListenerAttributes(0, newPos, default, in Forward, in Up);
  }

  public void PlayBebooSound(Sound sound, Beboo beboo, bool stopOthers = true)
  {
    if (beboo.Channel != null && stopOthers && beboo.Channel.IsPlaying) beboo.Channel.Stop();
    beboo.Channel = PlaySoundAtPosition(sound, beboo.Position);
    beboo.Channel.AddDSP(0, beboo.VoiceDsp);
  }

  public void PlayBebooSound(List<Sound> sounds, Beboo beboo, bool stopOthers = true, float volume = -1)
  {
    var sound = sounds[Game.Random.Next(sounds.Count())];
    if (beboo.Channel != null && stopOthers && beboo.Channel.IsPlaying) beboo.Channel.Stop();
    beboo.Channel = PlaySoundAtPosition(sound, beboo.Position);
    beboo.Channel.AddDSP(0, beboo.VoiceDsp);
    if (volume != -1) beboo.Channel.Volume = volume;
  }

  public Channel PlaySoundAtPosition(Sound sound, Vector3 position)
  {
    Channel channel = System.PlaySound(sound, paused: true)!;
    channel.Set3DMinMaxDistance(0f, 30f);
    channel.Set3DAttributes(position + new Vector3(0, 0, -2), default, default);
    channel.Paused = false;
    return channel;
  }

  public void Whistle()
  {
    System.PlaySound(WhistleSound);
  }

  public void ShakeTrees()
  {
    System.PlaySound(TreesShakeSound);
  }

  internal void WallBouncing()
  {
    System.PlaySound(WallSound);
  }

  internal void DropFruitSound(FruitSpecies fruitSpecies)
  {
    System.PlaySound(FruitsSounds[fruitSpecies]);
  }

  public void MusicTransition(Sound music, uint startLoop, uint endLoop, TimeUnit timeUnit, float volume = 0.5f)
  {
    var mute = Music?.Mute ?? true;
    Music?.Stop();
    Music = System.PlaySound(music, paused: false)!;
    if (endLoop != 0) Music?.SetLoopPoints(timeUnit, startLoop, timeUnit, endLoop);
    Music.Volume = volume;
    Music.Mute = mute;
  }

  internal Channel PlaySoundAtPosition(List<Sound> sounds, Vector3 position)
  {
    var sound = sounds[Game.Random.Next(sounds.Count())];
    var channel = PlaySoundAtPosition(sound, position);
    return channel;
  }

  public void DisableAmbiTimer() => _ambiTimer.Stop();
  public void EnableAmbiTimer() => _ambiTimer.Start();

  public void Pause(Map map)
  {
    foreach (var channel in map.TreesChannels) channel.Paused = true;
    foreach (var channel in map.WaterChannels) channel.Paused = true;
    foreach (var item in map.Items) item.SoundLoopTimer?.Stop();
    //if (map != null) map.BackgroundChannel.Paused = true;
  }

  public void Unpause(Map map)
  {
    try
    {
      foreach (var channel in map.TreesChannels) channel.Paused = false;
      foreach (var channel in map.WaterChannels) channel.Paused = false;
      foreach (var item in map.Items) item.SoundLoopTimer?.Start();
      //if (map.BackgroundChannel != null) map.BackgroundChannel.Paused = false;
    }
    catch
    {
      LoadMap(map);
    }
  }
  public void PlayCinematic(Sound sound, bool pauseMusic = true)
  {
    Game.GameWindow?.DisableInput();
    Music.Paused = pauseMusic;
    Channel channel = System.PlaySound(sound)!;
    while (channel.IsPlaying)
    {
    }

    Music.Paused = false;
    Game.GameWindow?.EnableInput();
  }

  internal void PlaySadMusic()
  {
    MusicTransition(Game.SoundSystem.SadMusicStream, 464375, 4471817, TimeUnit.PCM, 0.1f);
  }

  internal void PlayNeutralMusic()
  {
    Game.SoundSystem.MusicTransition(Game.SoundSystem.NeutralMusicStream, 12, 88369, TimeUnit.MS);
  }

  internal void PlayShopMusic()
  {
    Game.SoundSystem.MusicTransition(Game.SoundSystem.ShopMusicStream, 459264, 8156722, FmodAudio.TimeUnit.PCM);
  }

  internal void PlayRaceMusic()
  {
    Game.SoundSystem.MusicTransition(Game.SoundSystem.RaceMusicStream, 0, 0, FmodAudio.TimeUnit.PCM);
  }
  public void PlayMapMusic(Map map)
  {
    if (Game.Map == null || Game.Beboos[0] == null) return;
    if (Game.Beboos[0].Happy)
    {
      switch (map.Preset)
      {
        case MapPresets.garden: PlayNeutralMusic(); break; break;
        default: PlayNeutralMusic(); break;
      }
    } else PlaySadMusic();
  }
}