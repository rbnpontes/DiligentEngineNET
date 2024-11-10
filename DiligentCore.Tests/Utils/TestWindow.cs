using System.Drawing;
using SDL;
using static SDL.SDL3;

namespace Diligent.Tests.Utils;

public unsafe class TestWindow : IDisposable
{
    private SDL_Window* _window;

    public Size Size
    {
        get
        {
            var x = 0;
            var y = 0;
            SDL_GetWindowSize(_window, &x, &y);
            return new Size(x, y);
        }
    }

    public IntPtr Handle => SDL_GetPointerProperty(
        SDL_GetWindowProperties(_window), SDL_PROP_WINDOW_WIN32_HWND_POINTER, IntPtr.Zero
    );

    public TestWindow()
    {
        if (!SDL_Init(0))
            throw new Exception("Failed to initialize Test Window");

        _window = SDL_CreateWindow("Test Window", 800, 400, SDL_WindowFlags.SDL_WINDOW_BORDERLESS);
        SDL_ShowWindow(_window);
    }

    public void PollEvents()
    {
        SDL_PumpEvents();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        
        SDL_DestroyWindow(_window);
        _window = null;
    }
}