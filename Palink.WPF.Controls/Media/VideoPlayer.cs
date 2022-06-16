using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Palink.WPF.Controls.Media;

public class VideoPlayer : FrameworkElement
{
    private readonly MediaPlayer _player;
    private readonly VisualCollection _children;
    private int _head;

    public VideoPlayer()
    {
        _player = new MediaPlayer();
        Loaded += VideoPlayer_Loaded;
        _children = new VisualCollection(this);
    }

    #region Player Event

    private EventHandler? _changed;

    public event EventHandler Changed
    {
        add
        {
            _changed += value;
            _player.Changed += Player_Changed;
        }
        remove
        {
            _changed -= value;
            _player.Changed -= Player_Changed;
        }
    }

    private void Player_Changed(object? sender, EventArgs e)
    {
        _changed?.Invoke(this, e);
    }

    private EventHandler? _scriptCommand;

    public event EventHandler ScriptCommand
    {
        add
        {
            _scriptCommand += value;
            _player.ScriptCommand += Player_ScriptCommand;
        }
        remove
        {
            _scriptCommand -= value;
            _player.ScriptCommand -= Player_ScriptCommand;
        }
    }

    private void Player_ScriptCommand(object? sender, MediaScriptCommandEventArgs e)
    {
        _scriptCommand?.Invoke(sender, e);
    }

    private EventHandler? _videoFailed;

    public event EventHandler VideoFailed
    {
        add
        {
            _videoFailed += value;
            _player.MediaFailed += Player_MediaFailed;
        }
        remove
        {
            _videoFailed -= value;
            _player.MediaFailed -= Player_MediaFailed;
        }
    }

    private void Player_MediaFailed(object? sender, ExceptionEventArgs e)
    {
        _videoFailed?.Invoke(sender, e);
    }

    private EventHandler? _bufferingStarted;

    public event EventHandler BufferingStarted
    {
        add
        {
            _bufferingStarted += value;
            _player.BufferingStarted += Player_BufferingStarted;
        }
        remove
        {
            _bufferingStarted -= value;
            _player.BufferingStarted -= Player_BufferingStarted;
        }
    }

    private void Player_BufferingStarted(object? sender, EventArgs e)
    {
        _bufferingStarted?.Invoke(sender, e);
    }

    private EventHandler? _bufferingEnded;

    public event EventHandler BufferingEnded
    {
        add
        {
            _bufferingEnded += value;
            _player.BufferingEnded += Player_BufferingEnded;
        }
        remove
        {
            _bufferingEnded -= value;
            _player.BufferingEnded -= Player_BufferingEnded;
        }
    }

    private void Player_BufferingEnded(object? sender, EventArgs e)
    {
        _bufferingEnded?.Invoke(sender, e);
    }

    private EventHandler? _videoOpened;

    public event EventHandler VideoOpened
    {
        add
        {
            _videoOpened += value;
            _player.MediaOpened += Player_MediaOpened;
        }
        remove
        {
            _videoOpened -= value;
            _player.MediaOpened -= Player_MediaOpened;
        }
    }

    private void Player_MediaOpened(object? sender, EventArgs e)
    {
        _videoOpened?.Invoke(this, e);
    }

    private EventHandler? _videoEnded;

    public event EventHandler VideoEnded
    {
        add
        {
            _videoEnded += value;
            _player.MediaEnded += Player_MediaEnded;
        }
        remove
        {
            _videoEnded -= value;
            _player.MediaEnded -= Player_MediaEnded;
        }
    }

    private void Player_MediaEnded(object? sender, EventArgs e)
    {
        if (Loop)
        {
            Play();
        }

        _videoEnded?.Invoke(this, e);
    }

    #endregion

    private void VideoPlayer_Loaded(object sender, RoutedEventArgs e)
    {
        _children.Add(CreateDrawingVisualVideo(Left, Top));
    }

    // Provide a required override for the VisualChildrenCount property.
    protected override int VisualChildrenCount => _children.Count;

    // Provide a required override for the GetVisualChild method.
    protected override Visual GetVisualChild(int index)
    {
        if (index < 0 || index >= _children.Count)
        {
            throw new ArgumentOutOfRangeException();
        }

        return _children[index];
    }

    // Create a DrawingVisual that contains a rectangle.
    private DrawingVisual CreateDrawingVisualVideo(int left, int top)
    {
        var drawingVisual = new DrawingVisual();

        // Retrieve the DrawingContext in order to create new drawing content.
        var drawingContext = drawingVisual.RenderOpen();

        // Create a rectangle and draw it in the DrawingContext.
        var rect = new Rect(new Point(left, top), new Size(Width, Height));
        drawingContext.DrawVideo(_player, rect);
        // Persist the drawing content.
        drawingContext.Close();

        return drawingVisual;
    }

    public void Play()
    {
        if (_paused)
        {
            _paused = false;
            _player.Play();
            return;
        }

        if (!Sources.Any()) return;

        var path = Sources[_head];
        if (!File.Exists(path))
            throw new InvalidOperationException($"文件{path}不存在");

        _player.Open(new Uri(path));
        _player.Play();

        if (Loop)
        {
            _head = (_head + 1) % Sources.Count;
        }
    }

    private bool _paused;

    public void Pause()
    {
        _paused = true;
        _player.Pause();
    }

    public void Stop()
    {
        _head = 0;
        _paused = false;
        _player.Stop();
    }

    public TimeSpan Position
    {
        get => _player.Position;
        set => _player.Position = value;
    }

    public double Volume
    {
        get => _player.Volume;
        set => _player.Volume = value;
    }

    public double SpeedRatio
    {
        get => _player.SpeedRatio;
        set => _player.SpeedRatio = value;
    }

    public static readonly DependencyProperty LoopProperty = DependencyProperty.Register(
        "Loop", typeof(bool), typeof(VideoPlayer), new PropertyMetadata(default(bool)));

    /// <summary>
    /// 是否循环播放
    /// </summary>
    public bool Loop
    {
        get => (bool)GetValue(LoopProperty);
        set => SetValue(LoopProperty, value);
    }

    public static readonly DependencyProperty SourcesProperty =
        DependencyProperty.Register(
            "Sources", typeof(List<string>), typeof(VideoPlayer),
            new PropertyMetadata(default(List<string>)));

    public List<string> Sources
    {
        get => (List<string>)GetValue(SourcesProperty);
        set => SetValue(SourcesProperty, value);
    }

    public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(
        "Left", typeof(int), typeof(VideoPlayer), new PropertyMetadata(default(int)));

    public int Left
    {
        get => (int)GetValue(LeftProperty);
        set => SetValue(LeftProperty, value);
    }

    public static readonly DependencyProperty TopProperty = DependencyProperty.Register(
        "Top", typeof(int), typeof(VideoPlayer), new PropertyMetadata(default(int)));

    public int Top
    {
        get => (int)GetValue(TopProperty);
        set => SetValue(TopProperty, value);
    }
}