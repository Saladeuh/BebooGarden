using System.Numerics;
using System.Timers;
using BebooGarden.GameCore;
using FmodAudio;

namespace BebooGarden;
internal class SoundSystem
{
  private const string CONTENTFOLDER = "Content/";
  public FmodSystem System { get; }
  public List<Sound> AmbiSounds { get; set; }
  public Vector3 Up = new Vector3(0, 0, 1), Forward = new Vector3(0, 1, 0);
  public float Volume { get { return System.MasterSoundGroup.GetValueOrDefault().Volume; } set { System.MasterSoundGroup.GetValueOrDefault().Volume = value; } }
  public List<Sound> BebooCuteSounds { get; private set; }
  public List<Sound> BebooSleepSounds { get; private set; }
  public List<Sound> BebooYawningSounds { get; private set; }
  public Sound WhistleSound { get; set; }
  public List<Sound> BebooSleepingSounds { get; private set; }
  public Sound BebooStepSound { get; private set; }
  public Sound GrassSound { get; private set; }

  private static System.Timers.Timer AmbiTimer;

  public SoundSystem(float initialVolume)
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
    AmbiTimer.Enabled = true;
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
    Sound sound;
    sound = System.CreateStream(CONTENTFOLDER + "music/neutral.mp3", Mode.Loop_Normal);
    Channel channel = (Channel?)System.PlaySound(sound, paused: true);
    channel.SetLoopPoints(TimeUnit.MS, 12, TimeUnit.MS, 88369);
    channel.Volume = 0.5f;
    sound = System.CreateStream(CONTENTFOLDER + "sounds/WaterCalmWide.wav", Mode.Loop_Normal | Mode._3D | Mode._3D_InverseTaperedRolloff);
    channel = (Channel?)System.PlaySound(sound, paused: false);
    channel.SetLoopPoints(TimeUnit.MS, 2780, TimeUnit.MS, 17796);
    channel.Set3DAttributes(new Vector3(-20f, 0f, -5f), default, default);
    channel.Set3DMinMaxDistance(3f, 24f);
    channel.Volume = 0.1f;
    sound = System.CreateStream(CONTENTFOLDER + "sounds/Grass_Shake.wav", Mode.Loop_Normal);
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
    WhistleSound = System.CreateSound(CONTENTFOLDER + "sounds/character/se_sys_whistle_1p.wav", Mode._3D | Mode._3D_LinearSquareRolloff | Mode.Unique);
    BebooStepSound = System.CreateSound(CONTENTFOLDER + "sounds/beboo/step.wav", Mode._3D | Mode._3D_LinearSquareRolloff | Mode.Unique);
    GrassSound = System.CreateSound(CONTENTFOLDER + "sounds/grass_rustle.wav", Mode._3D | Mode._3D_LinearSquareRolloff | Mode.Unique);
    Reverb3D reverb = System.CreateReverb3D();
    reverb.SetProperties(Preset.Bathroom);
    reverb.Set3DAttributes(new Vector3(0, 0, 0), 0f, 500f);
  }
  public void LoadAmbiSounds()
  {
    var files = Directory.GetFiles(CONTENTFOLDER + "Sounds/birds/", "*.*");
    for (int i = 0; i < files.Length; i++)
    {
      var sound = System.CreateSound(files[i], Mode._3D | Mode._3D_LinearSquareRolloff);
      AmbiSounds.Add(sound);
    }
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
  public void MovePlayerOf(Vector3 movement)
  {
    System.Get3DListenerAttributes(0, out Vector3 currentPosition, out _, out _, out _);
    System.Set3DListenerAttributes(0, currentPosition + movement, default, in Forward, in Up);
  }
  public void PlayBebooSound(Sound sound, Beboo beboo)
  {
    Channel bebooChannel = System.PlaySound(sound, paused: true);
    bebooChannel.Set3DMinMaxDistance(0f, 30f);
    bebooChannel.Set3DAttributes(beboo.Position + new Vector3(0, 0, -2), default, default);
    bebooChannel.Paused = false;
  }
  public void PlayBebooSound(List<Sound> sounds, Beboo beboo, float volume=-1)
  {
    var rand = new Random();
    var sound = sounds[rand.Next(sounds.Count())];
    Channel bebooChannel = System.PlaySound(sound, paused: true);
    bebooChannel.Set3DMinMaxDistance(0f, 30f);
    bebooChannel.Set3DAttributes(beboo.Position + new Vector3(0, 0, -2), default, default);
    if (volume != -1) bebooChannel.Volume = volume;
    bebooChannel.Paused = false;
  }

  public void Whistle()
  {
    System.PlaySound(WhistleSound);
  }
}
