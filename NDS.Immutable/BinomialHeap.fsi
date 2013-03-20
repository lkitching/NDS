namespace NDS.Immutable
module BinomialHeap =
    type BinomialHeap<'T when 'T : comparison>
    val empty<'T when 'T : comparison> : 'T BinomialHeap

    val insert<'T when 'T : comparison> : 'T -> 'T BinomialHeap -> 'T BinomialHeap
