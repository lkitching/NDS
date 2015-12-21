using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace NDS 
{
    /// <summary>Represents an optional value of some type <typeparamref name="T"/> The default constructor is used to create empty instances with no value.</summary>
    /// <typeparam name="T">The type of the encapsulated value.</typeparam>
    public struct Maybe<T> : IEquatable<Maybe<T>>, IEnumerable<T>
    {
        private readonly bool hasValue;
        private readonly T value;

        /// <summary>Creates a new instance with a value.</summary>
        /// <param name="value">The encapsulated value.</param>
        /// <exception cref="ArgumentNullException">If the inner value is null.</exception>
        public Maybe(T value)
        {
            if (value == null) throw new ArgumentNullException("value");
            Contract.EndContractBlock();

            this.hasValue = true;
            this.value = value;
        }

        /// <summary>Gets the value from this instance if it exists, otherwise returns the given default value.</summary>
        /// <param name="default">The default value to return if this instance has no value.</param>
        /// <returns>The value from this mabye if it exists, otherwise <paramref name="@default"/>.</returns>
        public T GetOr(T @default = default(T)) { return this.hasValue ? this.value : @default; }

        /// <summary>Gets the value from this instance it it exists, otherwise returns the value returned from the given delegate.</summary>
        /// <param name="defaultFunc">Delegate to create the default value to return if this instance has none.</param>
        /// <returns>The value for this instance if it exists otherwise the value returned from <paramref name="defaultFunc"/>.</returns>
        public T GetOr(Func<T> defaultFunc)
        {
            Contract.Requires(defaultFunc != null);
            return this.hasValue ? this.value : defaultFunc();
        }

        /// <summaryGets whether this instance has a value.</summary>
        public bool HasValue { get { return this.hasValue; } }

        /// <summary>Gets the value from this instance if it exists.</summary>
        /// <exception cref="InvalidOperationException">If this instance has no value.</exception>
        public T Value
        {
            get
            {
                if (this.hasValue) return this.value;
                else throw new InvalidOperationException("Cannot get value for empty Maybe");
            }
        }

        /// <summary>Maps the value inside this instance with the given transform.</summary>
        /// <typeparam name="U">The type returned from <paramref name="mapFunc"/>.</typeparam>
        /// <param name="mapFunc">Delegate to transform the value for this instance if it exists.</param>
        /// <returns></returns>
        public Maybe<U> Select<U>(Func<T, U> mapFunc)
        {
            Contract.Requires(mapFunc != null);
            return this.hasValue ? new Maybe<U>(mapFunc(this.value)) : new Maybe<U>();
        }

        /// <summary>Filters the value in this instance with the given filter delegate.</summary>
        /// <param name="predicate">Delegate to filter the value encapsulated by this instance.</param>
        /// <returns>An empty maybe instance if this instance is empty or if the inner value fails the predicate. Otherwise an instance encapsulating the same inner value.</returns>
        public Maybe<T> Where(Func<T, bool> predicate)
        {
            Contract.Requires(predicate != null);
            return (!this.hasValue || predicate(this.value)) ? this : new Maybe<T>();
        }

        /// <summary>Monadic bind operation. Applies the given delegate to the value of this instance if it exists.</summary>
        /// <typeparam name="U">The type of value encapsulated in the maybe returned from <paramref name="bindFunc"/>.</typeparam>
        /// <param name="bindFunc">Delegate for the bind operation.</param>
        /// <returns>The value returned from <paramref name="bindFunc"/> if this instance has a value. Otherwise another empty instance is returned.</returns>
        public Maybe<U> SelectMany<U>(Func<T, Maybe<U>> bindFunc)
        {
            Contract.Requires(bindFunc != null);
            return this.hasValue ? bindFunc(this.value) : new Maybe<U>();
        }

        /// <summary>Extended monadic bind operator required for supporting the query pattern.</summary>
        /// <typeparam name="U">Type for the intermediate maybe value returned by <paramref name="bindFunc"/>.</typeparam>
        /// <typeparam name="R">Type for the value returned from the transform delegate <paramref name="selector"/>.</typeparam>
        /// <param name="bindFunc">Delegate for the bind operation.</param>
        /// <param name="selector">Delegate for the subsequent map transform.</param>
        /// <returns></returns>
        public Maybe<R> SelectMany<U, R>(Func<T, Maybe<U>> bindFunc, Func<T, U, R> selector)
        {
            Contract.Requires(bindFunc != null);
            Contract.Requires(selector != null);

            if (this.hasValue)
            {
                T val = this.value;
                return bindFunc(this.value).Select(u => selector(val, u));
            }
            else return new Maybe<R>();
        }

        /// <summary>Casts the inner value for this instance if it exists.</summary>
        /// <typeparam name="U">The type to cast the inner value to.</typeparam>
        /// <returns>A maybe encapsulating a value of the converted type.</returns>
        /// <exception cref="ClassCastException">If the inner value cannot be cast to type <typeparamref name="U"/>.</exception>
        public Maybe<U> Cast<U>()
        {
            return this.hasValue ? new Maybe<U>((U)(object)this.value) : new Maybe<U>();
        }

        /// <summary>Converts the inner value of this instance if it exists and can safely be converted to the given type.</summary>
        /// <typeparam name="U">The type to convert the inner value to.</typeparam>
        /// <returns>
        /// An empty maybe if this instance is empty, or if the inner value cannot be safely converted to type <typeparamref name="U"/>.
        /// Otherwise, returns a maybe encapsulating the inner converted value.
        /// </returns>
        public Maybe<U> OfType<U>()
        {
            if (this.hasValue && this.value is U) return new Maybe<U>((U)(object)this.value);
            else return new Maybe<U>();
        }

        /// <summary>Gets an enumerator for this instance. If this instance is empty then returned sequence is empty, otherwise the sequence contains the inner value.</summary>
        /// <returns>An enumerator for the sequence represented by this instance.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (this.hasValue)
            {
                yield return this.value;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>Override of Object.Equals.</summary>
        /// <param name="obj">The object to compare against this instance.</param>
        /// <returns>True if <paramref name="obj"/> is the same type as this instance and is equal according to <see cref="Maybe{T}.Equals(Maybe{T})"/>.</returns>
        public override bool Equals(object obj)
        {
            return (obj is Maybe<T>) && this.Equals((Maybe<T>)obj);
        }

        /// <summary>Compares this instance for equality to another maybe value.</summary>
        /// <param name="other">The maybe to compare with this instance.</param>
        /// <returns>True if this and <paramref name="other"/> are both empty, or if they both have values and the values are equal. Otherwise false.</returns>
        public bool Equals(Maybe<T> other)
        {
            if (other.hasValue != this.hasValue) return false;
            else if (other.hasValue && this.hasValue) return EqualityComparer<T>.Default.Equals(this.value, other.value);
            else return true;
        }

        /// <summary>Override of Object.GetHashCode.</summary>
        /// <returns>A hash code for this instance. Empty values for different types are not guarenteed to return the same hash code.</returns>
        public override int GetHashCode()
        {
            return this.hasValue ? this.value.GetHashCode() : typeof(T).GetHashCode();
        }

        /// <summary>Override of Object.ToString.</summary>
        /// <returns>A description of the value if one exists, otherwise "None".</returns>
        public override string ToString()
        {
            return this.hasValue ? string.Format("Some({0})", this.value) : "None";
        }
    }

    /// <summary>Utility class for creating <see cref="Maybe{T}"/> values.</summary>
    public static class Maybe
    {
        /// <summary>Creates an empty maybe of the given type.</summary>
        /// <typeparam name="T">The type of maybe to create.</typeparam>
        /// <returns>An empty maybe with the given value type.</returns>
        public static Maybe<T> None<T>() { return new Maybe<T>(); }

        /// <summary>Creates a maybe containing the given value.</summary>
        /// <typeparam name="T">The inner value type.</typeparam>
        /// <param name="value">The value of the maybe.</param>
        /// <returns>A maybe containing <paramref name="value"/>.</returns>
        public static Maybe<T> Some<T>(T value)
        {
            Contract.Requires(value != null);
            return new Maybe<T>(value);
        }
    }

    /// <summary>Extension methods for <see cref="Maybe{T}"/>.</summary>
    public static class MaybeExtensions
    {
        /// <summary>Applicative instance for maybes.</summary>
        /// <typeparam name="T">Argument type for the delegate encapsulated by <paramref name="mf"/>.</typeparam>
        /// <typeparam name="U">Result type for the delegate encapsulated by <paramref name="mf"/>.</typeparam>
        /// <param name="mf">Maybe instance with an inner delegate to apply.</param>
        /// <param name="mv">A maybe instance with an inner argument to the encapsulated delegate.</param>
        /// <returns>
        /// If <paramref name="mf"/> and <paramref name="mv"/> both have values, then an maybe instance encapsulating the delegate to the value in <paramref name="mv"/>.
        /// Otherwise an empty maybe.
        /// </returns>
        public static Maybe<U> Ap<T, U>(this Maybe<Func<T, U>> mf, Maybe<T> mv)
        {
            return mf.HasValue && mv.HasValue ? new Maybe<U>(mf.Value(mv.Value)) : new Maybe<U>();
        }

        /// <summary>Collapses a nested maybe value.</summary>
        /// <typeparam name="T">The type of the value for the inner maybe.</typeparam>
        /// <param name="mm">The nested maybe to collapse.</param>
        /// <returns>The inner maybe instance for <paramref name="mm"/>.</returns>
        public static Maybe<T> Join<T>(this Maybe<Maybe<T>> mm)
        {
            return mm.HasValue ? mm.Value : new Maybe<T>();
        }

        /// <summary>Converts a nullable struct to a maybe instance.</summary>
        /// <typeparam name="T">The inner struct type.</typeparam>
        /// <param name="n">The nullable value to convert.</param>
        /// <returns>An empty maybe if <paramref name="n"/> is null, otherwise a maybe instance encapsulating the inner value.</returns>
        public static Maybe<T> ToMaybe<T>(this T? n) where T : struct
        {
            return n.HasValue ? new Maybe<T>(n.Value) : new Maybe<T>();
        }

        /// <summary>Wraps a value in a <see cref="Maybe{T}"/>.</summary>
        /// <typeparam name="T">The value to wrap.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <returns>An empty maybe if <paramref name="value"/> is null, otherwise a maybe instance encapsulating the inner value.</returns>
        public static Maybe<T> ToMaybe<T>(this T value)
        {
            return value == null ? new Maybe<T>() : new Maybe<T>(value);
        }

        /// <summary>Converts the value of the given maybe to a nullable value type.</summary>
        /// <typeparam name="T">The value type of the maybe.</typeparam>
        /// <param name="maybe">The maybe.</param>
        /// <returns>Null if <paramref name="maybe"/> is empty, otherwise the inner value.</returns>
        public static T? ToNullable<T>(this Maybe<T> maybe) where T : struct
        {
            return maybe.HasValue ? maybe.Value : (T?)null;
        }

        /// <summary>Gets the value from the given maybe, or null if there is no value.</summary>
        /// <typeparam name="T">The value type of the maybe.</typeparam>
        /// <param name="maybe">The mabye.</param>
        /// <returns>Null if <paramref name="maybe"/> is empty, otherwise the non-null reference value.</returns>
        public static T ToClass<T>(this Maybe<T> maybe) where T : class
        {
            return maybe.GetOr((T)null);
        }
    }

    public class MaybeEqualityComparer<T> : IEqualityComparer<Maybe<T>>
    {
        private readonly IEqualityComparer<T> valueComparer;

        public MaybeEqualityComparer() : this(EqualityComparer<T>.Default) { }

        public MaybeEqualityComparer(IEqualityComparer<T> valueComparer)
        {
            this.valueComparer = valueComparer;
        }

        public bool Equals(Maybe<T> x, Maybe<T> y)
        {
            if (x.HasValue)
            {
                return y.HasValue && this.valueComparer.Equals(x.Value, y.Value);
            }
            else
            {
                return !y.HasValue;
            }
        }

        public int GetHashCode(Maybe<T> obj)
        {
            return obj.HasValue ? this.valueComparer.GetHashCode(obj.Value) : 1;
        }
    }
}
