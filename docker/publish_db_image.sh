#!/bin/bash
docker-compose build 
docker tag apollo-db:latest envyit2k20/apollo-db:latest
docker push envyit2k20/apollo-db:latest