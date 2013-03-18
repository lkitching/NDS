namespace NDS.Immutable
module LeftistHeap =
    open Utils
    type LeftistHeap<'T when 'T : comparison> = Empty | Node of int * 'T * LeftistHeap<'T> * LeftistHeap<'T>

    let empty<'T when 'T : comparison> : LeftistHeap<'T> = Empty

    let private rank = function Empty -> 0 | Node(r, _, _, _) -> r
    let private restoreLeftist x n1 n2 =
        let rankN1 = rank n1
        let rankN2 = rank n2
        if rankN1 >= rankN2 then Node(rankN2 + 1, x, n1, n2) else Node(rankN1 + 1, x, n2, n1)

    let rec private merge h1 h2 =
        match h1, h2 with
        | (Empty, _) -> h2
        | (_, Empty) -> h1
        | (Node(_, x, l1, r1), Node(_, y, l2, r2)) ->
            if x <= y then restoreLeftist x l1 (merge r1 h2) else restoreLeftist y l2 (merge h1 r2)

    let insert elem h = merge (Node(1, elem, Empty, Empty)) h
    let removeMin = function
    | Empty -> failwith "Empty heap"
    | Node(_, x, l, r) -> (x, merge l r)

    let tryFindMin = function
    | Empty -> None
    | Node(_, x, _, _) -> Some(x)

    let findMin = function
    | Empty -> failwith "Empty heap"
    | n -> (tryFindMin >> Option.get) n

    let ofSeq s = Seq.fold (flip insert) Empty s
    let ofList l = List.fold (flip insert) Empty l

