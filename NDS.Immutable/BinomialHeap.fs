namespace NDS.Immutable
module BinomialHeap =
    type Node<'T when 'T : comparison> = Node of int * 'T * Node<'T> list
    type BinomialHeap<'T when 'T : comparison> =  H of Node<'T> list

    let empty<'T when 'T : comparison> = H([] : Node<'T> list)

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
        

