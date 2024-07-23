using System.Numerics;
using System.Timers;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using FmodAudio;
using FmodAudio.Base;

namespace BebooGarden;
internal class SoundSystem
{
  public const string CONTENTFOLDER = "Content/";
  public FmodSystem System { get; }
  public List<Sound> AmbiSounds { get; set; }
  public Vector3 Up = new Vector3(0, 0, 1), Forward = new Vector3(0, 1, 0);
  public float Volume { get { return System.MasterSoundGroup.GetValueOrDefault().Volume; } set { System.MasterSoundGroup.GetValueOrDefault().Volume = value; } }
  public List<Sound> BebooCuteSounds { get; private set; }
  public List<Sound> BebooSleepSounds { get; private set; }
  public List<Sound> BebooYawningSounds { get; private set; }
  public List<Sound> BebooChewSounds { get; private set; }
  public Sound WhistleSound { get; set; }
  public Sound TreesShakeSound { get; private set; }
  public Sound WallSound { get; private set; }
  public List<Sound> BebooSleepingSounds { get; private set; }
  public Sound BebooStepSound { get; private set; }
  public Sound ItemPutSound { get; private set; }
  public Sound ItemTakeSound { get; private set; }
  public List<Sound> EggKrakSounds { get; private set; }
  public Sound GrassSound { get; private set; }
  public Sound MenuBipSound { get; private set; }
  public Sound MenuKeySound { get; private set; }
  public Sound MenuKeyDeleteSound { get; private set; }
  public Sound MenuKeyFullSound { get; private set; }
  public Sound MenuOkSound { get; private set; }
  public Sound WaterSound { get; private set; }
  public Sound TreeWindSound { get; private set; }
  public Sound JingleComplete { get; private set; }
  public Sound JingleStar { get; private set; }
  public List<Sound> BebooYumySounds { get; private set; }
  private SortedDictionary<FruitSpecies, Sound> FruitsSounds { get; set; }
  public Sound NeutralMusicStream { get; private set; }
  public Channel Music { get; private set; }
  public Sound SadMusicStream { get; private set; }
  public List<Sound> BebooPetSound { get; private set; }
  public List<Sound> BebooCrySounds { get; private set; }
  public Channel BebooChannel { get; private set; }
  public List<Sound> BebooDelightSounds { get; private set; }
  public Sound JingleStar2 { get; private set; }
  public Sound JingleWaw { get; private set; }
  public Sound JingleLittleStar { get; private set; }
  public Sound UpSound { get; private set; }
  public Sound DownSound { get; private set; }

  private static System.Timers.Timer AmbiTimer;

