module NDS.Immutable.Utils
    let flip f a b = f b a
    let curry f a b = f (a, b)
    let uncurry f (a, b) = f a b

