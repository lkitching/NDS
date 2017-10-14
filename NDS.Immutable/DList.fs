namespace NDS.Immutable

module ModDList =
    /// <summary>
    /// Represents a list as a function which constructs a list by appending it to the front
    /// of the argument list.
    /// </summary>
    type DList<'T> = DL of ('T list -> 'T list)

    /// <summary>Construct a DList from the given list.</summary>
    let ofList l = DL (fun l' -> List.append l l')

    /// <summary>Materialises a DList to a list.
    let toList (DL f) = f []

    /// <summary>Emtpy DList.</summary>
    let empty = DL id

    /// <summary>Appends two DLists.
    let append (DL f) (DL g) = DL (fun l' -> f (g l'))

    /// <summary>Constructs a DList containing the given value.
    let singleton x = DL (fun l' -> x :: l')

    /// <summary>Appends the given item to the front of a DList.
    let cons x (DL f) = DL (fun l' -> x :: (f l'))

    /// <summary>Appends the given item to the end of a DList.
    let snoc x (DL f) = DL (fun l' -> f (x :: l'))
    let head dl =
        match toList dl with
        | [] -> failwith "Empty difference list"
        | x::_ -> x
    let tail dl =
        match toList dl with
        | [] -> failwith "Empty difference list"
        | _::xs -> ofList xs

    /// <summary>Concatenates a sequence of DLists into a single DList.
    let concat dls = dls |> Seq.fold append empty

    /// <summary>Creates a DList of <paramref name="e"/> replicated <paramref name="n"> times.
    /// <param name="n">The number of replications.</param>
    /// <param name="e">The value to replicate.</param>
    let replicate n e =
        let rec aux l n =
            if n <= 0 then l else aux (e :: l) (n-1)
        DL (fun l -> aux l n)

    /// <summary>Transforms the elements of a DList by a transform function.</summary>
    let map f dl : DList<'b> = 
        DL (fun l' -> List.map f (toList dl))
    let fold f init dl = List.fold f init (toList dl)

    /// <summary>
    /// Generates a DList from an initial value and a function which calculates the
    /// next value and state if one exists. The generation is terminated by returning
    /// None.
    /// <param name="f">Function to generate next (value, state).</param>
    /// <param name="init">Initial state.</param>
    let rec unfold f init =
        match f init with
        | None -> empty
        | Some(e, state) -> cons e (unfold f state)

type public DList<'T> private (dl : ModDList.DList<'T>) =
    member private this.DList = dl
    member this.ToFSList = ModDList.toList dl   
    member this.Append (other : DList<'T>) = new DList<_>(ModDList.append dl other.DList)
    member this.Cons(x : 'T) = new DList<_>(ModDList.cons x dl)
    member this.Snoc(x : 'T) = new DList<_>(ModDList.snoc x dl)
    member this.Head() = ModDList.head dl
    member this.Tail() = new DList<_>(ModDList.tail dl)
    member this.Map(f) = new DList<_>(ModDList.map f dl)
    member this.Fold(f, init) = ModDList.fold f init dl
    
    static member Empty with get() = new DList<_>(ModDList.empty)
    static member Concat(dls : DList<'T> seq) = new DList<_>(dls |> Seq.map(fun dl -> dl.DList) |> ModDList.concat)
    static member Unfold(f, init) = new DList<_>(ModDList.unfold f init)
    static member Replicate(n, e) = new DList<_>(ModDList.replicate n e)


