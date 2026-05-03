namespace Zenith.Utility.Extensions;

public static class EnumerableExtensions {
    public static IEnumerable<(int Index, T Item)> Enumerate<T>(this IEnumerable<T> self)
        => Enumerable.Range(0, self.Count()).Zip(self);

    public static void ForEach<T>(this IEnumerable<T> self, Action<T> action) {
        foreach (T item in self) action(item);
    }

    public static IEnumerable<TResult> SelectWhere<TSource, TResult>(this IEnumerable<TSource> self, Func<TSource, TResult> action) {
        return self.SelectMany(x => {
            var result = action(x);
            return result == null ? Enumerable.Empty<TResult>() : [result];
        });
    }

    public static IEnumerable<(T, T)> Window2<T>(this IEnumerable<T> self) {
        if (!self.Any()) { yield break; }
        var previous = self.First();
        self = self.Skip(1);
        while (self.Any()) {
            var current = self.First();
            yield return (previous, current);
            previous = current;
            self = self.Skip(1);
        }
    }
}
