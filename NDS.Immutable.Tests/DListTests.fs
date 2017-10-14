namespace NDS.Immutable.Tests

open NUnit.Framework
open FsCheck

[<TestFixture>]
module DListTests =
    open NDS.Immutable.ModDList

    [<Test>]
    let ``empty is empty`` () =
        Assert.AreEqual([], toList empty)

    [<Test>]
    let ``toList ofList is id`` () =
        let p (xs : int list) = xs = toList (ofList xs)
        Check.QuickThrowOnFailure p

    [<Test>]
    let ``append`` () =
        let p (l1 : int list, l2 : int list) = toList (append (ofList l1) (ofList l2)) = l1 @ l2
        Check.QuickThrowOnFailure p

    [<Test>]
    let ``singleton`` () =
        let p (i : int) = toList (singleton i) =  [i]
        Check.QuickThrowOnFailure p

    [<Test>]
    let ``cons`` () =
        let p (i : int, xs : int list) = toList (cons i (ofList xs)) = i :: xs
        Check.QuickThrowOnFailure p

    [<Test>]
    let ``snoc`` () =
        let p (i : int, xs : int list) = toList (snoc i (ofList xs)) = xs @ [i]
        Check.QuickThrowOnFailure p

    [<Test>]
    let ``head non empty`` () =
        let p (i : int, xs : int list) = head (ofList (i::xs)) = i
        Check.QuickThrowOnFailure p

    [<Test>]
    let ``tail non empty`` () =
        let p (i : int, xs : int list) = toList (tail (ofList (i::xs))) = xs
        Check.QuickThrowOnFailure p

    [<Test>]
    let ``concat`` () =
       let p (xss : int list list) = toList (concat (List.map ofList xss)) = List.concat xss
       Check.QuickThrowOnFailure p

    [<Test>]
    let ``replicate`` () =
        let p (n : NonNegativeInt, s : string) = toList (replicate n.Get s) = List.replicate n.Get s
        Check.QuickThrowOnFailure p

    [<Test>]
    let ``map`` () =
        let f (s : string) =
            match s with
            | null -> 0
            | _ -> s.Length
            
        let p (xs : string list) = toList (NDS.Immutable.ModDList.map f (ofList xs)) = List.map f xs
        Check.QuickThrowOnFailure p

    [<Test>]
    let ``fold`` () =
        let p (xs : int list) = fold (+) 0 (ofList xs) = List.fold (+) 0 xs
        Check.QuickThrowOnFailure p

    let makeUnfolder (l : int list) =
        let mutable lr = l
        fun (i : int) ->
            match lr with
            | [] -> None
            | s :: ss ->
                 lr <- ss
                 Some((i, i + s))
                
    [<Test>]
    let ``unfold`` () =        
        let p (i : int) (steps : int list) =
            let f1 = makeUnfolder steps
            let f2 = makeUnfolder steps
            Seq.unfold f1 i |> List.ofSeq = toList (unfold f2 i)
        Check.QuickThrowOnFailure p
