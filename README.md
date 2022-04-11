# HomesForAll

Homes For All is a humanitarian aid platform designed to centralise and assign accomodations to people in need such as refugees or those who are financially constrained or disabled. 

## Installation

Using git:

```bash
git clone https://github.com/AndreiDr18/HomesForAll
```

We have to change the directory to the startup project:
```bash
cd ./HomesForAll
```

Then we have to create the database:

```bash
dotnet ef database update -p ../DAL
```

Or if no migrations folder is available add this line before the database update:

```bash
dotnet ef migrations add <MigrationName> -p ../DAL
```

Then just add the SendGrid API KEY, JWT Secret, Valid Audience and Valid Issuer, install all dependencies if not already installed and you're all good to go:

```bash
dotnet build
```

## Usage

HomesForAll exposes endpoints from four controllers: AuthController, TenantController, LandlordController and PropertyController

Base reponse is of object type ServerResponse<TBody>:
```bash
  {
    Success = bool,
    Message = string,
    Body = TBody
  }
```

## AuthController

  ```bash
  [Route("api/[controller]/register")]
  ```
  METHOD: POST

  CLEARANCE: AllowAnonymous

  Endpoint used for user registration, return Ok with no TBody if user registers succesfully.

  ```bash
  [Route("api/[controller]/login")]
  ```
  METHOD: POST

  CLEARANCE: AllowAnonymous

  Endpoint used for user authentication, returns Ok with authorization token and refresh token upon succesful authorization.

  ```bash
  [Route("api/[controller]/refreshToken")]
  ```
  METHOD: POST

  CLEARANCE: AllowAnonymous

  Endpoint used for acquiring a new authorization token based on a expired one, returns Ok with a new authorization token and refresh token. 

  ```bash
  [Route("api/[controller]/verifyEmail/{id}")]
  ```
  METHOD: POST

  CLEARANCE: AllowAnonymous

  Endpoint used for verifying email posession, returns Ok with no TBody upon succesful verification.

## TenantController


  ```bash
  [Route("api/[controller]/getTenant")]
  ```

  METHOD: GET

  CLEARANCE: Tenant

  Endpoint used for retrieving tenant information based on authorization token, returns Ok with Name, PhoneNumber, Date of Birth, Username and tenancy requests.


  ```bash
  [Route("api/[controller]/updateTenant")]
  ```

  METHOD: PUT

  CLEARANCE: Tenant

  Endpoint used for updating tenant information based on authorization token, returns Ok with no TBody.

  ```bash
  [Route("api/[controller]/requestProperty")]
  ```

  METHOD: POST

  CLEARANCE: Tenant

  Endpoint used for sending tenancy request to a certain property (restricted to one request per property), returns Ok with no TBody.

  ```bash
  [Route("api/[controller]/getCurrentRequests")]
  ```

  METHOD: GET

  CLEARANCE: Tenant

  Endpoint used for getting all tenancy requests sent to any property, returns Ok with an array of requests as TBody.

  ```bash
  [Route("api/[controller]/removeRequest/{requestId}")]
  ```

  METHOD: DELETE

  CLEARANCE: Tenant

  Endpoint used for removing a certain tenancy request based on its id, returns Ok with no TBody.
  
  ```bash
  [Route("api/[controller]/getLandlordContactDetails")]
  ```

  METHOD: GET

  CLEARANCE: Tenant

  Endpoint used for getting contact details of the landlord that owns the property in which the tenant's been accepted, returns Ok with the landlord details as TBody.
  
## LandlordController
  
  
  ```bash
  [Route("api/[controller]/getPropertyRequests")]
  ```

  METHOD: GET

  CLEARANCE: Landlord

  Endpoint used for getting all property requests made to any of the user's properties, returns Ok with an array of requests as TBody.
  
  
  ```bash
  [Route("api/[controller]/registerProperty")]
  ```

  METHOD: POST

  CLEARANCE: Landlord

  Endpoint used for registering a property to be up for tenancy requests, returns Ok with the property's name as TBody.
  
  
  ```bash
  [Route("api/[controller]/getLandlord")]
  ```

  METHOD: GET

  CLEARANCE: Landlord

  Endpoint used for retrieving user information, returns Ok with landlord's details as TBody.
  
  
  ```bash
  [Route("api/[controller]/updateLandlord")]
  ```

  METHOD: PUT

  CLEARANCE: Landlord

  Endpoint used for updating user information, returns Ok with the new information as TBody.
  
  ```bash
  [Route("api/[controller]/acceptRequest/{requestId}")]
  ```

  METHOD: GET

  CLEARANCE: Landlord

  Endpoint used for accepting a certain tenancy request based on its id, returns Ok with no TBody.
  
  ```bash
  [Route("api/[controller]/revokeRequest/{requestId}")]
  ```

  METHOD: DELETE

  CLEARANCE: Landlord

  Endpoint used for revoking a certain tenancy request based on its id, returns Ok with no TBody.
  
  ```bash
  [Route("api/[controller]/deleteProperty/{propertyId}")]
  ```

  METHOD: DELETE

  CLEARANCE: Landlord

  Endpoint used for deleting a certain property based on its id, returns Ok with no TBody.
  
## PropertyController
  
  
  ```bash
  [Route("api/[controller]/getAll")]
  ```

  METHOD: GET

  CLEARANCE: Landlord,Tenant

  Endpoint used for retrieving all properties available, returns Ok with an array of properties as TBody.
  
  ```bash
  [Route("api/[controller]/getById/{propertyId}")]
  ```

  METHOD: GET

  CLEARANCE: Landlord,Tenant

  Endpoint used for retrieving a certain property based on its id, returns Ok with the property details as TBody.

## License
[MIT](https://choosealicense.com/licenses/mit/)
