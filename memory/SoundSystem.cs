using System.Numerics;
using System.Text.RegularExpressions;
using System.Timers;
using FmodAudio;

namespace memoryGame;
public class SoundSystem
{
  private const string CONTENTFOLDER = "Content/";
  public FmodSystem System { get; }
  public List<Sound> AmbiSounds { get; set; }
  public Channel[] Channels { get; set; }
  public Vector3 Up = new Vector3(0, 0, 1), Forward = new Vector3(0, 1, 0);
  public List<Channel> Musics { get; private set; }
  public Sound JingleCaseWin { get; private set; }
  public Sound JingleCaseLose { get; private set; }
  public Sound JingleWin { get; private set; }
  public Sound JingleLose { get; private set; }
  public Sound JingleError { get; private set; }
  public float Volume { get { return System.MasterSoundGroup.GetValueOrDefault().Volume; } set { System.MasterSoundGroup.GetValueOrDefault().Volume = value; } }
  private static System.Timers.Timer AmbiTimer;

  public SoundSystem(float initialVolume)
  {
    //Creates the FmodSystem object
    System = FmodAudio.Fmod.CreateSystem();
    //System object Initialization
    System.Init(4093, InitFlags._3D_RightHanded);
    System.Set3DSettings(1.0f, 1.0f, 1.0f);
    Volume = initialVolume;
    //Set the distance Units (Meters/Feet etc)
    System.Set3DListenerAttributes(0, new Vector3(0,0,0), default, Forward, Up);
    Channels = Array.Empty<Channel>();
    AmbiSounds = new List<Sound>();
    Musics = new List<Channel>();
    AmbiTimer = new System.Timers.Timer(2000);
    AmbiTimer.Elapsed += onAmbiTimer;
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

  public void LoadMainScreen()
  {
    Sound sound;
    sound = sound = System.CreateStream(CONTENTFOLDER + "music/neutral.mp3", Mode.Loop_Normal);
    Channel channel = (Channel?)System.PlaySound(sound, paused: true);
    channel.SetLoopPoints(TimeUnit.MS, 12, TimeUnit.MS, 88369);
    channel.Volume = 0.5f;
    Musics.Add(channel);

    sound = System.CreateStream(CONTENTFOLDER + "sounds/WaterCalmWide.wav", Mode.Loop_Normal | Mode._3D | Mode._3D_LinearRolloff);
    channel = (Channel?)System.PlaySound(sound, paused: false);
    channel.SetLoopPoints(TimeUnit.MS, 2780, TimeUnit.MS, 17796);
    channel.Set3DAttributes(new Vector3(-20f, 0f, 0f), default, default);
    channel.Set3DMinMaxDistance(10, 30);
    channel.Volume = 0.1f;

    sound = System.CreateStream(CONTENTFOLDER + "sounds/Grass_Shake.wav", Mode.Loop_Normal);
    channel = (Channel?)System.PlaySound(sound, paused: false);
    channel.SetLoopPoints(TimeUnit.MS, 678, TimeUnit.MS, 6007);
    channel.Volume = 0.5f;
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

  private void LoadLevelMusics()
  {
    Sound sound;
    sound = System.CreateStream(CONTENTFOLDER + "music/OTOATE.wav", Mode.Loop_Normal);
    Channel channel = System.PlaySound(sound, paused: false);
    channel.SetLoopPoints(TimeUnit.MS, 5201, TimeUnit.MS, sound.GetLength(TimeUnit.MS) - 1);
    Musics.Add(channel);
    JingleCaseWin = System.CreateSound(CONTENTFOLDER + "music/Jingle_SLVSTAR1.mp3");
    JingleCaseLose = System.CreateSound(CONTENTFOLDER + "music/Jingle_DROPSTAR.mp3");
    JingleWin = System.CreateSound(CONTENTFOLDER + "music/Jingle_MINICLEAR.mp3");
    JingleLose = System.CreateSound(CONTENTFOLDER + "music/Jingle_MINIOVER.mp3");
    JingleError = System.CreateSound(CONTENTFOLDER + "music/SM64_Error.ogg");
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
  public void FreeRessources()
  {
    Musics.ForEach((music) =>
    {
      music?.Stop();
    });
    Musics.Clear();
  }
  public void MoveOf(Vector3 movement)
  {
    System.Get3DListenerAttributes(0, out Vector3 currentPosition, out _, out _, out _);
    System.Set3DListenerAttributes(0,currentPosition+movement, default, in Forward, in Up);
  }
}
