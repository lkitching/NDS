namespace NDS.Immutable
module BinarySearchTree =
    type BinarySearchTree<'T> when 'T : comparison

    val empty<'T> : BinarySearchTree<'T> when 'T : comparison
    val add<'T when 'T : comparison> : 'T -> BinarySearchTree<'T> -> BinarySearchTree<'T>
    val contains<'T when 'T : comparison> : 'T -> BinarySearchTree<'T> -> bool
    val remove<'T when 'T : comparison> : 'T -> BinarySearchTree<'T> -> BinarySearchTree<'T>
    val filter<'T when 'T : comparison> : ('T -> bool) -> BinarySearchTree<'T> -> BinarySearchTree<'T>
    val ofList<'T when 'T : comparison> : 'T list -> BinarySearchTree<'T>
    val ofSeq<'T when 'T : comparison> : 'T seq -> BinarySearchTree<'T>

    val findMin<'T when 'T : comparison> : BinarySearchTree<'T> -> 'T
    val tryFindMin<'T when 'T : comparison> : BinarySearchTree<'T> -> 'T option
    val findMax<'T when 'T : comparison> : BinarySearchTree<'T> -> 'T
    val tryFindMax<'T when 'T : comparison> : BinarySearchTree<'T> -> 'T option

    val fold<'a, 'T when 'T : comparison> : ('a -> 'a -> 'T -> 'a) -> 'a -> BinarySearchTree<'T> -> 'a
    val count<'T when 'T : comparison> : BinarySearchTree<'T> -> int
    val traverseInOrder<'T when 'T : comparison> : BinarySearchTree<'T> -> 'T seq
    val traversePreOrder<'T when 'T : comparison> : BinarySearchTree<'T> -> 'T seq
    val traversePostOrder<'T when 'T : comparison> : BinarySearchTree<'T> -> 'T seq
    val toSeq<'T when 'T : comparison> : BinarySearchTree<'T> -> 'T seq
