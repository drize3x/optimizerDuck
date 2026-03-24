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

    private async void OnRootNavigationLoaded(object sender, RoutedEventArgs e)
    {
        RootNavigation.Loaded -= OnRootNavigationLoaded;

        await Dispatcher.InvokeAsync(() => { }, System.Windows.Threading.DispatcherPriority.Loaded);

        _frame = VisualHelper.FindMainFrame(RootNavigation);

        if (_frame != null)
        {
            _frame.Navigated += Frame_Navigated;
        }

        RootNavigation.Navigate(typeof(DashboardPage));
    }

    private void Frame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
    {
        if (e.Content is not Page page)
            return;

        // reset
        page.Opacity = 0;

        var translate = new TranslateTransform(0, 30);
        var scale = new ScaleTransform(0.95, 0.95);

        var group = new TransformGroup();
        group.Children.Add(scale);
        group.Children.Add(translate);

        page.RenderTransform = group;
        page.RenderTransformOrigin = new Point(0.5, 0.5);

        var fade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(320));

        var move = new DoubleAnimation(30, 0, TimeSpan.FromMilliseconds(320))
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };

        var zoom = new DoubleAnimation(0.95, 1, TimeSpan.FromMilliseconds(320))
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };

        page.BeginAnimation(UIElement.OpacityProperty, fade);
        translate.BeginAnimation(TranslateTransform.YProperty, move);
        scale.BeginAnimation(ScaleTransform.ScaleXProperty, zoom);
        scale.BeginAnimation(ScaleTransform.ScaleYProperty, zoom);
    }
}