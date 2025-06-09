using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BebooGarden.MiniGames;

public interface IMiniGame
{
  public bool IsRunning { get; }
  public string Tips { get; }
  void Start();
  void Update(GameTime gameTime, KeyboardState currentKeyboardState);
}
