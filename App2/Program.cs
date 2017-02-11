using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App2
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            program.invoke();
        }


        void invoke()
        {
            Maze.CreateMaze(10, 10);
        }

        enum Direction
        {
            None,
            N,
            E,
            S,
            W,
        }

        class Maze
        {
            class MazeBuilder
            {
                public int w;
                public int h;

                enum Map
                {
                    Wall,
                    Hall
                }

                Map[] map;

                int Pos(int x, int y)
                {
                    return y * w + x;
                }

                public void build(Maze maze)
                {
                    w = maze.size_x * 2 + 1;
                    h = maze.size_y * 2 + 1;
                    map = new Map[w * h];
                    for (int i = 0; i < map.Length; ++i)
                    {
                        map[i] = Map.Wall;
                    }

                    Random rnd = new Random();
                    int start_x = rnd.Next(0, w / 2) * 2 + 1;
                    int start_y = rnd.Next(0, h / 2) * 2 + 1;
                    map[Pos(start_x, start_y)] = Map.Hall;
                    Digg(start_x, start_y);

                    Output();

                    for (int i = 0; i < maze.size_x; ++i)
                    {
                        for (int j = 0; j < maze.size_y; ++j)
                        {
                            int x = i * 2 + 1;
                            int y = j * 2 + 1;

                            if (IsWall(x, y - 1) || y == 0)
                                maze.units[maze.GetPos(i, j)] |= Wall.N;
                            if (IsWall(x + 1, y) || x == w)
                                maze.units[maze.GetPos(i, j)] |= Wall.E;
                            if (IsWall(x, y + 1) || y == h)
                                maze.units[maze.GetPos(i, j)] |= Wall.S;
                            if (IsWall(x - 1, y) || x == 0)
                                maze.units[maze.GetPos(i, j)] |= Wall.W;
                        }
                    }
                }

                void Output()
                {
                    int idx = 0;
                    for (int i = 0; i < w; ++i)
                    {
                        for (int j = 0; j < h; ++j)
                        {
                            switch (map[idx++])
                            {
                                case Map.Wall:
                                    Console.Write("#");
                                    break;
                                case Map.Hall:
                                    Console.Write(" ");
                                    break;
                            }
                        }
                        Console.Write("\n");
                    }
                    Console.Write("\n");
                }

                Direction[] RandomDirectionArray()
                {
                    Direction[] array = new Direction[] { Direction.N, Direction.E, Direction.S, Direction.W };
                    return array.OrderBy(_ => Guid.NewGuid()).ToArray();
                }

                bool IsWall(int x, int y)
                {
                    if (x < 0 || y < 0 || w <= x || h <= y)
                    {
                        return false;
                    }

                    return map[Pos(x, y)] == Map.Wall;
                }

                void Digg(int x, int y)
                {
                    foreach (Direction next_dir in RandomDirectionArray())
                    {
                        switch (next_dir)
                        {
                            case Direction.N:
                                if (IsWall(x, y - 2))
                                {
                                    map[Pos(x, y - 1)] = Map.Hall;
                                    map[Pos(x, y - 2)] = Map.Hall;

                                    Digg(x, y - 2);
                                }
                                break;
                            case Direction.E:
                                if (IsWall(x + 2, y))
                                {
                                    map[Pos(x + 1, y)] = Map.Hall;
                                    map[Pos(x + 2, y)] = Map.Hall;
                                    Digg(x + 2, y);
                                }
                                break;

                            case Direction.S:
                                if (IsWall(x, y + 2))
                                {
                                    map[Pos(x, y + 1)] = Map.Hall;
                                    map[Pos(x, y + 2)] = Map.Hall;
                                    Digg(x, y + 2);
                                }
                                break;

                            case Direction.W:
                                if (IsWall(x - 2, y))
                                {
                                    map[Pos(x - 1, y)] = Map.Hall;
                                    map[Pos(x - 2, y)] = Map.Hall;
                                    Digg(x - 2, y);
                                }
                                break;
                        }
                    }
                }
            }

            public static Maze CreateMaze(int sz_x, int sz_y)
            {
                Maze maze = new Maze(sz_x, sz_y);

                MazeBuilder builder = new MazeBuilder();
                builder.build(maze);

                return maze;
            }

            int size_x, size_y;
            Maze(int sz_x, int sz_y)
            {
                size_x = sz_x;
                size_y = sz_y;
                units = new Wall[sz_x * sz_y];
            }

            [Flags]
            enum Wall
            {
                None = 0,
                N = 1 << Direction.N,
                E = 1 << Direction.E,
                S = 1 << Direction.S,
                W = 1 << Direction.W
            }

            Wall[] units = null;

            int GetPos(int x, int y)
            {
                return y * size_x + x;
            }

            bool IsOpen(int from, int to)
            {
                int abs = Math.Abs(from - to);
                if (abs == 1)
                {
                    if (from < to)
                    {
                        if (units[from].HasFlag(Wall.E) || units[to].HasFlag(Wall.W))
                            return false;
                    }
                    else
                    {
                        if (units[from].HasFlag(Wall.W) || units[to].HasFlag(Wall.E))
                            return false;
                    }
                    return true;
                }
                else if (abs == size_x)
                {
                    if (from < to)
                    {
                        if (units[from].HasFlag(Wall.S) || units[to].HasFlag(Wall.N))
                            return false;
                    }
                    else
                    {
                        if (units[from].HasFlag(Wall.N) || units[to].HasFlag(Wall.S))
                            return false;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
