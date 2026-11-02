# sales-districts

The idea for this project is to start with the DB design first and foremost to get a concrete foundation of entities.

I wanted to capture as much of the business logic in the Constraints of the DB since this will help remove unnecessary complexity in the code and make it easier to maintain.

I created a LocalDB instance and created a database called SalesDistrict. I then created the tables and constraints for the entities that I wanted to capture in the DB.
script 01 creates the tables and constraints for the entities, script 02 fills the tables with data.
The WPF project connects to localhost using port 5255

Next step was mapping out the controllers and service layer, here I wanted to handle some of the more complex business logic in the service layer and keep the controllers as thin as possible.
As well as taking care of any constraints that might not be easy to handle in the DB without compromising performance and readability

There are only 2 controller classes, One for handling all actions that have something to do with districts and updating them, 
and 1 for handling all actions that have something to do with salespersons. The DistrictsController contains a standard Getter to get all districts, a Getter to get a district by id with details, 
and a PUT method to update the salespersons for a district. The SalespersonsController contains a standard Getter to get all salespersons.
The PUT replaces the whole assignment, and therefore there is no need for a DELETE method.

Response and request classes are neatly placed inside the Contracts folder.

ErrorHandling contains a handler class for our custom exception. The custom exception class was created such that user can get proper readable error messages when something goes wrong. 
The exception is thrown in either in the service layer or the repository layer depending on the error, it then goes up through the controller layer and is then caught by the ExceptionHandler 
where it is then returned to the user as a proper response. Most problems result in a 400 Bad Request. In the case that the primary or a secondary salesperson does not exist the DistrictRepository
throws an SQL error 547 which the handler translates to a 400 Bad Request.

The Service layer houses the business logic, the class for the custom exception. The way this is designed 
is using interfaces to define the contract for the service and then implementing that contract in a concrete class. This allows for easy testing and mocking of the service layer.
The Service classes themselves call upon the repository layer to get the data they need to perform their business logic. This helps remove data access logic from the service layer 
and keeps it focused on business logic only. These repositories are accessed through yet another abstraction which is the interfaces for the repositories. 
This again allows for easy mocking of the repositories in the service layer tests.

I chose to not enforce the rule that each salesperson has at least 1 district, this was done such that a new salesperson can be added to the system without a district.

The Data layer houses the repository classes that are responsible for accessing the data from the database. These classes implement the repository interfaces defined in the application layer.
It also houses the SqlConnectionFactory class which is responsible for creating the SqlConnection objects that are used to access the database. This class is used by the repository classes to get a connection to the database.

The Domain class houses all the entities that are used in the application. These entities are used to map the data from the database to the application and vice versa.

Finally the test folder contain both Integration and unitTests. IntegrationTests handle the end to end testing of the apis, while the unitTests utilise a fakeService and fakeRepository 
to test the service layer in isolation. The unitTests are designed to test the business logic in the service layer without having to worry about the data access layer or the database itself.
Furthermore a SalesDistrictApiFactory was introduced to replicate the schemas from the production database into a test DB to be used by all integrationTests. This was done to avoid any tests affecting 
the production database and to ensure that the tests are run in a controlled environment. The factory is responsible for creating the test database and seeding it with test data before once for the getter test
and for each PUT test the test DB is reset inbetween. 
This ensures that each test is run in a clean state and that the tests do not interfere with each other.

Serilog was introduced to log for each request. This was done to help with debugging and to provide a way to track down any issues that might arise in production.

Autofac was introduced to handle dependency injection. Autofac also provides a way to register modules which is used to register all the dependencies for each layer in one place.
It also enables registration via naming conventions which is used to register all the services and repositories in one place. This all helps keep the Program.cs file clean and easy to read.

Lastly I made the WPF client building on top of the API:

The client follows MVVM, using CommunityToolkit.Mvvm:

- **View** (`MainWindow.xaml`) contains bindings only. The code-behind sets the DataContext and starts the first load.
- **ViewModel** (`MainViewModel`) holds the state and the commands. It has no reference to WPF types, so it can be tested without a UI.
- **API client** (`ISalesDistrictApiClient`) is the only part of the client that knows about HTTP.

The client has its own copy of the API contracts (`Api/Contracts`) and maps them to its own models (`Models`). It does not reference the API project, so the API's internal types can change without the client being rebuilt. Only the JSON contract is shared.

The business rules live only on the server. The client does not duplicate the rules; it builds the complete new assignment and sends it with PUT. If the API rejects the request, the client shows the API's own `detail` message. The trade-off is that the buttons cannot disable themselves for invalid choices. In return, the rules exist in one place and cannot drift apart.

When the user clicks quickly between districts, a slow response for an earlier district could arrive last. The ViewModel ignores responses for a district that is no longer selected, and a test reproduces this race.

Dependencies are wired with Autofac in `App.OnStartup`. The whole app shares a single `HttpClient`, so connections are reused.


## Production notes

- **Logging:** Serilog writes structured logs to the console. In production a sink for Elasticsearch would be added through configuration only, with no code changes, so the logs could be searched in Kibana. Every request is logged through `UseSerilogRequestLogging`.
- **Authentication:** not implemented. In production the API would use Windows/AD authentication (`AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate()`), with a fallback policy that requires an authenticated user on every endpoint. The WPF client would send the logged-in Windows user with `HttpClientHandler { UseDefaultCredentials = true }`. In the tests, the Negotiate handler would be replaced by a test handler, because the in-memory test server cannot perform the Windows handshake.
- **UTC:** the model has no date or time values yet. Any timestamps added later, such as an audit log, would be stored and sent as UTC (`datetime2` / `DateTimeOffset`).
- **Backwards compatibility:** all routes are versioned (`/api/v1/...`). A breaking change would get a new version, so existing clients keep working. Non-breaking changes, such as new fields, stay in v1.
- **Database deployment:** the schema lives as SQL scripts in git. In production it would be deployed as a DacPac (an SQL Server Database Project) or through a migration tool, so that changes are versioned and can be applied to existing databases without data loss. The current script drops and recreates the tables.
- **Concurrency:** if two users edit the same district at the same time, the last PUT overwrites the first without a warning (a "lost update"). The fix would be optimistic concurrency: a `ROWVERSION` column on `District`, sent to the client as an `ETag`. The client sends it back in `If-Match`, and the API answers `412 Precondition Failed` if the district has changed in the meantime.