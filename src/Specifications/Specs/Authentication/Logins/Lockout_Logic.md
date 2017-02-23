# Lockout Logic

-> id = fde5e04e-f7db-44e8-9f5a-83cd9906c94e
-> lifecycle = Regression
-> max-retries = 0
-> last-updated = 2017-02-23T15:45:00.9937952-06:00
-> tags = 

[LockedOutLogic]
|> Logic
    [table]
    |> Logic-row NumberOfTries=1, MaximumNumberOfFailedAttempts=3, CoolingOffPeriod=60, CurrentLockedOutTime=NONE, IsLockedOut=false, LockedOutUntil=NONE
    |> Logic-row NumberOfTries=2, MaximumNumberOfFailedAttempts=3, CoolingOffPeriod=60, CurrentLockedOutTime=NONE, IsLockedOut=false, LockedOutUntil=NONE
    |> Logic-row NumberOfTries=3, MaximumNumberOfFailedAttempts=3, CoolingOffPeriod=60, CurrentLockedOutTime=NONE, IsLockedOut=true, LockedOutUntil=NOW+60
    |> Logic-row NumberOfTries=0, MaximumNumberOfFailedAttempts=3, CoolingOffPeriod=60, CurrentLockedOutTime=NOW+1, IsLockedOut=true, LockedOutUntil=NOW+1
    |> Logic-row NumberOfTries=0, MaximumNumberOfFailedAttempts=3, CoolingOffPeriod=60, CurrentLockedOutTime=NOW-1, IsLockedOut=false, LockedOutUntil=NONE
    |> Logic-row NumberOfTries=5, MaximumNumberOfFailedAttempts=6, CoolingOffPeriod=60, CurrentLockedOutTime=NONE, IsLockedOut=false, LockedOutUntil=NONE

~~~
