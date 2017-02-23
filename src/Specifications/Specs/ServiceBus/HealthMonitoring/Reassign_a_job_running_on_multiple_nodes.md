# Reassign a job running on multiple nodes

-> id = d901cebd-eee2-4a85-a7b4-8de978369425
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

        |> TheNodesAre-row#Node2 id=Node2
        ``` incoming
        lq.tcp://localhost:2000/control
        ```

        |> TheNodesAre-row#Node3 id=Node3
        ``` incoming
        lq.tcp://localhost:3000/control
        ```


    |> HealthMonitoringIsDisabled
    |> TheTasksAre
        [table]
        |> TheTasksAre-row task=foo://1, node=NONE, nodes=Node1


|> TaskStateIs
    [table]
    |> TaskStateIs-row Task=foo://1, Node=Node2, State=Healthy and Functional
    |> TaskStateIs-row Task=foo://1, Node=Node3, State=Healthy and Functional

|> AfterTheHealthChecksRunOnAllNodes
|> TheTaskAssignmentsShouldBe
    [rows]
    |> TheTaskAssignmentsShouldBe-row Task=foo://1, Node=Node1

|> ThePersistedAssignmentsShouldBe
    [rows]
    |> ThePersistedAssignmentsShouldBe-row Task=foo://1, Node=Node1

~~~
