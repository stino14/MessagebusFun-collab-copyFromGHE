Developer Notes

I decided to write this from scratch and planned to spend 8 hours to get as much as I could developed rather than using packages to 
accomplish the assignment with the intention of providing a rough representation of my design, development, and thought processes.  
I spent a few hours in the car brainstorming the project prior to beginning development and an extra 
hour or so finishing and cleaning up the code.  I underestimated how long this would take 
due to not having worked with asynch methods in quite some time and forgetting how challenging it is to debug them.  
Looking back, I'm not sure how effective this decision was for providing an overview 
of my abilities since the project is unfinished and a lot of corners were cut to get to where I am, so it is not clear to the 
reviewer what were conscious choices made due to the time constraint or bad practices.  
Please feel free to inquire.

Due to the scope and time constraint, I have broken a number of my development rules and took a number of shortcuts.  Some examples include 
creating unit tests after the fact and exposing temporary public properties for testing to get around the event handling passing the same data
to the client or server app.

The primary functionality from the specs has been implemented, but optimal "reliability" was not yet achieved primarily due to DB support not 
being finished.  Thus disconnects and reconnects are not currently supported and also messages are removed from the queue without send confirmation.

Please reference the MessageBusTests class for usage documentation and functionality requirements testing. On occassion the 
MessageBusTests fail most of the way through.  Running again should pass.


Missing functionality and practices in order of priority:

Proper component separation : separate client, server and tcp projects. Ran out of time to separate.

Sqlite DB support : I noticed the project was using the .Net Standard 4.5 library.  Not sure if this was an intentional restraint 
but this prevented quick DB development using Dapper due to a minimum requirement of 4.6.1.  Basic DB support probably would have 
taken another hour.  I changed the project to .NET Core but didn't find time to complete.  Code will compile and execute on 4.5 Standard.
If that is a requirement, another solution aside from using the ORM package would be found.

Ability to reconnect clients and receive messages in queue : underlying functionality was setup, just required DB support to switch over

Checking for send confirmation before removing messages from client and server queue

SSL support : Spent 30 minutes troubleshooting why SSLStream generated an exception on authentication and moved on 

Loading of IP, port, connection string, username, and password from app.config or similar : 10 minutes

Client and server console apps for further testing

Proper unit tests rather than end to end tests

Ensure proper object disposal

Multiple refactorings

Lacking proper comments

Lacking proper exception handling

Improve clarity of property names

General code cleanup


Message Bus Fun
===============

Your challenge – should you choose to accept it – is to design a component according to the
following criteria.

Non-functional acceptance criteria.
-----------------------------------

### Overview

Assume – in a corporate context - that you are creating a small solution to demonstrate your
design philosophy and essential concepts to a trainee. The solution should therefore contain an
example illustrating each fundamental concept or mechanism that you consider to be important.

### Submission format.

Submissions should take the form of a **pull request**.

Any Visual Studio solution version between 2013 and 2017 is permissible. As far as possible, the
solution should be useable with no special preparation of the development environment. Any
instructions or usage notes should be in added to the beginning  of this readme. Running tests from
the command-line is acceptable.

### Language, framework, and dependencies.
The solution should be authored in C# targeting the .Net Framework version 4.0 or above.
All non-core references should be obtainable via NuGet.

### Target Operating Environment

A library. Assume that the component can be shared in binary form with multiple other development
teams for incorporation into various applications. The component is intended to be utilised
directly in-process; it is not necessary to manage the exposure of any functionality or state via
any mechanism other than by the use of visibility modifiers. The applications that consume
the component will be running in either a server or client context.

Functional description.
-----------------------

### Library

* The component is intended to manage the routing of messages from multiple providers -
  grouped into channels - to multiple subscribers. The reliability of the routing is key. Any API or
  interface necessary to the routing mechanism are the domain of the component.
* Consuming applications should be able to register and deregister both subscribers and
  providers.
* Providers will nominate a channel name (a string) upon registration. Multiple providers
  may nominate the same channel name. A provider cannot change channel names without
  deregistering.
* The consuming application should be able to read a list of available channels. An available
  channel is one for which at least one provider is currently registered.
* Consuming applications should be able to register a subscriber to receive messages from any
  available channel/s without limitation.
* Once registered to an available channel, the subscriber will receive all messages sent by
  registered providers on that channel.
* The subscriber needs to identify which channel a message originates from, but not the identity
  of the provider.
* Any subscribers are notified if a channel becomes unavailable, but are not automatically
  unsubscribed. If the channel becomes available again, subscribers still registered will once again
  receive messages from it.
* Multiple attempts to register a subscriber to receive messages to the same channel should have
  no effect.
* Multiple attempts to deregister a subscriber from receiving messages from the same channel
  should have no effect.
* Attempts to deregister a subscriber which is not registered should have no effect.
* A consuming application should be able to deregister a subscriber from receiving messages from
  any channels without limitation.
* A consuming application should be able to deregister a provider from publishing messages.
* Multiple attempts to deregister the same provider from publishing messages should have no
  effect.
* Attempts to deregister a provider which is not registered should have no effect.
