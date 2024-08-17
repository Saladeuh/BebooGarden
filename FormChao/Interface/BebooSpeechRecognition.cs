using System.Globalization;
using System.Speech.Recognition;

namespace BebooGarden.Interface;

public class BebooSpeechRecognition
{
  public BebooSpeechRecognition(string bebooName)
  {
    BebooName = bebooName;
    var recognizer = new SpeechRecognitionEngine(CultureInfo.InstalledUICulture);
    recognizer.SetInputToDefaultAudioDevice();

    var choices = new Choices();
    choices.Add(bebooName);
    var builder = new GrammarBuilder();
    builder.Append(choices);
    builder.Culture = CultureInfo.InstalledUICulture;
    var grammar = new Grammar(builder);
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