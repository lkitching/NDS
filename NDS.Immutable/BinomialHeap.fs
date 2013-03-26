namespace NDS.Immutable
module BinomialHeap =
    type Node<'T when 'T : comparison> = Node of int * 'T * Node<'T> list
    type BinomialHeap<'T when 'T : comparison> =  H of Node<'T> list

    let empty<'T when 'T : comparison> = H([] : Node<'T> list)

    //gets the rank of a tree
    let private rank (Node(r, _, _)) = r
    let private value (Node(_, v, _)) = v

    //creates a tree of rank n+1 given two trees of rank n. The tree with the largest root element 
    //is made a child of the other to maintain heap order
    let private combine (Node(r1, x1, children1) as t1) (Node(_, x2, children2) as t2) =
        if x1 <= x2 then Node(r1+1, x1, t2 :: children1) else Node(r1+1, x2, t1 :: children2)

    //insert a tree into a heap. Trees in the heap are stored in reverse order of rank (i.e. lowest rank at the front of the list).
    //walk the list of trees and if a tree exists of the same rank, merge the two trees and insert that into the remaining list
    let rec private insertTree t heap =
        match (t, heap) with
        | (n, H([])) -> H([n])
        | (Node(rank, x, children) as t1, (H((Node(tr, _, _) as t2 :: ns) as ts))) ->
            if rank < tr then H(t1 :: ts) else (insertTree (combine t1 t2) (H(ns)))

    let insert elem heap = insertTree (Node(0, elem, [])) heap

    let tryFindMin = function
    | H([]) -> None
    | H(tl) -> List.map (fun (Node(_, e, _)) -> e) tl |> List.min |> Some

    let findMin = function
    | H([]) -> failwith "Empty heap"
    | h -> tryFindMin h |> Option.get

    //TODO: make tail recursive
    let rec private mergeHeapLists heap1 heap2 =
        match (heap1, heap2) with
        | ([], _) -> heap1
        | (_, []) -> heap2
        | (t1 :: ts1, t2 :: ts2) when rank t1 < rank t2 -> t1 :: (mergeHeapLists ts1 heap2)
        | (t1 :: ts1, t2 :: ts2) when rank t1 > rank t2 -> t2 :: (mergeHeapLists heap1 ts2)
        | (t1 :: ts1, t2 :: ts2) ->
            //both trees have the same rank. Combine them into a tree of rank n+1 and merge them into the remaining lists
            let mergedTree = combine t1 t2
            let mergedTail = mergeHeapLists ts1 ts2
            let mergedHeap = insertTree mergedTree (H(mergedTail))
            match mergedHeap with
            | H(l) -> l

    //TODO: rewrite using foldr?
    let rec private removeMinTree = function
    | H([]) -> failwith "Empty heap"
    | H([t]) -> (t, [])
    | H(Node(_, e1, _) as t1::ts) -> 
        let ((Node(_, e2, _)) as t2, ts') = removeMinTree (H(ts))
        if e1 < e2 then (t1, ts) else (t2, t1::ts')

    let removeMin heap =
        //find and remove the tree with the minimum root node from the heap.
        //the root of this tree is the minimum element in the heap (since trees have the heap property)
        //the child trees of this root node almost constitute a valid heap, except they are stored in reverse order
        //(higher-rank children occur before lower-ranked siblings)
        //to construct the new heap, reverse the child nodes of the min tree and merge the resulting heap with the 
        //list of trees remaining after removing the minimum tree
        match heap with
        | H([]) -> failwith "Empty heap"
        | H(ts) ->
            let (Node(_, v, children), ts') = removeMinTree heap
            let newHeapList = mergeHeapLists (List.rev children) ts'
            (v, H(newHeapList))

