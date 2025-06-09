using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BebooGarden.UI.ScriptedScene;

public interface IScriptedScene
{
  public void Update(GameTime gameTime);
  public void Show();
}
