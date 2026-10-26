# sales-districts

The idea for this project is to start with the DB design first and foremost to get a concrete foundation of entities.

I wanted to capture as much of the business logic in the Constraints of the DB since this will help remove unnecessary complexity in the code and make it easier to maintain.

Next step was mapping out the controllers and service layer, here I wanted to handle some of the more complex business logic in the service layer and keep the controllers as thin as possible.
Aswell as taking care of any constraints that might not be easy to handle in the DB without compromising performance and readability