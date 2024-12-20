run: 
	docker-compose build
	docker-compose up -d

drop:
	if [ -z "$(shell docker ps -a -q)" ]; then \
		echo "No containers to remove"; \
	else \
		docker stop $(shell docker ps -a -q); \
		docker rm $(shell docker ps -a -q); \
	fi
	docker-compose down -v

clean-all:
	echo "Removing all containers and prune"
	docker-compose down -v
	docker system prune
	docker volume prune -f 
