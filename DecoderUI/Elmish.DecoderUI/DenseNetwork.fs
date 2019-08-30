module DenseNetwork

open System.IO
open System.Diagnostics
open System.Threading.Tasks

open FSharp.Control.Tasks.V2

open Model

let invokeExternalAsync<'TRequest, 'TResponse> 
        exePath argFormat requestSerialize responseDeserialize (request:'TRequest) =

    let fileIn, fileOut = Path.GetTempFileName(), Path.GetTempFileName()
    (fileIn, requestSerialize request) |> File.WriteAllText

    let proc = 
        (exePath, sprintf argFormat fileIn fileOut)
        |> ProcessStartInfo
        |> Process.Start

    let tcs = TaskCompletionSource<'TResponse>()
    let fileOutToResult _ = 
        fileOut
        |> File.ReadAllText
        |> responseDeserialize
        |> tcs.SetResult
    proc.EnableRaisingEvents <- true
    proc.Exited.Add fileOutToResult
    tcs.Task

let estimateFromMatrixAndShiftsAsync model = async {
        let { SroMatrix = matrix; 
                IecCouchShift = couchShift; } = model
        let invokeFunc = invokeExternalAsync<string, string> "python" "module.py %s%s" id id
        let! result = invokeFunc (matrix.ToString()) |> Async.AwaitTask
        let newGeometryEstimates = uniformGeometryEstimates
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