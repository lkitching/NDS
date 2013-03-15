namespace NDS.Immutable
module BinarySearchTree =
    type BinarySearchTree<'T> when 'T : comparison

    val empty<'T> : BinarySearchTree<'T> when 'T : comparison
    val add<'T when 'T : comparison> : 'T -> BinarySearchTree<'T> -> BinarySearchTree<'T>
    val contains<'T when 'T : comparison> : 'T -> BinarySearchTree<'T> -> bool

