module DenseNetwork

open System.Diagnostics

open Model

let invokeCommandLineAsync<'TRequest, 'TResponse> 
        exePath argFormat 
        (requestSerialize:'TRequest->string)
        (responseDeserialize:string->'TResponse) 
        (request:'TRequest) = async {
    let proc = 
        (exePath, requestSerialize request |> sprintf argFormat)
        |> ProcessStartInfo
        |> Process.Start
    let! outputText =
        proc.StandardOutput.ReadToEndAsync() 
        |> Async.AwaitTask
    return responseDeserialize outputText }

let estimateFromMatrixAndShiftsAsync model = async {
    let { SroMatrix = matrix; 
            IecCouchShift = couchShift; } = model
    let invokeFunc = 
        invokeCommandLineAsync<decimal[], decimal[]> 
            "python" "sro_decoder_predict.py %s" 
                (fun input -> 
                    System.String.Join("/", input))
                (fun output -> 
                    // TODO skip to output line
                    output.Split('/') |> Array.map decimal)
    let! result = 
        matrix 
        |> Array.append [| couchShift.Rx; couchShift.Ry; couchShift.Rz |]
        |> invokeFunc
    let newGeometryEstimates : GeometryEstimates = function
    | _, RxRyRzT -> result.[0]
    | _, RzRyRxT -> result.[1]
    | _, TRxRyRz -> result.[2]
    | _, TRzRyRx -> result.[3]
    return newGeometryEstimates, couchShift  }

let optimizeWithConstraintsAsync model = async {
    let { SroMatrix = matrix; 
            IecCouchShift = couchShift;
            LockExpression = lockExpression;
            MaximizeForGeometry = geometryMaximizationParameters } = model
    let { SelectPatientPosition = patientPositionOption;
            SelectTransformationOrder = transformationOrderOption } = geometryMaximizationParameters
    let newCouchShift = couchShift
    let newGeometryEstimates = uniformGeometryEstimates
    return newGeometryEstimates, newCouchShift }
