namespace NDS.Immutable
module LeftistHeap =
    type LeftistHeap<'T when 'T : comparison>

    val empty<'T when 'T : comparison> : LeftistHeap<'T>
    val insert<'T when 'T : comparison> : 'T -> 'T LeftistHeap -> 'T LeftistHeap
    val removeMin<'T when 'T : comparison> : 'T LeftistHeap -> 'T * 'T LeftistHeap
    val findMin<'T when 'T : comparison> : LeftistHeap<'T> -> 'T
    val tryFindMin<'T when 'T : comparison> : LeftistHeap<'T> -> 'T option

    val ofSeq<'T when 'T : comparison> : 'T seq -> 'T LeftistHeap
    val ofList<'T when 'T : comparison> : 'T list -> 'T LeftistHeap
