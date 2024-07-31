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
  }

  public override void Take()
  {
    Game.Map?.Items.Remove(this);
    Game.GainTicket(Amount);
  }
}
