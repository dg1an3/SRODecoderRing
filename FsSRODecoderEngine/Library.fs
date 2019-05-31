namespace FsSRODecoderEngine

module FsSRODecoderEngine =
    open System
    open System.IO
    open System.Text
    open System.Diagnostics
    open Newtonsoft.Json
    open SRODecoderEngine

    let readTensorsFromLines (lines:seq<string>) =
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
                let tensor = Array2D.init width height (fun x y -> matrixRows.[x].[y])
                (label, tensor))
        |> Map.ofSeq
        
    let readModelAndWeights (modelStream:Stream) (weightStream:Stream) =
        use modelReader = new StreamReader(modelStream, Encoding.UTF8)
        let model = 
            modelReader.ReadToEnd()
            |> JsonConvert.DeserializeObject<SequentialModel>
        use weightsReader = new StreamReader(weightStream, Encoding.UTF8)
        seq { while not weightsReader.EndOfStream 
                do yield weightsReader.ReadLine() }
        |> readTensorsFromLines
        |> Map.iter
            (fun label tensor -> 
                let [layerName; variableName] = label.Split('/') |> List.ofArray
                let layer = 
                    model.Layers 
                    |> Seq.find (fun layer -> layer.Configuration.Name.CompareTo(layerName) = 0)
                layer.Variables.Add(variableName, tensor))
        model

    let predictForKernelBias (input:float[,]) (kernelTensor:float[,]) (biasTensor:float[,]) = 
        let batchCount = input.GetLength(0)
        Array2D.init batchCount (kernelTensor.GetLength(1))
            (fun atBatch m ->
                seq { 0..kernelTensor.GetLength(0) }                            
                |> Seq.fold 
                    (fun sum n -> 
                        sum + input.[atBatch,n] * kernelTensor.[n,m]) 0.0
                |> (+) (biasTensor.[0,m]))

    let predictForLayer (input:float[,]) (layer:Layer)  =
        let kernelTensor = layer.Variables.["kernel"]
        let biasTensor = layer.Variables.["bias"]
        let activation = 
            match layer.Configuration.Activation with 
            | "linear" -> id
            | "relu" -> (fun x -> if x < 0.0 then 0.0 else x)
            | _ -> raise(Exception())
        predictForKernelBias kernelTensor biasTensor input
        |> Array2D.map activation

    let predict (model:SequentialModel) (input:float[,]) =
        model.Layers 
        |> Seq.fold predictForLayer input
