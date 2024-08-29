using System.Globalization;
using System.Speech.Recognition;
using BebooGarden.GameCore;
using BebooGarden.GameCore.Pet;

namespace BebooGarden.Interface;

public class BebooSpeechRecognition
{
  public BebooSpeechRecognition(Beboo beboo)
  {
    Beboo = beboo;
    try
    {
      var recognizerInfos = SpeechRecognitionEngine.InstalledRecognizers();
      if (recognizerInfos != null && recognizerInfos.Count > 0)
      {
        SpeechRecognitionEngine recognizer;
        GrammarBuilder builder = new();
        recognizer = new(recognizerInfos[0].Culture);
        builder.Culture = recognizerInfos[0].Culture;
        recognizer.SetInputToDefaultAudioDevice();
        Choices choices = new();
        choices.Add(beboo.Name.ToLower());
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
  private Beboo Beboo { get; }
  public event EventHandler BebooCalled;

  private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
  {
    var name = Beboo.Name.ToLower();
    /*
    foreach (var alt in e.Result.Alternates)
    {
      if (alt != null && alt.Text.ToLower().Contains(name))
      {
        BebooCalled.Invoke(this, new EventArgs());
        return;
      }
    }
    */
    if (e.Result.Text.ToLower().Contains(name)) BebooCalled.Invoke(this, new EventArgs());
  }
}