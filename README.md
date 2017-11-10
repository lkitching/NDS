# NDS
Package of mutable and immutable data structures along with associated algorithms. This library attempts to avoid some of the problems associated with the .Net BCL namely:

* Overly-large interfaces (e.g. IList&lt;T>)
* Coupling of algorithms to the data structures they operate over (e.g. List&lt;T>.BinarySearch)
* Missing commonly-used structures (e.g. priority queues, graphs).

while maintaining the following design goals

* Eliminate duplication
* Avoid exceptions (e.g. KeyNotFoundException) in favour of more descriptive return types like Maybe, Either etc. Exceptions may be thrown for invalid arguments.
* Enable testing of whole process of invariant maintentance. OO encourages hiding the invariant maintentance process of a data structure in private methods and testing only the interface.
* Use property-based tests where possible.
