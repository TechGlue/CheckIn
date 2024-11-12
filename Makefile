#!/bin/sh
run: 
	docker-compose build
	docker-compose up -d

start-dev: 
	docker build CheckMeInDB/CheckMeInDBSetup/.
	docker run --platform linux/amd64 --cap-add SYS_PTRACE -e 'ACCEPT_EULA=1' -e 'MSSQL_SA_PASSWORD=StrongPassword!123' -p 1433:1433 --name azure-sql-edge -d mcr.microsoft.com/azure-sql-edge

clean: 
	docker-compose down -v

clean-all:
	docker-compose down -v
	docker system prune
