using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using Palink.WPF.Controls.Media;
using Sample.Annotations;

namespace Sample.Media;

/// <summary>
/// ImageFramePlayerWin.xaml 的交互逻辑
/// </summary>
public partial class ImageFramePlayerWin : INotifyPropertyChanged
{
    public ImageFramePlayerWin()
    {
        InitializeComponent();
        DataContext = this;
        for (var i = 0; i < 50; i++)
        {
            Sources.Add(
                $"{AppDomain.CurrentDomain.BaseDirectory}ImageFrame\\{i:0000}.png");
        }
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
        GifPlayer.Play();
    }

    private void Pause_OnClick(object sender, RoutedEventArgs e)
    {
        GifPlayer.Pause();
    }

    private void Stop_OnClick(object sender, RoutedEventArgs e)
    {
        GifPlayer.Stop();
    }

    private void RandomPos_OnClick(object sender, RoutedEventArgs e)
    {
        GifPlayer.SetPosition((new Random()).Next(GifPlayer.Sources.Count));
    }

    private void ImageFramePlayer_OnEnded(object? sender, EventArgs e)
    {
        Debug.WriteLine("image frame player ended");
    }

    private void ImageFramePlayer_OnCanceled(object? sender, EventArgs e)
    {
        Debug.WriteLine("image frame player canceled");
    }

    private void ImageFramePlayer_OnFaulted(object? sender, EventArgs e)
    {
        var msg = (e as GIFTaskExceptionArgs)?.Exception?.Message;
        Debug.WriteLine($"image frame player faulted: {msg}");
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}