  public SoundSystem(float initialVolume=0.5f)
  {
    //Creates the FmodSystem object
    System = FmodAudio.Fmod.CreateSystem();
    //System object Initialization
    System.Init(4095, InitFlags._3D_RightHanded | InitFlags.Vol0_Becomes_Virtual);
    var advancedSettings = new AdvancedSettings();
    advancedSettings.Vol0VirtualVol = 0.01f;
    System.SetAdvancedSettings(advancedSettings);
    System.Set3DSettings(1.0f, 1.0f, 1.0f);
    Volume = initialVolume;
    //Set the distance Units (Meters/Feet etc)
    System.Set3DListenerAttributes(0, new Vector3(0, 0, 0), default, Forward, Up);
    AmbiTimer = new System.Timers.Timer(2000);
    AmbiTimer.Elapsed += onAmbiTimer;
  }
  public void LoadSoundsInList(string[] files, List<Sound> sounds, string prefixe = "")
  {
    foreach (var file in files)
    {
      var sound = System.CreateSound(CONTENTFOLDER + prefixe + file, Mode._3D | Mode._3D_LinearSquareRolloff | Mode.Unique);
      sounds.Add(sound);
    }
  }
  public void LoadMainScreen()
  {
    Sound sound; Channel channel;
    NeutralMusicStream = System.CreateStream(CONTENTFOLDER + "music/neutral.mp3", Mode.Loop_Normal);
    Music = (Channel?)System.PlaySound(NeutralMusicStream, paused: false);
    Music.SetLoopPoints(TimeUnit.MS, 12, TimeUnit.MS, 88369);
    Music.Volume = 0.5f;
    SadMusicStream = System.CreateStream(CONTENTFOLDER + "music/Depressed.mp3", Mode.Loop_Normal);
    WaterSound = System.CreateStream(CONTENTFOLDER + "sounds/WaterCalmWide.wav", Mode.Loop_Normal | Mode._3D | Mode._3D_InverseTaperedRolloff);
    TreeWindSound = System.CreateStream(CONTENTFOLDER + "sounds/Wind_Trees_Cattails_Fienup_001.mp3", Mode.Loop_Normal | Mode._3D | Mode._3D_InverseTaperedRolloff);
    JingleComplete = System.CreateStream(CONTENTFOLDER + "music/coplete.wav");
    JingleStar = System.CreateStream(CONTENTFOLDER + "music/star.wav");
    JingleStar2 = System.CreateStream(CONTENTFOLDER + "music/star2.wav");
    JingleWaw = System.CreateStream(CONTENTFOLDER + "music/waw.wav");
    JingleLittleStar = System.CreateStream(CONTENTFOLDER + "music/LittleStar.wav");
    UpSound = System.CreateStream(CONTENTFOLDER + "sounds/menu/up.wav");
    DownSound = System.CreateStream(CONTENTFOLDER + "sounds/menu/down.wav");
    sound = System.CreateStream(CONTENTFOLDER + "sounds/Grass_Shake.wav");
    channel = (Channel?)System.PlaySound(sound, paused: false);
    channel.SetLoopPoints(TimeUnit.MS, 678, TimeUnit.MS, 6007);
    channel.Volume = 0.5f;
    AmbiSounds = new();
    LoadAmbiSounds();
    BebooCuteSounds = new();
    LoadSoundsInList(["ouou.wav", "ouou2.wav", "agougougou.wav"], BebooCuteSounds, "sounds/beboo/");
    BebooYawningSounds = new();
    LoadSoundsInList(["baille.wav", "baille2.wav"], BebooYawningSounds, "sounds/beboo/");
    BebooSleepingSounds = new();
    LoadSoundsInList(["ronfle.wav", "dodo.wav"], BebooSleepingSounds, "sounds/beboo/");
    BebooChewSounds = new();
    LoadSoundsInList(["crunch.wav", "crunch2.wav", "EatingApple.wav"], BebooChewSounds, "sounds/beboo/");
    BebooYumySounds = new();
    LoadSoundsInList(["miam.wav", "miam2.wav", "miam3.wav"], BebooYumySounds, "sounds/beboo/");
    BebooDelightSounds = new();
    LoadSoundsInList(["rourou.wav", "rourou2.wav", "rourou3.wav"], BebooDelightSounds, "sounds/beboo/");
    BebooPetSound = new();
    LoadSoundsInList(["pet.wav", "pet2.wav"], BebooPetSound, "sounds/character/");
    BebooCrySounds = new();
    LoadSoundsInList(["trist.wav", "trist2.wav"], BebooCrySounds, "sounds/beboo/");
    WhistleSound = System.CreateSound(CONTENTFOLDER + "sounds/character/se_sys_whistle_1p.wav", Mode.Unique);
    TreesShakeSound = System.CreateSound(CONTENTFOLDER + "sounds/character/Tree_Shake.wav");
    FruitsSounds = new();
    FruitsSounds[FruitSpecies.Normal] = System.CreateSound(CONTENTFOLDER + "sounds/character/fruit.wav", Mode.Unique);
    WallSound = System.CreateSound(CONTENTFOLDER + "sounds/wall.wav", Mode.Unique);
    BebooStepSound = System.CreateSound(CONTENTFOLDER + "sounds/beboo/step.wav", Mode._3D | Mode._3D_LinearSquareRolloff | Mode.Unique);
    LoadItemSound();
    GrassSound = System.CreateSound(CONTENTFOLDER + "sounds/grass_rustle.wav", Mode._3D | Mode._3D_LinearSquareRolloff | Mode.Unique);
    LoadMenuSounds();
    Reverb3D reverb = System.CreateReverb3D();
    reverb.SetProperties(Preset.Plain);
    reverb.Set3DAttributes(new Vector3(0, 0, 0), 0f, 500f);
  }

  private void LoadItemSound()
  {
    ItemPutSound = System.CreateSound(CONTENTFOLDER + "sounds/character/item.wav", Mode.Unique);
    ItemTakeSound = System.CreateSound(CONTENTFOLDER + "sounds/pwik.wav", Mode.Unique);
    EggKrakSounds = new();
    LoadSoundsInList(["krak.wav", "krak2.wav"], EggKrakSounds, "sounds/egg/");
  }

  public void LoadMenuSounds()
  {
    MenuBipSound = System.CreateSound(CONTENTFOLDER + "sounds/menu/boup.wav", Mode.Unique);
    MenuOkSound = System.CreateSound(CONTENTFOLDER + "sounds/menu/ok.wav", Mode.Unique);
    MenuKeySound = System.CreateSound(CONTENTFOLDER + "sounds/menu/key.wav", Mode.Unique);
    MenuKeyDeleteSound = System.CreateSound(CONTENTFOLDER + "sounds/menu/keydelete.wav", Mode.Unique);
    MenuKeyFullSound = System.CreateSound(CONTENTFOLDER + "sounds/menu/keyfull.wav", Mode.Unique);
  }

