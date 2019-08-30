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

let update msg model = 
    match msg with
    | LoadSroMatrix matrix -> 
        { model with SroMatrix = matrix }, 
            Cmd.OfFunc.either DenseNetwork.optimizeWithConstraints model UpdateEstimates Error
    | UpdateShift newShift -> 
        { model with IecCouchShift = newShift },
            Cmd.OfAsync.either DenseNetwork.estimateFromMatrixAndShiftsAsync model UpdateEstimates Error
    | Lock singleLockExpresssion ->  // update offset
        let shift = model.IecCouchShift
        let newLockExpression couchShift = 
            singleLockExpresssion couchShift :: model.LockExpression couchShift
        { model with 
            IecCouchShift = shift; 
            LockExpression = newLockExpression },
            Cmd.OfFunc.either DenseNetwork.optimizeWithConstraints model UpdateEstimates Error
    | Unlock singleLockExpresssion -> // update offset if matrix is set
        let shift = model.IecCouchShift
        { model with 
            IecCouchShift = shift; 
            LockExpression = model.LockExpression },
            Cmd.OfFunc.either DenseNetwork.optimizeWithConstraints model UpdateEstimates Error
    | Maximize expr -> 
        { model with 
            MaximizeForGeometry = expr },
            Cmd.OfFunc.either DenseNetwork.optimizeWithConstraints model UpdateEstimates Error
    | UpdateEstimates (estimateFunc, couchShifts) ->
        { model with 
            GeometryEstimates = estimateFunc;
            IecCouchShift = couchShifts },
            Cmd.none
    | Error excn -> 
        model, Cmd.none

