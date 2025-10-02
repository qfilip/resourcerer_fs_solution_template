module Resourcerer.Utilities.Crypto

open System.Text;
open System.Security.Cryptography

let private getHash (x: string) (algo: HashAlgorithm) =
    let hash = StringBuilder()
    algo.ComputeHash(Encoding.UTF8.GetBytes(x))
    |> Array.iter(fun b -> hash.Append(b.ToString("x2")) |> ignore)

    hash.ToString()


let toSha1 (x: string) =
    use sha = SHA1.Create()
    getHash x sha