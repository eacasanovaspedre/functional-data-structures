﻿// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open CollectionsA.Hamt

open FSharpx.Collections
open System
open CollectionsA
open System.Collections.Generic
open FDS.Core
open CollectionsE

[<CustomEquality; CustomComparison>]
type KeyHash = KeyHash of int with
    override this.GetHashCode () = match this with KeyHash value -> value / 2

    interface IEquatable<KeyHash> with
        member this.Equals other = match this, other with KeyHash v1, KeyHash v2 -> v1 = v2

    interface IComparable<KeyHash> with
        member this.CompareTo other = match this, other with KeyHash v1, KeyHash v2 -> compare v1 v2

    interface IComparable with
        member this.CompareTo other = 
            match other with
            | :? KeyHash as keyHash -> (this :> IComparable<KeyHash>).CompareTo keyHash
            | _ -> failwith "weird"

    override this.Equals other = 
        match other with 
        | :? KeyHash as keyHash -> (this :> IEquatable<KeyHash>).Equals keyHash
        | _ -> false

[<EntryPoint>]
let main argv = 

    let a = RTQueue.empty ()
    let b = RTQueue.snoc 1 a
    let c = RTQueue.snoc 2 b
    let d = RTQueue.snoc 3 c
    let e = RTQueue.snoc 4 d
    let f = RTQueue.snoc 5 e

    let n = 8000000
    let items = Seq.init n id

    let m = GC.GetTotalMemory true
    let k = System.Diagnostics.Stopwatch.StartNew()

    let skew = 
        items
        |> Seq.fold (fun skew item -> CollectionsC.SkewList.cons item skew) CollectionsC.SkewList.empty

    let skewc = 
        items
        |> Seq.map (fun index -> CollectionsC.SkewList.count (CollectionsC.SkewList.skip index skew))
        |> Seq.last

    //let h0 = 
    //    items
    //    |> Seq.fold (fun hamt item -> Hamt.add (KeyHash item) item hamt) Hamt.empty

    //let p0 =
    //    items
    //    |> Seq.fold (fun hamt item -> PersistentHashMap.add (KeyHash item) item hamt) PersistentHashMap.empty

    //let m0 =
    //    items
    //    |> Seq.fold (fun map item -> Map.add (KeyHash item) item map) Map.empty

    //let d0 =
    //    items
    //    |> Seq.fold (fun (map: Dictionary<KeyHash, int>) item -> map.Add (KeyHash item, item); map ) (new Dictionary<KeyHash, int>())

    
    let elapsed = k.ElapsedMilliseconds
    let memory = float (GC.GetTotalMemory true - m) / 1024.0 / 1024.0

    printfn "%A Time: %A ms Memory: %A MB" (CollectionsC.SkewList.count skew) elapsed memory

    //printfn "%A Time: %A ms Memory: %A MB" (Hamt.count h0) elapsed memory
    //printfn "%A Time: %A ms Memory: %A MB" p0.Length elapsed memory
    //printfn "%A Time: %A ms Memory: %A MB" m0.Count elapsed memory
    //printfn "%A Time: %A ms Memory: %A MB" d0.Count elapsed memory

    0 // return an integer exit code1
