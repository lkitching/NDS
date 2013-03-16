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

    let rec tryFindMin = function
    | Empty -> None
    | Node(Empty, x, _) -> Some(x)
    | Node(l, _, _) -> tryFindMin l

    let rec tryFindMax = function
    | Empty -> None
    | Node(_, x, Empty) -> Some(x)
    | Node(_, _, r) -> tryFindMax r

    let private find<'T when 'T : comparison> (search : BinarySearchTree<'T> -> 'T option) = function
    | Empty -> failwith "Empty tree"
    | n -> (search >> Option.get) n

    let findMin t = find tryFindMin t
    let findMax t = find tryFindMax t

    let rec remove elem = function
    | Empty -> Empty
    | Node(left, x, right) when elem > x -> Node(left, x, remove elem right)
    | Node(left, x, right) when elem < x -> Node(remove elem left, x, right)
    | Node(Empty, _, Empty) -> Empty
    | Node(left, _, Empty) -> left
    | Node(Empty, _, right)-> right
    | Node(left, _, right) ->
        let x' = findMin right
        let r' = remove x' right
        Node(left, x', r')

    let ofList l = List.fold (fun bst e -> add e bst) Empty l
    let ofSeq s = Seq.fold (fun bst e -> add e bst) Empty s

    let fold<'a, 'T when 'T : comparison> (accumulator : 'a -> 'a -> 'T -> 'a) initial (tree : BinarySearchTree<'T>) =
        let rec foldk t k =
            match t with
            | Empty -> k initial
            | Node(l, x, r) -> 
                foldk l (fun lr ->
                    foldk r (fun rr ->
                        k (accumulator lr rr x)))

        foldk tree id

    let count<'T when 'T : comparison> (t : BinarySearchTree<'T>) = fold (fun lc rc _ -> lc + rc + 1) 0 t

    let private traverse yieldFun tree = fold yieldFun Seq.empty tree

    let traverseInOrder tree = traverse (fun ls rs x -> seq { yield! ls; yield x; yield! rs }) tree
    let traversePreOrder tree = traverse (fun ls rs x -> seq { yield x; yield! ls; yield! rs }) tree
    let traversePostOrder tree = traverse (fun ls rs x -> seq { yield! ls; yield! rs; yield x }) tree
    let toSeq tree = traverseInOrder tree

    let filter p tree = tree |> traversePreOrder |> Seq.filter p |> ofSeq

    
                    

   
    

