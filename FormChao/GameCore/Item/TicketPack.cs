using System.Numerics;

namespace BebooGarden.GameCore.Item;

internal class TicketPack(int amouot) : Item
{
  protected override string _translateKeyName { get; } = "ticketpack.name";
  protected override string _translateKeyDescription { get; } = "ticketpack.description";

  public override void Action()
  {
    Take();
  }
  private int Amount { get; set; } = amouot;
  public override void PlaySound()
  {
    if (Position == null) return;
    Channel = Game.SoundSystem.PlaySoundAtPosition(Game.SoundSystem.ItemTicketPackSound, (Vector3)Position);

  }

  public override Vector3? Position { get; set; } // position null=in inventory
  public override void Take()
  {
    Game.Map?.Items.Remove(this);
    Position = null;
    Game.GainTicket(Amount);
  }
}
