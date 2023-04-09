using System.Collections.Generic;
using System.Linq;

namespace Dungeon;

public class BfsTask
{
	public static IEnumerable<SinglyLinkedList<Point>> FindPaths(Map map, Point start, Point[] chests)
	{
		var queue = new Queue<SinglyLinkedList<Point>>();
		var visited = new HashSet<Point>();
		var hashChest = new HashSet<Point>(chests);
		var pointStart = new SinglyLinkedList<Point>(start, null);
		var route = Walker.PossibleDirections.ToList();
		queue.Enqueue(pointStart);
		while (queue.Count > 0)
		{
			var path = queue.Dequeue();
			var point = path.Value;
			if (map.InBounds(point) && map.Dungeon[point.X, point.Y] != 0)
			{
				if (hashChest.Contains(point)) yield return path;
				foreach (var step in route)
				{
					var p = new Point(point.X + step.X, point.Y + step.Y);
					if (!visited.Contains(p))
					{
						visited.Add(p);
						queue.Enqueue(new SinglyLinkedList<Point>(p, path));
					}
				}
			}
		}
	}
}