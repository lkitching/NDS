namespace NDS.Immutable.Tests

open NDS.Immutable.ModQueue
open NUnit.Framework
open FsCheck
open FsCheck.Gen

[<TestFixture>]
module QueueTests =
    let genQueue<'a> () =
        let listGen = Arb.Default.FsList<'a>().Generator
        let gen' size =
            match size with
            | 0 -> gen { return empty }
            | n -> gen {
                 let! front = listGen
                 let! back = listGen
                 let queue = 
                    match front, back with
                    | ([], []) -> empty
                    | (f::fs), _ -> { front = front; back = back }
                    | [], (b::bs) -> { front = [b]; back = bs }
                return queue
            }
        Gen.sized gen'

    let enqueueAll l = List.fold (fun q e -> enqueue e q) empty l
    let rec dequeueN n q =
        if n = 0
            then q
            else dequeueN (n - 1) (snd (tryDequeue q))

    let dequeueAll q = 
        let dequeueState q =
            let (eo, q') = tryDequeue q
            eo |> Option.map (fun e -> (e, q'))
        Seq.unfold dequeueState q

    [<Test>]
    let ``ofSeq >> toSeq = id`` () =
        let p (l: int list) = List.ofSeq (toSeq (ofSeq l)) = l
        Check.Quick p

    [<Test>]
    let ``should enqueue in order`` () =
        let p (l: int list) = List.ofSeq (toSeq (enqueueAll l)) = l
        Check.Quick p

    [<Test>]
    let ``should dequeue in order`` () =
        let p (l: int list) = List.ofSeq (dequeueAll (enqueueAll l)) = l
        Check.Quick p

    [<Test>]
    let ``dequeue should maintain items in front list`` () =
        let deqGen = gen {
            let! q = ((genQueue<int>()) |> Gen.filter (fun q -> length q > 1))
            let! toDeq = Gen.choose (1, (length q - 1))
            return (q, toDeq)
        }
        let p (q, n) =
            let { front = f } = dequeueN n q
            not (List.isEmpty f)

        let prop = Prop.forAll (Arb.fromGen deqGen) p
        Check.Quick prop

