# Do not reassign a task that is running

-> id = 64cad02e-e059-4382-9370-b7717773555b
-> lifecycle = Regression
-> max-retries = 0
-> last-updated = 2015-09-22T00:00:00.0000000
-> tags = 

[Monitoring]
|> Context
    [MonitoringSetup]
    |> TheNodesAre
        [table]
        |> TheNodesAre-row#Node1 id=Node1
        ``` incoming
        lq.tcp://localhost:1000/control
        ```


    |> HealthMonitoringIsDisabled
    |> TheTasksAre
        [table]
        |> TheTasksAre-row task=foo://1, node=NONE, nodes=Node1


|> TaskStateIs
    [table]
    |> TaskStateIs-row Task=foo://1, Node=Node1, State=Healthy and Functional

|> AfterTheHealthChecksRunOnAllNodes
|> TaskWasNotReassigned task=foo://1
|> TheTaskAssignmentsShouldBe
    [rows]
    |> TheTaskAssignmentsShouldBe-row Task=foo://1, Node=Node1

|> ThePersistedAssignmentsShouldBe
    [rows]
    |> ThePersistedAssignmentsShouldBe-row Task=foo://1, Node=Node1

~~~
