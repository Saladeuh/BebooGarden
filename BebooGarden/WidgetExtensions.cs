using Myra.Graphics2D.UI;

namespace BebooGarden.Interface.Shop;

public static class WidgetExtensions
{
  public static Widget? FindWidgetById(this Panel panel, string id)
  {
    return FindWidgetByIdRecursive(panel, id);
  }

  private static Widget? FindWidgetByIdRecursive(Widget widget, string id)
  {
    if (widget.Id == id)
      return widget;

    if (widget is Panel panel)
    {
      foreach (var child in panel.Widgets)
      {
        var result = FindWidgetByIdRecursive(child, id);
        if (result != null)
          return result;
      }
    }
    else if (widget is VerticalStackPanel stackPanel)
    {
      foreach (var child in stackPanel.Widgets)
      {
        var result = FindWidgetByIdRecursive(child, id);
        if (result != null)
          return result;
      }
    }

    return null;
  }
}