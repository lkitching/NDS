namespace NDS.Immutable
module Stack =

    type Stack<'T>
    val empty<'T> : Stack<'T>
    val push : 'T -> Stack<'T> -> Stack<'T>
    val pop : Stack<'T> -> ('T * Stack<'T>)
    val tryPop : Stack<'T> -> ('T * Stack<'T>) option
    val peek : Stack<'T> -> 'T
    val tryPeek : Stack<'T> -> 'T option

    val ofList : 'T list -> Stack<'T>