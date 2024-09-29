run: 
	docker-compose build
	docker-compose up 

run-background: 
	docker-compose build
	docker-compose up -d

clean: 
	docker-compose down -v

clean-all:
	docker-compose down -v
	docker system prune
