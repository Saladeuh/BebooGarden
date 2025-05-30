using Myra.Graphics2D.UI;
using System.Collections.Generic;

namespace BebooGarden;

public static class DesktopExtensions
{
  public static void FocusNext(this Desktop desktop)
  {
    List<Widget> widgets = CollectFocusableWidgets(desktop.Root);
    FocusNextWidget(widgets, desktop);
  }

  public static void FocusPrevious(this Desktop desktop)
  {
    List<Widget> widgets = CollectFocusableWidgets(desktop.Root);
    FocusPreviousWidget(widgets, desktop);
  }

  private static List<Widget> CollectFocusableWidgets(Widget root)
  {
    List<Widget> result = new();
    CollectFocusableWidgetsRecursive(root, result);
    return result;
  }

  private static void CollectFocusableWidgetsRecursive(Widget widget, List<Widget> result)
  {
    if (widget == null)
      return;

    if (widget.AcceptsKeyboardFocus)
      result.Add(widget);

    // Si c'est un conteneur, parcourir ses enfants
    if (widget is Container container)
    {
      foreach (Widget child in container.Widgets)
      {
        CollectFocusableWidgetsRecursive(child, result);
      }
    }
  }

  private static void FocusNextWidget(List<Widget> widgets, Desktop desktop)
  {
    if (widgets.Count == 0)
      return;

    // Find focused widget index
    int currentIndex = -1;
    for (int i = 0; i < widgets.Count; i++)
    {
      if (widgets[i] == desktop.FocusedKeyboardWidget)
      {
        currentIndex = i;
        break;
      }
    }

    // Next index
    int nextIndex = (currentIndex + 1) % widgets.Count;
    widgets[nextIndex].SetKeyboardFocus();
  }

  private static void FocusPreviousWidget(List<Widget> widgets, Desktop desktop)
  {
    if (widgets.Count == 0)
      return;

    // Trouver l'index du widget actuellement focalisé
    int currentIndex = -1;
    for (int i = 0; i < widgets.Count; i++)
    {
      if (widgets[i] == desktop.FocusedKeyboardWidget)
      {
        currentIndex = i;
        break;
      }
    }

    // Calculer l'index précédent
    int prevIndex = currentIndex - 1;
    if (prevIndex < 0)
      prevIndex = widgets.Count - 1;

    widgets[prevIndex].SetKeyboardFocus();
  }
}