namespace FsSRODecoderEngine

module Model =
    open System
    open System.IO
    open System.Text
    open Newtonsoft.Json

#if FSHARP_TYPES
    open System.Collections.Generic
    type DType =
    | Float
    | Int

    type Activation =
    | Linear
    | ReLu
    | Softmax

    type LayerConfiguration = {
        Name:string;
        Trainable:bool;
        BatchInputShape:array<int option>;
        DType:DType;
        Units:int;
        Activation:Activation;
        UseBias:bool;
    }

    type Layer = {
        ClassName:string;
        Config:LayerConfiguration;
        Variables:IDictionary<string, float[,]>;
    }

    type SequentialModel = {
        Name:string;
        Layers:IList<Layer>;
    }
#else 
    open SRODecoderEngine
#endif

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

    let predict (model:SequentialModel) (input:float[,]) =
        model.Layers 
        |> Seq.fold 
            (fun tensor layer -> 
                match layer.ClassName with 
                | "dense" -> Dense.predictForLayer tensor layer
                | "conv2d" -> Conv2d.predictForLayer tensor layer
                | _ -> raise(Exception())) input
