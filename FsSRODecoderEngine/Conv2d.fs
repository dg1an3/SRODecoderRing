namespace FsSRODecoderEngine

module Conv2d =
#if FSHARP_MODEL_TYPES
    open Types
#else
    open SRODecoderEngine
#endif

    let predictForLayer (input:float[,]) (layer:Layer) =
        input

