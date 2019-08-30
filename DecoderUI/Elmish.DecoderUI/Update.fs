module Update

open Elmish
open Microsoft.FSharp.Quotations
open Model


type Message = 
| UpdateShift of IecCouchShiftModel
| Lock of (IecCouchShiftModel -> Expr<decimal>)
| Unlock of (IecCouchShiftModel -> Expr<decimal>)
| Maximize of GeometryEstimateMaximizationParameters
| UpdateEstimates of GeometryEstimates * IecCouchShiftModel
| Error of exn

let init (sroMatrix:decimal[]) =
    let initModel =
        { SroMatrix = sroMatrix;
            IecCouchShift = initCouchShift;
            LockExpression = lockAllShifts;
            GeometryEstimates = uniformGeometryEstimates;
            MaximizeForGeometry = noMaximization; }
    let initCmd = 
        Cmd.OfAsync.either 
            DenseNetwork.optimizeWithConstraintsAsync initModel
            UpdateEstimates Error
    initModel, initCmd

let update msg model = 
    let updatedModel =
        match msg with
        | UpdateShift newShift -> 
            { model with IecCouchShift = newShift }
        | Lock singleLockExpresssion ->
            let shift = model.IecCouchShift
            let newLockExpression couchShift = 
                singleLockExpresssion couchShift :: model.LockExpression couchShift
            { model with IecCouchShift = shift; 
                            LockExpression = newLockExpression }
        | Unlock singleLockExpresssion ->
            let shift = model.IecCouchShift
            { model with IecCouchShift = shift; 
                            LockExpression = model.LockExpression }
        | Maximize expr -> 
            { model with MaximizeForGeometry = expr }
        | UpdateEstimates (estimateFunc, couchShifts) ->
            { model with 
                GeometryEstimates = estimateFunc;
                IecCouchShift = couchShifts }
        | Error excn -> model

    let nextCmd =         
        (msg, lockAllShifts initCouchShift  // test if we are lock all
                |> List.except (model.LockExpression initCouchShift)
                |> List.isEmpty)
        |> function 
        | UpdateShift _, true -> 
            Cmd.OfAsync.either 
                DenseNetwork.estimateFromMatrixAndShiftsAsync updatedModel 
                UpdateEstimates Error
        | UpdateShift _, false
        | Lock _, _ 
        | Unlock _, _ 
        | Maximize _, _ -> 
            Cmd.OfAsync.either 
                DenseNetwork.optimizeWithConstraintsAsync updatedModel 
                UpdateEstimates Error
        | UpdateEstimates (_, _), _ 
        | Error _, _ -> Cmd.none

    updatedModel, nextCmd