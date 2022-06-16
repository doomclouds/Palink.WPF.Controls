using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Palink.WPF.Controls.Media;

public class ImageFramePlayer : FrameworkElement
{
    private readonly VisualCollection _children;

    private int _head;

    public ImageFramePlayer()
    {
        _children = new VisualCollection(this);
    }

    private EventHandler? _ended;

    public event EventHandler Ended
    {
        add => _ended += value;
        remove => _ended -= value;
    }

    private EventHandler? _canceled;

    public event EventHandler Canceled
    {
        add => _canceled += value;
        remove => _canceled -= value;
    }

    private EventHandler? _faulted;

    public event EventHandler Faulted
    {
        add => _faulted += value;
        remove => _faulted -= value;
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
    private DrawingVisual CreateDrawingVisualImage(ImageSource img, int left, int top)
    {
        var drawingVisual = new DrawingVisual();

        // Retrieve the DrawingContext in order to create new drawing content.
        var drawingContext = drawingVisual.RenderOpen();

        // Create a rectangle and draw it in the DrawingContext.
        var rect = new Rect(new Point(left, top), new Size(Width, Height));
        drawingContext.DrawImage(img, rect);
        // Persist the drawing content.
        drawingContext.Close();

        return drawingVisual;
    }

    public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(
        "Left", typeof(int), typeof(ImageFramePlayer), new PropertyMetadata(default(int)));

    public int Left
    {
        get => (int)GetValue(LeftProperty);
        set => SetValue(LeftProperty, value);
    }

    public static readonly DependencyProperty TopProperty = DependencyProperty.Register(
        "Top", typeof(int), typeof(ImageFramePlayer), new PropertyMetadata(default(int)));

    public int Top
    {
        get => (int)GetValue(TopProperty);
        set => SetValue(TopProperty, value);
    }

    public static readonly DependencyProperty LoopProperty = DependencyProperty.Register(
        "Loop", typeof(bool), typeof(ImageFramePlayer), new PropertyMetadata(default(bool)));

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
            "Sources", typeof(List<string>), typeof(ImageFramePlayer),
            new PropertyMetadata(default(List<string>)));

    public List<string> Sources
    {
        get => (List<string>)GetValue(SourcesProperty);
        set => SetValue(SourcesProperty, value);
    }

    public static readonly DependencyProperty FPSProperty = DependencyProperty.Register(
        "FPS", typeof(int), typeof(ImageFramePlayer), new PropertyMetadata(25));

    public int FPS
    {
        get => (int)GetValue(FPSProperty);
        set => SetValue(FPSProperty, value);
    }

    public CancellationTokenSource? TokenSource { get; set; }

    public void Play()
    {
        if (_running) return;

        if (!_paused)
            _head = 0;
        TokenSource = new CancellationTokenSource();
        if (Sources.Any())
            UpdateImage(TokenSource.Token).ContinueWith(task =>
            {
                _running = false;
                switch (task.Status)
                {
                    case TaskStatus.RanToCompletion:
                        _ended?.Invoke(this, EventArgs.Empty);
                        break;
                    case TaskStatus.Canceled:
                        _canceled?.Invoke(this, EventArgs.Empty);
                        break;
                    case TaskStatus.Faulted:
                        _faulted?.Invoke(this, new GIFTaskExceptionArgs(task.Exception));
                        break;
                }
            });
    }

    private bool _running;
    private bool _paused;

    private async Task UpdateImage(CancellationToken token)
    {
        _running = true;
        if (Loop)
        {
            while (Loop)
            {
                await Task.Delay(1000 / FPS, token);
                var path = Sources[_head];
                _head = (_head + 1) % Sources.Count;
                if (!File.Exists(path))
                    throw new InvalidOperationException($"文件{path}不存在");
                _children.Clear();
                _children.Add(CreateDrawingVisualImage(new BitmapImage(new Uri(path)),
                    Left, Top));
            }
        }
        else
        {
            for (var i = _head; i < Sources.Count; i++)
            {
                await Task.Delay(1000 / FPS, token);
                var path = Sources[_head];

                if (!File.Exists(path))
                    throw new InvalidOperationException($"文件{path}不存在");
                _children.Clear();
                _children.Add(CreateDrawingVisualImage(new BitmapImage(new Uri(path)),
                    Left, Top));
                _head += 1;
                if (_head >= Sources.Count)
                    break;
            }
        }
    }

    public void Pause()
    {
        _paused = true;
        TokenSource?.Cancel();
    }

    public void Stop()
    {
        _head = 0;
        TokenSource?.Cancel();
    }

    public void SetPosition(int frameIndex)
    {
        if (frameIndex < 0 || frameIndex > Sources.Count)
            throw new ArgumentOutOfRangeException(nameof(frameIndex));
        _head = frameIndex;
    }
}

public class GIFTaskExceptionArgs : EventArgs
{
    public Exception? Exception { get; set; }

    public GIFTaskExceptionArgs(Exception? e)
    {
        Exception = e;
    }
}