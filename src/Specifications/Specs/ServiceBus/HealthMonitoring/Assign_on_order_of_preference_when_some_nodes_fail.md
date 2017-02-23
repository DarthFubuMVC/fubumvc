# Assign on order of preference when some nodes fail

-> id = d0fa8772-7313-4120-9eec-979dd4f8dc84
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

        |> TheNodesAre-row#Node4 id=Node4
        ``` incoming
        lq.tcp://localhost:4000/control
        ```


    |> HealthMonitoringIsDisabled
    |> TheTasksAre
        [table]
        |> TheTasksAre-row task=foo://1, node=NONE
        ``` nodes
        Node1, Node2, Node3, Node4
        ```



|> TaskStateIs
    [table]
    |> TaskStateIs-row Task=foo://1, Node=Node1
    ``` State
    Throws exception on startup or health check
    ```

    |> TaskStateIs-row Task=foo://1, Node=Node2
    ``` State
    Throws exception on startup or health check
    ```


|> AfterTheHealthChecksRunOnNode node=Node1
|> ThePersistedNodesShouldBe
    [rows]
    |> ThePersistedNodesShouldBe-row Id=Node1
    ``` ControlChannel
    lq.tcp://localhost:1000/control
    ```

    |> ThePersistedNodesShouldBe-row Id=Node2
    ``` ControlChannel
    lq.tcp://localhost:2000/control
    ```

    |> ThePersistedNodesShouldBe-row Id=Node3
    ``` ControlChannel
    lq.tcp://localhost:3000/control
    ```

    |> ThePersistedNodesShouldBe-row Id=Node4
    ``` ControlChannel
    lq.tcp://localhost:4000/control
    ```


|> TheTaskAssignmentsShouldBe
    [rows]
    |> TheTaskAssignmentsShouldBe-row Task=foo://1, Node=Node3

|> ThePersistedAssignmentsShouldBe
    [rows]
    |> ThePersistedAssignmentsShouldBe-row Task=foo://1, Node=Node3

~~~
