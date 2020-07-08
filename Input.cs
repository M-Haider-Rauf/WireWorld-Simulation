using SFML.Window;
using SFML.System;
using SFML.Graphics;

static class Input {

    static Input()
    {
        keyboardStates = new bool[(int)Keyboard.Key.KeyCount];
        mouseStates = new bool[(int)Mouse.Button.ButtonCount];
        prevMousePos = new Vector2i(0, 0);
    }

    public static bool KeyDown(Keyboard.Key key)
    {
        return Keyboard.IsKeyPressed(key);
    }

    public static bool KeyPressed(Keyboard.Key key)
    {
        return KeyDown(key) && !keyboardStates[(uint)key];
    }

    public static void UpdateInput(RenderWindow window)
    {
        for (int i = 0; i < keyboardStates.Length; ++i) {
            keyboardStates[i] = Keyboard.IsKeyPressed((Keyboard.Key) i);
        }

        for (int i = 0; i < mouseStates.Length; ++i) {
            mouseStates[i] = Mouse.IsButtonPressed((Mouse.Button) i);
        }

        prevMousePos = Mouse.GetPosition(window);
    }

    public static (int x, int y) GetMousePos(Window window)
    {
        Vector2i pos = Mouse.GetPosition(window);
        return (pos.X, pos.Y);
    }

    public static bool MouseDown(Mouse.Button button)
    {
        return Mouse.IsButtonPressed(button);
    }

    public static bool MousePressed(Mouse.Button button)
    {
        return MouseDown(button) && !mouseStates[(int) button];
    }

    public static (int, int) GetPrevMousePos()
    {
        return (prevMousePos.X, prevMousePos.Y);
    }

    public static void ResetPrevPos()
    {
        prevMousePos = new Vector2i(-1, -1);
    }

    private static readonly bool[] keyboardStates;
    private static readonly bool[] mouseStates;
    private static Vector2i prevMousePos;
}