# Assign on order or preference when some nodes timeout on activation

-> id = 6a5baf9e-184a-4374-afb5-f06c7bfd00fa
-> lifecycle = Acceptance
-> max-retries = 0
-> last-updated = 2017-02-23T15:45:01.1677952-06:00
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



|> TaskStateIs
    [table]
    |Task   |Node |State                               |
    |foo://1|Node1|Times out on startup or health check|
    |foo://1|Node2|Times out on startup or health check|

|> AfterTheHealthChecksRunOnNode node=Node1
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
