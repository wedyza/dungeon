using System;
using System.Linq;

namespace Dungeon;

public class DungeonTask
{
	public static MoveDirection[] FindShortestPath(Map map)
	{
		var start = map.InitialPosition;
		var end = map.Exit;
		var chests = map.Chests;

		var routeToExit = BfsTask.FindPaths(map, start, new Point[] { end }).FirstOrDefault();
		if (routeToExit == null) return new MoveDirection[0];
		var toExit = routeToExit.ToList();
		toExit.Reverse();

		if (chests.Any(c => toExit.Contains(c)))
			return toExit.Zip(toExit.Skip(1), Move).ToArray();
		var startToChests = BfsTask.FindPaths(map, start, chests);
		var exitToChests = BfsTask.FindPaths(map, end, chests).DefaultIfEmpty();

		if (startToChests.FirstOrDefault() == null)
			return toExit.Zip(toExit.Skip(1), Move).ToArray();
		var startToExit = startToChests.Join(exitToChests, f => f.Value, s => s.Value, (f, s) =>
		new
		{
            Length = f.Length + s.Length,
			listFinish = f.ToList(),
			listStart = s.ToList()
		});

		var listsTuple = startToExit.OrderBy(l => l.Length)
			.Select(v => Tuple.Create(v.listFinish, v.listStart))
			.First();
		listsTuple.Item1.Reverse();
		listsTuple.Item1.AddRange(listsTuple.Item2.Skip(1));

		return listsTuple.Item1.Zip(listsTuple.Item1.Skip(1), Move).ToArray();
	}

    private static MoveDirection Move(Point start, Point finish)
    {
        var d = new Point(finish.X - start.X, finish.Y - start.Y);
        return d.X == 1 ? MoveDirection.Right : d.X == 0 ? d.Y == 1 ? MoveDirection.Down : MoveDirection.Up : MoveDirection.Left;
    }
}