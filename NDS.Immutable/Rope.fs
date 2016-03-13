namespace NDS.Immutable
module ModRope =
    open System;
    open System.Text;

    type RopeNode = Nil | Leaf of string | Concat of int * RopeNode * RopeNode

    ///<summary>The empty rope.</summary>
    let empty = RopeNode.Nil

    ///<summary>Creates a rope from the given string.</summary>
    let ofString (s: String) =
        match s with
        | null -> Nil
        | s when s.Length = 0 -> Nil
        | s -> Leaf(s)

    ///<summary>Gets the length of a rope</summary>
    let length = function
    | Nil -> 0
    | Leaf(s) -> s.Length
    | Concat(len, _, _) -> len

    ///<summary>
    ///Gets the character at the given index in a rope. Returns None if the index is out of range.
    ///</summary>
    let rec charAt i = function
    | Nil -> None
    | Leaf(s) -> Some(s.[i])
    | Concat(len, l, r) ->
        match i with
        | _ when i < 0 -> None
        | _ when i >= len -> None
        | _ when i < (length l) -> charAt i l
        | _ -> charAt (i - (length l)) r

    ///<summary>Concats two ropes.</summary>
    let concat r1 r2 =
        match r1, r2 with
        | Nil, _ -> r2
        | _, Nil -> r1
        | _, _ -> let l = (length r1) + (length r2) in Concat(l, r1, r2)

    ///<summary>Splits a rope at the given index.</summary>
    ///<param name="i">The index to split at.</param>
    ///<param name="r">The rope to split.</param>
    ///<returns>
    ///A pair (left, right) where left contains a rope representing the string [0, i) and right contains
    ///a rope representing [i, (length r)) of the source rope <paramref name="r"/>.
    ///</returns>
    let rec splitAt i r =
        let len = (length r)
        match i with
        | 0 -> (Nil, r)
        | _ when i < 0 || i > len -> raise (new ArgumentOutOfRangeException())
        | _ when i = len -> (r, Nil)
        | _ -> 
            match r with
            | Nil -> failwith "should never happen!"
            | Leaf(s) -> let ls = s.Substring(0, i)
                         let rs = s.Substring(i)
                         (Leaf(ls), Leaf(rs))
            | Concat(len, l, r) ->
                let llen = length l
                if i = llen then (l, r)
                elif i < llen then
                    let (ll, lr) = splitAt i l
                    (ll, concat lr r)
                else
                    let (rl, rr) = splitAt (i - llen) r
                    (concat l rl, rr)

    ///<summary>Inserts a rope into another at the given index.</summary>
    let insertAt i toInsert r =
        let (left, right) = splitAt i r
        concat (concat left toInsert) right

    ///<summary>Removes a range [fromIndex, toIndex) from a rope.</summary>
    ///<param name="fromIndex">Start index of the range to remove.</param>
    ///<param name="toIndex">Exclusive end index of the range to remove.</param>
    ///<param name="r">The rope to remove from.</param>
    let removeRange fromIndex toIndex r =
        let (left, middle) = splitAt fromIndex r
        let (removed, right) = splitAt (toIndex - fromIndex) middle
        concat left right

    let rec private toStringAcc (sb: StringBuilder) r k =
        match r with
        | Nil -> k sb
        | Leaf(s) ->
            sb.Append(s) |> ignore
            k sb
        | Concat(_, l, r) -> toStringAcc sb l (fun sb' -> toStringAcc sb' r (fun sb'' -> k sb''))

    ///<summary>Converts a rope to a string.</summary>
    let toString r = toStringAcc (new StringBuilder()) r (fun sb -> sb.ToString())

///<summary>An immutable string type which supports efficient concatenation and insertion into and removal from the middle.</summary>
type public Rope private (root: ModRope.RopeNode) =
    ///<summary>Gets the length of this rope.</summary>
    member this.Length = ModRope.length root

    ///<summary>Gets the character at the given index within this rope.</summary>
    ///<param name="index">The index to retrieve</param>
    ///<returns>The character at index <paramref name="index"/> in this rope.</returns>
    ///<exception cref="IndexOutOfRange">If <paramref name="index"/> is not in the range [0, this.Length).</exception>
    member this.Item
        with get(index) =
            match (ModRope.charAt index root) with
            | None -> raise (new System.IndexOutOfRangeException())
            | Some(c) -> c

    ///<summary>Appends the given rope to this instance.</summary>
    member this.Append(other: Rope) = new Rope(ModRope.concat root other.Root)

    ///<summary>Prepends the given rope to the beginning of this rope.</summary>
    member this.Prepend(other: Rope) = new Rope(ModRope.concat other.Root root)

    ///<summary>Splits this rope at the given index.</summary>
    ///<param name="index">The index to split at.</param>
    ///<returns>A pair of ropes representing the ranges ([0, index), [index, this.Length)) of this instance.</returns>
    ///<exception cref="ArgumentOutOfRangeException">If <paramref name="index"> is not in the range [0, this.Length].
    member this.SplitAt(index) = let (lr, rr) = ModRope.splitAt index root in (new Rope(lr), new Rope(rr))

    ///<summary>Inserts a rope into this instance at the given index.</summary>
    ///<param name="index">The index to insert at</param>
    ///<param name="toInsert">The rope to insert.</param>
    ///<returns>A new rope representing [[0, index), toInsert, [index, this.Length)]</returns>
    member this.InsertAt(index, toInsert: Rope) = new Rope(ModRope.insertAt index toInsert.Root root)
    member this.RemoveRange(fromIndex, toIndex) = new Rope(ModRope.removeRange fromIndex toIndex root)

    ///<summary>Gets the string representation of this rope.</summary>
    override this.ToString() = ModRope.toString root
    member private this.Root = root

    ///<summary>The empty rope.</summary>
    static member Empty = new Rope(ModRope.empty)

    ///<summary>Creates a new rope from the given string.</summary>
    static member FromString(s: string) = new Rope(ModRope.ofString s)


