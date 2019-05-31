namespace FsSRODecoderEngine

module FsSRODecoderEngine =
    open System
    open System.IO
    open System.Text
    open System.Diagnostics
    open Newtonsoft.Json
    open SRODecoderEngine

    let readTensorFromLines (lines:seq<string>) =
        let extractWidthHeightFromLines (rowLines) =
            rowLines
            |> Seq.map (List.take 2 >> List.map int) 
            |> Seq.distinct
            |> function 
                seqWidthHeights -> 
                    assert ((seqWidthHeights|> Seq.length) = 1)
                    seqWidthHeights |> Seq.head
        let extractMatrixRowsFromLines (rowLines) = 
            rowLines
            |> Seq.map (List.skip 2)
            |> Seq.map 
                (fun rowOfString -> 
                    (rowOfString |> List.head |> int, 
                        rowOfString |> List.skip 1 |> List.map float))
            |> Map.ofSeq
        lines
        |> Seq.map (fun line -> line.Split(',') |> List.ofArray)
        |> Seq.groupBy List.head
        |> Seq.map (fun (label, rest) -> (label, rest |> Seq.map (List.skip 3)))
        |> Seq.map 
            (fun (label, rowLines) -> 
                let [width; height] = extractWidthHeightFromLines rowLines
                let matrixRows = extractMatrixRowsFromLines rowLines
                let tensor = 
                    Array2D.init width height 
                        (fun x y -> matrixRows.[x].[y])
                printfn "%A" tensor
                (label, tensor))
        |> Map.ofSeq

    let testReader () =
        seq { 
            yield "kernel,2,3,0,0,0,0";
            yield "kernel,2,3,1,0,0,0"
            yield "bias,1,3,0,0,0,0";
        }
        |> readTensorFromLines

    let readModel(modelStream:Stream) =
        use modelReader = new StreamReader(modelStream, Encoding.UTF8)
        modelReader.ReadToEnd()
        |> JsonConvert.DeserializeObject<SequentialModel>

    let readWeights(weightStream:Stream) =
        use weightsReader = new StreamReader(weightStream, Encoding.UTF8)
        seq { while not weightsReader.EndOfStream 
                do yield weightsReader.ReadLine() }
        |> readTensorFromLines

