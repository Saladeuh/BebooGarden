using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BebooGarden.MiniGames;

public interface IMiniGame
{
  public bool IsRunning { get; set; }
  public int Score {  get; }
  public string Tips { get; }
  void Update(GameTime gameTime, KeyboardState currentKeyboardState);
  void Win();
}
