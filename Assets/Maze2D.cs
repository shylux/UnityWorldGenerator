using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Maze2D {
	int width, length;
	bool[,] cells, walls;


	public Maze2D(int _width, int _length) {
		width = _width;
		length = _length;

		cells = new bool[width, length];
		walls = new bool[width + 2, 2 * length + 2];
	}

	public Cell cell(int x,int  y) {return new Cell(this, x, y);}
	public Wall wall(int x,int  y) {return new Wall(this, x, y);}

	public class Element: IEquatable<Element> {
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
	}
	public class Cell: Element, IEquatable<Cell> {
		public Cell (Maze2D _parent, int _x, int _y): base(_parent, _x, _y) {}
		public bool Equals(Cell c) {return base.Equals (c);}
		public void set(bool newState) {
			parent.cells [x, y] = newState;
		}
		public bool get() {
			return parent.cells [x, y];
		}
	}

	public class Wall: Element, IEquatable<Wall> {
		public Wall (Maze2D _parent, int _x, int _y): base(_parent, _x, _y) {}
		public bool Equals(Wall w) {return base.Equals (w);}
		public void set(bool newState) {
			parent.walls [x, y] = newState;
		}
		public bool get() {
			return parent.walls [x, y];
		}
		public Cell getAdjacentCell(int difx, int dify) {

		}
	}
	
	public IEnumerable<Cell> Cells() {
		for (int x = 0; x < cells.GetUpperBound(0); x++) {
			for (int y = 0; y < cells.GetUpperBound(1); y++) {
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
}

