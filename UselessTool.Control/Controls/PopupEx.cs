using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;

namespace UselessTool.Control.Controls;

public class PopupEx : Popup
{
    /// <summary>
    /// 是否窗口随动，默认为随动（true）
    /// </summary>
    public bool IsPositionUpdate
    {
        get => (bool)GetValue(IsPositionUpdateProperty);
        set => SetValue(IsPositionUpdateProperty, value);
    }

    public static readonly DependencyProperty IsPositionUpdateProperty =
        DependencyProperty.Register(
            nameof(IsPositionUpdate),
            typeof(bool),
            typeof(PopupEx),
            new PropertyMetadata(true, IsPositionUpdateChanged)
        );

    private static void IsPositionUpdateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PopupEx popupEx)
        {
            popupEx.Pup_Loaded(popupEx);
        }
    }

    /// <summary>
    /// 加载窗口随动事件
    /// </summary>
    public PopupEx()
    {
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Pup_Loaded(sender);
    }

    private void Pup_Loaded(object sender)
    {
        if (sender is not Popup popup || popup.Child == null) return;

        Window? window = FindParentWindow(popup);
        if (window == null) return;

        UnsubscribeFromWindowEvents(window);
        if (IsPositionUpdate)
        {
            SubscribeToWindowEvents(window);
        }
    }

    private void SubscribeToWindowEvents(Window window)
    {
        window.LocationChanged += PositionChanged;
        window.SizeChanged += PositionChanged;
    }

    private void UnsubscribeFromWindowEvents(Window window)
    {
        window.LocationChanged -= PositionChanged;
        window.SizeChanged -= PositionChanged;
    }

    /// <summary>
    /// 刷新位置
    /// </summary>
    private void PositionChanged(object? sender, EventArgs e)
    {
        try
        {
            MethodInfo? updateMethod = typeof(Popup).GetMethod("UpdatePosition", BindingFlags.NonPublic | BindingFlags.Instance);
            if (updateMethod != null && IsOpen)
            {
                updateMethod.Invoke(this, null);
            }
        }
        catch (Exception ex)
        {
            // Log the exception or handle it appropriately
            Console.WriteLine($"Error updating position: {ex.Message}");
        }
    }

    // 是否最前，默认为非最前（false）
    public static readonly DependencyProperty TopmostProperty = Window.TopmostProperty.AddOwner(
        typeof(Popup),
        new FrameworkPropertyMetadata(false, OnTopmostChanged)
    );

    public bool Topmost
    {
        get => (bool)GetValue(TopmostProperty);
        set => SetValue(TopmostProperty, value);
    }

    private static void OnTopmostChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        if (obj is PopupEx popupEx)
        {
            popupEx.UpdateWindow();
        }
    }

    /// <summary>
    /// 重写拉开方法，置于非最前
    /// </summary>
    /// <param name="e"></param>
    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        UpdateWindow();
    }

    /// <summary>
    /// 刷新Popup层级
    /// </summary>
    private void UpdateWindow()
    {
        if (Child == null) return;

        IntPtr hwnd = ((HwndSource)PresentationSource.FromVisual(Child)).Handle;
        if (hwnd == IntPtr.Zero || !NativeMethods.GetWindowRect(hwnd, out Rect rect)) return;
        int result = NativeMethods.SetWindowPos(
            hwnd,
            Topmost ? -1 : -2,
            rect.Left,
            rect.Top,
            (int)ActualWidth,
            (int)ActualHeight,
            0
        );

        if (result != 0) return;
        // Handle error
        int errorCode = Marshal.GetLastWin32Error();
        Console.WriteLine($"SetWindowPos failed with error code: {errorCode}");
    }

    private static Window? FindParentWindow(Popup popup)
    {
        DependencyObject? parent = VisualTreeHelper.GetParent(popup);
        while (parent != null && !(parent is Window))
        {
            parent = VisualTreeHelper.GetParent(parent);
        }
        return parent as Window;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    #region P/Invoke imports & definitions

    private static class NativeMethods
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        internal static extern int SetWindowPos(
            IntPtr hWnd,
            int hwndInsertAfter,
            int x,
            int y,
            int cx,
            int cy,
            int wFlags
            );
    }
    #endregion
}