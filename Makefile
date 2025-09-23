migrate:
	dotnet ef migrations add InitialCreate -p ../TodoList.Infrastructure -s . -o Persistence/Migrations

dbupdate:
	dotnet ef database update -p ../TodoList.Infrastructure -s .
