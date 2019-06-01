namespace FsSRODecoderEngine

module Types = 

#if FSHARP_MODEL_TYPES
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
    pass
#endif

