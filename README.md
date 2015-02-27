AspNet.Identity.PostgreSQL
==========================

This is an ASP.NET Identity provider for those who wish to use PostgreSQL as their database engine for storing user credentials. It has been tested to work with PostgreSQL 9.3.

This is a port of [AspNet.Identity.MySQL](https://github.com/raquelsa/AspNet.Identity.MySQL). 

To use this Indentity provider in the default ASP.NET MVC5 template project, follow these steps:

1. Add the AspNet.Identity.PostgreSQL project as reference to your main MVC project.
2. In your MVC project, replace all references to "using Microsoft.AspNet.Identity.EntityFramework;" with "using AspNet.Identity.PostgreSQL;".
3. In IdentityModel.cs, ensure that that ApplicationDbContext inherits from "PostgreSQLDatabase".
4. Ensure that Web.config has your PostgreSQL connection string included in "DefaultConnection". If your PostgreSQL database is located in a different schema, make sure you specify the schema name, because this Indentity provider is going to assume a public schema otherwise. If you do not know what this means, then do not be concerned because then your schema is likely just public.
5. In Solution Explorer, right-click your MVC5 project and select "Manage NuGet Packages". In the search text box dialog, type "Identity.EntityFramework". Select this package in the list of results and click "Uninstall". You will be prompted to uninstall the dependency package EntityFramework. Click on Yes as we will no longer need this package.
6. Lastly, execute the SQL script found in the solution against your database in pgAdmin (or via the command line).
