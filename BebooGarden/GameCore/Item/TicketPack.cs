using System.Numerics;
using BebooGarden.GameCore.Pet;

namespace BebooGarden.GameCore.Item;

internal class TicketPack(int amount) : Item
{
  protected override string _translateKeyName { get; } = "ticketpack.name";
  protected override string _translateKeyDescription { get; } = "ticketpack.description";
  public override bool IsTakable { get; set; } = true;

  public override void Action()
  {
    Take();
  }
  public int Amount { get; set; } = amount;
  public override void PlaySound()
  {
    if (Position == null) return;
    Channel = Game1.Instance.SoundSystem.PlaySoundAtPosition(Game1.Instance.SoundSystem.ItemTicketPackSound, (Vector3)Position);
  }

  public override Vector3? Position { get; set; } // position null=in inventory
  public override void Take()
  {
    Game1.Instance.Map?.Items.Remove(this);
    Position = null;
    Game1.Instance.GainTicket(Amount);
  }
  public override void BebooAction(Beboo beboo) => Take();

}
