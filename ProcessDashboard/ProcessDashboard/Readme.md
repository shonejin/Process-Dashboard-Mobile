This contains the shared logic that encompasses communication with the server and storing the data in the database.

Communication with the server:

Overview:
Communication with the enterprise server is achieved by using a combination of libraries. The libraries used are

i) Refit
ii) ModernHttpClient
iii) Fusilade


Refit makes it simple to perform JSON parsing into OO objects. The core of this logic is written in IPDashApi.

Preferred order for review:

1.) Interfaces:
IApiTypes.cs -> IPDashApi -> IPDashServices.cs 

2.) Service
ApiTypes.cs -> PDashService.cs 

3.) Controller.cs
