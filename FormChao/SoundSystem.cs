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
    AdvancedSettings advancedSettings = new()
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
  public Sound BebooStepSnowSound { get; private set; }
  public Sound BebooStepSlipSound { get; set; }
  public Sound BebooScreamSound { get; private set; }
  public Sound ItemPutSound { get; private set; }
  public Sound ItemTakeSound { get; private set; }
  public Sound ItemPutWaterSound { get; private set; }
  public Sound ItemDuckSound { get; private set; }
  public Sound ItemTicketPackSound { get; private set; }
  public Sound ItemSnowBallKickSound { get; private set; }
  public List<Sound> EggKrakSounds { get; private set; }
  public Sound GrassSound { get; private set; }
  public Sound UnderWaterSound { get; private set; }
  public Sound ColdWindSound { get; private set; }
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
  public Sound UnderWaterMusicStream { get; private set; }
  public Sound RaceMusicStream { get; private set; }
  public Sound RaceLolMusicStream { get; private set; }
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
  public List<Sound> BoingSounds { get; private set; }
  public List<Sound> BubbleSounds { get; private set; }
  public Sound BubblePopSound { get; private set; }
  public Sound BubbleUpSound { get; private set; }

  private void LoadSoundsInList(string[] files, List<Sound> sounds, string prefixe = "")
  {
    foreach (string file in files)
    {
      Sound sound = System.CreateSound(CONTENTFOLDER + prefixe + file,
          Mode._3D | Mode._3D_LinearSquareRolloff);
      sounds.Add(sound);
    }
  }

  public void LoadMainScreen(bool startMusic)
  {
    NeutralMusicStream = System.CreateStream(CONTENTFOLDER + "music/neutral.mp3", Mode.Loop_Normal);
    SadMusicStream = System.CreateStream(CONTENTFOLDER + "music/Depressed.mp3", Mode.Loop_Normal);
    ShopMusicStream = System.CreateStream(CONTENTFOLDER + "music/Boutique.mp3", Mode.Loop_Normal);
    SnowyMusicStream = System.CreateStream(CONTENTFOLDER + "music/snowy.mp3", Mode.Loop_Normal);
    UnderWaterMusicStream = System.CreateStream(CONTENTFOLDER + "music/Aquatic.mp3", Mode.Loop_Normal);
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
      [FruitSpecies.Normal] = System.CreateSound(CONTENTFOLDER + "sounds/character/fruit.wav", Mode.Unique),
      [FruitSpecies.Energetic] = JingleLittleStar
    };
    WallSound = System.CreateSound(CONTENTFOLDER + "sounds/wall.wav", Mode._3D | Mode._3D_InverseTaperedRolloff);
    BebooStepSound = System.CreateSound(CONTENTFOLDER + "sounds/beboo/step.wav",
        Mode._3D | Mode._3D_LinearSquareRolloff);
    BebooStepWaterSound = System.CreateSound(CONTENTFOLDER + "sounds/bubble4.wav",
        Mode._3D | Mode._3D_LinearSquareRolloff);
    BebooStepSnowSound = System.CreateSound(CONTENTFOLDER + "sounds/snow/step.wav",
        Mode._3D | Mode._3D_LinearSquareRolloff);
    BebooStepSlipSound = System.CreateSound(CONTENTFOLDER + "sounds/slip.wav",
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
    ColdWindSound = System.CreateSound(CONTENTFOLDER + "sounds/snow/winter_day.wav",
    Mode.Loop_Normal | Mode.Unique);
    UnderWaterSound = System.CreateSound(CONTENTFOLDER + "sounds/underwater_ambience.ogg",
        Mode.Loop_Normal | Mode.Unique);
    LoadMenuSounds();
    Reverb3D reverb = System.CreateReverb3D();
    System.SetReverbProperties(1, Preset.Off);
    reverb.Set3DAttributes(new Vector3(0, 0, 0), 0f, 500f);
  }

  private void LoadRace()
  {
    RaceMusicStream = System.CreateStream(CONTENTFOLDER + "music/race.mp3", Mode.Loop_Normal);
    RaceLolMusicStream = System.CreateStream(CONTENTFOLDER + "music/race2.mp3", Mode.Loop_Normal);
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
    ItemSnowBallKickSound = System.CreateSound(CONTENTFOLDER + "sounds/character/snowballkick.wav", Mode._3D | Mode._3D_LinearSquareRolloff);
    EggKrakSounds = [];
    LoadSoundsInList(["krak.wav", "krak2.wav"], EggKrakSounds, "sounds/egg/");
    BoingSounds = new List<Sound>();
    LoadSoundsInList(["boing2.wav", "boing.wav"], BoingSounds, "sounds/");
    BubbleSounds = new List<Sound>();
    LoadSoundsInList(["bubble.wav", "bubble2.wav", "bubble3.wav"], BubbleSounds, "sounds/");
    BubblePopSound = System.CreateSound(CONTENTFOLDER + "sounds/bubblepop.wav", Mode._3D | Mode._3D_LinearSquareRolloff | Mode.Unique);
    BubbleUpSound = System.CreateSound(CONTENTFOLDER + "sounds/bubbleup.wav", Mode._3D | Mode._3D_LinearSquareRolloff | Mode.Unique);
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
    if (map.Preset == MapPreset.underwater)
    {
      map.WaterChannels.Clear();
    }
    else if (map.WaterPoint != null)
    {
      Channel waterChannel = System.PlaySound(WaterSound, paused: true)!;
      waterChannel.SetLoopPoints(TimeUnit.MS, 2780, TimeUnit.MS, 17796);
      waterChannel.Set3DAttributes(map.WaterPoint.Value + new Vector3(0, 0, 10), default, default);
      waterChannel.Set3DMinMaxDistance(30f, 35f);
      waterChannel.Volume = 0.1f;
      waterChannel.Paused = false;
      map.WaterChannels.Clear();
      map.WaterChannels.Add(waterChannel);
    }
    if (map.TreeLines.Count > 0)
    {
      Channel treeChannel = System.PlaySound(TreeWindSound, paused: true)!;
      treeChannel.SetLoopPoints(TimeUnit.PCM, 668725, TimeUnit.PCM, 2961327);
      Vector2 treePosVector2 = map.TreeLines[0].X;
      Vector3 treePosVector3 = new(treePosVector2.X, treePosVector2.Y, 10);
      treeChannel.Set3DAttributes(treePosVector3, default, default);
      treeChannel.Set3DMinMaxDistance(80f, 80f);
      treeChannel.Set3DConeSettings(200, 2001, 0.3f);
      treeChannel.Set3DConeOrientation(new Vector3(1, 0, 0));
      treeChannel.Volume = 0.2f;
      treeChannel.Paused = false;
      map.TreesChannels.Clear();
      map.TreesChannels.Add(treeChannel);
    }
    switch (map.Preset)
    {
      case MapPreset.garden:
        Sound sound = System.CreateStream(CONTENTFOLDER + "sounds/Grass_Shake.wav");
        map.BackgroundChannel = System.PlaySound(sound, paused: false)!;
        map.BackgroundChannel.SetLoopPoints(TimeUnit.MS, 678, TimeUnit.MS, 6007);
        map.BackgroundChannel.Volume = 0.5f;
        LoadAmbiSounds();
        break;
      case MapPreset.snowy:
        map.BackgroundChannel = System.PlaySound(ColdWindSound, paused: false)!;
        map.BackgroundChannel.Volume = 1f;
        _ambiTimer.Enabled = false;
        break;
      case MapPreset.underwater:
        map.BackgroundChannel = System.PlaySound(UnderWaterSound, paused: false)!;
        map.BackgroundChannel.SetLoopPoints(TimeUnit.PCM, 85488, TimeUnit.PCM, 1265135);
        map.BackgroundChannel.Volume = 0.5f;
        _ambiTimer.Enabled = false;
        break;
    }
    foreach (GameCore.Item.Item item in map.Items) item.Unpause();
    foreach (Beboo beboo in map.Beboos) beboo.Unpause();
    System.SetReverbProperties(1, map.ReverbPreset);
  }

  public void LoadAmbiSounds()
  {
    string[] files = Directory.GetFiles(CONTENTFOLDER + "Sounds/birds/", "*.*");
    foreach (string file in files)
    {
      Sound sound = System.CreateSound(file, Mode._3D | Mode._3D_LinearSquareRolloff);
      AmbiSounds.Add(sound);
    }

    if (_ambiTimer != null) _ambiTimer.Enabled = true;
  }

  private void onAmbiTimer(object? sender, ElapsedEventArgs e)
  {
    Sound sound = AmbiSounds[Game.Random.Next(AmbiSounds.Count())];
    Channel channel = System.PlaySound(sound, paused: false)!;
    channel.Set3DAttributes(new Vector3(Game.Random.Next(-20, 20), Game.Random.Next(-20, 20), 5f), default, default);
    channel.Set3DMinMaxDistance(0, 35);
    bool enabled = _ambiTimer.Enabled;
    _ambiTimer?.Dispose();
    _ambiTimer = new Timer(Game.Random.Next(4000, 8000));
    _ambiTimer.Elapsed += onAmbiTimer;
    _ambiTimer.Enabled = enabled;
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
    beboo.Channel = PlaySoundAtPosition(sound, beboo.Position, 0, beboo.VoiceDsp);
  }

  public void PlayBebooSound(List<Sound> sounds, Beboo beboo, bool stopOthers = true, float volume = -1)
  {
    Sound sound = sounds[Game.Random.Next(sounds.Count())];
    if (beboo.Channel != null && stopOthers && beboo.Channel.IsPlaying) beboo.Channel.Stop();
    beboo.Channel = PlaySoundAtPosition(sound, beboo.Position, 0, beboo.VoiceDsp);
    if (volume != -1) beboo.Channel.Volume = volume;
  }

  public Channel PlaySoundAtPosition(Sound sound, Vector3 position, float volumeModifier = 0, Dsp? pitchDsp = null)
  {
    Channel channel = System.PlaySound(sound, paused: true)!;
    channel.Set3DMinMaxDistance(0f, 30f);
    channel.Set3DAttributes(position + new Vector3(0, 0, -2), default, default);
    channel.Volume += volumeModifier;
    //if (pitchDsp != null) channel.AddDSP(0, pitchDsp.Value);
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

  internal void DropFruitSound(FruitSpecies fruitSpecies)
  {
    System.PlaySound(FruitsSounds[fruitSpecies]);
  }

  public void MusicTransition(Sound music, uint startLoop, uint endLoop, TimeUnit timeUnit, float volume = 0.5f)
  {
    bool mute = Music?.Mute ?? false;
    Music?.Stop();
    Music = System.PlaySound(music, paused: false)!;
    if (endLoop != 0) Music?.SetLoopPoints(timeUnit, startLoop, timeUnit, endLoop);
    if (Music != null) Music.Volume = volume;
    if (Music != null) Music.Mute = mute;
  }

  public Channel PlaySoundAtPosition(List<Sound> sounds, Vector3 position, float volumeModifier = 0, Dsp? pitchDsp = null)
  {
    Sound sound = sounds[Game.Random.Next(sounds.Count())];
    Channel channel = PlaySoundAtPosition(sound, position, 0, pitchDsp);
    //if (pitchDsp != null) channel.AddDSP(0, pitchDsp.Value);
    channel.Volume += volumeModifier;
    return channel;
  }

  public void DisableAmbiTimer() => _ambiTimer.Stop();
  public void EnableAmbiTimer() => _ambiTimer.Start();

  public void Pause(Map map)
  {
    foreach (Channel channel in map.TreesChannels) channel.Paused = true;
    foreach (Channel channel in map.WaterChannels) channel.Paused = true;
    foreach (GameCore.Item.Item item in map.Items) item.Pause();
    try { if (map != null && map.BackgroundChannel != null) map.BackgroundChannel.Paused = true; } catch { }
    foreach (Beboo beboo in map.Beboos) beboo.Pause();
    DisableAmbiTimer();
  }

  public void Unpause(Map map)
  {
    System.SetReverbProperties(1, Preset.Off);
    try
    {
      foreach (Channel channel in map.TreesChannels) channel.Paused = false;
      foreach (Channel channel in map.WaterChannels) channel.Paused = false;
      foreach (GameCore.Item.Item item in map.Items) item.Unpause();
      if (map.BackgroundChannel != null) map.BackgroundChannel.Paused = false;
      if (map.Preset == MapPreset.garden) EnableAmbiTimer();
    }
    catch
    {
      LoadMap(map);
    }
    foreach (Beboo beboo in map.Beboos) beboo.Unpause();
  }
  public void PlayCinematic(Sound sound, bool pauseMusic = true)
  {
    Game.GameWindow?.DisableInput();
    if (Music != null) Music.Paused = pauseMusic;
    Channel channel = System.PlaySound(sound)!;
    while (channel.IsPlaying)
    {
    }

    if (Music != null) Music.Paused = false;
    Game.GameWindow?.EnableInput();
  }

  internal void PlaySadMusic()
  {
    MusicTransition(Game.SoundSystem.SadMusicStream, 464375, 4471817, TimeUnit.PCM, 0.1f);
  }

  internal void PlayNeutralMusic()
  {
    MusicTransition(NeutralMusicStream, 12, 88369, TimeUnit.MS);
  }

  internal void PlayShopMusic()
  {
    MusicTransition(ShopMusicStream, 459264, 8156722, FmodAudio.TimeUnit.PCM);
  }

  internal void PlayRaceMusic()
  {
    MusicTransition(RaceMusicStream, 0, 0, FmodAudio.TimeUnit.PCM);
  }
  internal void PlayRaceLolMusic()
  {
    MusicTransition(RaceLolMusicStream, 0, 0, FmodAudio.TimeUnit.PCM);
  }
  internal void PlaySnowyMusic()
  {
    MusicTransition(SnowyMusicStream, 1922069, 6508548, FmodAudio.TimeUnit.PCM);
  }
  internal void PlayUnderWaterMusic()
  {
    MusicTransition(UnderWaterMusicStream, 2684920, 6164501, FmodAudio.TimeUnit.PCM, 0.3f);
  }
  public void PlayMapMusic(Map map)
  {
    if (map.Beboos.Count == 0 && Music == null) return;
    bool everyoneHappy = true;
    foreach (Beboo beboo in map.Beboos)
    {
      if (everyoneHappy) everyoneHappy = beboo.Happy;
    }
    if (everyoneHappy)
    {
      switch (map.Preset)
      {
        case MapPreset.garden: PlayNeutralMusic(); break;
        case MapPreset.snowy: PlaySnowyMusic(); break;
        case MapPreset.underwater: PlayUnderWaterMusic(); break;
        default: PlayNeutralMusic(); break;
      }
    }
    else PlaySadMusic();
  }
}