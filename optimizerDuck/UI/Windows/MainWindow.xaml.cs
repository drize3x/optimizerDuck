using optimizerDuck.Domain.Abstractions;
using optimizerDuck.Services;
using optimizerDuck.UI.Helpers;
using optimizerDuck.UI.Pages;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Wpf.Ui;
using Wpf.Ui.Abstractions;

namespace optimizerDuck.UI.Windows;

public partial class MainWindow : IWindow
{
    private Frame? _frame;

    public MainWindow(
        INavigationService navigationService,
        IContentDialogService contentDialogService,
        INavigationViewPageProvider pageProvider,
        ISnackbarService snackbarService,
        FeatureRegistry featureRegistry)
    {
        InitializeComponent();

        snackbarService.SetSnackbarPresenter(SnackbarPresenter);
        contentDialogService.SetDialogHost(RootContentDialogPresenter);
        navigationService.SetNavigationControl(RootNavigation);

        RootNavigation.SetPageProviderService(pageProvider);

        var toggleItems = featureRegistry.GetNavigationItems();
        foreach (var item in toggleItems)
            FeaturesMenuItem.MenuItems.Add(item);

        RootNavigation.Loaded += OnRootNavigationLoaded;

    }

    private void OnRootNavigationLoaded(object sender, RoutedEventArgs e)
    {
        RootNavigation.Loaded -= OnRootNavigationLoaded;

        _frame = VisualHelper.FindMainFrame(RootNavigation);

        if (_frame != null)
            _frame.Navigated += Frame_Navigated;

        RootNavigation.Navigate(typeof(DashboardPage));
    }

    private void Frame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
    {
        if (e.Content is not Page page)
            return;

        var transform = new ScaleTransform(0.96, 0.96);
        page.RenderTransform = transform;
        page.RenderTransformOrigin = new Point(0.5, 0.5);

        page.Opacity = 0;

        var fade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));

        var scale = new DoubleAnimation(0.96, 1, TimeSpan.FromMilliseconds(200))
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };

        page.BeginAnimation(UIElement.OpacityProperty, fade);
        transform.BeginAnimation(ScaleTransform.ScaleXProperty, scale);
        transform.BeginAnimation(ScaleTransform.ScaleYProperty, scale);
    }
}