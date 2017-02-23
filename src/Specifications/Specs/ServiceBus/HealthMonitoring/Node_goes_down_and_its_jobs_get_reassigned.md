# Node goes down and its jobs get reassigned

-> id = b2a0a037-3031-4407-a354-6791b1cc7aa8
-> lifecycle = Regression
-> max-retries = 0
-> last-updated = 2016-06-21T00:00:00.0000000
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
        |task   |node |nodes                     |
        |foo://1|Node4|Node1, Node2, Node3, Node4|
        |foo://2|Node4|Node2, Node1, Node3, Node4|
        |foo://3|NONE |Node3, Node1, Node2, Node4|
        |foo://4|NONE |Node4, Node1, Node2, Node3|



The initial state should look like this

|> TheTaskAssignmentsShouldBe
    [rows]
    |Task   |Node |
    |foo://1|Node4|
    |foo://2|Node4|


Now, Node4 drops offline and its owned tasks should be reassigned

|> NodeDropsOffline Node=Node4
|> AfterTheHealthChecksRunOnNode node=Node1
|> ThePersistedAssignmentsShouldBe
    [rows]
    |Task   |Node |
    |foo://1|Node1|
    |foo://2|Node2|
    |foo://3|Node3|
    |foo://4|Node1|

|> TheTaskAssignmentsShouldBe
    [rows]
    |Task   |Node |
    |foo://1|Node1|
    |foo://2|Node2|
    |foo://3|Node3|
    |foo://4|Node1|

~~~
