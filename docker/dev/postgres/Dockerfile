FROM library/postgres:12.0

ENV POSTGRES_PASSWORD=password
ENV POSTGRES_USER=postgres

ADD init.sh /docker-entrypoint-initdb.d/

EXPOSE 5432 5432
