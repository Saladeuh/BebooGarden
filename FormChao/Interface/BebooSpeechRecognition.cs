using System.Globalization;
using System.Speech.Recognition;

namespace BebooGarden.Interface;

public class BebooSpeechRecognition
{
  public BebooSpeechRecognition(string bebooName)
  {
    BebooName = bebooName;
    SpeechRecognitionEngine recognizer = new(CultureInfo.InstalledUICulture);
    recognizer.SetInputToDefaultAudioDevice();

    Choices choices = new();
    choices.Add(bebooName);
    GrammarBuilder builder = new();
    builder.Append(choices);
    builder.Culture = CultureInfo.InstalledUICulture;
    Grammar grammar = new(builder);
    recognizer.LoadGrammar(grammar);

    recognizer.SpeechRecognized += SpeechRecognized;

    // Démarrer la reconnaissance vocale
    recognizer.RecognizeAsync(RecognizeMode.Multiple);
  }

  private string BebooName { get; }
  public event EventHandler BebooCalled;

  private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
  {
    if (e.Result.Text == BebooName) BebooCalled.Invoke(this, new EventArgs());
  }
}