using System.Windows;
using Sample.Media;

namespace Sample;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OpenImageFramePlayer_OnClick(object sender, RoutedEventArgs e)
    {
        var win = new ImageFramePlayerWin();
        win.Show();
    }

    private void OpenVideoPlayer_OnClick(object sender, RoutedEventArgs e)
    {
        var win = new VideoPlayerWin();
        win.Show();
    }
}