  public void LoadMap(Map map)
  {
    Channel channel = System.PlaySound(WaterSound, paused: true);
    channel.SetLoopPoints(TimeUnit.MS, 2780, TimeUnit.MS, 17796);
    channel.Set3DAttributes(map.WaterPoint+new Vector3(0,0,10), default, default);
    channel.Set3DMinMaxDistance(30f, 35f);
    channel.Volume = 0.1f;
    channel.Paused = false;
    channel = System.PlaySound(TreeWindSound, paused: true);
    channel.SetLoopPoints(TimeUnit.PCM, 668725, TimeUnit.PCM, 2961327);
    var treePosVector2 = map.TreeLines[0].X;
    var treePosVector3 = new Vector3(treePosVector2.X, treePosVector2.Y, 10);
    channel.Set3DAttributes(treePosVector3, default, default);
    channel.Set3DMinMaxDistance(60f, 60f);
    channel.Set3DConeSettings(200, 2001, 0.3f);
    channel.Set3DConeOrientation(new Vector3(1, 0, 0));
    channel.Volume = 0.2f;
    channel.Paused = false;
    //foreach (var item in map.Items) item.PlaySound();
  }
  public void LoadAmbiSounds()
  {
    var files = Directory.GetFiles(CONTENTFOLDER + "Sounds/birds/", "*.*");
    for (int i = 0; i < files.Length; i++)
    {
      var sound = System.CreateSound(files[i], Mode._3D | Mode._3D_LinearSquareRolloff);
      AmbiSounds.Add(sound);
    }
    AmbiTimer.Enabled = true;
  }

  private void onAmbiTimer(object? sender, ElapsedEventArgs e)
  {
    var rand = new Random();
    var sound = AmbiSounds[rand.Next(AmbiSounds.Count())];
    Channel channel = System.PlaySound(sound, paused: false);
    channel.Set3DAttributes(new Vector3(rand.Next(-20, 20), rand.Next(-20, 20), 5f), default, default);
    channel.Set3DMinMaxDistance(0, 35);
    AmbiTimer.Dispose();
    AmbiTimer = new System.Timers.Timer(rand.Next(4000, 8000));
    AmbiTimer.Elapsed += onAmbiTimer;
    AmbiTimer.Enabled = true;
  }

  public List<Task> tasks = new();
  public void PlayQueue(Sound sound, bool queued = true)
  {
    if (queued)
    {
      tasks.Add(Task.Factory.StartNew(() =>
      {
        int real;
        do
        {
          System.GetChannelsPlaying(out int all, out real);
        } while (real > 2);
        Channel? channel = System.PlaySound(sound, paused: false);
        if (channel != null)
        {
          while (channel.IsPlaying) { Thread.Sleep(5); }
          try
          {
            channel.Stop();
          }
          catch { }
        }
      }));
    }
    else
    {
      tasks.Add(Task.Factory.StartNew(() =>
      {
        Channel? channel = System.PlaySound(sound, paused: false);
        if (channel != null)
        {
          while (channel.IsPlaying) { Thread.Sleep(5); }
          channel.Stop();
        }
      }));
    }
  }
  public void MovePlayerTo(Vector3 newPos)
  {
    System.Set3DListenerAttributes(0, newPos, default, in Forward, in Up);
  }
  public void PlayBebooSound(Sound sound, Vector3 position, bool stopOthers=true)
  {
    if (BebooChannel != null && stopOthers && BebooChannel.IsPlaying) BebooChannel.Stop();
    BebooChannel = PlaySoundAtPosition(sound, position);  }
  public void PlayBebooSound(List<Sound> sounds, Vector3 position, bool stopOthers=true, float volume = -1)
  {
    var rand = new Random();
    var sound = sounds[rand.Next(sounds.Count())];
    if (BebooChannel != null && stopOthers && BebooChannel.IsPlaying) BebooChannel.Stop();
    BebooChannel = PlaySoundAtPosition(sound, position);
    if (volume != -1) BebooChannel.Volume = volume;
  }

  public Channel PlaySoundAtPosition(Sound sound, Vector3 position)
  {
    Channel channel= System.PlaySound(sound, paused: true);
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
    var mute=Music.Mute;
    Music.Stop();
    Music = System.PlaySound(music, paused: false);
    if(endLoop!=0) Music.SetLoopPoints(timeUnit, startLoop, timeUnit, endLoop);
    //Music.SetLoopPoints(TimeUnit.PCM, 464375, TimeUnit.PCM, 4471817);
    Music.Volume = volume;
    Music.Mute = mute;
  }

  internal Channel PlaySoundAtPosition(List<Sound> sounds, Vector3 position)
  {
    var rand = new Random();
    var sound = sounds[rand.Next(sounds.Count())];
    var channel = PlaySoundAtPosition(sound, position);
    return channel;
  }
}
