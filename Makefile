run: 
	docker-compose build
	docker-compose up -d
	./CheckMeInDB/scripts/dbInit.sh

drop:
	if [ -z "$(shell docker ps -a -q)" ]; then \
		echo "No containers to remove"; \
	else \
		docker stop $(shell docker ps -a -q); \
		docker rm $(shell docker ps -a -q); \
	fi
	docker-compose down -v

clean-all:
	echo "Removing all containers and pruning system and volumes"
	docker-compose down -v
	docker system prune
	docker volume prune -f 
