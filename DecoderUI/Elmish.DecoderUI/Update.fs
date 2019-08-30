module Update

open Elmish
open Microsoft.FSharp.Quotations
open Model


type Message = 
| LoadSroMatrix of decimal[]
| UpdateShift of IecCouchShiftModel
| Lock of (IecCouchShiftModel -> Expr<decimal>)
| Unlock of (IecCouchShiftModel -> Expr<decimal>)
| Maximize of GeometryEstimateMaximizationParameters
| UpdateEstimates of GeometryEstimates * IecCouchShiftModel
| Error of exn

let init (initMatrix:decimal[]) =
    let initModel =
        { SroMatrix = initMatrix;
            IecCouchShift = initCouchShift;
            LockExpression = noLocks;
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
        | LoadSroMatrix matrix -> 
            { model with SroMatrix = matrix }
        | UpdateShift newShift -> 
            { model with IecCouchShift = newShift }
        | Lock singleLockExpresssion ->
            let shift = model.IecCouchShift
            let newLockExpression couchShift = 
                singleLockExpresssion couchShift :: model.LockExpression couchShift
            { model with 
                IecCouchShift = shift; 
                LockExpression = newLockExpression }
        | Unlock singleLockExpresssion ->
            let shift = model.IecCouchShift
            { model with 
                IecCouchShift = shift; 
                LockExpression = model.LockExpression }
        | Maximize expr -> 
            { model with 
                MaximizeForGeometry = expr }
        | UpdateEstimates (estimateFunc, couchShifts) ->
            { model with 
                GeometryEstimates = estimateFunc;
                IecCouchShift = couchShifts }
        | Error excn -> 
            model
    
    let nextCmd = 
        match msg with
        | UpdateShift _ -> 
            Cmd.OfAsync.either 
                DenseNetwork.estimateFromMatrixAndShiftsAsync updatedModel 
                UpdateEstimates Error
        | LoadSroMatrix _ | Lock _ | Unlock _ | Maximize _ -> 
            Cmd.OfAsync.either 
                DenseNetwork.optimizeWithConstraintsAsync updatedModel 
                UpdateEstimates Error
        | UpdateEstimates (_, _) | Error _ -> Cmd.none

    updatedModel, nextCmd