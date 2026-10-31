# sales-districts

The idea for this project is to start with the DB design first and foremost to get a concrete foundation of entities.

I wanted to capture as much of the business logic in the Constraints of the DB since this will help remove unnecessary complexity in the code and make it easier to maintain.

Next step was mapping out the controllers and service layer, here I wanted to handle some of the more complex business logic in the service layer and keep the controllers as thin as possible.
Aswell as taking care of any constraints that might not be easy to handle in the DB without compromising performance and readability

There are only 2 controller classes, One for handling all actions that have something to do with districts and updating them, 
and 1 for handling all actions that have something to do with salespersons.

Response and request classes are neatly placed inside the Contracts folder.

ErrorHandling contains a handler class for our custom exception. The custom exception class was created such that user can get proper readable error messages when something goes wrong. 
The exception is thrown in the service layer and repository layer it is caught by the handler middleware before reaching the controller layer 
where it is then returned to the user as a proper response.

The Service layer houses the business logic, the class for the custom exception. The way this is designed 
is using interfaces to define the contract for the service and then implementing that contract in a concrete class. This allows for easy testing and mocking of the service layer.
The Service classes themselves call upon the repository layer to get the data they need to perform their business logic. This helps remove data access logic from the service layer 
and keeps it focused on business logic only. These repositories are accessed through yet another abstraction which is the interfaces for the repositories. 
This again allows for easy mocking of the repositories in the service layer tests.

The Data layer houses the repository classes that are responsible for accessing the data from the database. These classes implement the repository interfaces defined in the application layer.
It also houses the SqlConnectionFactory class which is responsible for creating the SqlConnection objects that are used to access the database. This class is used by the repository classes to get a connection to the database.

The Domain class houses all the entities that are used in the application. These entities are used to map the data from the database to the application and vice versa.

Finally the test folder contain both Integration and unitTests. IntegrationTests handle the end to end testing of the apis, while the unitTests utilise a fakeService and fakeRepository 
to test the service layer in isolation. The unitTests are designed to test the business logic in the service layer without having to worry about the data access layer or the database itself.
Furthermore a SalesDistrictApiFactory was introduced to replicate the schemas from the production database into a test DB to be used by all integrationTests. This was done to avoid any tests affecting 
the production database and to ensure that the tests are run in a controlled environment. The factory is responsible for creating the test database and seeding it with test data before each test is run. 
This ensures that each test is run in a clean state and that the tests do not interfere with each other.

Serilog was introduced to log all requests and responses to the api. This was done to help with debugging and to provide a way to track down any issues that might arise in production.

Autofac was introduced to handle dependency injection. This was done to help with testing and to provide a way to easily swap out implementations of interfaces for testing purposes.
Autofac also provides a way to register modules which is used to register all the dependencies for each layer in one place.
it also enables registration via naming conventions which is used to register all the services and repositories in one place. This all helps keep the Program.cs file clean and easy to read.