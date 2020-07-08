using SFML.System;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

class Engine {
    public Engine()
    {
        window = new RenderWindow(new VideoMode(Consts.WIN_WIDTH, Consts.WIN_HEIGHT), "SFML.NET", Styles.Close);
        window.SetVerticalSyncEnabled(true);
        window.SetTitle("SFML.NET");

        stateMachine = new Dictionary<StateId, GameState>();
        stateMachine.Add(StateId.Playing, new PlayState());
        currentState = stateMachine[StateId.Playing];

        SetEvents();
    }

    private void SetEvents()
    {
        window.Closed += (sender, args) =>
        {
            window.Close();
        };
    }

    private void HandleInput()
    {
        window.DispatchEvents();
        currentState.HandleInput(window);
        Input.UpdateInput(window);
    }

    private void Update()
    {
        currentState.Update();
        if (currentState.NextState != 0) {
            currentState = stateMachine[StateId.Playing];
        }
    }

    private void Render()
    {
        this.window.Clear();

        currentState.Render(window);

        this.window.Display();
    }

    public void MainLoop()
    {
        while (window.IsOpen) {
            HandleInput();
            Update();
            Render();
        }
    }

    private readonly RenderWindow window;
    private readonly Dictionary<StateId, GameState> stateMachine;
    private GameState currentState;

}