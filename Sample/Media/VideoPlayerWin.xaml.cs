using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using Sample.Annotations;

namespace Sample.Media;

/// <summary>
/// VideoPlayerWin.xaml 的交互逻辑
/// </summary>
public partial class VideoPlayerWin : INotifyPropertyChanged
{
    public VideoPlayerWin()
    {
        InitializeComponent();
        DataContext = this;
        // Sources.Add(@"D:\palink\Resources\4.机械手臂游戏画面.mp4");
        // Sources.Add(@"D:\palink\Resources\守护者-成功界面.mp4");
        Sources.Add(@"D:\palink\Resources\守护者-信息读取界面.mp4");
        // Sources.Add(@"D:\palink\Resources\守护者-游戏界面.mp4");
    }

    private List<string> _sources = new();

    public List<string> Sources
    {
        get => _sources;
        set
        {
            _sources = value;
            OnPropertyChanged(nameof(Sources));
        }
    }

    private void Play_OnClick(object sender, RoutedEventArgs e)
    {
        VideoPlayer.Play();
    }

    private void VideoPlayer_OnVideoEnded(object? sender, EventArgs e)
    {
        Debug.WriteLine("video player ended");
    }

    private void Pause_OnClick(object sender, RoutedEventArgs e)
    {
        VideoPlayer.Pause();
    }

    private void Stop_OnClick(object sender, RoutedEventArgs e)
    {
        VideoPlayer.Stop();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}