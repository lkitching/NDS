namespace NDS.Immutable
module ModQueue =
    type Queue<'T> = { front : 'T list; back : 'T list }
    let empty<'T> : Queue<'T> =  { front = []; back = [] }
    let isEmpty ({ front = front; back = back }) = List.isEmpty front && List.isEmpty back
    let enqueue e ({ front = front; back = back } as q) =
        match front with
        | [] -> 
            match back with
            | [] -> { front = [e]; back = []}
            | rev -> { front = (List.rev back); back = [e] }
        | _ -> { q with back = e :: back }

    let tryDequeue ({ front = front; back = back } as q) =
        match front with
        | (f::s::es) -> (Some(f), { q with front = (s :: es)})
        | [e] ->
            match back with
            | [] -> (Some(e), empty)
            | rev -> (Some(e), { front = List.rev rev; back = [] })
        | [] ->
            match back with
            | [] -> (None, q)
            | rev ->
                //items in back list are stored in reverse order
                let (e::es) = List.rev rev
                (Some(e), { front = es; back = []})

    let peek { front = front } =
        match front with
        | [] -> None
        | e::_ -> Some(e)

    let length ({ front = front; back = back }) =
        List.length front + List.length back

    let ofSeq s = { front = List.ofSeq s; back = [] }
    let toSeq ({ front = front; back = back}) =
        seq {
            yield! front
            yield! (List.rev back)
        }

type public Queue<'T> private (q : ModQueue.Queue<'T>) =
    member this.Enqueue(item: 'T) = new Queue<'T>(ModQueue.enqueue item q)
    static member Empty = new Queue<'T>(ModQueue.empty)
    static member OfSeq(s : 'T seq) = new Queue<'T>(ModQueue.ofSeq s)