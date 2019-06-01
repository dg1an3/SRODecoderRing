namespace FsSRODecoderEngine

module Dense =
    open System
    open SRODecoderEngine

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
            match layer.Config.Activation with 
            | "linear" -> id
            | "relu" -> (fun x -> if x < 0.0 then 0.0 else x)
            | _ -> raise(Exception())
        predictForKernelBias kernelTensor biasTensor input
        |> Array2D.map activation
