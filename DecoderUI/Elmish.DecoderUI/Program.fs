open Microsoft.FSharp.Quotations
open Elmish

type PatientPosition =
| HeadFirstSupine
| HeadFirstDecubitusLeft
| HeadFirstProne
| HeadFirstDecubitusRight
| FootFirstSupine
| FootFirstDecubitusLeft
| FootFirstProne
| FootFirstDecubitusRight

type TransformationOrder =
| TRxRyRz
| TRzRyRx
| RxRyRzT
| RzRyRxT

type IecCouchShiftModel = 
    { Tx:decimal; Ty:decimal; Tz:decimal; 
        Rx:decimal; Ry:decimal; Rz:decimal; }

let initCouchShift = 
    { Tx=0.0m; Ty = 0.0m; Tz = 0.0m;
        Rx=0.0m; Ry = 0.0m; Rz = 0.0m; }

type Model = 
    { SroMatrix : decimal[];
        IecCouchShift : IecCouchShiftModel;
        GeometryEstimates : PatientPosition * TransformationOrder -> decimal;
        LockExpression : IecCouchShiftModel -> Expr<bool>;
        MaximizeExpression : PatientPosition * TransformationOrder -> Expr<bool>; }

let init () =     
    let initLockExpression (couchShift:IecCouchShiftModel) =
        let _example = <@couchShift.Tx = 0.0m && couchShift.Ty = 0.0m@>
        <@true@>        
    let initGeometryEstimate (patpos,xord) = 
        1.0m / (8.0m*4.0m)
    let initMaximizeExpression (patpos,xord) =
        let _example = <@patpos = HeadFirstSupine && xord = TRzRyRx@>
        <@true@>
    { SroMatrix = [||];
        IecCouchShift = initCouchShift;
        LockExpression = initLockExpression;
        GeometryEstimates = initGeometryEstimate;
        MaximizeExpression = initMaximizeExpression; },
        Cmd.none

type Message = 
| LoadSroMatrix of decimal[]
| UpdateShift of IecCouchShiftModel
| Lock of (IecCouchShiftModel -> Expr<bool>)
| Unlock of (IecCouchShiftModel -> Expr<bool>)
| Maximize of (PatientPosition * TransformationOrder -> Expr<bool>)
| UpdateEstimates of (PatientPosition * TransformationOrder -> decimal) * IecCouchShiftModel
| Error of exn

let estimateFromMatrixAndShifts model =
    let { SroMatrix = matrix; 
            IecCouchShift = couchShift; } = model
    (fun (patpos,xord) -> 0.0m), couchShift

let optimizeWithConstraints model =
    let { SroMatrix = matrix; 
            IecCouchShift = couchShift;
            LockExpression = lockExpr;
            MaximizeExpression = maxExpr; } = model            
    (fun (patpos,xord) -> 0.0m), couchShift

let update msg model = 
    match msg with
    | LoadSroMatrix matrix -> 
        { model with SroMatrix = matrix }, 
            Cmd.OfFunc.either optimizeWithConstraints model UpdateEstimates Error
    | UpdateShift newShift -> 
        { model with IecCouchShift = newShift },
            Cmd.OfFunc.either estimateFromMatrixAndShifts model UpdateEstimates Error
    | Lock expr -> 
        // update offset
        let shift = model.IecCouchShift
        { model with 
            IecCouchShift = shift; 
            LockExpression = expr },
            Cmd.OfFunc.either optimizeWithConstraints model UpdateEstimates Error
    | Unlock expr -> 
        // update offset if matrix is set
        let shift = model.IecCouchShift
        { model with 
            IecCouchShift = shift; LockExpression = expr },
            Cmd.OfFunc.either optimizeWithConstraints model UpdateEstimates Error
    | Maximize expr -> 
        { model with 
            MaximizeExpression = expr },
            Cmd.OfFunc.either optimizeWithConstraints model UpdateEstimates Error
    | UpdateEstimates (estimateFunc, couchShifts) ->
        { model with 
            GeometryEstimates = estimateFunc;
            IecCouchShift = couchShifts },
            Cmd.none
    | Error excn -> 
        model, Cmd.none

[<EntryPoint>]
let main argv = 

    (fun model _ -> 
        printf "%A\n" model)
    |> Elmish.Program.mkProgram init update
    |> Elmish.Program.run

    0