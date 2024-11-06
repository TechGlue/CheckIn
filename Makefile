run: 
	docker-compose build
	docker-compose up -d

start-dev: 
	docker build -t azuresqledge-testdb  CheckMeInDB/. 
	docker run -p 1433:1433 --name azuresqledge-testdb -d azuresqledge 

clean: 
	docker-compose down -v

clean-all:
	docker-compose down -v
	docker system prune
