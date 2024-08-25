using System.Globalization;
using System.Speech.Recognition;

namespace BebooGarden.Interface;

public class BebooSpeechRecognition
{
  public BebooSpeechRecognition(string bebooName)
  {
    BebooName = bebooName;
    try
    {
      if (SpeechRecognitionEngine.InstalledRecognizers()!=null && SpeechRecognitionEngine.InstalledRecognizers().Count > 0)
      {
        SpeechRecognitionEngine recognizer;
        GrammarBuilder builder = new();
        recognizer = new(SpeechRecognitionEngine.InstalledRecognizers()[0].Culture);
        builder.Culture = SpeechRecognitionEngine.InstalledRecognizers()[0].Culture;
        recognizer.SetInputToDefaultAudioDevice();
        Choices choices = new();
        choices.Add(bebooName);
        builder.Append(choices);
        Grammar grammar = new(builder);
        recognizer.LoadGrammar(grammar);

        recognizer.SpeechRecognized += SpeechRecognized;

        // Démarrer la reconnaissance vocale
        recognizer.RecognizeAsync(RecognizeMode.Multiple);
      }
    }
    catch { }
  }
  private string BebooName { get; }
  public event EventHandler BebooCalled;

  private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
  {
    if (e.Result.Text == BebooName) BebooCalled.Invoke(this, new EventArgs());
  }
}