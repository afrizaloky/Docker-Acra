FROM postgres:13

# Original postgresql init script expects empty $PGDATA so we initially place
# certificates into the image to the intermediate directory
COPY ./files/postgresql.crt /tmp.ssl/server.crt
COPY ./files/postgresql.key /tmp.ssl/server.key
COPY ./files/example.cossacklabs.com.crt /tmp.ssl/example.cossacklabs.com.crt
RUN chown -R postgres:postgres /tmp.ssl

COPY ./files/postgresql-ssl-configure.sh /docker-entrypoint-initdb.d/
