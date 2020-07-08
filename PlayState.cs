using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System;

class PlayState : GameState {

    public PlayState() : base (StateId.Playing)
    {
    
        currentGridState = new int[Consts.GRID_ROWS, Consts.GRID_COLS];
        clock = new Clock();
        started = false;
        selectedCell = NULL_CELL;
    }

    public override void HandleInput(RenderWindow window)
    {
        if (Input.KeyPressed(Keyboard.Key.Enter)) started = !started;
        if (Input.KeyPressed(Keyboard.Key.S)) TakeScreenShot(window);

        if (started) {
            window.SetTitle("Simulation running...");

            return;
        }
        else {
            window.SetTitle("WireWorld Simulation by Haider Rauf");
        }

        mousePos = Mouse.GetPosition(window);
        
        if (Input.KeyPressed(Keyboard.Key.C)) {
            ++selectedCell;
            selectedCell %= 4;
        }

        if (Input.MouseDown(Mouse.Button.Left)) {
            (int x, int y) = (mousePos.X, mousePos.Y);

            x /= Consts.CELL_SIZE;
            y /= Consts.CELL_SIZE;

            x = Math.Clamp(x, 0, Consts.GRID_COLS - 1);
            y = Math.Clamp(y, 0, Consts.GRID_ROWS - 1);


            int cellType = currentGridState[y, x];

            currentGridState[y, x] = selectedCell;
        }

        if (Input.KeyPressed(Keyboard.Key.R)) ResetGrid();
    }

    public override void Render(RenderWindow window)
    {
        DrawCells(window);
        DrawGrid(window);
        DrawUnderCursor(window);
        window.SetMouseCursorVisible(false);
    }

    public override void Update()   
    {
        if (!started) return;

        const float threshold = 1 / 6.0f;
        if (clock.ElapsedTime.AsSeconds() > threshold) {
            UpdateGrid();
            clock.Restart();
        }
        if (SimulationCompleted()) started = false;
    }


    private void DrawGrid(RenderWindow window)
    {
        window.Draw(Consts.gridLines.ToArray(), PrimitiveType.Lines);
    }


    private void DrawCells(RenderWindow window)
    {
        for (int y = 0; y < Consts.GRID_ROWS; ++y) {
            for (int x = 0; x < Consts.GRID_COLS; ++x) {
                int cellType = currentGridState[y, x];
                if (cellType != NULL_CELL) {
                    RectangleShape rect = new RectangleShape(
                        new Vector2f(Consts.CELL_SIZE, Consts.CELL_SIZE));
                    rect.Position = new Vector2f(x * Consts.CELL_SIZE, y * Consts.CELL_SIZE);
                    rect.FillColor = cellColors[cellType];

                    window.Draw(rect);
                   
                }
            }
        }
    }


    private void DrawUnderCursor(RenderWindow window)
    {
        if (started) return;
        RectangleShape rect = new RectangleShape(new Vector2f(10.0f, 10.0f));
        rect.FillColor = cellColors[selectedCell];
        rect.OutlineThickness = 1.0f;
        rect.OutlineColor = new Color(127, 127, 127);
        Vector2f rectPos = (Vector2f)mousePos;
        rect.Position = rectPos;
        window.Draw(rect);
    }


    private void UpdateGrid()
    {
        int[,] nextGridState = currentGridState.Clone() as int[,];

        for (int y = 0; y < Consts.GRID_ROWS; ++y) {
            for (int x = 0; x < Consts.GRID_COLS; ++x) {
                int cellType = currentGridState[y, x];

                switch (cellType) {
                case HEAD_CELL:
                    nextGridState[y, x] = TAIL_CELL;
                    break;

                case TAIL_CELL:
                    nextGridState[y, x] = CONDUCTOR_CELL;
                    break;

                case CONDUCTOR_CELL:
                    int count = GetNeighbourHeads(x, y);
                    if (count == 1 || count == 2) {
                        nextGridState[y, x] = HEAD_CELL;
                    }
                    break;

                case NULL_CELL:
                    break;

                default:
                    throw new ArgumentException("invalid cell type");
                    break;
                }
            }
        }
        currentGridState = nextGridState;
    }

    private void ResetGrid()
    {
        for (int y = 0; y < Consts.GRID_ROWS; ++y) {
            for (int x = 0; x < Consts.GRID_COLS; ++x) {
                currentGridState[y, x] = NULL_CELL;
            }
        }
    }
    private int GetNeighbourHeads(int x, int y)
    {
        //A litle helper lambda for checking Head
        Func<int, int, bool> HeadAt = (x, y) => currentGridState[y, x] == HEAD_CELL;

        int count = 0;
        bool topExists = y > 0;
        bool bottomExists = y < Consts.GRID_ROWS - 1;
        bool leftExists = x > 0;
        bool rightExists = x < Consts.GRID_COLS - 1;

        if (topExists) {
            if (HeadAt(x, y - 1)) ++count; //top
            if (leftExists && HeadAt(x - 1, y - 1)) ++count; //top-left
            if (rightExists && HeadAt(x + 1, y - 1)) ++count; //top-right
        }

        if (bottomExists) {
            if (HeadAt(x, y + 1)) ++count; //bottom
            if (leftExists && HeadAt(x - 1, y + 1)) ++count; //bottom-left
            if (rightExists && HeadAt(x + 1, y + 1)) ++count; //bottom-right
        }

        if (leftExists && HeadAt(x - 1, y)) ++count; //left
        if (rightExists && HeadAt(x + 1, y)) ++count; //right

        return count;
    }


    private bool SimulationCompleted()
    {
        foreach (int cell in currentGridState) {
            if (!(cell == NULL_CELL || cell == CONDUCTOR_CELL)) {
                return false;
            }
        }

        return true;
    }


    private void TakeScreenShot(RenderWindow window)
    {
        Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        Texture scrnTexture = new Texture(Consts.WIN_WIDTH, Consts.WIN_HEIGHT);
        scrnTexture.Update(window);

        scrnTexture.CopyToImage().SaveToFile(String.Format("sc-{0}.bmp", unixTimestamp));
    }

    const int NULL_CELL = 0;
    const int HEAD_CELL = 1;
    const int TAIL_CELL = 2;
    const int CONDUCTOR_CELL = 3;
    private static readonly Color[] cellColors = {
        Color.Black, Color.Blue, Color.Red, Color.Yellow
    };



    int[,] currentGridState;
    bool started;
    Vector2i mousePos;
    int selectedCell;
    readonly Clock clock;
}
