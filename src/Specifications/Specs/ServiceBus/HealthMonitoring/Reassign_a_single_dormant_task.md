# Reassign a single dormant task

-> id = 32abf9bd-1976-4200-b8aa-1438b99012a2
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


|> AfterTheHealthChecksRunOnAllNodes
|> TheTaskAssignmentsShouldBe
    [rows]
    |> TheTaskAssignmentsShouldBe-row Task=foo://1, Node=Node1

|> ThePersistedAssignmentsShouldBe
    [rows]
    |> ThePersistedAssignmentsShouldBe-row Task=foo://1, Node=Node1

~~~
