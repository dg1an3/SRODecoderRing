open Microsoft.FSharp.Quotations
open Elmish
open System

type IecCouchShiftModel = 
    { Tx:decimal; Ty:decimal; Tz:decimal; 
        Rx:decimal; Ry:decimal; Rz:decimal; }

type IecCouchShiftLockExpression =
    IecCouchShiftModel -> Expr<decimal> list

let noLocks : IecCouchShiftLockExpression = 
    function
    | _ -> []

let lockRotations : IecCouchShiftLockExpression = 
    function
    | couchShift ->
        [ <@couchShift.Rx@>; 
            <@couchShift.Ry@>; 
            <@couchShift.Rz@> ]

let initCouchShift = 
    { Tx=0.0m; Ty = 0.0m; Tz = 0.0m;
        Rx=0.0m; Ry = 0.0m; Rz = 0.0m; }

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

type GeometryEstimates =
    PatientPosition * TransformationOrder -> decimal

let uniformGeometryEstimates : GeometryEstimates =
    let numberOfPositions = 
        Enum.GetValues(typeof<PatientPosition>).Length
    let numberOfOrders = 
        Enum.GetValues(typeof<TransformationOrder>).Length
    function
    | (_:PatientPosition, _:TransformationOrder) ->
        1.0m / decimal (numberOfPositions * numberOfOrders)

type GeometryEstimateMaximizationParameters =
    { SelectPatientPosition : PatientPosition option;
        SelectTransformationOrder : TransformationOrder option; }

let noMaximization = 
    { SelectPatientPosition = None;
        SelectTransformationOrder = None }

let maximizePosition position = 
    { SelectPatientPosition = Some position;
        SelectTransformationOrder = None }

type Model = 
    { SroMatrix : decimal[];
        IecCouchShift : IecCouchShiftModel;
        LockExpression : IecCouchShiftLockExpression;
        GeometryEstimates : GeometryEstimates;
        MaximizeForGeometry : GeometryEstimateMaximizationParameters; }

let init () =
    { SroMatrix = [||];
        IecCouchShift = initCouchShift;
        LockExpression = noLocks;
        GeometryEstimates = uniformGeometryEstimates;
        MaximizeForGeometry = noMaximization; },
        Cmd.none

type Message = 
| LoadSroMatrix of decimal[]
| UpdateShift of IecCouchShiftModel
| Lock of (IecCouchShiftModel -> Expr<decimal>)
| Unlock of (IecCouchShiftModel -> Expr<decimal>)
| Maximize of GeometryEstimateMaximizationParameters
| UpdateEstimates of GeometryEstimates * IecCouchShiftModel
| Error of exn

let estimateFromMatrixAndShifts model =
    let { SroMatrix = matrix; 
            IecCouchShift = couchShift; } = model
    let newGeometryEstimates = uniformGeometryEstimates
    newGeometryEstimates, couchShift

let optimizeWithConstraints model =
    let { SroMatrix = matrix; 
            IecCouchShift = couchShift;
            LockExpression = lockExpression;
            MaximizeForGeometry = geometryMaximizationParameters } = model
    let { SelectPatientPosition = patientPositionOption;
            SelectTransformationOrder = transformationOrderOption } = geometryMaximizationParameters
    let newCouchShift = couchShift
    let newGeometryEstimates = uniformGeometryEstimates
    newGeometryEstimates, newCouchShift

let update msg model = 
    match msg with
    | LoadSroMatrix matrix -> 
        { model with SroMatrix = matrix }, 
            Cmd.OfFunc.either optimizeWithConstraints model UpdateEstimates Error
    | UpdateShift newShift -> 
        { model with IecCouchShift = newShift },
            Cmd.OfFunc.either estimateFromMatrixAndShifts model UpdateEstimates Error
    | Lock singleLockExpresssion ->  // update offset
        let shift = model.IecCouchShift
        let newLockExpression couchShift = 
            singleLockExpresssion couchShift :: model.LockExpression couchShift
        { model with 
            IecCouchShift = shift; 
            LockExpression = newLockExpression },
            Cmd.OfFunc.either optimizeWithConstraints model UpdateEstimates Error
    | Unlock singleLockExpresssion -> // update offset if matrix is set
        let shift = model.IecCouchShift
        { model with 
            IecCouchShift = shift; 
            LockExpression = model.LockExpression },
            Cmd.OfFunc.either optimizeWithConstraints model UpdateEstimates Error
    | Maximize expr -> 
        { model with 
            MaximizeForGeometry = expr },
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