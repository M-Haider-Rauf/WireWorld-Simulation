using SFML.Graphics;

enum StateId {
    NULL, Playing
};

abstract class GameState {
    public GameState(StateId id)
    {
        this.id = id;
        nextState = StateId.NULL;
    }
    public abstract void HandleInput(RenderWindow window);
    public abstract void Update();
    public abstract void Render(RenderWindow window);

    readonly StateId id;
    public StateId Id
    {
        get { return id; }
    }

    private StateId nextState;
    public StateId NextState
    {
        get { return nextState; }
    }
}

