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
}