using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

static class Consts {
    public const int WIN_WIDTH = 800;
    public const int WIN_HEIGHT = 600;
    public const int CELL_SIZE = 20;
    public const int GRID_COLS = WIN_WIDTH / CELL_SIZE;
    public const int GRID_ROWS = WIN_HEIGHT / CELL_SIZE;


    public static readonly List<Vertex> gridLines;


    static Consts()
    {
        gridLines = new List<Vertex>();

        for (int y = 1; y < GRID_ROWS; ++y) {
            gridLines.Add(new Vertex(new Vector2f(0, y * CELL_SIZE), new Color(127, 127, 127)));
            gridLines.Add(new Vertex(new Vector2f(WIN_WIDTH, y * CELL_SIZE), new Color(127, 127, 127)));
        }

        for (int x = 1; x < GRID_COLS; ++x) {
            gridLines.Add(new Vertex(new Vector2f(x * CELL_SIZE, 0), new Color(127, 127, 127)));
            gridLines.Add(new Vertex(new Vector2f(x * CELL_SIZE, WIN_HEIGHT), new Color(127, 127, 127)));
        }
    }
}