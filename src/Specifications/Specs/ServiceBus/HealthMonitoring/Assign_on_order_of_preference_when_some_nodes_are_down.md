# Assign on order of preference when some nodes are down

-> id = cb1b865b-20ad-4c20-93f6-c07d76eedaf7
-> lifecycle = Acceptance
-> max-retries = 0
-> last-updated = 2015-09-22T00:00:00.0000000
-> tags = 

[Monitoring]
|> Context
    [MonitoringSetup]
    |> TheNodesAre
        [table]
        |id   |incoming                       |
        |Node1|lq.tcp://localhost:1000/control|
        |Node2|lq.tcp://localhost:2000/control|
        |Node3|lq.tcp://localhost:3000/control|
        |Node4|lq.tcp://localhost:4000/control|

    |> HealthMonitoringIsDisabled
    |> TheTasksAre
        [table]
        |> row task=foo://1, node=NONE
        ``` nodes
        Node1, Node2, Node3, Node4
        ```



|> NodeDropsOffline Node=Node1
|> NodeDropsOffline Node=Node2
|> AfterTheHealthChecksRunOnNode node=Node3
|> ThePersistedNodesShouldBe
    [rows]
    |Id   |ControlChannel                 |
    |Node1|lq.tcp://localhost:1000/control|
    |Node2|lq.tcp://localhost:2000/control|
    |Node3|lq.tcp://localhost:3000/control|
    |Node4|lq.tcp://localhost:4000/control|

|> TheTaskAssignmentsShouldBe
    [rows]
    |> row Task=foo://1, Node=Node3

|> ThePersistedAssignmentsShouldBe
    [rows]
    |> row Task=foo://1, Node=Node3

~~~
