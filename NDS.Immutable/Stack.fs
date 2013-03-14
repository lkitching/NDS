namespace NDS.Immutable
module Stack =

    type Stack<'a> = Empty | S of 'a * Stack<'a>

    let empty<'T> : Stack<'T> = Empty
    let push e s = S(e, s)
    let pop = function
    | Empty -> failwith "Cannot pop empty stack"
    | S(e, r) -> (e, r)

    let tryPop = function
    | Empty -> None
    | S(e, r) -> Some((e, r))

    let peek = function
    | Empty -> failwith "Cannot peek at empty stack"
    | S(e, _) -> e

    let tryPeek = function
    | Empty -> None
    | S(e, _) -> Some(e)

    let ofList l = List.foldBack (fun e s -> S(e, s)) l Empty