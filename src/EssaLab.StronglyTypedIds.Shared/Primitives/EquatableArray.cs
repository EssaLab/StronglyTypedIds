using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EssaLab.StronglyTypedIds.Shared.Primitives;

/// <summary>
/// An immutable, equatable array wrapper that provides efficient equality comparison based on content.
/// </summary>
/// <typeparam name="T">The type of elements in the array.</typeparam>
public readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IReadOnlyList<T>
    where T : IEquatable<T>
{
    private readonly T[]? _array;
    private readonly int _hashCode;

    public static EquatableArray<T> Empty { get; } = new EquatableArray<T>(Array.Empty<T>());

    public EquatableArray(T[]? array)
    {
        if (array == null || array.Length == 0)
        {
            _array = null;
            _hashCode = 0;
            return;
        }

        _array = array;
        
        var hash = 17;
        foreach (var item in array)
        {
            hash = hash * 31 + (item?.GetHashCode() ?? 0);
        }
        _hashCode = hash;
    }

    public int Count => _array?.Length ?? 0;

    public T this[int index] => _array![index];

    public bool Equals(EquatableArray<T> other)
    {
        if (_hashCode != other._hashCode) return false;
        if (Count != other.Count) return false;

        if (_array == null || other._array == null) 
            return _array == other._array;

        for (int i = 0; i < _array.Length; i++)
        {
            if (!_array[i].Equals(other._array[i])) return false;
        }

        return true;
    }

    public override bool Equals(object? obj) => obj is EquatableArray<T> other && Equals(other);

    public override int GetHashCode() => _hashCode;

    public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right) => left.Equals(right);
    public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right) => !left.Equals(right);

    public static implicit operator EquatableArray<T>(T[]? array) => new EquatableArray<T>(array);

    public Enumerator GetEnumerator() => new Enumerator(_array ?? Array.Empty<T>());

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => (IEnumerator<T>)GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<T>
    {
        private readonly T[] _array;
        private int _index;

        internal Enumerator(T[] array)
        {
            _array = array;
            _index = -1;
        }

        public bool MoveNext() => ++_index < _array.Length;
        public T Current => _array[_index];
        object? IEnumerator.Current => Current;
        public void Reset() => _index = -1;
        public void Dispose() { }
    }
}
