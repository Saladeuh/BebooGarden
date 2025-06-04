using BebooGarden.Content;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.Interface.UI;
using CrossSpeak;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BebooGarden;

public partial class Game1
{
  private void SayBebooState()
  {
    if (Map?.Beboos.Count == 0)
    {
      CrossSpeakManager.Instance.Output(GameText.nobeboo);
    }
    foreach (Beboo beboo in Map?.Beboos)
    {
      string sentence;
      string name = beboo.Name;
      if (beboo.Sleeping)
      {
        sentence = GameText.beboo_sleep;
      }
      else
      {
        if (beboo.Happiness < 0) sentence = GameText.beboo_verysad;
        else if (beboo.Energy < 0) sentence = GameText.beboo_verytired;
        else if (beboo.Energy < 3) sentence = GameText.beboo_littletired;
        else sentence = beboo.Happiness < 3 ? GameText.beboo_littlesad : (beboo.Energy < 8 ? GameText.beboo_good : GameText.beboo_verygood);
      }
      CrossSpeakManager.Instance.Output(String.Format(sentence, name));
#if DEBUG
      ScreenReader.Output(beboo.Age.ToString());
#endif


    }
  }
  private void FeedBeboo()
  {
    Beboo? bebooUnderCursor = BebooUnderCursor();
    if (Save.FruitsBasket == null || bebooUnderCursor == null) return;
    Dictionary<string, FruitSpecies> options = [];
    foreach (KeyValuePair<FruitSpecies, int> fruit in Save.FruitsBasket)
    {
      if (fruit.Value > 0) options.Add(fruit.Key.ToString() + " " + fruit.Value.ToString(), fruit.Key);
    }
    if (options.Count == 1)
    {
      OnFruitSelected(options.First().Value);
    }
    else if (options.Count > 0)
    {
      new ChooseMenu<FruitSpecies>(GameText.ui_chooseitem, options, OnFruitSelected)
       .Show();
    }
  }

  private void OnFruitSelected(FruitSpecies choice)
  {
    if (choice != FruitSpecies.None)
    {
      Beboo? bebooUnderCursor = BebooUnderCursor();
      bebooUnderCursor.Eat(choice);
      Save.FruitsBasket[choice]--;
    }
  }
}
