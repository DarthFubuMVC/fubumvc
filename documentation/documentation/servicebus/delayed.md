<!--Title:Delayed Messages-->
<!--Url:delayed-->

<div class="alert alert-info">All DateTime's are converted to UTC dates behind the scenes before transmitting through any transport channel. The successor to FubuMVC
 will likely change the API to use DataTimeOffset's</div>

Sometimes you will need to send a message, but request that it be executed at a later time. For that purpose, FubuMVC
exposes the "delayed messages" functionality demonstrated below:

<[sample:DelayedMessageSender]>

In the sample above, we're sending a `DoLaterMessage` to the bus. Behind the scenes, FubuMVC is 

1. Selecting the proper channels to send the message to based on the normal routing rules
1. Adding a header value for "time-to-send" to the outgoing envelope that will tell the receiver when the message should be processed
1. When the receiving node sees the new message with a "time-to-send" header value, it takes that message and places it into 
   local storage
1. An internal polling job in FubuMVC (the "DelayedEnvelopeProcessor") checks for delayed messages that are ready to process according
   to the system clock and executes them locally


