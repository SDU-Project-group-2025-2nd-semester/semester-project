using Avalonia;
using Avalonia.Controls;
using Avalonia.Reactive;
using Avalonia.VisualTree;
using System;
using System.Linq;

namespace HeatManager.Views.Overview;

public partial class WeeklyStatistics : UserControl
{
    public WeeklyStatistics()
    {
        InitializeComponent();

        var border = this.FindControl<Border>("WSBorder");
        if (border != null)
        {
            border.GetObservable(BoundsProperty).Subscribe(
                new AnonymousObserver<Rect>(bounds =>
                {
                    // Calculate spacing based on height of the border
                    double spacing = Math.Max(3, bounds.Height * 0.033);

                    foreach (var tb in border.GetVisualDescendants().OfType<TextBlock>())
                    {
                        if (tb.Classes.Contains("normaltext") || tb.Classes.Contains("subheading"))
                            tb.Margin = new Thickness(0, 0, 0, spacing);
                    }
                })
            );
        }
    }
}