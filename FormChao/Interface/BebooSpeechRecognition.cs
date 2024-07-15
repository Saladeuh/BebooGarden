using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using BebooGarden.GameCore;

namespace BebooGarden.Interface;

internal class BebooSpeechRecognition
{
  private string BebooName { get; set; }
  public event EventHandler BebooCalled;
  public BebooSpeechRecognition(string bebooName)
  {
    BebooName = bebooName;
    SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine(System.Globalization.CultureInfo.CurrentCulture);
    recognizer.SetInputToDefaultAudioDevice();

    Choices choices = new Choices();
    choices.Add(bebooName);
    GrammarBuilder builder = new GrammarBuilder();
    builder.Append(choices);
    builder.Culture = System.Globalization.CultureInfo.CurrentCulture;
    Grammar grammar = new Grammar(builder);
    recognizer.LoadGrammar(grammar);

    recognizer.SpeechRecognized += SpeechRecognized;

    // Démarrer la reconnaissance vocale
    recognizer.RecognizeAsync(RecognizeMode.Multiple);
  }

  void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
  {
    if (e.Result.Text == BebooName)
    {
      BebooCalled.Invoke(this, new EventArgs());
    }
  }
}