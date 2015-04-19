using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Maze2D {
	int width, length;
	bool[,] cells, walls;

    public static List<Vector2> DIRECTIONS = new List<Vector2> { Vector2.right, Vector2.up, -Vector2.right, -Vector2.up };

	public Maze2D(int _width, int _length) {
		width = _width;
		length = _length;

		cells = new bool[width, length];
		walls = new bool[width + 2, 2 * length + 2];
	}

	public Cell cell(int x,int  y) {
        if (x < 0 || y < 0 || x > cells.GetUpperBound(0) || y > cells.GetUpperBound(1)) return null;
        return new Cell(this, x, y);
    }
	public Wall wall(int x,int  y) {
        if (x < 0 || y < 0 || x > walls.GetUpperBound(0) || y > walls.GetUpperBound(1)) return null;
        if (y % 2 == 0 && x > walls.GetUpperBound(0) - 1) return null;
        return new Wall(this, x, y);
    }
    public Cell mcell(int x, int y) { return new Cell(this, x, y); }
    public Wall mwall(int x, int y) { return new Wall(this, x, y); }

    public class Element : IEquatable<Element> {
		protected Maze2D parent;
		public int x, y;
		public Element(Maze2D _parent, int _x, int _y) {
			parent = _parent;
			x = _x;
			y = _y;
		}
		public bool Equals(Element p) {
			if ((object)p == null) {return false;}
			return (x == p.x) && (y == p.y);
		}
        public string ToString() {
            return "x:" + x + " y:" + y;
        }
	}
	public class Cell: Element, IEquatable<Cell> {
		public Cell (Maze2D _parent, int _x, int _y): base(_parent, _x, _y) {
            if (x < 0) x += parent.cells.GetUpperBound(0);
            if (y < 0) y += parent.cells.GetUpperBound(1);
        }
		public bool Equals(Cell c) {return base.Equals (c);}
		public void set(bool newState) {
			parent.cells [x, y] = newState;
		}
		public bool get() {
			return parent.cells [x, y];
		}
        public Maze2D.Wall getWall(Vector2 direction) {
            Wall w = parent.wall(x + Mathf.Max((int)direction.x, 0),
                                 2 * (y +  Mathf.Max((int)direction.y, 0)));
            if (direction.x != 0) w.y += 1;
            return w;
        }
        public static Cell operator +(Cell cell, Vector2 direction) {
            return cell.parent.cell(cell.x + (int)direction.x, cell.y + (int)direction.y);
        }
        public string ToString() {
            return "Cell<" + base.ToString() + ">";
        }
	}

	public class Wall: Element, IEquatable<Wall> {
		public Wall (Maze2D _parent, int _x, int _y): base(_parent, _x, _y) {
            if (y < 0) y += parent.walls.GetUpperBound(1);
            if (x < 0) {
                x += parent.walls.GetUpperBound(0);
                if (y % 2 == 0) x -= 1; // one wall less on horizontal
            }
        }
		public bool Equals(Wall w) {return base.Equals (w);}
		public void set(bool newState) {
			parent.walls [x, y] = newState;
		}
		public bool get() {
			return parent.walls [x, y];
		}
		public Cell getAdjacentCell(int awayFromRoot) {
			if (y == 0 && awayFromRoot == 0) return null;
            int rx, ry;
            if (y % 2 == 0) { // horizontal
                rx = x;
                ry = (int)Mathf.Floor(y / 2f) + awayFromRoot - 1;
            } else { // vertical
                if (x == 0 && awayFromRoot == 0) return null;
                rx = x - 1 + awayFromRoot;
                ry = (int)Mathf.Floor(y / 2f);
            }
			if (rx > parent.cells.GetUpperBound (0) || ry > parent.cells.GetUpperBound (1)) return null;
			return parent.cell (rx, ry);
		}
        public string ToString() {
            return "Wall<" + base.ToString() + ">";
        }
	}
	
	public IEnumerable<Cell> Cells() {
		for (int x = 0; x <= cells.GetUpperBound(0); x++) {
			for (int y = 0; y <= cells.GetUpperBound(1); y++) {
				yield return new Cell(this, x, y);
			}
		}
	}
	public IEnumerable<Wall> Walls() {
		for (int x = 0; x < walls.GetUpperBound(0); x++) {
			for (int y = 0; y < walls.GetUpperBound(1); y++) {
				if (y % 2 == 0 && walls.GetUpperBound (0) - 1 == x)
					continue;
				yield return new Wall(this, x, y);
			}
		}
	}

    public void Shuffle<Element>(this IList<Element> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            Element value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

}

