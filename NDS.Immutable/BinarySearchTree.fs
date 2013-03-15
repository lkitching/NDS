namespace NDS.Immutable
module BinarySearchTree =
    type BinarySearchTree<'T> = Empty | Node of BinarySearchTree<'T> * 'T * BinarySearchTree<'T>

    let empty<'T> : BinarySearchTree<'T> = Empty

    let rec add (elem: 'T) = function
    | Empty -> Node(Empty, elem, Empty)
    | Node(left, x, right) when elem > x -> Node(left, x, add elem right)
    | Node(left, x, right) when elem < x -> Node(add elem left, x, right)
    | Node(_, _, _) as n -> n

    let rec contains elem = function
    | Empty -> false
    | Node(_, x, _) when elem = x -> true
    | Node(_, x, right) when elem > x -> contains elem right
    | Node(left, x, _) -> contains elem left
    

